using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using Repository.Domain;
using System;
using System.Linq;

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

        public string DeleteWebsite(string[] ids)
        {
            try
            {
                if (ids.Count() == 1)
                {
                    var websiteId = ids[0];
                    _websiteApp.DeleteWebsite(websiteId);
                }
                else
                {
                    Result.Code = 500;
                    Result.Message = "删除失败！";
                }
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.ToString();
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public string ModifyWebsite(Website website)
        {
            try
            {
                _websiteApp.Update(website);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public string ReGetWebsite(string websiteId)
        {
            try
            {
                if (!_websiteApp.ReSearch(websiteId))
                {
                    Result.Code = 500;
                    Result.Message = "重新获取失败！";
                }
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        public string SwitchWebsite(string websiteId, int state)
        {
            try
            {
                _websiteApp.UpdateState(websiteId, state);
                switch (state)
                {
                    case 1: //开始
                        _websiteApp.ReSearch(websiteId);
                        break;
                    case 3: //暂停 
                        break;
                    case 4: //停止
                        break;
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