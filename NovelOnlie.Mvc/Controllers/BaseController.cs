using System;
using System.Linq;
using System.Reflection;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NovelOnline.App.Interface;
using NovelOnline.App.SSO;
using OpenAuth.Mvc.Models;

namespace NovelOnlie.Mvc.Controllers
{
    /// <summary>
    /// 基础控制器
    /// <para>用于控制登录用户是否有权限访问指定的Action</para>
    /// <para>李玉宝新增于2016-07-19 11:12:09</para>
    /// </summary>
    public class BaseController : SSOController
    {
        protected Response Result = new Response();
        protected string Controllername;   //当前控制器小写名称
        protected string Actionname;        //当前Action小写名称

        public BaseController(IAuth authUtil) : base(authUtil)
        {
           
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!_authUtil.CheckLogin()) return;

            var description =
                (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)filterContext.ActionDescriptor;

            Controllername = description.ControllerName.ToLower();
            Actionname = description.ActionName.ToLower();

            var function = ((TypeInfo)GetType()).DeclaredMethods.FirstOrDefault(u => u.Name.ToLower() == Actionname);

            if (function == null)
                throw new Exception("未能找到Action");
            //权限验证标识
            var authorize = function.GetCustomAttribute(typeof(AuthenticateAttribute));
            if (authorize == null)
            {
                return;
            }
            var currentModule = _authUtil.GetCurrentUser().Modules.FirstOrDefault(u => u.Link !=null && u.Link.ToLower().Contains(Controllername));
            //当前登录用户没有Action记录&&Action有authenticate标识
            if (currentModule == null)
            {
                filterContext.Result = new RedirectResult("/Account/Index");
                return;
            }
        }


    }
}