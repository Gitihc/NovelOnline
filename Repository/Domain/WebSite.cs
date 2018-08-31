using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 网站类
    /// </summary>
    public class Website : EntityBase
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 网址
        /// </summary>
        public string OriginLink { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态：0-未获取，1-获取中，2-获取完成
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatorId { get; set; }
    }
}
