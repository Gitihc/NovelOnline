using Infrastructure;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NovelOnline.App
{
    public class ChapterManagerApp : BaseApp<Chapter>
    {

        public ChapterManagerApp(IUnitWork unitWork, IRepository<Chapter> repository) : base(unitWork, repository)
        {
        }
        public List<Chapter> GetChapterList(string novelId)
        {
            var listChapter = new List<Chapter>();
            if (!IsExistChapterTable(novelId)) return listChapter;
            var sqlStr = string.Format("SELECT * FROM [{0}] ORDER BY SORT;", novelId);
            listChapter = Repository.ChapterQueryFromSql(sqlStr).ToList();
            return listChapter;
        }

        public string GetChapterContent(string novelId, string chapterId)
        {
            var objNovel = UnitWork.FindSingle<Novel>(x => x.Id == novelId);
            if (objNovel == null) return string.Empty;

            var chapterTitle = string.Empty;
            var content = string.Empty;
            switch (objNovel.FromType)
            {
                case 0://本地
                    var sqlStr = string.Format("SELECT TOP 1 * FROM [{0}] ORDER BY SORT;", novelId);
                    if (!string.IsNullOrEmpty(chapterId))
                    {
                        sqlStr = string.Format("SELECT * FROM [{0}] WHERE Id='{1}';", novelId,chapterId);
                    }
                    var chapter = Repository.ChapterQueryFromSql(sqlStr).ToList().FirstOrDefault();
                    if (chapter != null)
                    {
                        long startPos = chapter.ChapterStartPosition;
                        long endPos = chapter.ChapterEndPosition;
                        string bookPath = objNovel.PhysicalPath;
                        chapterTitle = chapter.Name;
                        content = GetLocalNovelContent(bookPath, startPos, endPos);
                    }
                    break;
                case 1://网络
                    break;
            }
            var obj = new { Title = chapterTitle.ToBase64CodeCN(), Content = content.ToBase64CodeCN() };
            return JsonHelper.Instance.Serialize(obj);
        }

        private string GetLocalNovelContent(string physicalPath, long startPos, long endPos)
        {
            var contentSb = new StringBuilder();
            using (FileStream fs = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bt = new byte[endPos - startPos];
                fs.Seek(startPos, SeekOrigin.Begin);
                fs.Read(bt, 0, bt.Length);
                using (MemoryStream ms = new MemoryStream(bt))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        var line = string.Empty;
                        while (sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                            contentSb.AppendFormat("<p>{0}</p>", line);
                        }
                    }
                }
            }
            return contentSb.ToString();
        }

        /// <summary>
        /// 自动匹配章节
        /// </summary>
        /// <param name="bookPath"></param>
        public List<Chapter> AutoMatchSubchapter(string novelId)
        {
            var objNovel = UnitWork.FindSingle<Novel>(x => x.Id == novelId && x.FromType == 0);
            var chapterList = new List<Chapter>();
            if (objNovel == null) return chapterList;
            var filePath = objNovel.PhysicalPath;
            if (!System.IO.File.Exists(filePath)) return chapterList;
            string matchStr = string.Empty;
            string matchStr1 = @"第\d*章\s*.*";
            string matchStr2 = @"第[零一二三四五六七八九十千百万亿兆]+章\s*.*";

            long totalLength = 0;
            long curLength = 0;

            string preChapterName = string.Empty;

            long chapterStart = 0;
            long chapterEnd = 0;


            using (StreamReader sr = new StreamReader(filePath))
            {
                totalLength = sr.BaseStream.Length;
                while (sr.Peek() >= 0)
                {
                    var lineStr = sr.ReadLine();
                    curLength += Encoding.UTF8.GetBytes(lineStr).Count() + 2;
                    if (lineStr.Contains("第") && lineStr.Contains("章"))
                    {
                        if (string.IsNullOrEmpty(matchStr))
                        {
                            Regex reg1 = new Regex(matchStr1);
                            Regex reg2 = new Regex(matchStr2);
                            Match match1 = reg1.Match(lineStr);
                            Match match2 = reg2.Match(lineStr);
                            if (match1.Success)
                            {
                                matchStr = matchStr1;
                            }
                            else if (match2.Success)
                            {
                                matchStr = matchStr2;
                            }
                            else
                            {
                                matchStr = String.Empty;
                            }
                        }
                        if (!string.IsNullOrEmpty(matchStr))
                        {
                            Regex reg = new Regex(matchStr);
                            Match match = reg.Match(lineStr);
                            if (match.Success)
                            {
                                chapterEnd = curLength - Encoding.UTF8.GetBytes(lineStr).Count() - 2;
                                if (!string.IsNullOrEmpty(preChapterName))
                                {
                                    preChapterName = Regex.Replace(preChapterName, @"[^0-9a-zA-Z\u4e00-\u9fa5()\s]*", "");
                                    Chapter chapter = new Chapter()
                                    {
                                        Name = preChapterName,
                                        NovelId = objNovel.Id,
                                        NovelName = objNovel.Name,
                                        ChapterStartPosition = chapterStart,
                                        ChapterEndPosition = chapterEnd,
                                        Sort = chapterList.Count() + 1
                                    };
                                    chapterList.Add(chapter);
                                    chapterStart = chapterEnd + 1;
                                }
                                preChapterName = match.Value;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(matchStr))
                {
                    Chapter chapter = new Chapter()
                    {
                        Name = preChapterName,
                        ChapterStartPosition = chapterStart,
                        ChapterEndPosition = chapterEnd,
                        Sort = chapterList.Count() + 1
                    };
                    chapterList.Add(chapter);
                }
            }
            return chapterList;
        }

        public bool AddRandChapter(string novelId, List<Chapter> listChapter)
        {
            CreateChapterTable(novelId);
            var sqlSb = new StringBuilder();
            foreach (var chapter in listChapter)
            {
                sqlSb.AppendFormat(" INSERT INTO [{0}](Id,NovelId,NovelName,Name,OriginLink,Sort,State,ChapterStartPosition,ChapterEndPosition) ", novelId);
                sqlSb.AppendFormat("VALUES(");
                sqlSb.AppendFormat("'{0}'", chapter.Id);
                sqlSb.AppendFormat(",'{0}'", chapter.NovelId);
                sqlSb.AppendFormat(",'{0}'", chapter.NovelName);
                sqlSb.AppendFormat(",'{0}'", chapter.Name);
                sqlSb.AppendFormat(",'{0}'", chapter.OriginLink);
                sqlSb.AppendFormat(",'{0}'", chapter.Sort);
                sqlSb.AppendFormat(",'{0}'", chapter.State);
                sqlSb.AppendFormat(",'{0}'", chapter.ChapterStartPosition);
                sqlSb.AppendFormat(",'{0}'", chapter.ChapterEndPosition);
                sqlSb.AppendFormat(")");
            }
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public bool AddChapter(string novelId, Chapter chapter)
        {
            CreateChapterTable(novelId);
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" INSERT INTO [{0}](Id,NovelId,NovelName,Name,OriginLink,Sort,State,ChapterStartPosition,ChapterEndPosition) ", novelId);
            sqlSb.AppendFormat("VALUES(");
            sqlSb.AppendFormat("'{0}'", chapter.Id);
            sqlSb.AppendFormat(",'{0}'", chapter.NovelId);
            sqlSb.AppendFormat(",'{0}'", chapter.NovelName);
            sqlSb.AppendFormat(",'{0}'", chapter.Name);
            sqlSb.AppendFormat(",'{0}'", chapter.OriginLink);
            sqlSb.AppendFormat(",'{0}'", chapter.Sort);
            sqlSb.AppendFormat(",'{0}'", chapter.State);
            sqlSb.AppendFormat(",'{0}'", chapter.ChapterStartPosition);
            sqlSb.AppendFormat(",'{0}'", chapter.ChapterEndPosition);
            sqlSb.AppendFormat(")");
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }
        /// <summary>
        /// 创建章节表
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public bool CreateChapterTable(string novelId)
        {
            if (IsExistChapterTable(novelId)) return true;
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF NOT EXISTS(SELECT * FROM SYSOBJECTS WHERE NAME='[{0}]' AND TYPE='U') ", novelId);
            sqlSb.AppendFormat(" CREATE TABLE [{0}]( ", novelId);
            sqlSb.AppendFormat(" [Id] [varchar](64) PRIMARY KEY, ");
            sqlSb.AppendFormat(" [NovelId] [varchar](64) NULL, ");
            sqlSb.AppendFormat(" [NovelName] [varchar](100) NULL, ");
            sqlSb.AppendFormat(" [Name] [varchar](100) NULL, ");
            sqlSb.AppendFormat(" [OriginLink] [varchar](200) NULL, ");
            sqlSb.AppendFormat(" [Sort] [int] NULL DEFAULT(0), ");
            sqlSb.AppendFormat(" [State] [int] NULL DEFAULT(0), ");
            sqlSb.AppendFormat(" [ChapterStartPosition] [bigint] NULL DEFAULT(0), ");
            sqlSb.AppendFormat(" [ChapterEndPosition] [bigint] NULL DEFAULT(0) ");
            sqlSb.AppendFormat(");");
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }
        /// <summary>
        /// 判断章节表是否存在
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public bool IsExistChapterTable(string novelId)
        {
            var judgeSb = new StringBuilder();
            judgeSb.AppendFormat("if exists ( select * from sysobjects where name = '{0}' and type = 'U') select 1 else  select 0", novelId);
            var result = Repository.ExecuteScalar(judgeSb.ToString());
            return (int)result == 1 ? true : false;
        }
        /// <summary>
        /// 删除章节表
        /// </summary>
        /// <param name="novelId"></param>
        public void DropNovelTable(string novelId)
        {
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF OBJECT_ID('{0}', 'U') IS NOT NULL ", novelId);
            sqlSb.AppendFormat(" Drop Table [{0}]; ", novelId);
            Repository.ExecuteSql(sqlSb.ToString());
        }
    }
}
