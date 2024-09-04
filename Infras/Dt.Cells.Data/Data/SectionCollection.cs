#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a collection of <see cref="T:Dt.Cells.Data.GcSection" /> objects. 
    /// </summary>
    internal class SectionCollection : Collection<GcSection>
    {
        /// <summary>
        /// Gets the sections list by the type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The list of sections.</returns>
        public List<T> GetSections<T>() where T: GcSection
        {
            List<T> list = new List<T>();
            foreach (GcSection section in this)
            {
                if (section is T)
                {
                    list.Add((T) section);
                }
            }
            return list;
        }

        /// <summary>
        /// Removes the sections by type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The section list that has been removed.</returns>
        public List<T> RemoveSections<T>() where T: GcSection
        {
            List<T> sections = this.GetSections<T>();
            foreach (T local in sections)
            {
                base.Remove(local);
            }
            return sections;
        }
    }
}

