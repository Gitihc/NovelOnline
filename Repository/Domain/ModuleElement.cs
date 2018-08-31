using Repository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    public class ModuleElement : EntityBase
    {
        /// <summary>
        /// 模块id
        /// </summary>
        public string ModuleId { get; set; }
        /// <summary>
        /// DomId
        /// </summary>
        public string DomId { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单类型（默认为0，按钮）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public string Class { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

    }
}
