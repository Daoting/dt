#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using Dt.Core.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    [Tag(TblName = "cm_role")]
    public class Role : Root
    {
        /// <summary>
        /// 任何人角色ID
        /// </summary>
        public const long AnyoneID = 1;
    }
}
