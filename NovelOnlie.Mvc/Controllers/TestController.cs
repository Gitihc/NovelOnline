using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;

namespace NovelOnlie.Mvc.Controllers
{
    public class TestController : Controller
    {
        private ModuleManagerApp _app;

        public TestController(ModuleManagerApp app)
        {
            _app = app;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetLeftMenu()
        {
            var moduleTree = _app.GetAllModules().GenerateTree(u => u.Id, u => u.ParentId, Guid.Empty.ToString());
            return Json(moduleTree);
            //return null;
        }

        public IActionResult GetAllModules()
        {
            var allModules = _app.GetAllModules();
            return Json(new { code = 0, msg = "", count = allModules.Count, data = allModules });
        }

        public IActionResult GetTreeGridOfAllModules()
        {
            var allModules = _app.GetAllModules();
            return Json(new { flag = 1, msg = "", total = allModules.Count, rows = allModules });
        }
    }
}