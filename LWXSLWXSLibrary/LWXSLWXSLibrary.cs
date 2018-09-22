using BaseLibrary;
using HtmlAgilityPack;
using Infrastructure;
using StanSoft;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LWXSLWXSLibrary
{
    public class LWXSLWXSLibrary : INVBase
    {
        private string fileName = "Novel.txt";
        private Encoding baseEncoding = Encoding.GetEncoding("gbk");
        public string GetChapterContent(NVChapter nVChapter)
        {
            var html = HttpHelper.GetString(nVChapter.Link, baseEncoding);
            Article article = Html2Article.GetArticle(html);
            var cSb = new StringBuilder();
            cSb.AppendLine();
            cSb.Append(nVChapter.Title).AppendLine();
            cSb.Append(article.Content).AppendLine();
            return cSb.ToString();
        }

        public List<NVChapter> GetList(string url)
        {
            List<NVChapter> list = new List<NVChapter>();
            string html = HttpHelper.GetString(url, baseEncoding);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nameNode = doc.DocumentNode.SelectSingleNode("//h1");
            if (nameNode == null)
                return list;
            string NovelName = nameNode.InnerHtml.Trim();
            fileName = string.IsNullOrEmpty(NovelName) ? fileName : NovelName;    // 获取文件名称


            var chapterListNode = doc.DocumentNode.SelectSingleNode("//ul[starts-with(@class,'list-group list-charts')]");
            if (chapterListNode == null) return list;
            // Dim regStr1 As String = "<a([^>]*?)href=['""]([^'""]*?)['""]([^>]*?)>(.*?)<\/a>"
            string regStr1 = @"<a([^>]*?)href=['""]((/(\d*))*\.html)['""]([^>]*?)>(.*?)</a>";

            MatchCollection matchs = Regex.Matches(chapterListNode.InnerHtml, regStr1);
            int index = 1;
            foreach (Match match in matchs)
            {
                if (match.Success)
                {
                    var link = match.Groups[2].Value;
                    var title = match.Groups[6].Value;

                    var obj = new NVChapter();
                    obj.Title = title;
                    obj.Link = String.Format("http://www.lwxslwxs.com{0}", link);
                    obj.Sort = index;
                    index += 1;
                    list.Add(obj);
                }
            }
            return list;
        }

        public NVNovel GetNovelInfo(string url)
        {
            var bName = string.Empty;
            var bAuthor = string.Empty;
            List<NVChapter> list = new List<NVChapter>();
            string html = HttpHelper.GetString(url, baseEncoding);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var nameNode = doc.DocumentNode.SelectSingleNode("//h1");
            var authorNode = doc.DocumentNode.SelectSingleNode("//h3");
            if (nameNode != null)
            {
                bName = nameNode.InnerHtml.Trim();
            }
            if (authorNode != null)
            {
                bAuthor = authorNode.InnerText.Trim();
            }

            return new NVNovel { Id = Guid.NewGuid().ToString(), WebsiteId = "", Name = bName, Author = bAuthor, Link = url };
        }

        /// <summary>
        /// 获取书籍名称
        /// </summary>
        /// <returns></returns>
        public string GetNovelName()
        {
            return fileName;
        }
        /// <summary>
        /// 全站搜索正则匹配书籍地址
        /// </summary>
        /// <returns></returns>
        public string GetSpecRegex()
        {
            return @"http://www\.lwxslwxs\.com/\d+/\d+/$";
        }

        public string GetWebName()
        {
            return "www.lwxslwxs.com";
        }
    }
}
