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
        /// <summary>
        /// 书籍名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 书籍引用地址
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 书籍状态
        /// </summary>
        public String Satus { get; set; }
        /// <summary>
        /// 书籍类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
}
}
