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
        private IHostingEnvironment _hostingEnv;
        public NovelManagerController(IAuth authUtil, NovelManagerApp app, IHostingEnvironment hostingEnv) : base(authUtil)
        {
            _app = app;
            _hostingEnv = hostingEnv;
        }

        [Authenticate]
        public IActionResult Index()
        {
            return View();
        }

        public string GetNovelList()
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
    }
}