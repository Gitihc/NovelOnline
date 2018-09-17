using BaseLibrary;
using Infrastructure;
using Repository.Domain;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NovelOnline.App
{
    public class ChapterManagerApp : BaseApp<Chapter>
    {
        private readonly UserNovelManager _userNovelManager;
        public ChapterManagerApp(IUnitWork unitWork, IRepository<Chapter> repository, UserNovelManager userNovelManager) : base(unitWork, repository)
        {
            _userNovelManager = userNovelManager;
        }
        public List<Chapter> GetChapterList(string novelId)
        {
            var listChapter = new List<Chapter>();
            if (!IsExistChapterTable(novelId)) return listChapter;
            var sqlStr = string.Format("SELECT * FROM [{0}] ORDER BY SORT;", novelId);
            listChapter = Repository.ChapterQueryFromSql(sqlStr).ToList();
            return listChapter;
        }

        public string GetChapterContent(User user, string novelId, string chapterId = "")
        {
            var novelObj = UnitWork.FindSingle<Novel>(x => x.Id == novelId);
            if (novelObj == null) return string.Empty;

            var chapterTitle = string.Empty;
            var chapterContent = string.Empty;
            var sqlStr = string.Format("SELECT TOP 1 * FROM [{0}] ORDER BY SORT;", novelId);
            if (!string.IsNullOrEmpty(chapterId))
            {
                sqlStr = string.Format("SELECT * FROM [{0}] WHERE Id='{1}';", novelId, chapterId);
            }
            var chapter = Repository.ChapterQueryFromSql(sqlStr).ToList().FirstOrDefault();
            if (chapter != null)
            {
                chapterTitle = chapter.Name;
                switch (novelObj.FromType)
                {
                    case 0://本地
                        chapterId = chapter.Id;
                        long startPos = chapter.ChapterStartPosition;
                        long endPos = chapter.ChapterEndPosition;
                        string bookPath = novelObj.PhysicalPath;
                        chapterContent = GetLocalNovelContent(bookPath, startPos, endPos);
                        break;
                    case 1://网络
                        //判断是否获取过（获取过从本地读取，未获取，先获取再读取）
                        if (chapter.State != 2) //未获取
                        {
                            UpdateChapterState(novelId, chapter.Id, 1);
                            var tmpChapterContent = GetWebChapterContent(chapter);
                            SaveToLocal(novelObj, chapter, tmpChapterContent);
                            UpdateChapterState(novelId, chapter.Id, 2);
                        }

                        var title = chapterTitle;
                        title = regexOfSaveChapter.Replace(title, "");
                        var filePath = Path.Combine(novelObj.PhysicalPath, title + ".txt");
                        //本地读取
                        chapterContent = GetWebNovelContent(filePath);
                        break;
                }
            }
            _userNovelManager.RecordLastOpenTime(user.Id, novelId, chapterId);
            var obj = new { Title = chapterTitle.ToBase64CodeCN(), Content = chapterContent.ToBase64CodeCN() };
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

        private string GetWebNovelContent(string filePath)
        {
            var contentSb = new StringBuilder();
            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8")))
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
            if (contentSb.Length > 0)
            {
                return contentSb.ToString();
            }
            return string.Empty;
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
                            lineStr = lineStr.Replace(" ", "");
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
                            var tmplineStr = lineStr.Replace(" ", "");
                            Regex reg = new Regex(matchStr);
                            Match match = reg.Match(tmplineStr);
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
                                        Sort = chapterList.Count() + 1,
                                        State = 2
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
                sqlSb.AppendFormat(" IF NOT EXISTS( ");
                sqlSb.AppendFormat("    SELECT * ");
                sqlSb.AppendFormat("    FROM [{0}] ", novelId);
                sqlSb.AppendFormat("    WHERE Name='{0}' AND OriginLink='{1}' ", chapter.Name, chapter.OriginLink);
                sqlSb.AppendFormat(" ) ");
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
                sqlSb.AppendFormat("); ");
            }
            return Repository.ExecuteSql(sqlSb.ToString()) > 0;
        }

        public bool AddChapter(string novelId, Chapter chapter)
        {
            CreateChapterTable(novelId);
            var sqlSb = new StringBuilder();
            sqlSb.AppendFormat(" IF NOT EXISTS( ");
            sqlSb.AppendFormat("    SELECT * ");
            sqlSb.AppendFormat("    FROM [{0}] ", novelId);
            sqlSb.AppendFormat("    WHERE Name='{0}' AND OriginLink='{1}' ", chapter.Name, chapter.OriginLink);
            sqlSb.AppendFormat(" ) ");
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
        /// <summary>
        /// 更新章节状态
        /// </summary>
        /// <param name="novelId"></param>
        /// <param name="chapterId"></param>
        /// <param name="state"></param>
        public void UpdateChapterState(string novelId, string chapterId, int state)
        {
            var sqlStr = string.Format(" UPDATE [{0}] SET state={1} WHERE Id='{2}' ", novelId, state, chapterId);
            Repository.ExecuteSql(sqlStr);
        }

        #region 网络novel获取保存到本地
        /// <summary>
        /// 判断是否全部获取
        /// </summary>
        /// <param name="novelId"></param>
        /// <returns></returns>
        public bool IsAllChapterToLocal(string novelId)
        {
            var sqlStr = string.Format("SELECT COUNT(Id) FROM [{0}] WHERE State <> 2 ", novelId);
            var result = (int)Repository.ExecuteScalar(sqlStr);
            return result == 0;
        }
        /// <summary>
        /// 获取网络章节内容
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public string GetWebChapterContent(Chapter chapter)
        {
            var objNVB = NVHelper.NVBaseObject(chapter.OriginLink); //获取对应处理类
            if (objNVB != null)
            {

                return objNVB.GetChapterContent(new NVChapter { Title = chapter.Name, Link = chapter.OriginLink, Sort = chapter.Sort });
            }
            return string.Empty;
        }
        /// <summary>
        /// 文件名称正则替换
        /// </summary>
        public Regex regexOfSaveChapter = new Regex(@"[\\s\\\\/:\\*\\?\\\"" <>\\|]");
        /// <summary>
        /// 保存到本地
        /// </summary>
        /// <param name="novel"></param>
        /// <param name="chapter"></param>
        /// <param name="chapterContent"></param>
        public void SaveToLocal(Novel novel, Chapter chapter, string chapterContent)
        {
            try
            {
                var dirPath = novel.PhysicalPath;
                if (string.IsNullOrEmpty(dirPath)) return;
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var title = chapter.Name;
                title = regexOfSaveChapter.Replace(title, "");
                var fileName = Path.Combine(dirPath, title + ".txt");
                using (TextWriter txtWriter = new StreamWriter(fileName, false))
                {
                    txtWriter.Write(chapterContent);
                    txtWriter.Close();
                    txtWriter.Dispose();
                }
            }
            catch (Exception ex)
            {
                UnitWork.Update<Novel>(x => x.Id == novel.Id, n => new Novel { State = -1 });
                UpdateChapterState(novel.Id, chapter.Id, -1);
            }

        }

        #endregion
    }
}
