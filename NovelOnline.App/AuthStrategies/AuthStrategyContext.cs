using OpenAuth.App.Response;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App
{
    /// <summary>
    ///  授权策略上下文，一个典型的策略模式
    /// </summary>
    public class AuthStrategyContext
    {
        private readonly IAuthStrategy _strategy;
        public AuthStrategyContext(IAuthStrategy strategy)
        {
            this._strategy = strategy;
        }

        public User User
        {
            get { return _strategy.User; }
        }

        public List<ModuleView> Modules
        {
            get { return _strategy.Modules; }
        }

        //public List<Role> Roles
        //{
        //    get { return _strategy.Roles; }
        //}

        //public List<Resource> Resources
        //{
        //    get { return _strategy.Resources; }
        //}

        //public List<Org> Orgs
        //{
        //    get { return _strategy.Orgs; }
        //}

    }
}
