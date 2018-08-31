using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    /// <summary>
    /// 模块
    /// </summary>
    public class Module : TreeEntity
    {
        /// <summary>
        /// 模块标识
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public int IsEnable { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}
