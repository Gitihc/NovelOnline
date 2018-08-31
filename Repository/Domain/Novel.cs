using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 书籍类
    /// </summary>
    public class Novel:EntityBase
    {
        public Novel()
        {
            Name = "";
            PhysicalPath = "";
            OriginLink = "";
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 物理路径
        /// </summary>
        public string PhysicalPath { get; set; }
        /// <summary>
        /// 源始地址
        /// </summary>
        public string OriginLink { get; set; }
        /// <summary>
        /// 来源 默认0:本地，1：网络
        /// </summary>
        public int FromType { get; set; }
        /// <summary>
        /// 状态（本地：0：处理中，1：完成，；网络：-2:失败,-1：部分未完成 ，1:获取中，2：完成）
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
