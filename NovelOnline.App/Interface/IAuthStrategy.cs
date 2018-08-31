using OpenAuth.App.Response;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App
{
    public interface IAuthStrategy
    {
        List<ModuleView> Modules { get; }

        //List<Role> Roles { get; }

        //List<Resource> Resources { get; }

        //List<Org> Orgs { get; }

        User User
        {
            get; set;
        }

    }
}
