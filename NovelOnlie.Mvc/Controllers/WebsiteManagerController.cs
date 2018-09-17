using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using System;

namespace NovelOnlie.Mvc.Controllers
{
    public class WebsiteManagerController : BaseController
    {
        private readonly WebsiteApp _websiteApp;

        public WebsiteManagerController(IAuth authUtil, WebsiteApp websiteApp) : base(authUtil)
        {
            _websiteApp = websiteApp;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string GetResourceList()
        {
            var allWebsite = _websiteApp.GetWebsiteList();
            return JsonHelper.Instance.Serialize(allWebsite);
        }

        public string DeleteWebsite(string websiteId)
        {
            try
            {
                
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