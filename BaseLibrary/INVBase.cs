using System;
using System.Collections.Generic;

namespace BaseLibrary
{
    public interface INVBase
    {
        /// <summary>
        /// 获取站点名称
        /// </summary>
        /// <returns></returns>
        string GetWebName();
        /// <summary>
        /// 获取书籍名称
        /// </summary>
        /// <returns></returns>
        string GetNovelName();
        /// <summary>
        /// 获取章节列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        List<NVChapter> GetList(string url);
        /// <summary>
        /// 获取章节内容
        /// </summary>
        /// <returns></returns>
        string GetChapterContent(NVChapter nVChapter);
        /// <summary>
        /// 特殊匹配规则
        /// </summary>
        /// <returns></returns>
        string GetSpecRegex();

        NVNovel GetNovelInfo(string url);
    }
}
