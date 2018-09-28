using System;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;

namespace NovelOnlie.Mvc.Controllers
{
    public class RelevanceManagerController : BaseController
    {
        private readonly RevelanceManagerApp _app;
        public RelevanceManagerController(IAuth authUtil, IHostingEnvironment hostingEnvironment, RevelanceManagerApp app) : base(authUtil,hostingEnvironment)
        {
            _app = app;
        }
        
        [HttpPost]
        public string Assign(string type, string firstId, string[] secIds)
        {
            try
            {
                var userContext = _authUtil.GetCurrentUser();
                var operatorId = userContext.User.Id;
                _app.Assign(operatorId,type, firstId, secIds);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }
        [HttpPost]
        public string UnAssign(string type, string firstId, string[] secIds)
        {
            try
            {
                _app.UnAssign(type, firstId, secIds);
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