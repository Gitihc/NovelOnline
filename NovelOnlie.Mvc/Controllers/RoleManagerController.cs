﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NovelOnlie.Mvc.Controllers
{
    public class RoleManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}