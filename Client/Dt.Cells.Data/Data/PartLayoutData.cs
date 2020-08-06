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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// PartLayoutData
    /// </summary>
    internal class PartLayoutData
    {
        double allSize;
        int offsetIndex;
        readonly List<double> sizes;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PartLayoutData" /> class.
        /// </summary>
        /// <param name="sizes">The sizes.</param>
        public PartLayoutData(List<double> sizes)
        {
            this.allSize = Utilities.CalcCount((IEnumerable<double>) sizes);
            this.sizes = sizes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PartLayoutData" /> class.
        /// </summary>
        /// <param name="sizes">The sizes.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        public PartLayoutData(List<double> sizes, int startIndex, int count)
        {
            this.allSize = Utilities.CalcCount((IEnumerable<double>) sizes);
            this.sizes = new List<double>();
            int num = 0;
            for (int i = 0; i < count; i++)
            {
                if ((startIndex <= i) && (num < sizes.Count))
                {
                    this.sizes.Add(sizes[num]);
                    num++;
                }
                else
                {
                    this.sizes.Add(-1.0);
                }
            }
        }

        /// <summary>
        /// Sets the sizes.
        /// </summary>
        /// <param name="newSizes">The new sizes.</param>
        /// <param name="startIndex">The start index.</param>
        public void AppendSizes(List<double> newSizes, int startIndex)
        {
            for (int i = startIndex; (i < (startIndex + newSizes.Count)) && (i < this.sizes.Count); i++)
            {
                this.sizes[i] = newSizes[i - startIndex];
            }
            this.allSize = Utilities.CalcCount((IEnumerable<double>) this.sizes);
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public double GetSize(int index)
        {
            return this.GetValue(index - this.offsetIndex);
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public double GetSize(int startIndex, int count)
        {
            double num = 0.0;
            int num2 = startIndex - this.offsetIndex;
            for (int i = num2; i < (num2 + count); i++)
            {
                num += this.GetValue(i);
            }
            return num;
        }

        /// <summary>
        /// Gets the sizes.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public List<double> GetSizes(int startIndex, int count)
        {
            List<double> list = new List<double>();
            int num = startIndex - this.offsetIndex;
            for (int i = num; i < (num + count); i++)
            {
                list.Add(this.GetValue(i));
            }
            return list;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        double GetValue(int index)
        {
            return this.sizes[index];
        }

        /// <summary>
        /// Determines whether the specified index has size.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// <c>true</c> if the specified index has size; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSize(int index)
        {
            return ((this.sizes.Count > index) && (this.sizes[index] >= 0.0));
        }

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void SetSize(int index, double value)
        {
            this.sizes[index] = value;
        }

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <value>All sizes.</value>
        public double AllSize
        {
            get { return  this.allSize; }
        }

        /// <summary>
        /// Gets or sets the index of the offset.
        /// </summary>
        /// <value>The index of the offset.</value>
        public int OffsetIndex
        {
            get { return  this.offsetIndex; }
            set { this.offsetIndex = value; }
        }

        /// <summary>
        /// Gets the sizes.
        /// </summary>
        /// <value>The sizes.</value>
        public List<double> Sizes
        {
            get { return  this.sizes; }
        }
    }
}

