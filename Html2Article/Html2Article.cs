using System;


#region 程序集 Html2Article, Version=1.13.0.5, Culture=neutral, PublicKeyToken=null
// C:\Users\fan\.nuget\packages\html2article\1.17.0\lib\40\Html2Article.dll
// Decompiled with ICSharpCode.Decompiler 3.1.0.3652
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace StanSoft
{
    /// <summary>
    /// 解析Html页面的文章正文内容,基于文本密度的HTML正文提取类
    /// Date:   2012/12/30
    /// Update: 
    ///     2013/7/10   优化文章头部分析算法，优化
    ///
    /// </summary>
    public class Html2Article
    {
        private static readonly string[][] _filters = new string[3][]
        {
            new string[2]
            {
                "(?is)<script.*?>.*?</script>",
                ""
            },
            new string[2]
            {
                "(?is)<style.*?>.*?</style>",
                ""
            },
            new string[2]
            {
                "(?is)</a>",
                "</a>\n"
            }
        };

        private static bool _appendMode = false;

        private static int _depth = 6;

        private static int _limitCount = 180;

        private static int _headEmptyLines = 2;

        private static int _endLimitCharCount = 20;

        /// <summary>
        /// 是否使用追加模式，默认为false
        /// 使用追加模式后，会将符合过滤条件的所有文本提取出来
        /// </summary>
        public static bool AppendMode
        {
            get
            {
                return _appendMode;
            }
            set
            {
                _appendMode = value;
            }
        }

        /// <summary>
        /// 按行分析的深度，默认为6
        /// </summary>
        public static int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
            }
        }

        /// <summary>
        /// 字符限定数，当分析的文本数量达到限定数则认为进入正文内容
        /// 默认180个字符数
        /// </summary>
        public static int LimitCount
        {
            get
            {
                return _limitCount;
            }
            set
            {
                _limitCount = value;
            }
        }
        public static Func<char, bool> CS_0024_003C_003E9__CachedAnonymousMethodDelegate2;
        /// <summary>
        /// 从给定的Html原始文本中获取正文信息
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static unsafe Article GetArticle(string html)
        {
            //IL_0047: Unknown result type (might be due to invalid IL or missing references)
            //IL_004c: Expected O, but got Unknown
            //IL_0096: Unknown result type (might be due to invalid IL or missing references)
            string text = html;
            if (CS_0024_003C_003E9__CachedAnonymousMethodDelegate2 == null)
            {
                CS_0024_003C_003E9__CachedAnonymousMethodDelegate2 = new Func<char, bool>((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
            }
            if (Enumerable.Count<char>((IEnumerable<char>)text, CS_0024_003C_003E9__CachedAnonymousMethodDelegate2) < 10)
            {
                html = html.Replace(">", ">\n");
            }
            string text2 = "";
            string text3 = "(?is)<body.*?</body>";
            Match val = Regex.Match(html, text3);
            if (val.Success)
            {
                text2 = ((object)val).ToString();
            }
            string[][] filters = _filters;
            foreach (string[] array in filters)
            {
                text2 = Regex.Replace(text2, array[0], array[1]);
            }
            text2 = Regex.Replace(text2, "(<[^<>]+)\\s*\\n\\s*", new MatchEvaluator((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
            GetContent(text2, out string content, out string contentWithTags);
            Article article = new Article();
            article.Title = GetTitle(html);
            article.PublishDate = GetPublishDate(html);
            article.Content = content;
            article.ContentWithTags = contentWithTags;
            return article;
        }

        /// <summary>
        /// 格式化标签，剔除匹配标签中的回车符
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private static string FormatTag(Match match)
        {
            //IL_0000: Unknown result type (might be due to invalid IL or missing references)
            //IL_0005: Expected O, but got Unknown
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            StringBuilder val = new StringBuilder();
            string value = match.Value;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value.ToCharArray()[i];
                if (c != '\r' && c != '\n')
                {
                    val.Append(c);
                }
            }
            return ((object)val).ToString();
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string GetTitle(string html)
        {
            //IL_001b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0020: Expected O, but got Unknown
            //IL_002d: Unknown result type (might be due to invalid IL or missing references)
            //IL_0033: Unknown result type (might be due to invalid IL or missing references)
            //IL_004c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0051: Expected O, but got Unknown
            //IL_005e: Unknown result type (might be due to invalid IL or missing references)
            //IL_0064: Unknown result type (might be due to invalid IL or missing references)
            string text = "<title>[\\s\\S]*?</title>";
            string text2 = "<h1.*?>.*?</h1>";
            string text3 = "<.*?>";
            string text4 = "";

            Match val = Regex.Match(html, text, RegexOptions.IgnoreCase);
            if (val.Success)
            {
                text4 = Regex.Replace(val.Groups[0].Value, text3, "");
            }
            val = Regex.Match(html, text2, RegexOptions.IgnoreCase);
            if (val.Success)
            {
                string text5 = Regex.Replace(val.Groups[0].Value, text3, "");
                if (!string.IsNullOrEmpty(text5) && text4.StartsWith(text5))
                {
                    text4 = text5;
                }
            }
            return text4;
        }

        /// <summary>
        /// 获取文章发布日期
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static DateTime GetPublishDate(string html)
        {
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            //IL_001d: Expected O, but got Unknown
            //IL_0043: Unknown result type (might be due to invalid IL or missing references)
            //IL_004a: Unknown result type (might be due to invalid IL or missing references)
            //IL_0066: Unknown result type (might be due to invalid IL or missing references)
            //IL_007f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0084: Expected O, but got Unknown
            //IL_00b2: Unknown result type (might be due to invalid IL or missing references)
            //IL_00c7: Unknown result type (might be due to invalid IL or missing references)
            //IL_00d3: Unknown result type (might be due to invalid IL or missing references)
            string text = Regex.Replace(html, "(?is)<.*?>", "");
            Match val = Regex.Match(text, "((\\d{4}|\\d{2})(\\-|\\/)\\d{1,2}\\3\\d{1,2})(\\s?\\d{2}:\\d{2})?|(\\d{4}年\\d{1,2}月\\d{1,2}日)(\\s?\\d{2}:\\d{2})?", RegexOptions.IgnoreCase);
            DateTime result = new DateTime(1900, 1, 1);
            if (val.Success)
            {
                try
                {
                    string text2 = "";
                    for (int i = 0; i < val.Groups.Count(); i++)
                    {
                        text2 = val.Groups[i].Value;
                        if (!string.IsNullOrEmpty(text2))
                        {
                            break;
                        }
                    }
                    if (text2.Contains("年"))
                    {
                        StringBuilder val2 = new StringBuilder();
                        string text3 = text2;
                        for (int j = 0; j < text3.Length; j++)
                        {
                            char c = text3.ToCharArray()[j];
                            switch (c)
                            {
                                case '年':
                                case '月':
                                    val2.Append("/");
                                    break;
                                case '日':
                                    val2.Append(' ');
                                    break;
                                default:
                                    val2.Append(c);
                                    break;
                            }
                        }
                        text2 = ((object)val2).ToString();
                    }
                    result = Convert.ToDateTime(text2);
                }
                catch (Exception)
                {
                }
                if (result.Year < 1900)
                {
                    result = new DateTime(1900, 1, 1);
                }
            }
            return result;
        }

        /// <summary>
        /// 从body标签文本中分析正文内容
        /// </summary>
        /// <param name="bodyText">只过滤了script和style标签的body文本内容</param>
        /// <param name="content">返回文本正文，不包含标签</param>
        /// <param name="contentWithTags">返回文本正文包含标签</param>
        private static void GetContent(string bodyText, out string content, out string contentWithTags)
        {
            //IL_005f: Unknown result type (might be due to invalid IL or missing references)
            //IL_0064: Expected O, but got Unknown
            //IL_0066: Unknown result type (might be due to invalid IL or missing references)
            //IL_006b: Expected O, but got Unknown
            //IL_0115: Unknown result type (might be due to invalid IL or missing references)
            //IL_0121: Unknown result type (might be due to invalid IL or missing references)
            //IL_0157: Unknown result type (might be due to invalid IL or missing references)
            //IL_0163: Unknown result type (might be due to invalid IL or missing references)
            string[] array = null;
            string[] array2 = null;
            array = bodyText.Split(new char[1]
            {
                '\n'
            });
            array2 = new string[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                text = Regex.Replace(text, "(?is)</p>|<br.*?/>", "[crlf]");
                array2[i] = Regex.Replace(text, "(?is)<.*?>", "").Trim();
            }
            StringBuilder val = new StringBuilder();
            StringBuilder val2 = new StringBuilder();
            int num = 0;
            int num2 = -1;
            for (int j = 0; j < array2.Length - _depth; j++)
            {
                int num3 = 0;
                for (int k = 0; k < _depth; k++)
                {
                    num3 += array2[j + k].Length;
                }
                if (num2 == -1)
                {
                    if (num > _limitCount && num3 > 0)
                    {
                        int num4 = 0;
                        for (int num5 = j - 1; num5 > 0; num5--)
                        {
                            num4 = (string.IsNullOrEmpty(array2[num5]) ? (num4 + 1) : 0);
                            if (num4 == _headEmptyLines)
                            {
                                num2 = num5 + _headEmptyLines;
                                break;
                            }
                        }
                        if (num2 == -1)
                        {
                            num2 = j;
                        }
                        for (int l = num2; l <= j; l++)
                        {
                            val.Append(array2[l]);
                            val2.Append(array[l]);
                        }
                    }
                }
                else
                {
                    if (num3 <= _endLimitCharCount && num < _endLimitCharCount)
                    {
                        if (!_appendMode)
                        {
                            break;
                        }
                        num2 = -1;
                    }
                    val.Append(array2[j]);
                    val2.Append(array[j]);
                }
                num = num3;
            }
            string text2 = ((object)val).ToString();
            content = text2.Replace("[crlf]", Environment.NewLine);
            content = HttpUtility.HtmlDecode(content);
            contentWithTags = ((object)val2).ToString();
        }
    }
}
