using BaseLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NovelOnline.App
{
    public class NovelManagerApp : BaseApp<Novel>
    {
        private readonly UserNovelManager _userNovelApp;
        private readonly ChapterManagerApp _chapterManager;
        public NovelManagerApp(IUnitWork unitWork
                            , IRepository<Novel> repository
                            , UserNovelManager userNovelApp
                            , ChapterManagerApp chapterManager) : base(unitWork, repository)
        {
            _userNovelApp = userNovelApp;
            _chapterManager = chapterManager;
        }

        public List<Chapter> GetChapterList(string novelId)
        {
            return _chapterManager.GetChapterList(novelId);
        }

        #region Novel操作

        public IQueryable<Novel> GetAllNovel()
        {
            return Repository.GetAll();
        }

        public Novel GetNovel(string novelId)
        {
            return Repository.FindSingle(x => x.Id == novelId);
        }

        public IQueryable<Novel> UserNovelList(User user)
        {
            var userNovelIds = _userNovelApp.GetUserNovelIds(user.Id).ToList<string>();
            return Repository.Find(n => userNovelIds.Contains(n.Id));
        }

        public void RemoveNovel(User user, string novelId)
        {
            _userNovelApp.RemoveRelationShip(user.Id, novelId);//移除与当前用户关联
            if (_userNovelApp.GetCount(novelId) == 0)   //判断是否还有其他用户引用
            {
                var objNovel = Repository.FindSingle(x => x.Id == novelId);
                var saveFilePath = objNovel.PhysicalPath;

                Repository.Delete(x => x.Id == novelId);//删除Novel记录
                _chapterManager.DropNovelTable(novelId);//删除章节表
                var files = Directory.GetFiles(saveFilePath);
                foreach (var f in files)
                {
                    if (File.Exists(f))   //删除物理文件
                    {
                        File.Delete(f);
                    }
                }
             
                var dirPath = saveFilePath;
                if (Directory.Exists(dirPath)) //删除物理文件夹
                {

                    Directory.Delete(dirPath);
                }
            }
        }

        public void Update(Novel model)
        {
            Repository.Update(model);
        }

        public void UpdateState(string novelId, int state)
        {
            Repository.Update(x => x.Id == novelId, n => new Novel { State = state });
        }
        #endregion

        #region 获取网络Novel
        public bool GetWebsiteNovel(User user, string url, string novelName)
        {
            if (Repository.Find(x => x.OriginLink == url).Count() > 0) return true;
            var tableListUrl = url;
            var objNVB = NVHelper.NVBaseObject(tableListUrl); //获取对应处理类
            if (objNVB == null) return false;
            var newNovel = new Novel
            {
                Id = Guid.NewGuid().ToString(),
                Name = string.IsNullOrEmpty(novelName) ? DateTime.Now.ToString("yyyyMMddHHmmss") : novelName,
                PhysicalPath = "",
                OriginLink = tableListUrl,
                FromType = 1
            };
            newNovel.PhysicalPath = Path.Combine(AppContext.BaseDirectory, @"books\" + newNovel.Id);

            Repository.Add(newNovel);//添加到Novel表
            _userNovelApp.AddRelationShip(user.Id, user.Name, newNovel.Id, newNovel.Name);//添加用户与novel关系

            var listNVChapter = objNVB.GetList(tableListUrl);//获取章节信息
            if (listNVChapter.Count() > 0)
            {
                var chapterName = objNVB.GetNovelName();
                //更改novel状态为获取中
                Repository.Update(x => x.Id == newNovel.Id, n => new Novel { State = 1, Name = chapterName });

                var listChapter = new List<Chapter>();
                foreach (var c in listNVChapter.OrderBy(x => x.Sort))
                {
                    var newChapter = new Chapter
                    {
                        NovelId = newNovel.Id,
                        NovelName = newNovel.Name,
                        Name = c.Title,
                        OriginLink = c.Link,
                        Sort = c.Sort,
                        State = 0,
                        ChapterStartPosition = 0,
                        ChapterEndPosition = 0
                    };
                    listChapter.Add(newChapter);
                }
                //保存章节信息到 以novelId为表名的章节表中
                _chapterManager.AddRandChapter(newNovel.Id, listChapter);
                //线程获取章节数据保存到本地
            }
            return false;
        }
        /// <summary>
        /// 重新获取
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public bool ReGetWebsiteNovel(string novelId)
        {
            var novel = Repository.FindSingle(x => x.Id == novelId);
            if (novel == null) return false;
            var objNVB = NVHelper.NVBaseObject(novel.OriginLink); //获取对应处理类
            if (objNVB == null) return false;
            var listNVChapter = objNVB.GetList(novel.OriginLink);//获取章节信息
            if (listNVChapter.Count() > 0)
            {
                var listChapter = new List<Chapter>();
                foreach (var c in listNVChapter.OrderBy(x => x.Sort))
                {
                    var newChapter = new Chapter
                    {
                        NovelId = novel.Id,
                        NovelName = novel.Name,
                        Name = c.Title,
                        OriginLink = c.Link,
                        Sort = c.Sort,
                        State = 0,
                        ChapterStartPosition = 0,
                        ChapterEndPosition = 0
                    };
                    listChapter.Add(newChapter);
                }
                //保存章节信息到 以novelId为表名的章节表中
                _chapterManager.AddRandChapter(novel.Id, listChapter);
                return true;
            }
            return false;
        }
        #endregion

        #region 处理上传本地文件
        public async void HandLocalFiles(User user, IFormFile file, string saveBookPath)
        {
            var novelId = Guid.NewGuid().ToString();
            saveBookPath = Path.Combine(saveBookPath, novelId);

            var filename = file.FileName;

            if (!System.IO.Directory.Exists(saveBookPath))
            {
                System.IO.Directory.CreateDirectory(saveBookPath);
            }
            var filePath = Path.Combine(saveBookPath, filename);
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            Repository.Add(new Novel()
            {
                Id = novelId,
                Name = filename,
                PhysicalPath = filePath,
                OriginLink = "",
            });
            _userNovelApp.AddRelationShip(user.Id, user.Name, novelId, filename);

            await Task.Run(() =>
             {
                 try
                 {
                     var _tmpChapterManager = AutofacExt.GetFromFac<ChapterManagerApp>();
                     var listChapter = _tmpChapterManager.AutoMatchSubchapter(novelId);
                     if (listChapter.Count() == 0) return;
                     _tmpChapterManager.AddRandChapter(novelId, listChapter);
                     var _tmpNovelManager = AutofacExt.GetFromFac<NovelManagerApp>();
                     _tmpNovelManager.UpdateState(novelId, 1);
                 }
                 catch (Exception ex)
                 {
                     throw ex;
                 }
             });
        }
        #endregion
        /// <summary>
        /// 添加本地书籍到我的书架
        /// </summary>
        /// <param name="user"></param>
        /// <param name="novel"></param>
        public void AddLocalNovelInMyNovel(User user, Novel novel)
        {
            _userNovelApp.AddRelationShip(user.Id, user.Name, novel.Id, novel.Name);
        }

        #region 下载

        public void DownNovel(HttpContext httpContext, Novel novel)
        {
            switch (novel.FromType)
            {
                case 0:
                    break;
                case 1:
                    DownWebNovel(httpContext, novel);
                    break;
            }
        }

        private void DownWebNovel(HttpContext httpContext, Novel novel)
        {
            var bookDir = novel.PhysicalPath;
            if (Directory.Exists(bookDir))
            {
                var sqlStr = string.Format("SELECT * FROM [{0}] ORDER BY SORT ", novel.Id);
                var listChapter = Repository.ChapterQueryFromSql(sqlStr).ToList();

                var novelName = string.Format("{0}.txt", novel.Name);
                string agent = httpContext.Request.Headers["USER-AGENT"];

                if (!string.IsNullOrEmpty(agent) && agent.ToLower().IndexOf("firefox") > 0)
                {
                    novelName = HttpUtility.UrlEncode(novelName);
                    var headerValue = string.Format("attachment; filename=\"{0}\"", novelName);
                    httpContext.Response.Headers.Add("Content-disposition", headerValue);
                }
                else
                {
                    novelName = HttpUtility.UrlEncode(novelName);
                    httpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=" + novelName);
                }


                foreach (Chapter chapter in listChapter)
                {
                    var fileName = chapter.Name;
                    fileName = _chapterManager.regexOfSaveChapter.Replace(fileName, "");
                    fileName = string.Format("{0}.txt", fileName);
                    var filePath = Path.Combine(bookDir, fileName);
                    if (File.Exists(filePath))
                    {

                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8")))
                            {
                                var line = string.Empty;
                                while (sr.Peek() >= 0)
                                {
                                    line = sr.ReadLine() + Environment.NewLine;
                                    var fbt = Encoding.UTF8.GetBytes(line);
                                    httpContext.Response.Body.Write(fbt, 0, fbt.Length);
                                }
                            }
                        }
                    }
                }
                httpContext.Response.Body.Flush();
            }
        }

        #endregion
    }
}
