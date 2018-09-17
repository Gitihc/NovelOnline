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

                WebSearchHelper.SpecRegStr = objNVB.GetSpecRegex();
                WebSearchHelper.GetNovelInfo += objNVB.GetNovelInfo;
                WebSearchHelper.Search(url);

                string guid = Guid.NewGuid().ToString();
                Repository.Add(new Website { Id = guid, Name = "新的搜索任务", OriginLink = url, CreatorId = user.Id });
                ThreadPool.QueueUserWorkItem(x =>
                {
                    try
                    {
                        var websiteNovelApp = AutofacExt.GetFromFac<WebsiteNovelApp>();
                        while (true)
                        {
                            if (WebSearchHelper.IsAllDone)
                            {
                                Repository.Update(m => m.Id == guid, n => new Website { State = 1 });
                                break;
                            }
                            Thread.Sleep(100);

                            if (WebSearchHelper.WebSiteBookInfoQueue.Count > 0)
                            {
                                NVNovel nVNovel = (NVNovel)WebSearchHelper.WebSiteBookInfoQueue.Dequeue();
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
                        throw ex;
                    }
                });
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
            return Repository.GetAll();
        }
    }
}
