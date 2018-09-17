using OpenAuth.App.Response;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovelOnline.App
{
    /// <summary>
    /// 普通用户授权策略
    /// </summary>
    public class NormalAuthStrategy : BaseApp<User>, IAuthStrategy
    {

        protected User _user;

        private List<string> _userRoleIds;    //用户角色GUID

        public List<ModuleView> Modules
        {
            get
            {
                var moduleIds = UnitWork.Find<Relevance>(
                    u =>
                        (u.FirstId == _user.Id && u.Key == Define.USERMODULE)).Select(u => u.SecondId);

                var modules = (from module in UnitWork.Find<Module>(u => moduleIds.Contains(u.Id))
                               select new ModuleView
                               {
                                   Id = module.Id,
                                   Name = module.Name,
                                   CascadeId = module.CascadeId,
                                   ParentId = module.ParentId,
                                   ParentName = module.ParentName,
                                   Code = module.Code,
                                   Type = module.Type,
                                   Link = module.Link,
                                   Icon = module.Icon,
                                   IsEnable = module.IsEnable,
                                   Sort = module.Sort,
                                   CreateDate = module.CreateDate
                               }).ToList();

                var elementIds = UnitWork.Find<Relevance>(
                  u =>
                      (u.FirstId == _user.Id && u.Key == Define.USERELEMENT)).Select(u => u.SecondId);

                var usermoduleelements = UnitWork.Find<ModuleElement>(u => elementIds.Contains(u.Id));

                foreach (var module in modules)
                {
                    module.Elements = usermoduleelements.Where(u => u.ModuleId == module.Id).ToList();
                }

                return modules.OrderBy(x => x.Sort).ToList();
            }
        }

        //public List<Role> Roles
        //{
        //    get { return UnitWork.Find<Role>(u => _userRoleIds.Contains(u.Id)).ToList(); }
        //}

        //public List<Resource> Resources
        //{
        //    get
        //    {
        //        var resourceIds = UnitWork.Find<Relevance>(
        //            u =>
        //                (u.FirstId == _user.Id && u.Key == Define.USERRESOURCE) ||
        //                (u.Key == Define.ROLERESOURCE && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
        //        return UnitWork.Find<Resource>(u => resourceIds.Contains(u.Id)).ToList();
        //    }
        //}

        //public List<Org> Orgs
        //{
        //    get
        //    {
        //        var orgids = UnitWork.Find<Relevance>(
        //            u =>
        //                (u.FirstId == _user.Id && u.Key == Define.USERORG) ||
        //                (u.Key == Define.ROLEORG && _userRoleIds.Contains(u.FirstId))).Select(u => u.SecondId);
        //        return UnitWork.Find<Org>(u => orgids.Contains(u.Id)).ToList();
        //    }
        //}

        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                _userRoleIds = UnitWork.Find<Relevance>(u => u.FirstId == _user.Id && u.Key == Define.USERROLE).Select(u => u.SecondId).ToList();
            }
        }

        //用户角色

        public NormalAuthStrategy(IUnitWork unitWork, IRepository<User> repository) : base(unitWork, repository)
        //public NormalAuthStrategy(IUnitWork unitWork) : base(unitWork)
        {
        }
    }
}
