using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 网站书籍类
    /// </summary>
    public class WebSiteNovel : EntityBase
    {
        /// <summary>
        /// 网站id
        /// </summary>
        public string WebSiteId { get; set; }
        /// <summary>
        /// 书籍名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string NovelUrl { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态：0-未获取，1-获取中，2-已获取
        /// </summary>
        public int State { get; set; }
    }
}
