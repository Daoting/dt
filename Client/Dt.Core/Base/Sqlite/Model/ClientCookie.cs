#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// 客户端Cookie字典
    /// </summary>
    [StateTable]
    public class ClientCookie
    {
        /// <summary>
        /// 键
        /// </summary>
        [PrimaryKey]
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Val { get; set; }
    }
}
