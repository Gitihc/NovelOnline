using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using OpenAuth.App.Response;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovelOnlie.Mvc.Controllers
{
    public class UserSessionController : BaseController
    {
        private readonly AuthStrategyContext _authStrategyContext;
        public UserSessionController(IAuth authUtil) : base(authUtil)
        {
            _authStrategyContext = _authUtil.GetCurrentUser();
        }

        public string GetUserName()
        {
            return _authUtil.GetUserName();
        }

        /// <summary>
        /// 获取登录用户可访问的所有模块，及模块的操作菜单
        /// </summary>
        public string GetModulesTree()
        {
            var moduleTree = _authStrategyContext.Modules.GenerateTree(u => u.Id, u => u.ParentId,Guid.Empty.ToString());
            return JsonHelper.Instance.Serialize(moduleTree);
        }

        public string GetModulesComboTree()
        {
            var listObject = GetTree(_authStrategyContext.Modules, Guid.Empty.ToString());
            return JsonHelper.Instance.Serialize(listObject);
        }

        /// <summary>
        /// combotree模块树
        /// </summary>
        /// <param name="allModules"></param>
        /// <param name="rooId"></param>
        /// <returns></returns>
        private List<object> GetTree(List<ModuleView> allModules, string rooId)
        {
            var listObject = new List<object>();
            var listMatchModules = from p in allModules where p.ParentId == rooId select p;
            foreach (var m in listMatchModules)
            {
                var obj = new { id = m.Id, text = m.Name, children = GetTree(allModules, m.Id) };
                listObject.Add(obj);
            }
            return listObject;
        }

        /// <summary>
        /// 模块子集
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetModulesTable(string id)
        {
            string cascadeId = ".0.";
            if (!string.IsNullOrEmpty(id))
            {
                var obj = _authStrategyContext.Modules.SingleOrDefault(u => u.Id == id);
                if (obj == null)
                    throw new Exception("未能找到指定对象信息");
                cascadeId = obj.CascadeId;
            }

            var query = _authStrategyContext.Modules.Where(u => u.CascadeId.Contains(cascadeId));

            return JsonHelper.Instance.Serialize(query);
        }
        /// <summary>
        /// 获取用户可访问的模块列表
        /// </summary>
        public string GetModules()
        {
            return JsonHelper.Instance.Serialize(_authStrategyContext.Modules);
        }
    }
}