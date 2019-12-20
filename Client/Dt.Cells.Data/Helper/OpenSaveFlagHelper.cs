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
    internal static class OpenSaveFlagHelper
    {
        internal static bool AutoRowHeight(this ExcelSaveFlags saveFlags)
        {
            return ((saveFlags & ExcelSaveFlags.AutoRowHeight) == ExcelSaveFlags.AutoRowHeight);
        }

        internal static bool ColumnHeaders(this ExcelOpenFlags openFlags)
        {
            if ((openFlags & ExcelOpenFlags.ColumnHeaders) != ExcelOpenFlags.ColumnHeaders)
            {
                return ((openFlags & ExcelOpenFlags.RowAndColumnHeaders) == ExcelOpenFlags.RowAndColumnHeaders);
            }
            return true;
        }

        internal static bool DataFormulaOnly(this ExcelOpenFlags openFlags)
        {
            return ((openFlags & ExcelOpenFlags.DataAndFormulasOnly) == ExcelOpenFlags.DataAndFormulasOnly);
        }

        internal static bool DataOnly(this ExcelOpenFlags openFlags)
        {
            return (((openFlags & ExcelOpenFlags.DataOnly) == ExcelOpenFlags.DataOnly) && !openFlags.DataFormulaOnly());
        }

        internal static bool DoNotRecalculateAfterLoad(this ExcelOpenFlags openFlags)
        {
            return ((openFlags & ExcelOpenFlags.DoNotRecalculateAfterLoad) == ExcelOpenFlags.DoNotRecalculateAfterLoad);
        }

        internal static bool NoFormulas(this ExcelSaveFlags saveFlags)
        {
            return ((saveFlags & ExcelSaveFlags.NoFormulas) == ExcelSaveFlags.NoFormulas);
        }

        internal static bool NoSet(this ExcelOpenFlags openFlags)
        {
            return (0 == 0);
        }

        internal static bool NoSet(this ExcelSaveFlags saveFlags)
        {
            return (0 == 0);
        }

        internal static bool RowHeaders(this ExcelOpenFlags openFlags)
        {
            if ((openFlags & ExcelOpenFlags.RowHeaders) != ExcelOpenFlags.RowHeaders)
            {
                return ((openFlags & ExcelOpenFlags.RowAndColumnHeaders) == ExcelOpenFlags.RowAndColumnHeaders);
            }
            return true;
        }

        internal static bool SaveAsFiltered(this ExcelSaveFlags saveFlags)
        {
            return ((saveFlags & ExcelSaveFlags.SaveAsFiltered) == ExcelSaveFlags.SaveAsFiltered);
        }

        internal static bool SaveAsViewed(this ExcelSaveFlags saveFlags)
        {
            return ((saveFlags & ExcelSaveFlags.SaveAsViewed) == ExcelSaveFlags.SaveAsViewed);
        }

        internal static bool SaveCustomColumnHeaders(this ExcelSaveFlags saveFlags)
        {
            if ((saveFlags & ExcelSaveFlags.SaveCustomColumnHeaders) != ExcelSaveFlags.SaveCustomColumnHeaders)
            {
                return ((saveFlags & ExcelSaveFlags.SaveBothCustomRowAndColumnHeaders) == ExcelSaveFlags.SaveBothCustomRowAndColumnHeaders);
            }
            return true;
        }

        internal static bool SaveCustomRowHeaders(this ExcelSaveFlags saveFlags)
        {
            if ((saveFlags & ExcelSaveFlags.SaveCustomRowHeaders) != ExcelSaveFlags.SaveCustomRowHeaders)
            {
                return ((saveFlags & ExcelSaveFlags.SaveBothCustomRowAndColumnHeaders) == ExcelSaveFlags.SaveBothCustomRowAndColumnHeaders);
            }
            return true;
        }

        internal static bool SaveDataOnly(this ExcelSaveFlags saveFlags)
        {
            return ((saveFlags & ExcelSaveFlags.DataOnly) == ExcelSaveFlags.DataOnly);
        }
    }
}

