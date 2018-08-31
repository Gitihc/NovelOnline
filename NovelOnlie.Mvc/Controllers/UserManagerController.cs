using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using OpenAuth.App.Response;

namespace NovelOnlie.Mvc.Controllers
{
    public class UserManagerController : BaseController
    {
        private readonly UserManagerApp _app;
        public UserManagerController(IAuth authUtil, UserManagerApp app) : base(authUtil)
        {
            _app = app;
        }
        public IActionResult Index()
        {
            return View();
        }

        public string GetAllUser()
        {
            var allUsers = _app.Repository.GetAll().OrderBy("CreateDate Desc").ToList();

            return JsonHelper.Instance.Serialize(allUsers);
        }

        //添加或修改组织
        [HttpPost]
        public string AddOrUpdate(UserView view)
        {
            try
            {
                if (string.IsNullOrEmpty(view.CreatorId))
                {
                    var user = _authUtil.GetCurrentUser();
                    view.CreatorId = user.User.Id;
                }
                _app.AddOrUpdate(view);

            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        [HttpPost]
        public string Delete(string[] ids)
        {
            try
            {
                _app.Delete(ids);
            }
            catch (Exception e)
            {
                Result.Code = 500;
                Result.Message = e.InnerException?.Message ?? e.Message;
            }

            return JsonHelper.Instance.Serialize(Result);
        }
    }
}