#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal use only.
    /// </summary>
    public class ExcelBuiltInStyleHelper
    {
        /// <summary>
        /// Gets all excel built-in styles.
        /// </summary>
        /// <returns>A collection of <seealso cref="T:Dt.Cells.Data.StyleInfo" /> objects used to represents excel built-in styles.</returns>
        public static List<StyleInfo> GetExcelBuiltInStyles()
        {
            BuiltInExcelStyles builtInExcelStyleCollection = new BuiltInExcelStyles();
            IEnumerable<IExcelStyle> _builtInStyles = null;
            Task.Factory.StartNew(delegate {
                _builtInStyles = from item in (IEnumerable<IExcelStyle>) builtInExcelStyleCollection.GetBuiltInStyls()
                                 select item;
            }, CancellationToken.None, (TaskCreationOptions) TaskCreationOptions.None, null).Wait();
            List<StyleInfo> list = new List<StyleInfo>();
            if (_builtInStyles != null)
            {
                foreach (ExcelStyle style in _builtInStyles)
                {
                    list.Add(new StyleInfo(style.Name, "", style.Format.ToCellStyleInfo(null)));
                }
            }
            return list;
        }
    }
}

