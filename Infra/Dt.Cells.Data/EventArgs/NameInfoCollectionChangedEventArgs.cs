#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class NameInfoCollectionChangedEventArgs : EventArgs
    {
        public NameInfoCollectionChangedEventArgs(Dt.Cells.Data.NameInfo nameInfo, NameInfoCollectionChangedAction action)
        {
            this.NameInfo = nameInfo;
            this.Action = action;
        }

        public NameInfoCollectionChangedAction Action { get; private set; }

        public Dt.Cells.Data.NameInfo NameInfo { get; private set; }
    }
}

