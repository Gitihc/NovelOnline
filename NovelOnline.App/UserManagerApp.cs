using OpenAuth.App.Response;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App
{
   public class UserManagerApp:BaseApp<User>
    {
        public UserManagerApp(IUnitWork unitWork,IRepository<User> repository):base(unitWork, repository)
        {

        }

        public void AddOrUpdate(UserView view)
        {
            User user = view;
            if (string.IsNullOrEmpty(view.Id))
            {
                if (UnitWork.IsExist<User>(u => u.Account == view.Account))
                {
                    throw new Exception("用户账号已存在");
                }
                
                user.CreateDate = DateTime.Now;
                user.Password = user.Account; //初始密码与账号相同
                UnitWork.Add(user);
                view.Id = user.Id;   //要把保存后的ID存入view
            }
            else
            {
                UnitWork.Update<User>(u => u.Id == view.Id, u => new User
                {
                    Account = user.Account,
                    Name = user.Name,
                    Sex = user.Sex,
                });
            }
            UnitWork.Save();
        }
    }
}
