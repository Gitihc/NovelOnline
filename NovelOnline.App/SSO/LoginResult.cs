using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace NovelOnline.App.SSO
{
    public class LoginResult : Response<string>
    {
        public string ReturnUrl;
        public string Token;
    }
}
