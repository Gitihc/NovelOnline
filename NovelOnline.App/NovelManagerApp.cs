using Microsoft.AspNetCore.Http;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
                if (File.Exists(saveFilePath))   //删除物理文件
                {
                    File.Delete(saveFilePath);
                }
                var dirPath = Path.GetDirectoryName(saveFilePath);
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
        #endregion

        #region 处理Novel目录地址

        #endregion

        #region 处理搜索Website地址

        #endregion

        #region 处理上传本地文件
        public void HandLocalFiles(User user, IFormFile file, string saveBookPath)
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
            var listChapter = _chapterManager.AutoMatchSubchapter(novelId);
            _chapterManager.AddRandChapter(novelId, listChapter);
        }
        #endregion
    }
}
