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
    /// 数据源包装类
    /// </summary>
    public class RptData
    {
        public RptData(Table p_data)
        {
            Data = p_data;
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        public Table Data { get; }

        /// <summary>
        /// 获取设置当前行索引
        /// </summary>
        public int Current { get; set; }

        /// <summary>
        /// 获取当前行数据
        /// </summary>
        public Row CurrentRow
        {
            get
            {
                if (Data.Count == 0
                    || Current >= Data.Count
                    || Current < 0)
                {
                    return null;
                }
                return Data[Current];
            }
        }

        /// <summary>
        /// 获取下一行数据
        /// </summary>
        public Row NextRow
        {
            get
            {
                if (Data.Count == 0
                    || Current >= (Data.Count - 1)
                    || Current < 0)
                {
                    return null;
                }
                return Data[Current + 1];
            }
        }

        /// <summary>
        /// 获取上一行数据
        /// </summary>
        public Row PreRow
        {
            get
            {
                if (Data.Count == 0
                    || Current >= Data.Count
                    || Current < 1)
                {
                    return null;
                }
                return Data[Current - 1];
            }
        }
    }
}
