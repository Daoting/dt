#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent a parser context which is used while <see cref="T:Dt.CalcEngine.CalcParser" /> parse or unparse process.
    /// </summary>
    public class CalcParserContext
    {
        private int _baseColumnIndex;
        private int _baseRowIndex;
        private CultureInfo _culture = CultureInfo.InvariantCulture;
        private bool _useR1C1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcParserContext" /> class.
        /// </summary>
        /// <param name="useR1C1">if set to <see langword="true" /> [use r1 c1].</param>
        /// <param name="baseRowIndex">Index of the base row.</param>
        /// <param name="baseColumnIndex">Index of the base column.</param>
        /// <param name="culture">The culture.</param>
        public CalcParserContext(bool useR1C1 = false, int baseRowIndex = 0, int baseColumnIndex = 0, CultureInfo culture = null)
        {
            this._useR1C1 = useR1C1;
            this._baseRowIndex = baseRowIndex;
            this._baseColumnIndex = baseColumnIndex;
            this._culture = culture ?? CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets the external source.
        /// </summary>
        /// <param name="workbookName">Name of the workbook.</param>
        /// <param name="worksheetName">Name of the worksheet.</param>
        /// <returns>
        /// An <see cref="T:Dt.CalcEngine.ICalcSource" /> indicates the source which name is <paramref name="worksheetName" />.
        /// </returns>
        public virtual ICalcSource GetExternalSource(string workbookName, string worksheetName)
        {
            return null;
        }

        /// <summary>
        /// Gets the external source token.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A <see cref="T:System.String" /> indicates the external source token.</returns>
        public virtual string GetExternalSourceToken(ICalcSource source)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public int Column
        {
            get
            {
                return this._baseColumnIndex;
            }
        }

        /// <summary>
        /// Gets culture for comparing string token while parsing.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Globalization.CultureInfo" /> indicates the culture used in parsing.
        /// The default value is <see cref="P:System.Globalization.CultureInfo.InvariantCulture" />.
        /// </value>
        public CultureInfo Culture
        {
            get
            {
                return this._culture;
            }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>The row.</value>
        public int Row
        {
            get
            {
                return this._baseRowIndex;
            }
        }

        /// <summary>
        /// Gets whether using R1C1 notation for cell reference and range reference.
        /// </summary>
        /// <value>
        /// A <see langword="bool" /> indicates whether using R1C1 notation.
        /// The default value is <see langword="false" />.
        /// </value>
        public bool UseR1C1
        {
            get
            {
                return this._useR1C1;
            }
        }
    }
}

