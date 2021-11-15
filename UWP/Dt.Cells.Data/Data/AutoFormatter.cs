#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an automatic format.
    /// </summary>
    public sealed class AutoFormatter : IFormatter, IColorFormatter, ICloneable
    {
        GeneralFormatter _innerformatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.AutoFormatter" /> class.
        /// </summary>
        /// <param name="innerformatter">The inner formatter.</param>
        public AutoFormatter(GeneralFormatter innerformatter)
        {
            this._innerformatter = innerformatter;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            return new AutoFormatter((this._innerformatter == null) ? null : (this._innerformatter.Clone() as GeneralFormatter));
        }

        /// <summary>
        /// Determines whether the specified formatter is equal to the current formatter.
        /// </summary>
        /// <param name="obj">The formatter to compare with the current formatter.</param>
        /// <returns>
        /// <c>true</c> if the specified formatter is equal to the current formatter; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            AutoFormatter formatter = obj as AutoFormatter;
            return ((formatter != null) && this._innerformatter.Equals(formatter._innerformatter));
        }

        /// <summary>
        /// Formats the specified object as a string.
        /// </summary>
        /// <param name="obj">The object with cell data to format.</param>
        /// <returns>
        /// Returns the formatted string.
        /// </returns>
        public string Format(object obj)
        {
            Color? nullable;
            return ((IColorFormatter) this).Format(obj, out nullable);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return this._innerformatter.GetHashCode();
        }

        string IColorFormatter.Format(object obj, out Color? conditionalForeColor)
        {
            conditionalForeColor = null;
            if (this._innerformatter != null)
            {
                return this._innerformatter.Format(obj, out conditionalForeColor);
            }
            if (obj != null)
            {
                return obj.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public object Parse(string text)
        {
            if (this._innerformatter == null)
            {
                return text;
            }
            return this._innerformatter.Parse(text);
        }

        /// <summary>
        /// The expression that is used to format and parse.
        /// </summary>
        public string FormatString
        {
            get { return  this._innerformatter.FormatString; }
        }

        bool IColorFormatter.HasFormatedColor
        {
            get { return  this._innerformatter.HasFormatedColor; }
        }

        /// <summary>
        /// Gets or sets the inner formatter.
        /// </summary>
        /// <value>
        /// The inner formatter.
        /// </value>
        public GeneralFormatter InnerFormatter
        {
            get { return  this._innerformatter; }
            set { this._innerformatter = value; }
        }
    }
}

