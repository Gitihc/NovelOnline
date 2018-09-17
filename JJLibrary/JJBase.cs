using BaseLibrary;
using HtmlAgilityPack;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JJLibrary
{
    public class JJBase : INVBase
    {
        private string fileName = "Novel.txt";
        private Encoding baseEncoding = Encoding.GetEncoding("gb2312");

        public string GetNovelName()
        {
            return fileName;
        }

        public string GetSpecRegex()
        {
            return @"http://www\.jjwxc\.net\/onebook\.php\?novelid=\d+$";
        }

        public string GetWebName()
        {
            return "www.jjwxc.net";
        }

        public string GetChapterContent(NVChapter nVChapter)
        {
            string html = HttpHelper.GetString(nVChapter.Link, baseEncoding);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.GetElementbyId("oneboolt");
            HtmlNode htmlNode = node.SelectSingleNode("//*[@class=\"noveltext\"]");
            StringBuilder cSb = new StringBuilder("");
            cSb.AppendLine();
            cSb.Append(nVChapter.Title).AppendLine();
            foreach (HtmlNode hn in htmlNode.ChildNodes)
            {
                if (hn.Name == "#text")
                {
                    string s = hn.OuterHtml.Trim();
                    if (s.Length > 0)
                    {
                        cSb.Append(s).AppendLine();
                    }
                }
            }
            return cSb.ToString();
        }

        public List<NVChapter> GetList(string url)
        {
            List<NVChapter> list = new List<NVChapter>();
            string html = HttpHelper.GetString(url, baseEncoding);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.GetElementbyId("oneboolt");
            if (node == null)
                return list;
            HtmlNode fn = node.SelectSingleNode("//*[@itemprop=\"articleSection\"]");
            if (fn == null)
                return list;
            string NovelName = fn.InnerHtml.Trim();
            fileName = string.IsNullOrEmpty(NovelName) ? fileName : NovelName;    // 获取文件名称

            // Dim regStr1 As String = "<a([^>]*?)href=['""]([^'""]*?)['""]([^>]*?)>(.*?)<\/a>"
            string regStr1 = @"<a[^>]+href=\s*(?:'(?<href>[^']+)'|""(?<href>[^""]+)""|(?<href>[^>\s]+))\s*[^>]*>(?<text>.*?)</a>";
            string regStr2 = @"http:\/\/www.jjwxc.net\/onebook.php\?novelid=\d*&chapterid=\d*";
            MatchCollection matchs = Regex.Matches(html, regStr1);
            int index = 1;
            foreach (Match match in matchs)
            {
                Match rzt = Regex.Match(match.Value, regStr2);
                if (rzt.Success)
                {
                    var obj = new NVChapter();
                    obj.Title = match.Groups["text"].Value;
                    obj.Link = match.Groups["href"].Value;
                    obj.Sort = index;
                    index += 1;
                    list.Add(obj);
                }
            }
            return list;
        }

        public NVNovel GetNovelInfo(string url)
        {
            string html = HttpHelper.GetString(url, baseEncoding);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode node = doc.GetElementbyId("oneboolt");
            if (node == null)
                return null;
            HtmlNode name = node.SelectSingleNode("//*[@itemprop=\"articleSection\"]");
            HtmlNode author = node.SelectSingleNode("//*[@itemprop=\"author\"]");
            string bName = string.Empty;
            string bAuthor = string.Empty;
            if (name != null)
            {
                bName = name.InnerText;
            }
            if (author != null)
            {
                bAuthor = author.InnerText;
            }
            return new NVNovel { Id = Guid.NewGuid().ToString(), WebsiteId = "", Name = bName, Author = bAuthor, Link = url };
        }
    }
}
