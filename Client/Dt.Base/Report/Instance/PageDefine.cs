#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 单个页面行列定义描述
    /// </summary>
    internal class PageDefine
    {
        public PageDefine()
        {
            Size = new List<double>();
        }

        /// <summary>
        /// 获取设置页面行列开始索引
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 获取页面所有内容行高（列宽）定义
        /// </summary>
        public List<double> Size { get; }

        /// <summary>
        /// 获取页面所包含的行（列）数
        /// </summary>
        public int Count
        {
            get { return Size.Count; }
        }

        /// <summary>
        /// 获取页面渲染时在Sheet中的起始行（列）索引
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 获取页面渲染时在Sheet中的总行（列）数
        /// </summary>
        public int Total { get; set; }
    }
}
