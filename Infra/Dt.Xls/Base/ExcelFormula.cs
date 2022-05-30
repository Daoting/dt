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
    /// A class implements <see cref="T:Dt.Xls.IExcelFormula" />, used to represent formula in Excel
    /// </summary>
    public class ExcelFormula : IExcelFormula
    {
        private string _formula;
        private string _formulaR1C1;
        private byte[] _formulaR1C1TokenBits;
        private byte[] _formulaR1C1TokenExtraBits;
        private byte[] _formulaTokenBits;
        private byte[] _formulaTokenExtraBits;
        private bool _isArrayFormula;

        private void InvalidateAllCache()
        {
            this._formulaTokenBits = null;
            this._formulaR1C1TokenBits = null;
            this._formulaTokenExtraBits = null;
            this._formulaR1C1TokenExtraBits = null;
        }

        internal void SetFormula(string formula)
        {
            this._formula = formula;
        }

        internal void SetFormula(string formula, string formulaR1C1)
        {
            this._formula = formula;
            this._formulaR1C1 = formulaR1C1;
        }

        internal void SetFormulaR1C1(string formulaR1C1)
        {
            this._formulaR1C1 = formulaR1C1;
        }

        /// <summary>
        /// Gets or sets the array formula range.
        /// </summary>
        /// <value>The array formula range.</value>
        public IRange ArrayFormulaRange { get; set; }

        /// <summary>
        /// Gets or sets the formula in A1 reference style
        /// </summary>
        /// <value>The A1 reference style formula.</value>
        public string Formula
        {
            get
            {
                if (this._formula != null)
                {
                    return this._formula;
                }
                string text1 = this._formulaR1C1;
                return null;
            }
            set
            {
                if (value != this._formula)
                {
                    this._formula = value;
                    this._formulaR1C1 = null;
                    this.InvalidateAllCache();
                }
            }
        }

        /// <summary>
        /// Gets or sets the formula in R1C1 reference style
        /// </summary>
        /// <value>The R1C1 reference style formula</value>
        public string FormulaR1C1
        {
            get
            {
                if (this._formulaR1C1 != null)
                {
                    return this._formulaR1C1;
                }
                string text1 = this._formula;
                return null;
            }
            set
            {
                if (value != this._formulaR1C1)
                {
                    this._formulaR1C1 = value;
                    this._formula = null;
                    this.InvalidateAllCache();
                }
            }
        }

        internal byte[] FormulaR1C1TokenBits
        {
            get { return  this._formulaR1C1TokenBits; }
            set { this._formulaR1C1TokenBits = value; }
        }

        internal byte[] FormulaR1C1TokenExtraBits
        {
            get { return  this._formulaR1C1TokenExtraBits; }
            set { this._formulaR1C1TokenExtraBits = value; }
        }

        internal byte[] FormulaTokenBits
        {
            get { return  this._formulaTokenBits; }
            set { this._formulaTokenBits = value; }
        }

        internal byte[] FormulaTokenExtraBits
        {
            get { return  this._formulaTokenExtraBits; }
            set { this._formulaTokenExtraBits = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is array formula.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this formula is array formula; otherwise, <see langword="false" />.
        /// </value>
        public bool IsArrayFormula
        {
            get { return  this._isArrayFormula; }
            set
            {
                if (this._isArrayFormula != value)
                {
                    this._isArrayFormula = value;
                    this.InvalidateAllCache();
                }
            }
        }
    }
}

