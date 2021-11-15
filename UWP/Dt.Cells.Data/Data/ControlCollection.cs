#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a collection of <see cref="T:Dt.Cells.Data.GcControl" /> objects. 
    /// </summary>
    internal class ControlCollection : Collection<GcControl>
    {
        /// <summary>
        /// Gets the control list by type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The control list.</returns>
        public List<T> GetControls<T>() where T: GcControl
        {
            List<T> list = new List<T>();
            foreach (GcControl control in this)
            {
                if (control is T)
                {
                    list.Add((T) control);
                }
            }
            return list;
        }

        /// <summary>
        /// Internal only.
        /// Sorts this instance by z-index.
        /// </summary>
        internal void Sort()
        {
            for (int i = 0; i < (base.Count - 1); i++)
            {
                for (int j = base.Count - 1; j >= 1; j--)
                {
                    int zIndex = -2147483648;
                    int num4 = -2147483648;
                    if (base[j - 1] is GcPrintableControl)
                    {
                        zIndex = ((GcPrintableControl) base[j - 1]).ZIndex;
                    }
                    if (base[j] is GcPrintableControl)
                    {
                        num4 = ((GcPrintableControl) base[j]).ZIndex;
                    }
                    if (num4 < zIndex)
                    {
                        GcControl control = base[j];
                        base[j] = base[j - 1];
                        base[j - 1] = control;
                    }
                }
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets the printable controls.
        /// </summary>
        /// <value>The printable controls</value>
        internal List<GcPrintableControl> PrintableControls
        {
            get { return  this.GetControls<GcPrintableControl>(); }
        }
    }
}

