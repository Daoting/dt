#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Dt.Base.Docking;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Tv扩展方法
    /// </summary>
    public static class TvEx
    {
        /// <summary>
        /// 根据Row.ID选择行
        /// </summary>
        /// <param name="p_tv"></param>
        /// <param name="p_id"></param>
        public static void SelectByID(this Tv p_tv, long p_id)
        {
            Table tbl;
            if (p_tv == null || (tbl = p_tv.Data as Table) == null)
                return;
            
            var row = (from r in tbl
                       where r.ID == p_id
                       select r).FirstOrDefault();
            p_tv.SelectedItem = row;
        }
    }
}
