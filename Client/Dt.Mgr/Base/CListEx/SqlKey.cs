#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 简单的SqlKey获取下拉框数据源，不支持sql变量
    /// </summary>
    [CListEx]
    public class SqlKey : CListEx
    {
        /// <summary>
        /// 分号隔开的查询，格式：服务名:sql键名，不支持sql变量，如：
        /// <para>cm:用户-所有</para>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override async Task<INotifyList> GetData()
        {
            string[] info = _params.Trim().Split(':');
            if (info.Length != 2 || string.IsNullOrEmpty(info[0]) || string.IsNullOrEmpty(info[1]))
                throw new Exception("Key格式不正确！" + _params);
            var tbl = await Kit.Rpc<Table>(info[0], "Da.Query", info[1], null);
            return tbl;
        }
    }
}