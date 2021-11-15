#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Globalization;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class LegacyMaskLiteral : LegacyMaskPrimitive
    {
        #region 成员变量
        char _literal;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override string CapturingExpression
        {
            get { return string.Format(CultureInfo.InvariantCulture, ((this._literal == '^') || (this._literal == '\\')) ? @"[\{0}]" : "[{0}]", new object[] { this._literal }); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsLiteral
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MaxMatches
        {
            get { return 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinMatches
        {
            get { return 1; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="literal"></param>
        public LegacyMaskLiteral(char literal)
            : base('A')
        {
            this._literal = literal;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementValue"></param>
        /// <param name="blank"></param>
        /// <returns></returns>
        public override string GetDisplayText(string elementValue, char blank)
        {
            return this._literal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementValue"></param>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <returns></returns>
        public override string GetEditText(string elementValue, char blank, bool saveLiteral)
        {
            if (!saveLiteral)
            {
                return string.Empty;
            }
            return this._literal.ToString();
        }
        #endregion
    }
}

