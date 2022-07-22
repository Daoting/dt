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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// PaperCutter
    /// </summary>
    internal class PaperCutter
    {
        readonly int allLength;
        readonly List<int> breaks = new List<int>();
        int current;
        int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PaperCutter" /> class.
        /// </summary>
        /// <param name="maxLength">The maximum length.</param>
        /// <param name="allLength">All lengths.</param>
        /// <param name="breaks">The break values.</param>
        public PaperCutter(int maxLength, int allLength, IEnumerable<int> breaks)
        {
            this.maxLength = maxLength;
            this.allLength = allLength;
            if (breaks != null)
            {
                this.breaks.AddRange(breaks);
                this.breaks.Sort();
            }
            this.current = 0;
        }

        /// <summary>
        /// Adds the break if first.
        /// </summary>
        /// <param name="offset">The offset.</param>
        public void AddBreakIfFirst(int offset)
        {
            if (offset < this.maxLength)
            {
                if (this.breaks.Count <= 0)
                {
                    this.breaks.Add(offset);
                }
                if (this.breaks[0] > offset)
                {
                    this.breaks.Insert(offset, 0);
                }
            }
        }

        /// <summary>
        /// Gets whether to use the next specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        public bool Next(out int size)
        {
            bool flag = true;
            if ((this.allLength >= 0) && (this.current >= this.allLength))
            {
                flag = false;
            }
            for (int i = 0; i < this.breaks.Count; i++)
            {
                int num2 = this.breaks[i];
                if (num2 >= (this.current + this.maxLength))
                {
                    break;
                }
                if (num2 > this.current)
                {
                    size = num2 - this.current;
                    this.current = num2;
                    return flag;
                }
            }
            size = this.maxLength;
            this.current += this.maxLength;
            return flag;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.current = 0;
        }

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        /// <value>The current value.</value>
        public int Current
        {
            get { return  this.current; }
            set { this.current = value; }
        }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>The maximum length.</value>
        public int MaxLength
        {
            get { return  this.maxLength; }
            set { this.maxLength = value; }
        }
    }
}

