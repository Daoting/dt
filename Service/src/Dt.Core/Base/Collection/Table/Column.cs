#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据列，已移除列默认值功能！
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 列名不可为空，列类型默认为string
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_colType">列数据类型</param>
        public Column(string p_colName, Type p_colType = null)
        {
            if (string.IsNullOrEmpty(p_colName))
                throw new Exception("未指定列名！");
            ID = p_colName;
            Type = (p_colType == null) ? typeof(string) : p_colType;
        }

        /// <summary>
        /// 列字段名
        /// </summary>
        public string ID { get; }

        /// <summary>
        ///  列类型
        /// </summary>
        public Type Type { get; internal set; }
    }
}
