using System;
using System.Linq;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;

namespace NovelOnlie.Mvc.Controllers
{
    public class WebsiteNovelController : BaseController
    {

        private readonly WebsiteNovelApp _websiteNovelApp;
        public WebsiteNovelController(IAuth authUtil, IHostingEnvironment hostingEnvironment, WebsiteNovelApp websiteNovelApp) : base(authUtil, hostingEnvironment)
        {
            _websiteNovelApp = websiteNovelApp;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string GetWebsiteNovelList(string websiteId, int page, int rows, string key)
        {
            if (string.IsNullOrEmpty(websiteId)) return string.Empty;
            var listWebsiteNovel = _websiteNovelApp.GetWebsiteNovelList(websiteId);
            if (!string.IsNullOrEmpty(key))
            {
                listWebsiteNovel = (from p in listWebsiteNovel where p.Name.Contains(key) || p.Author.Contains(key) select p).ToList();
            }
            var r = listWebsiteNovel.Skip((page - 1) * rows).Take(rows);

            var obj = new { total = listWebsiteNovel.Count(), rows = r };

            return JsonHelper.Instance.Serialize(obj);
        }

        /// <summary>
        /// 删除WebsiteNovel
        /// </summary>
        /// <param name="novelIds"></param>
        /// <returns></returns>
        public string RemoveWebsiteNovel(string websiteId, string[] ids)
        {
            try
            {
                _websiteNovelApp.RemoveWebsiteNovel(websiteId, ids);
            }
            catch (Exception ex)
            {

                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        //webnovel to mynovel
        public string AddWebsiteNovelInMyNovel(string websiteId, string id)
        {
            try
            {
                var user = _authUtil.GetCurrentUser().User;
                _websiteNovelApp.AddWebsiteNovelInMyNovel(user, websiteId, id);
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