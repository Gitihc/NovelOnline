using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 章节类
    /// </summary>
    public class Chapter : EntityBase
    {
        public Chapter()
        {
            Id = Guid.NewGuid().ToString();
            OriginLink = "";
            State = 0;
            ChapterStartPosition = 0;
            ChapterEndPosition = 0;
        }
        /// <summary>
        /// 书籍id
        /// </summary>
        public string NovelId { get; set; }

        /// <summary>
        /// 书籍名称
        /// </summary>
        public string NovelName { get; set; }
        /// <summary>
        /// 章节名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 源始地址
        /// </summary>
        public string OriginLink { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 状态（获取网络章节记录状态）：-1：失败，0：未开始，1：获取中，2：完成
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 章节开始位置（用于本地文件）
        /// </summary>
        public long ChapterStartPosition { get; set; }
        /// <summary>
        /// 章节结束位置（用于本地文件）
        /// </summary>
        public long ChapterEndPosition { get; set; }
    }
}
