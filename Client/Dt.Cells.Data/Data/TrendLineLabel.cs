#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the label for the trendline
    /// </summary>
    internal class TrendLineLabel
    {
        Dt.Xls.Chart.RichText _richText;
        string _text;

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        internal IFormatter Formatter { get; set; }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        internal Dt.Cells.Data.Layout Layout { get; set; }

        /// <summary>
        /// Gets or sets the rich text.
        /// </summary>
        /// <value>
        /// The rich text.
        /// </value>
        internal Dt.Xls.Chart.RichText RichText
        {
            get { return  this._richText; }
            set
            {
                if (value != this._richText)
                {
                    this._richText = value;
                    this._text = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        internal string Text
        {
            get { return  this._text; }
            set
            {
                if (value != this.Text)
                {
                    this._text = value;
                    this._richText = null;
                }
            }
        }
    }
}

