using BaseLibrary;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NovelOnline.App
{
    public class WebsiteNovelApp : BaseApp<WebsiteNovel>
    {
        private readonly NovelManagerApp _novelManagerApp;

        public WebsiteNovelApp(IUnitWork unitWork,
                                IRepository<WebsiteNovel> repository,
                                NovelManagerApp novelManagerApp) : base(unitWork, repository)
        {
            _novelManagerApp = novelManagerApp;
        }

        public List<WebsiteNovel> GetWebsiteNovelList(string websiteId)
        {
            var listWebsiteNovel = new List<WebsiteNovel>();
            if (!IsExistWebsiteNovelTable(websiteId)) return listWebsiteNovel;
            var sqlStr = string.Format("SELECT * FROM [{0}] ORDER BY CreateDate;", websiteId);
            listWebsiteNovel = Repository.WebsiteNovelQueryFromSql(sqlStr).ToList();
            return listWebsiteNovel;
        }

        public bool AddRandWebsitNovel(string novelId, List<WebsiteNovel> listChapter)
        {
            CreateWebsiteNovelTable(novelId);
            var sqlSb = new StringBuilder();
            foreach (var chapter in listChapter)
            {
                sqlSb.AppendFormat(" INSERT INTO [{0}](Id ,WebSiteId ,Name ,Author ,OriginLink ,CreateDate ,State) ", novelId);
                sqlSb.AppendFormat("VALUES(");
                sqlSb.AppendFormat("'{0}'", chapter.Id);
                sqlSb.AppendFormat(",'{0}'", chapter.WebSiteId);
                sqlSb.AppendFormat(",'{0}'", chapter.Author);
                sqlSb.AppendFormat(",'{0}'", chapter.Name);
                sqlSb.AppendFormat(",'{0}'", chapter.OriginLink);
                sqlSb.AppendFormat(")");
            }
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public bool AddWebsitNovel(string WebSiteId, WebsiteNovel novel)
        {
            CreateWebsiteNovelTable(WebSiteId);
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF NOT EXISTS( ");
            sqlSb.AppendFormat("    SELECT * ");
            sqlSb.AppendFormat("    FROM [{0}] ", WebSiteId);
            sqlSb.AppendFormat("    WHERE Name='{0}' AND OriginLink='{1}' ", novel.Name, novel.OriginLink);
            sqlSb.AppendFormat(" ) ");
            sqlSb.AppendFormat(" INSERT INTO [{0}](Id ,WebSiteId ,Name ,Author ,OriginLink) ", WebSiteId);
            sqlSb.AppendFormat("VALUES(");
            sqlSb.AppendFormat("'{0}'", novel.Id);
            sqlSb.AppendFormat(",'{0}'", novel.WebSiteId);
            sqlSb.AppendFormat(",'{0}'", novel.Name);
            sqlSb.AppendFormat(",'{0}'", novel.Author);
            sqlSb.AppendFormat(",'{0}'", novel.OriginLink);
            sqlSb.AppendFormat(")");
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public bool CreateWebsiteNovelTable(string novelId)
        {
            if (IsExistWebsiteNovelTable(novelId)) return true;
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF NOT EXISTS(SELECT * FROM SYSOBJECTS WHERE NAME='[{0}]' AND TYPE='U') ", novelId);
            sqlSb.AppendFormat(" CREATE TABLE [{0}]( ", novelId);
            sqlSb.AppendFormat(" [Id] [varchar](64) PRIMARY KEY, ");
            sqlSb.AppendFormat(" [WebSiteId] [varchar](64) NULL, ");
            sqlSb.AppendFormat(" [Author] [varchar](100) NULL, ");
            sqlSb.AppendFormat(" [Name] [varchar](100) NULL, ");
            sqlSb.AppendFormat(" [OriginLink] [varchar](200) NULL, ");
            sqlSb.AppendFormat(" [CreateDate] [datetime] NULL DEFAULT(getdate()), ");
            sqlSb.AppendFormat(" [State] [int] NULL DEFAULT(0) ");
            sqlSb.AppendFormat(");");
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }
        /// <summary>
        /// 判断章节表是否存在
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public bool IsExistWebsiteNovelTable(string novelId)
        {
            var judgeSb = new StringBuilder();
            judgeSb.AppendFormat("if exists ( select * from sysobjects where name = '{0}' and type = 'U') select 1 else  select 0", novelId);
            var result = Repository.ExecuteScalar(judgeSb.ToString());
            return (int)result == 1 ? true : false;
        }

        public bool DropNovelTable(string novelId)
        {
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF OBJECT_ID('{0}', 'U') IS NOT NULL ", novelId);
            sqlSb.AppendFormat(" Drop Table [{0}]; ", novelId);
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public bool RemoveWebsiteNovel(string websiteId, string[] ids)
        {
            if (!IsExistWebsiteNovelTable(websiteId) || ids.Count() == 0) return false;
            var sqlSb = new StringBuilder();
            foreach (var id in ids)
            {
                sqlSb.AppendFormat("Delete [{0}] Where Id='{1}';", websiteId, id);
            }
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public WebsiteNovel GetObject(string websiteid, string id)
        {
            var sqlStr = string.Format(" Select * From [{0}] Where Id='{1}' ", websiteid, id);
            return Repository.WebsiteNovelQueryFromSql(sqlStr).FirstOrDefault();
        }

        public bool AddWebsiteNovelInMyNovel(User user, string websiteId, string id)
        {
            var websiteNovel = GetObject(websiteId, id);
            if (websiteNovel == null) return false;
            return _novelManagerApp.GetWebsiteNovel(user, websiteNovel.OriginLink, websiteNovel.Name);
        }

    }
}
