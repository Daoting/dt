#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// The Comparer Class
    /// </summary>
    internal sealed class Comparer : IComparer
    {
        /// <summary>
        /// The compare information.
        /// </summary>
        CompareInfo compareInfo;
        /// <summary>
        /// The compare information name.
        /// </summary>
        const string CompareInfoName = "CompareInfo";
        /// <summary>
        /// The default comparer.
        /// </summary>
        public static readonly Comparer Default = new Comparer(CultureInfo.CurrentCulture);
        /// <summary>
        /// The default invariant comparer.
        /// </summary>
        public static readonly Comparer DefaultInvariant = new Comparer(CultureInfo.InvariantCulture);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Comparer" /> class.
        /// </summary>
        Comparer()
        {
            this.compareInfo = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Comparer" /> class.
        /// </summary>
        /// <param name="culture">The culture information.</param>
        public Comparer(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            this.compareInfo = culture.CompareInfo;
        }

        /// <summary>
        /// Compares the specified arguments.
        /// </summary>
        /// <param name="a">The first argument.</param>
        /// <param name="b">The second argument.</param>
        /// <returns>The compared result.</returns>
        public int Compare(object a, object b)
        {
            if (a == b)
            {
                return 0;
            }
            if (a == null)
            {
                return -1;
            }
            if (b == null)
            {
                return 1;
            }
            if (this.compareInfo != null)
            {
                string str = (string) (a as string);
                string str2 = (string) (b as string);
                if ((str != null) && (str2 != null))
                {
                    return this.compareInfo.Compare(str, str2);
                }
            }
            IComparable comparable = a as IComparable;
            if (comparable == null)
            {
                throw new ArgumentException(ResourceStrings.SortCompareError);
            }
            return comparable.CompareTo(b);
        }
    }
}

