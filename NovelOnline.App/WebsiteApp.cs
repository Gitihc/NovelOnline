using AutoMapper;
using BaseLibrary;
using Infrastructure;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NovelOnline.App
{
    public class WebsiteApp : BaseApp<Website>
    {
        public WebsiteApp(IUnitWork unitWork, IRepository<Website> repository) : base(unitWork, repository)
        {

        }

        public bool Search(User user, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var count = Repository.Find(x => x.OriginLink == url).Count();
                if (count > 0) return false;
                var objNVB = NVHelper.NVBaseObject(url);
                if (objNVB == null) return false;
                ThreadPool.QueueUserWorkItem(x =>
                {
                    var websiteNovelApp = AutofacExt.GetFromFac<WebsiteNovelApp>();
                    var websiteApp = AutofacExt.GetFromFac<WebsiteApp>();
                    var webSearchHelper = new WebSearchHelper();
                    webSearchHelper.SpecRegStr = objNVB.GetSpecRegex();
                    webSearchHelper.GetNovelInfo += objNVB.GetNovelInfo;
                    webSearchHelper.Search(url);

                    string guid = Guid.NewGuid().ToString();
                    websiteApp.Repository.Add(new Website { Id = guid, Name = "新的搜索任务", OriginLink = url, CreatorId = user.Id });
                    var websiteId = guid;

                    ThreadPool.QueueUserWorkItem(m =>
                    {
                        CheckWebsiteState(websiteId, websiteApp, ref webSearchHelper);
                    });

                    try
                    {
                        websiteApp.UpdateState(websiteId, 1);
                        while (true)
                        {
                            if (webSearchHelper.IsAllDone)
                            {
                                websiteApp.UpdateState(websiteId, 2);
                                break;
                            }
                            Thread.Sleep(100);

                            if (webSearchHelper.WebSiteBookInfoQueue.Count > 0)
                            {
                                NVNovel nVNovel = (NVNovel)webSearchHelper.WebSiteBookInfoQueue.Dequeue();
                                if (nVNovel != null)
                                {
                                    nVNovel.WebsiteId = websiteId;
                                    var websiteNovel = Mapper.Map<WebsiteNovel>(nVNovel);
                                    websiteNovelApp.AddWebsitNovel(websiteId, websiteNovel);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        websiteApp.UpdateState(websiteId, -1);
                        //throw ex;
                    }
                }, objNVB);
                return true;
            }
            return false;
        }

        public bool DeleteWebsite(string websiteId)
        {
            //var sqlStr = string.Format("DELETE [{0}] WHERE WebSiteId='{0}' ", websiteId);
            //Repository.ExecuteSql(sqlStr);
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF OBJECT_ID('{0}', 'U') IS NOT NULL ", websiteId);
            sqlSb.AppendFormat(" Drop Table [{0}]; ", websiteId);
            Repository.ExecuteSql(sqlSb.ToString());
            Repository.Delete(x => x.Id == websiteId);
            return true;
        }

        public IQueryable<Website> GetWebsiteList()
        {
            return Repository.GetAll().OrderByDescending(x => x.CreateDate);
        }

        public bool Update(Website website)
        {
            Repository.Update(website);
            return true;
        }

        public bool UpdateState(string websiteId, int state)
        {
            Repository.Update(x => x.Id == websiteId, n => new Website { State = state });
            return true;
        }

        public bool ReSearch(string websiteId)
        {
            var website = Repository.FindSingle(x => x.Id == websiteId);
            if (website != null)
            {
                var objNVB = NVHelper.NVBaseObject(website.OriginLink);
                if (objNVB == null) return false;

                ThreadPool.QueueUserWorkItem(x =>
                {
                    var websiteNovelApp = AutofacExt.GetFromFac<WebsiteNovelApp>();
                    var websiteApp = AutofacExt.GetFromFac<WebsiteApp>();
                    var webSearchHelper = new WebSearchHelper();
                    webSearchHelper.SpecRegStr = objNVB.GetSpecRegex();
                    webSearchHelper.GetNovelInfo += objNVB.GetNovelInfo;
                    webSearchHelper.Search(website.OriginLink);
                    var guid = website.Id;

                    ThreadPool.QueueUserWorkItem(m =>
                    {
                        CheckWebsiteState(websiteId, websiteApp, ref webSearchHelper);
                    });

                    try
                    {
                        websiteApp.UpdateState(guid, 1);
                        while (true)
                        {
                            if (webSearchHelper.IsAllDone)
                            {
                                websiteApp.UpdateState(guid, 2);
                                break;
                            }
                            Thread.Sleep(100);

                            if (webSearchHelper.WebSiteBookInfoQueue.Count > 0)
                            {
                                NVNovel nVNovel = (NVNovel)webSearchHelper.WebSiteBookInfoQueue.Dequeue();
                                if (nVNovel != null)
                                {
                                    nVNovel.WebsiteId = guid;
                                    var websiteNovel = Mapper.Map<WebsiteNovel>(nVNovel);
                                    websiteNovelApp.AddWebsitNovel(guid, websiteNovel);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        websiteApp.UpdateState(guid, -1);
                        //throw ex;
                    }
                });
                return true;
            }
            return false;
        }

        private void CheckWebsiteState(string websiteId, WebsiteApp websiteApp, ref WebSearchHelper webSearchHelper)
        {
            while (true)
            {
                var website = websiteApp.Repository.FindSingle(x => x.Id == websiteId);
                if (website == null) break;
                if (website.State == 2) break;
                if (website.State <= 0) break;
                if (website.State == 3) { webSearchHelper.IsPause = true; break; }
                if (website.State == 4) { webSearchHelper.IsStop = true; break; }
                if (website.State == 1) { Thread.Sleep(10000); }
            }
        }
    }
}
