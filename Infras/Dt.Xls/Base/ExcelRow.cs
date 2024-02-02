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
    /// represent a single row in  <see cref="T:Dt.Xls.ExcelWorksheet" />
    /// </summary>
    public class ExcelRow : IExcelRow
    {
        private bool _customHeight = true;
        private int _formatId = -1;
        private double _height = 15.0;
        private IExcelWorksheet _owner;
        private IExtendedFormat _rowFormat;

        /// <summary>
        /// Initialize a new instance of <see cref="T:Dt.Xls.ExcelRow" />.
        /// </summary>
        /// <param name="workSheet">The owner <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</param>
        public ExcelRow(IExcelWorksheet workSheet)
        {
            if (workSheet == null)
            {
                throw new ArgumentNullException("workSheet");
            }
            this._owner = workSheet;
            this.Collapsed = false;
            this.Height = 15.0;
            this.Visible = true;
        }

        private void InitExcelRowDefaultFormat()
        {
            this._rowFormat = ExtendedFormat.Default;
            if (this._owner != null)
            {
                if (this._owner.ExtendedFormats == null)
                {
                    this._owner.ExtendedFormats = new Dictionary<int, IExtendedFormat>();
                }
                this._formatId = this._owner.ExtendedFormats.Count;
                this._owner.ExtendedFormats.Add(this._owner.ExtendedFormats.Count, this._rowFormat);
            }
        }

        /// <summary>
        /// Set the format id of the row. The row will use the id to locate the <see cref="T:Dt.Xls.IExtendedFormat" /> from
        /// its parent <see cref="T:Dt.Xls.IExcelWorksheet" /> instance
        /// </summary>
        /// <param name="id">The zero-based index of the format id of the row.</param>
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
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelRow" /> is collapsed.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if collapsed; otherwise, <see langword="false" />.
        /// </value>
        public bool Collapsed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the height of this row is manually set.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's manually set; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// If the value is false, excel will invoke AutoFit row height on the current row.
        /// </remarks>
        public bool CustomHeight
        {
            get { return  this._customHeight; }
            set { this._customHeight = value; }
        }

        /// <summary>
        /// Gets or sets the format of the row.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExtendedFormat" /> instance represents the format setting of the row.
        /// </value>
        public IExtendedFormat Format
        {
            get
            {
                if (this._rowFormat == null)
                {
                    if (((this._owner != null) && (this._owner.ExtendedFormats != null)) && this._owner.ExtendedFormats.ContainsKey(this._formatId))
                    {
                        this._rowFormat = this._owner.ExtendedFormats[this._formatId].Clone();
                        this._formatId = this._owner.ExtendedFormats.Count;
                        this._owner.ExtendedFormats.Add(this._formatId, this._rowFormat);
                    }
                    else
                    {
                        this.InitExcelRowDefaultFormat();
                    }
                }
                return this._rowFormat;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Format");
                }
                this._rowFormat = value;
                if (this._owner != null)
                {
                    this._owner.ExtendedFormats = this._owner.ExtendedFormats ?? new Dictionary<int, IExtendedFormat>();
                    foreach (KeyValuePair<int, IExtendedFormat> pair in this._owner.ExtendedFormats)
                    {
                        if (pair.Value.Equals(this._rowFormat))
                        {
                            this._formatId = pair.Key;
                            break;
                        }
                    }
                    this._formatId = this._owner.ExtendedFormats.Count;
                    this._owner.ExtendedFormats.Add(this._formatId, this._rowFormat);
                }
            }
        }

        /// <summary>
        /// Gets the format id of the row.
        /// </summary>
        /// <value>The format id of the row.</value>
        public int FormatId
        {
            get { return  this._formatId; }
            private set { this._formatId = value; }
        }

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the two.</value>
        public double Height
        {
            get { return  this._height; }
            set
            {
                if (Math.Abs((double) (value - 0.0)) < 0.001)
                {
                    this.Visible = false;
                    this._height = 15.0;
                }
                else
                {
                    if (value <= 0.0)
                    {
                        throw new InvalidOperationException(ResourceHelper.GetResourceString("rowHeightError"));
                    }
                    this._height = value;
                    if (!this.Visible)
                    {
                        this.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets zero-based index of the row.
        /// </summary>
        /// <value>The index of the row</value>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets or sets the outline level of the row.
        /// </summary>
        /// <value>The outline level of the row</value>
        public byte OutLineLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the row is page break row.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the row is page break row; otherwise, <see langword="false" />.
        /// </value>
        public bool PageBreak { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Dt.Xls.IExcelRow" /> is visible.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if visible; otherwise, <see langword="false" />.
        /// </value>
        public bool Visible { get; set; }
    }
}

