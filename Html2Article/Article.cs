#region 程序集 Html2Article, Version=1.13.0.5, Culture=neutral, PublicKeyToken=null
// C:\Users\fan\.nuget\packages\html2article\1.17.0\lib\40\Html2Article.dll
// Decompiled with ICSharpCode.Decompiler 3.1.0.3652
#endregion
using System;
using System.Runtime.CompilerServices;

namespace StanSoft
{
    /// <summary>
    /// 文章正文数据模型
    /// </summary>
    public class Article
    {
        public string Title
        {
            [CompilerGenerated]
            get;
            [CompilerGenerated]
            set;
        }

        /// <summary>
        /// 正文文本
        /// </summary>
        public string Content
        {
            [CompilerGenerated]
            get;
            [CompilerGenerated]
            set;
        }

        /// <summary>
        /// 带标签正文
        /// </summary>
        public string ContentWithTags
        {
            [CompilerGenerated]
            get;
            [CompilerGenerated]
            set;
        }

        public DateTime PublishDate
        {
            [CompilerGenerated]
            get;
            [CompilerGenerated]
            set;
        }
    }
}
