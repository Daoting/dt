#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单击行事件参数
    /// </summary>
    public class ItemClickArgs : EventArgs
    {
        public ItemClickArgs(object p_data, object p_oldData)
        {
            Data = p_data;
            OldData = p_oldData;
        }

        /// <summary>
        /// 当前点击行是否和上次点击行相同
        /// </summary>
        public bool IsChanged
        {
            get { return Data != OldData; }
        }

        /// <summary>
        /// 当前点击行
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// 上次点击行
        /// </summary>
        public object OldData { get; }

        /// <summary>
        /// 当前点击的Row
        /// </summary>
        public Row Row
        {
            get { return Data as Row; }
        }
    }
}
