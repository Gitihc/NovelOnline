using System;
using System.IO;
using System.Linq;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using OpenAuth.Mvc.Models;
using Repository.Domain;

namespace NovelOnlie.Mvc.Controllers
{
    public class NovelManagerController : BaseController
    {
        private readonly NovelManagerApp _app;
        private readonly WebsiteApp _websiteApp;
        private IHostingEnvironment _hostingEnv;
        public NovelManagerController(IAuth authUtil, IHostingEnvironment hostingEnvironment, NovelManagerApp app, WebsiteApp websiteApp, IHostingEnvironment hostingEnv) : base(authUtil, hostingEnvironment)
        {
            _app = app;
            _websiteApp = websiteApp;
            _hostingEnv = hostingEnv;
        }

        [Authenticate]
        public IActionResult Index()
        {
            return View();
        }
        [Authenticate]
        public IActionResult LocalNovelList()
        {
            return View();
        }
        /// <summary>
        /// 获取我的书籍列表
        /// </summary>
        /// <returns></returns>
        public string GetMyNovelList()
        {
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                var listNovel = _app.UserNovelList(user).ToList();
                return JsonHelper.Instance.Serialize(listNovel);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        /// <summary>
        /// 获取书籍列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetNovelList(int page, int rows, string key)
        {
            try
            {
                var pageSize = rows;
                var allNovel = _app.GetAllNovel().ToList();
                if (!string.IsNullOrEmpty(key))
                {
                    allNovel = (from p in allNovel where p.Name.Contains(key) select p).ToList();
                }
                var total = allNovel.Count();
                var r = allNovel.Skip((page - 1) * pageSize).Take(pageSize);
                var result = new { total = total, rows = r };
                return JsonHelper.Instance.Serialize(result);
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public string GetChapterList(string novelId)
        {
            try
            {
                var listChapter = _app.GetChapterList(novelId);
                return JsonHelper.Instance.Serialize(listChapter);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public String DeleteNovel(string[] ids)
        {
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                foreach (var id in ids)
                {
                    _app.RemoveNovel(user, id);
                }
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        //添加模块
        [HttpPost]
        public string Update(Novel model)
        {
            try
            {
                _app.Update(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        [HttpPost]
        public string GetNovelByLink(string ListLink)
        {
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                _app.GetWebsiteNovel(user, ListLink, string.Empty);
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public string ReGetNovel(string novelId)
        {
            try
            {
                var result = _app.ReGetWebsiteNovel(novelId);
                if (!result)
                {
                    Result.Code = 500;
                    Result.Message = "获取失败！";
                }
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        [HttpPost]
        public string SearchWebsite(string websiteLink)
        {
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                bool result = _websiteApp.Search(user, websiteLink);
                if (!result)
                {
                    Result.Code = 500;
                    Result.Message = "搜索任务失败或资源列表中已存在！";
                }
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        [HttpPost]
        public string UploadLocalfile()
        {
            try
            {
                var saveFilePath = Path.Combine(_hostingEnv.WebRootPath, "books");

                var userContext = _authUtil.GetCurrentUser();
                var user = userContext.User;

                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    _app.HandLocalFiles(user, file, saveFilePath);
                }
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        #region 本地书籍列表按钮事件
        public string AddLocalNovelInMyNovel(string novelId)
        {
            try
            {
                User user = _authUtil.GetCurrentUser().User;
                Novel novel = _app.GetNovel(novelId);
                _app.AddLocalNovelInMyNovel(user, novel);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public void DownNovel(string novelId)
        {
            try
            {
                var novel = _app.GetNovel(novelId);
                if (novel != null)
                {
                    _app.DownNovel(HttpContext, _hostingEnvironment.ContentRootPath, novel);
                }
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
        }
        #endregion
    }
}