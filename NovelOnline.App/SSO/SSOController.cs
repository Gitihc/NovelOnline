using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NovelOnline.App.Interface;

namespace NovelOnline.App.SSO
{
    public class SSOController : Controller
    {
        public const string Token = "Token";

        protected IAuth _authUtil;

        public SSOController(IAuth authUtil)
        {
            _authUtil = authUtil;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = "";

            //Token by QueryString
            var request = filterContext.HttpContext.Request;
            if (request.Cookies[Token] != null)  //从Cookie读取Token
            {
                token = request.Cookies[Token];
            }

            if (string.IsNullOrEmpty(token))
            {
                //直接登录
                filterContext.Result = LoginResult("");
                return;
            }
            //验证
            if (_authUtil.CheckLogin(token, request.Path) == false)
            {
                //会话丢失，跳转到登录页面
                filterContext.Result = LoginResult("");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        public virtual ActionResult LoginResult(string username)
        {
            return new RedirectResult("/Accout/Index");
        }
    }
}
