#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 值变化事件参数
    /// </summary>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// 获取设置新值
        /// </summary>
        public T NewValue { get; set; }

        /// <summary>
        /// 获取设置旧值
        /// </summary>
        public T OldValue { get; set; }
    }
}