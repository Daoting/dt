#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class LegacyMaskPrimitive
    {
        #region 成员变量
        char _CaseConversion;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public abstract string CapturingExpression { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsLiteral { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract int MaxMatches { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract int MinMatches { get; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public char GetAcceptableChar(char input)
        {
            char ch;
            char caseConversion = this._CaseConversion;
            if (caseConversion != 'L')
            {
                if (caseConversion != 'U')
                {
                    ch = input;
                    if (!this.IsAcceptableStrong(ch))
                    {
                        ch = char.ToUpper(input);
                        if (!this.IsAcceptableStrong(ch))
                        {
                            ch = char.ToLower(input);
                        }
                    }
                }
                else
                {
                    ch = char.ToUpper(input);
                }
            }
            else
            {
                ch = char.ToLower(input);
            }
            if (!this.IsAcceptableStrong(ch))
            {
                throw new InvalidOperationException();
            }
            return ch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsAcceptable(char input)
        {
            switch (this._CaseConversion)
            {
                case 'L':
                    return this.IsAcceptableStrong(char.ToLower(input));

                case 'U':
                    return this.IsAcceptableStrong(char.ToUpper(input));
            }
            if (!this.IsAcceptableStrong(input) && !this.IsAcceptableStrong(char.ToUpper(input)))
            {
                return this.IsAcceptableStrong(char.ToLower(input));
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsAcceptableStrong(char input)
        {
            return Regex.IsMatch(input.ToString(), this.CapturingExpression);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caseConversion"></param>
        protected LegacyMaskPrimitive(char caseConversion)
        {
            this._CaseConversion = caseConversion;
        }
        #endregion
        
        #region 抽象方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementValue"></param>
        /// <param name="blank"></param>
        /// <returns></returns>
        public abstract string GetDisplayText(string elementValue, char blank);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementValue"></param>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <returns></returns>
        public abstract string GetEditText(string elementValue, char blank, bool saveLiteral);
        #endregion
    }
}

