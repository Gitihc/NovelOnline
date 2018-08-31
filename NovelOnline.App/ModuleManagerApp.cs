using Infrastructure;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelOnline.App
{
    public class ModuleManagerApp : BaseApp<Module>
    {
        private RevelanceManagerApp _revelanceApp;
        public ModuleManagerApp(IUnitWork unitWork, IRepository<Module> repository, RevelanceManagerApp revelanceApp) : base(unitWork, repository)
        //public ModuleManagerApp(IUnitWork unitWork,  RevelanceManagerApp revelanceApp) : base(unitWork)
        {
            _revelanceApp = revelanceApp;
        }

        #region 模块
        public List<Module> GetAllModules()
        {
            return Repository.GetAll().OrderBy("CascadeId").ToList();
        }

        /// <summary>
        /// 获取模块子集（包含当前模块）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Module> GetModules(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Repository.GetAll().OrderBy("CascadeId,Sort").ToList();
            }
            else
            {
                return Repository.Find(m => m.Id == id || m.ParentId == id).OrderBy("CascadeId,Sort").ToList(); ;
            }
        }

        #region "模块操作"
        public void Add(Module model)
        {
            ChangeModuleCascade(model);
            Repository.Add(model);
        }
        public void Update(Module model)
        {
            //ChangeModuleCascade(model);
            Repository.Update(model);
        }
        #endregion
        #endregion
        
        #region 菜单
        /// <summary>
        /// 根据某用户ID获取可访问某模块的菜单项
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<ModuleElement> LoadMenusForUser(string moduleId, string userId)
        {
            var elementIds = _revelanceApp.Get(Define.USERELEMENT, true, userId);
            return UnitWork.Find<ModuleElement>(u => elementIds.Contains(u.Id) && u.ModuleId == moduleId);
        }

        /// <summary>
        /// 加载当前用户可访问模块的菜单
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <returns>System.String.</returns>
        public IEnumerable<ModuleElement> LoadMenus(string moduleId)
        {
            //var user = _authUtil.GetCurrentUser();

            //var module = user.Modules.Single(u => u.Id == moduleId);

            //var data = new TableData
            //{
            //    data = module.Elements,
            //    count = module.Elements.Count(),
            //};
            return UnitWork.Find<ModuleElement>(u => u.ModuleId == moduleId).OrderBy("Sort");
        }

        #region 菜单操作
        /// <summary>
        /// 删除指定的菜单
        /// </summary>
        /// <param name="ids"></param>
        public void DelMenu(string[] ids)
        {
            UnitWork.Delete<ModuleElement>(u => ids.Contains(u.Id));
        }

        public void AddMenu(ModuleElement model)
        {
            UnitWork.Add(model);
            UnitWork.Save();
        }

        public void UpdateMenu(ModuleElement model)
        {
            UnitWork.Update<ModuleElement>(model);
            UnitWork.Save();
        }
        #endregion
        #endregion

        #region 用户分配模块
        /// <summary>
        /// 加载特定用户的模块
        /// TODO:这里会加载用户及用户角色的所有模块，“为用户分配模块”功能会给人一种混乱的感觉，但可以接受
        /// </summary>
        /// <param name="userId">The user unique identifier.</param>
        public IEnumerable<Module> LoadForUser(string userId)
        {
            var moduleIds = UnitWork.Find<Relevance>(
                u =>
                    (u.FirstId == userId && u.Key == Define.USERMODULE)).Select(u => u.SecondId);
            return UnitWork.Find<Module>(u => moduleIds.Contains(u.Id)).OrderBy(u => u.Sort);
        }

        #endregion
        
    }
}
