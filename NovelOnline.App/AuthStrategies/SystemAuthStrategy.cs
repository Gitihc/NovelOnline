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
    /// 领域服务
    /// <para>超级管理员权限</para>
    /// <para>超级管理员使用guid.empty为ID，可以根据需要修改</para>
    /// </summary>
    public class SystemAuthStrategy : BaseApp<User>, IAuthStrategy
    {
        protected User _user;

        public List<ModuleView> Modules
        {
            get
            {
                var modules = (from module in UnitWork.Find<Module>(null)
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
                foreach (var module in modules)
                {
                    module.Elements = UnitWork.Find<ModuleElement>(u => u.ModuleId == module.Id).ToList();
                }
                return modules.OrderBy(x => x.Sort).ToList();
            }
        }

        //public List<Role> Roles
        //{
        //    get { return UnitWork.Find<Role>(null).ToList(); }
        //}

        //public List<Resource> Resources
        //{
        //    get { return UnitWork.Find<Resource>(null).ToList(); }
        //}

        //public List<Org> Orgs
        //{
        //    get { return UnitWork.Find<Org>(null).ToList(); }
        //}

        public User User
        {
            get { return _user; }
            set   //禁止外部设置
            {
                throw new Exception("超级管理员，禁止设置用户");
            }
        }

        public SystemAuthStrategy(IUnitWork unitWork, IRepository<User> repository) : base(unitWork, repository)
        //public SystemAuthStrategy(IUnitWork unitWork) : base(unitWork)
        {
            _user = new User
            {
                Account = "System",
                Name = "超级管理员",
                Id = Guid.Empty.ToString()
            };
        }
    }
}
