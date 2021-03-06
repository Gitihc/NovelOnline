﻿using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;
using System;

namespace NovelOnlie.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IAuth authUtil, IHostingEnvironment hostingEnvironment) : base(authUtil,hostingEnvironment)
        {
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }

        //public IActionResult GetLeftMenu()
        //{
        //    var moduleTree = _app.GetAllModules().GenerateTree(u => u.Id, u => u.ParentId, Guid.Empty.ToString());
        //    return Json(moduleTree);
        //}
    }
}