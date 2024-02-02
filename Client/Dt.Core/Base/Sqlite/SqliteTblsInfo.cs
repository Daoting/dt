#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// sqlite库的描述信息
    /// </summary>
    public class SqliteTblsInfo
    {
        /// <summary>
        /// 库结构的版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 表结构的映射类型
        /// </summary>
        public IList<Type> Tables { get; set; }
    }
}
