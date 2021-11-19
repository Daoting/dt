#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class CopyMoveFloatingObjectsInfo
    {
        public void SaveFloatingObjects(CellRange range, FloatingObject[] floatingObjects)
        {
            Range = range;
            SavedFloatingObjects = floatingObjects;
            IsFloatingObjectsSaved = true;
        }

        public bool IsFloatingObjectsSaved { get; private set; }

        public CellRange Range { get; private set; }

        public FloatingObject[] SavedFloatingObjects { get; private set; }
    }
}

