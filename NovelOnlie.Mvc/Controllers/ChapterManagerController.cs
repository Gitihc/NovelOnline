using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NovelOnline.App;
using NovelOnline.App.Interface;

namespace NovelOnlie.Mvc.Controllers
{
    public class ChapterManagerController : BaseController
    {
        private readonly ChapterManagerApp _app;
        public ChapterManagerController(IAuth authUtil, ChapterManagerApp app) : base(authUtil)
        {
            _app = app;
        }
        public IActionResult ChapterView()
        {
            return View();
        }

        public string GetChapterContent(string novelId, string chapterId)
        {
            return _app.GetChapterContent(novelId, chapterId);
        }


    }
}