using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public static class SendData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new HLDBContext(serviceProvider.GetRequiredService<DbContextOptions<HLDBContext>>()))
            {
                var ObjUser = context.Set<User>();
                if (ObjUser.Any())
                {
                    return;
                }
                //增加一个超级管理员用户
                ObjUser.Add(
                     new User
                     {
                         Id = Guid.Empty.ToString(),
                         Account = "admin",
                         Password = "123456", //暂不进行加密
                         Name = "超级管理员",
                         Sex = 1
                     }
                );
                string MenuId = Guid.NewGuid().ToString();
                context.Set<Menu>().AddRange(
                  new Menu
                  {
                      Id = MenuId,
                      Name = "基础配置",
                      Sort = 0,
                      ParentId = Guid.Empty.ToString()
                  },
                   new Menu
                   {
                       Id = Guid.NewGuid().ToString(),
                       Name = "菜单管理",
                       Sort = 0,
                       ParentId = MenuId
                   },
                    new Menu
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "用户管理",
                        Sort = 0,
                        ParentId = MenuId
                    }
                   );
                context.SaveChanges();
            }
        }
    }
}
