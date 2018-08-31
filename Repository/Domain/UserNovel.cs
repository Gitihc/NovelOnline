using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 用户与书籍关系表
    /// </summary>
    public class UserNovel : EntityBase
    {
        public UserNovel()
        {
            Id = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 书籍id
        /// </summary>
        public string NovelId { get; set; }
        /// <summary>
        /// 书籍名称
        /// </summary>
        public string NovelName { get; set; }
        /// <summary>
        /// 最后阅读时间
        /// </summary>
        public DateTime LastOpenTime { get; set; }
        /// <summary>
        /// 状态：0-正常，1-删除
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 最后阅读章节id
        /// </summary>
        public string LastChapterId { get; set; }

    }
}
