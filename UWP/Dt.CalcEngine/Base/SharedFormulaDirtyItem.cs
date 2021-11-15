#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    internal class SharedFormulaDirtyItem : DirtyItem
    {
        public SharedFormulaDirtyItem(CalcService service, ICalcSource source, CalcLocalIdentity id, CalcNode node) : base(service, source, id, node)
        {
            this.DirtySubIds = new List<CalcLocalIdentity>();
            this.DirtySubIds2 = new List<CalcLocalIdentity>();
        }

        public List<CalcLocalIdentity> DirtySubIds { get; private set; }

        public List<CalcLocalIdentity> DirtySubIds2 { get; private set; }

        public bool IsFullRangeDirty { get; set; }
    }
}

