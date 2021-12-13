#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IExcelStyle" /> used to represents custom cell style.
    /// </summary>
    public class CustomExcelStyle : IExcelStyle
    {
        private ExtendedFormat _format;

        /// <summary>
        /// Copy and create a new instance of <see cref="T:Dt.Xls.CustomExcelStyle" /> instance.
        /// </summary>
        /// <returns></returns>
        public CustomExcelStyle Copy()
        {
            return new CustomExcelStyle { Name = this.Name, Format = this.Format.Clone() };
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Xls.IExtendedFormat" /> used in the cell style.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExtendedFormat" /> represents the cell style format setting
        /// </value>
        public IExtendedFormat Format
        {
            get
            {
                if (this._format == null)
                {
                    this._format = new ExtendedFormat();
                }
                return this._format;
            }
            set { this._format = value as ExtendedFormat; }
        }

        /// <summary>
        /// Gets a value indicating whether the style is built-in style.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this style is built-in style; otherwise, <see langword="false" />.
        /// </value>
        public bool IsBuiltInStyle
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets the name of the style
        /// </summary>
        /// <value>The name of the style</value>
        public string Name { get; set; }
    }
}

