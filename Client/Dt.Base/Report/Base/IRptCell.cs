#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 单元格信息，提供给RptInfo脚本用
    /// </summary>
    public interface IRptCell
    {
        /// <summary>
        /// 获取单元格字符串
        /// </summary>
        string Text { get; }

        /// <summary>
        /// 获取对应的数据行
        /// </summary>
        Row Data { get; }

        /// <summary>
        /// 获取单元格行索引
        /// </summary>
        int Row { get; }

        /// <summary>
        /// 获取单元格列索引
        /// </summary>
        int Col { get; }
    }
}
