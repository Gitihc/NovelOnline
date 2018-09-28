using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;

namespace NovelOnlie.Mvc.Controllers
{
    public class ChapterManagerController : BaseController
    {
        private readonly ChapterManagerApp _app;
        private readonly NovelManagerApp _novelManagerApp;
        public ChapterManagerController(IAuth authUtil, IHostingEnvironment hostingEnvironment, NovelManagerApp novelManagerApp, ChapterManagerApp app) : base(authUtil, hostingEnvironment)
        {
            _novelManagerApp = novelManagerApp;
            _app = app;
        }
        public IActionResult ChapterView()
        {
            return View();
        }

        public IActionResult ChapterList()
        {
            return View();
        }
        //获取章节内容
        public String GetChapterContent(String novelId, String chapterId = "")
        {
            var data = string.Empty;
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                var basePath = _hostingEnvironment.ContentRootPath;
                data = _app.GetChapterContent(basePath, user, novelId, chapterId);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
                throw ex;
            }
            var resultObj = new { Code = Result.Code, Message = Result.Message, Data = data };
            return JsonHelper.Instance.Serialize(resultObj);
        }
        //获取章节目录
        public string GetChapterList(string novelId, int page, int rows, string key)
        {
            var pageSize = rows;
            var chapterList = _app.GetChapterList(novelId);
            if (!string.IsNullOrEmpty(key))
            {
                chapterList = (from p in chapterList where p.Name.Contains(key) select p).ToList();
            }
            var total = chapterList.Count();
            var r = chapterList.Skip((page - 1) * pageSize).Take(pageSize);
            var result = new { total = total, rows = r };
            return JsonHelper.Instance.Serialize(result);
        }
        /// <summary>
        /// 获取章节到本地
        /// </summary>
        /// <param name="novelId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string GetChapterToLocal(string novelId, string id)
        {
            try
            {
                var novel = _novelManagerApp.GetNovel(novelId);
                var sqlStr = string.Format("Select * From [{0}] Where Id= '{1}' and state <> 2 ", novelId, id);
                var listChapter = _app.Repository.ChapterQueryFromSql(sqlStr).ToList();
                if (listChapter.Count() > 0)
                {

                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        var _chapterManagerApp = AutofacExt.GetFromFac<ChapterManagerApp>();

                        foreach (var chapter in listChapter)
                        {
                            _chapterManagerApp.UpdateChapterState(novel.Id, chapter.Id, 1);
                            var tmpChapterContent = _chapterManagerApp.GetWebChapterContent(chapter);
                            _chapterManagerApp.SaveToLocal(_hostingEnvironment.ContentRootPath, novel, chapter, tmpChapterContent);
                            _chapterManagerApp.UpdateChapterState(novel.Id, chapter.Id, 2);
                        }

                        if (_chapterManagerApp.IsAllChapterToLocal(novel.Id))
                        {
                            novel.State = 2;
                            var _novelManagerApp = AutofacExt.GetFromFac<NovelManagerApp>();
                            _novelManagerApp.Update(novel);
                        }
                    });
                }
                else
                {
                    Result.Code = 500;
                    Result.Message = "未找到要获取的章节或者章节全部获取完成！";
                }
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        /// <summary>
        /// 获取所有章节到本地
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public string GetAllChapterToLocal(string novelId)
        {
            try
            {
                var novel = _novelManagerApp.GetNovel(novelId);
                var sqlStr = string.Format("Select * From [{0}] Where state <> 2 ", novelId);
                var listChapter = _app.Repository.ChapterQueryFromSql(sqlStr).ToList();
                if (listChapter.Count() > 0)
                {
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        var _chapterManagerApp = AutofacExt.GetFromFac<ChapterManagerApp>();

                        foreach (var chapter in listChapter)
                        {
                            _chapterManagerApp.UpdateChapterState(novel.Id, chapter.Id, 1);
                            var tmpChapterContent = _chapterManagerApp.GetWebChapterContent(chapter);
                            _chapterManagerApp.SaveToLocal(_hostingEnvironment.ContentRootPath, novel, chapter, tmpChapterContent);
                            _chapterManagerApp.UpdateChapterState(novel.Id, chapter.Id, 2);
                        }

                        if (_chapterManagerApp.IsAllChapterToLocal(novel.Id))
                        {
                            novel.State = 2;
                            var _novelManagerApp = AutofacExt.GetFromFac<NovelManagerApp>();
                            _novelManagerApp.Update(novel);
                        }
                    });
                }
                else
                {
                    Result.Code = 500;
                    Result.Message = "未找到要获取的章节或者章节全部获取完成！";
                }
            }
            catch (Exception ex)

            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }

    }
}