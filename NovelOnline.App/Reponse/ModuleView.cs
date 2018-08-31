using System;
using System.Collections.Generic;
using Infrastructure;
using Repository.Domain;

namespace OpenAuth.App.Response
{
    public class ModuleView
    {
        /// <summary>
        /// ID
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }

        /// <summary>
	    /// 节点语义ID
	    /// </summary>
        public string CascadeId { get; set; }
        /// <summary>
        /// 父节点名称
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 父节点名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        public string Name { get; set; }

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

        /// <summary>
        /// 模块中的元素
        /// </summary>
        public List<ModuleElement> Elements { get; set; }

        public static implicit operator ModuleView(Module module)
        {
            return module.MapTo<ModuleView>();
        }

        public static implicit operator Module(ModuleView view)
        {
            return view.MapTo<Module>();
        }
    }
}