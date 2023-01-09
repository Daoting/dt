﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm.Domain
{
    public partial class RoleObj
    {
        public static async Task<RoleObj> New(
            string Name = default,
            string Note = default)
        {
            long id = await NewID();
            return new RoleObj(id, Name, Note);
        }

        protected override void InitHook()
        {
        }
    }
}