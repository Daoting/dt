#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Core
{
    public class Table<TRow> : Table
        where TRow : Row
    {
        /// <summary>
        /// 创建行
        /// </summary>
        /// <returns></returns>
        protected override Row CreateRowInstance()
        {
            return Activator.CreateInstance<TRow>();
        }
    }
}