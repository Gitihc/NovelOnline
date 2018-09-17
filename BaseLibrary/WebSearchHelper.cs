using Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BaseLibrary
{
    public static class WebSearchHelper
    {

        public struct UrlGroup
        {
            /// <summary>
            /// 地址
            /// </summary>
            public string Url;
            /// <summary>
            /// 当前搜索深度
            /// </summary>
            public int DepathNumber;
        }

        public static ReaderWriterLockSlim WriteTxtLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 最大线程数（默认5个，最多20）
        /// </summary>
        public static int _MaxThreadCount = 5;
        /// <summary>
        /// 已访问的url
        /// </summary>
        public static Queue _DoneQueue;
        /// <summary>
        /// 未访问的url
        /// </summary>
        public static Queue _UrlQueue;
        /// <summary>
        /// 特殊url
        /// </summary>
        public static Queue _SpecUrlQueue;
        public static Queue _WebSiteBookInfoQueue;
        /// <summary>
        /// 搜索深度
        /// </summary>
        public static int MaxSearchDepth = 6;

        public static string BaseURL;
        public static string SpecRegStr = string.Empty;
        private static object UrlQueueLock = new object();
        private static object DoneQueueLock = new object();
        private static object SpecQueueLock = new object();
        private static object WebSiteBookInfoQueueLock = new object();
        public static bool IsAllDone = false; // 结束标识


        public static Queue DoneQueue
        {
            get
            {
                if (_DoneQueue == null)
                    _DoneQueue = new Queue();
                return _DoneQueue;
            }
        }
        public static Queue UrlQueue
        {
            get
            {
                if (_UrlQueue == null)
                    _UrlQueue = new Queue();
                return _UrlQueue;
            }
        }
        public static Queue SpecUrlQueue
        {
            get
            {
                if (_SpecUrlQueue == null)
                    _SpecUrlQueue = new Queue();
                return _SpecUrlQueue;
            }
        }
        public static Queue WebSiteBookInfoQueue
        {
            get
            {
                if (_WebSiteBookInfoQueue == null)
                    _WebSiteBookInfoQueue = new Queue();
                return _WebSiteBookInfoQueue;
            }
        }

        /// <summary>
        /// 最大线程数（默认5个，最多20）
        /// </summary>
        public static int MaxThreadCount
        {
            get
            {
                return _MaxThreadCount;
            }
            set
            {
                _MaxThreadCount = value < 0 ? 5 : (value > 20 ? 20 : value);
            }
        }

        public static void Search(string url)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(BaseURL)) return;
            if (!string.IsNullOrEmpty(url))
            {
                BaseURL = url;
            }

            HandUrl(new UrlGroup() { Url = BaseURL, DepathNumber = 0 }, 0);

            Thread.Sleep(1000);

            //for (int i = 0; i < MaxThreadCount; i++)
            //{
                ThreadPool.QueueUserWorkItem(x =>
                {
                    try
                    {
                        while (true)
                        {
                            if (IsAllDone) break;
                            if (UrlQueue.Count == 0 && SpecUrlQueue.Count == 0 && UrlQueue.Count == 0) break;
                            if (UrlQueue.Count == 0) Thread.Sleep(10000);
                            UrlGroup urlGroup = (UrlGroup)UrlQueue.Dequeue();
                            HandUrl(urlGroup, urlGroup.DepathNumber);
                            Thread.Sleep(1000);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            //}
            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    while (true)
                    {
                        if (IsAllDone && SpecUrlQueue.Count == 0) break;
                        if (SpecUrlQueue.Count == 0) Thread.Sleep(10000);
                        HandSpecialUrl(SpecUrlQueue.Dequeue().ToString());
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        public static event GetChapterInfoEventHandler GetNovelInfo;

        public delegate NVNovel GetChapterInfoEventHandler(string url);

        public static void HandSpecialUrl(string url)
        {
            try
            {
                string jsonStr = string.Empty;
                var obj = GetNovelInfo(url);
                if (obj != null)
                {
                    lock (WebSiteBookInfoQueueLock)
                    {
                        WebSiteBookInfoQueue.Enqueue(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrorMessage:    " + ex.Message);
                Console.WriteLine("StackTraceMessage:    " + ex.StackTrace.ToString());
            }
        }

        public static void HandUrl(UrlGroup urlGroup, int depath)
        {
            if (DoneQueue.Contains(urlGroup)) return;
            string html = HttpHelper.GetString(urlGroup.Url, Encoding.Default);
            lock (DoneQueueLock)
            {
                DoneQueue.Enqueue(urlGroup);
            }
            //1、获取页面所有url
            List<string> listAllUrl = MatchDomainURL(html);
            //2、过滤：去掉外站、js、图片等url
            string[] extArray = new string[] { ".jpg", ".png", ".gif", ".js" };
            List<string> mlstUrl = new List<string>();
            foreach (string url in listAllUrl)
            {
                var tmpurl = url.ToLower();
                bool isfile = false;
                foreach (string ext in extArray)
                {
                    if (tmpurl.Contains(ext))
                    {
                        isfile = true;
                        break;
                    }
                }
                //匹配特殊url
                SepcialMatch(url);

                if (!isfile && !DoneQueue.Contains(url) && !UrlQueue.Contains(url))
                {
                    mlstUrl.Add(url);
                }
            }
            if (depath + 1 > MaxSearchDepth) return; //达到最大搜索深度

            lock (UrlQueueLock)
            {
                foreach (string url in mlstUrl)
                {
                    UrlQueue.Enqueue(new UrlGroup() { Url = url, DepathNumber = depath + 1 });
                    if (UrlQueue.Count > 10000)
                    {
                        Thread.Sleep(3000);
                    }
                    else if (UrlQueue.Count > 50000)
                    {
                        Thread.Sleep(10000);
                    }
                }
            }
        }

        // 匹配特殊url
        public static void SepcialMatch(string url)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(SpecRegStr) || SpecUrlQueue.Contains(url))
                return;
            Regex reg = new Regex(SpecRegStr);
            Match m = reg.Match(url);
            if (m.Success)
            {
                lock ((SpecQueueLock))
                    SpecUrlQueue.Enqueue(url);
            }
        }

        // 获取站内url
        public static List<string> MatchDomainURL(string html)
        {
            string regStr = string.Format("(https?|ftp|file)://{0}/[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]", GetDomain());
            Regex defReg = new Regex(regStr);
            MatchCollection matchs = defReg.Matches(html);
            List<string> lstUrl = new List<string>();
            foreach (Match m in matchs)
            {
                if (m.Success)
                    lstUrl.Add(m.Value);
            }
            return lstUrl;
        }

        private static string GetDomain()
        {
            if (string.IsNullOrEmpty(BaseURL))
            {
                throw new Exception("BaseURL is nothing!");
            }
            string regStr = @"(\w+\.){2}\w+";
            return new Regex(regStr).Match(BaseURL).Value;
        }

    }
}
