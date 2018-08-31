using NovelOnline.App.SSO;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App.Interface
{
    public interface IAuth
    {
        /// <summary>
        /// 检验token是否有效
        /// </summary>
        /// <param name="token">token值</param>
        /// <param name="otherInfo"></param>
        /// <returns></returns>
        bool CheckLogin(string token = "", string otherInfo = "");
        AuthStrategyContext GetCurrentUser(string otherInfo = "");
        string GetUserName(string otherInfo = "");
        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="appKey">登录的应用appkey</param>
        /// <param name="username">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        LoginResult Login(string appKey, string username, string pwd);
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        bool Logout();
    }
}
