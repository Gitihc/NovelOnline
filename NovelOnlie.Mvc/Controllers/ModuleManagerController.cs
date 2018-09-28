using System;
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
    public class ModuleManagerController : BaseController
    {
        private ModuleManagerApp _app;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="app"></param>
        public ModuleManagerController(IAuth authUtil, IHostingEnvironment hostingEnvironment, ModuleManagerApp app) : base(authUtil,hostingEnvironment)
        {
            _app = app;
        }
        [Authenticate]
        public ActionResult Index()
        {
            return View();
        }

        [Authenticate]
        public ActionResult Assign()
        {
            return View();
        }

        /// 加载特定用户的模块
        /// </summary>
        /// <param name="firstId">The user identifier.</param>
        /// <returns>System.String.</returns>
        public string LoadForUser(string firstId)
        {
            var modules = _app.LoadForUser(firstId);
            return JsonHelper.Instance.Serialize(modules);
        }

        /// <summary>
        /// 根据某用户ID获取可访问某模块的菜单项
        /// </summary>
        /// <returns></returns>
        public string LoadMenusForUser(string moduleId, string firstId)
        {
            var menus = _app.LoadMenusForUser(moduleId, firstId);
            return JsonHelper.Instance.Serialize(menus);
        }

        /// <summary>
        /// 获取发起页面的菜单权限
        /// </summary>
        /// <returns>System.String.</returns>
        public string LoadAuthorizedMenus(string modulecode)
        {
            var user = _authUtil.GetCurrentUser();
            var module = user.Modules.FirstOrDefault(u => u.Code == modulecode);
            if (module != null)
            {
                return JsonHelper.Instance.Serialize(module.Elements.OrderBy(e => e.Sort));

            }
            return "";
        }

        #region 模块

        #region 模块操作
        //添加模块
        [HttpPost]
        public string Add(Module model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ParentId))
                {
                    model.ParentId = Guid.Empty.ToString();
                }
                _app.Add(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        //添加模块
        [HttpPost]
        public string Update(Module model)
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
        #endregion
        #endregion

        #region 模块菜单
        /// <summary>
        /// 模块子集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetMenus(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = _authUtil.GetCurrentUser();

                var module = user.Modules.FirstOrDefault(u => u.Id == id);

                var Elements = module.Elements.OrderBy(e => e.Sort);

                return JsonHelper.Instance.Serialize(Elements);
            }
            return "";
        }
        //添加菜单
        [HttpPost]

        public string AddMenu(ModuleElement model)
        {
            try
            {
                _app.AddMenu(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }

        //编辑菜单
        [HttpPost]

        public string UpdateMenu(ModuleElement model)
        {
            try
            {
                _app.UpdateMenu(model);
            }
            catch (Exception ex)
            {
                Result.Code = 500;
                Result.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return JsonHelper.Instance.Serialize(Result);
        }


        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpPost]
        public string DelMenu(params string[] ids)
        {
            try
            {
                _app.DelMenu(ids);
            }
            catch (Exception e)
            {
                Result.Code = 500;
                Result.Message = e.InnerException?.Message ?? e.Message;
            }

            return JsonHelper.Instance.Serialize(Result);
        }


        #endregion
    }
}