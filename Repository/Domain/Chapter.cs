﻿using Repository.Core;
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
        /// <summary>
        /// 书籍id
        /// </summary>
        public string NovelId { get; set; }
        /// <summary>
        /// 章节名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string SourceUrl { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 状态：0-未获取，1-获取中，2-获取完成
        /// </summary>
        public int State { get; set; }
    }
}
