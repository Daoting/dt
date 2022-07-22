#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent a excel column in <see cref="T:Dt.Xls.ExcelWorksheet" />
    /// </summary>
    public class ExcelColumn : IExcelColumn
    {
        private IExtendedFormat _columnFormat;
        private int _formatId = -1;
        private IExcelWorksheet _owner;
        private double _width = 8.0;

        /// <summary>
        /// Initialize a new instance of <see cref="T:Dt.Xls.ExcelColumn" />.
        /// </summary>
        /// <param name="workSheet">The owner <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        public ExcelColumn(IExcelWorksheet workSheet)
        {
            if (workSheet == null)
            {
                throw new ArgumentNullException("workSheet");
            }
            this._owner = workSheet;
            this.Width = 8.0;
            this.Collapsed = false;
            this.Visible = true;
        }

        private void InitExcelColumnDefaultFormat()
        {
            this._columnFormat = ExtendedFormat.Default;
            if (this._owner != null)
            {
                if (this._owner.ExtendedFormats == null)
                {
                    this._owner.ExtendedFormats = new Dictionary<int, IExtendedFormat>();
                }
                this.FormatId = this._owner.ExtendedFormats.Count;
                this._owner.ExtendedFormats.Add(this._owner.ExtendedFormats.Count, this._columnFormat);
            }
        }

        /// <summary>
        /// Set the format id of the column. The column will use the id to locate the <see cref="T:Dt.Xls.IExtendedFormat" /> from
        /// its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance
        /// </summary>
        /// <param name="id">The zero-based index of the format id of the column.</param>
        /// <remarks> If the passed value is -1, it means use the default format setting.</remarks>
        public void SetFormatId(int id)
        {
            if (id == -1)
            {
                this._formatId = id;
            }
            else
            {
                if (((this._owner == null) || (this._owner.ExtendedFormats == null)) || !this._owner.ExtendedFormats.ContainsKey(id))
                {
                    throw new ArgumentOutOfRangeException("id");
                }
                this._formatId = id;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelColumn" /> is collapsed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if collapsed; otherwise, <see langword="false" />.
        /// </value>
        public bool Collapsed { get; set; }

        /// <summary>
        /// Gets or sets the format of the column
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExtendedFormat" /> instance represents the format setting of the column.
        /// </value>
        public IExtendedFormat Format
        {
            get
            {
                if (this._columnFormat == null)
                {
                    if (((this._owner != null) && (this._owner.ExtendedFormats != null)) && this._owner.ExtendedFormats.ContainsKey(this.FormatId))
                    {
                        this._columnFormat = this._owner.ExtendedFormats[this.FormatId].Clone();
                        this.FormatId = this._owner.ExtendedFormats.Count;
                        this._owner.ExtendedFormats.Add(this.FormatId, this._columnFormat);
                    }
                    else
                    {
                        this.InitExcelColumnDefaultFormat();
                    }
                }
                return this._columnFormat;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Format");
                }
                this._columnFormat = value;
                if (this._owner != null)
                {
                    this._owner.ExtendedFormats = this._owner.ExtendedFormats ?? new Dictionary<int, IExtendedFormat>();
                    foreach (KeyValuePair<int, IExtendedFormat> pair in this._owner.ExtendedFormats)
                    {
                        if (pair.Value.Equals(this._columnFormat))
                        {
                            this.FormatId = pair.Key;
                            break;
                        }
                    }
                    this.FormatId = this._owner.ExtendedFormats.Count;
                    this._owner.ExtendedFormats.Add(this.FormatId, this._columnFormat);
                }
            }
        }

        /// <summary>
        /// Gets the format id of the column.
        /// </summary>
        /// <value>The format id of the column.</value>
        public int FormatId
        {
            get { return  this._formatId; }
            private set { this._formatId = value; }
        }

        /// <summary>
        /// Gets zero based index of the column.
        /// </summary>
        /// <value>The index of the column.</value>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets or sets the outline level of the column.
        /// </summary>
        /// <value>The outline level of the column</value>
        public byte OutLineLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the column is a page break column.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's a page break column; otherwise, <see langword="false" />.
        /// </value>
        public bool PageBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelColumn" /> is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if visible; otherwise, <see langword="false" />.
        /// </value>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        /// <value>The width of the column</value>
        public double Width
        {
            get { return  this._width; }
            set
            {
                if (Math.Abs((double) (value - 0.0)) < 0.001)
                {
                    this._width = 8.0;
                    this.Visible = false;
                }
                else
                {
                    if (value <= 0.0)
                    {
                        throw new InvalidOperationException(ResourceHelper.GetResourceString("columnWidthError"));
                    }
                    this._width = value;
                    if (!this.Visible)
                    {
                        this.Visible = true;
                    }
                }
            }
        }
    }
}

