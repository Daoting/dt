#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class LegacyMaskChar : LegacyMaskPrimitive
    {
        #region 成员变量
        string _capturing;
        int _maxMatches;
        int _minMatches;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="capturing"></param>
        /// <param name="caseConversion"></param>
        /// <param name="minMatches"></param>
        /// <param name="maxMatches"></param>
        public LegacyMaskChar(string capturing, char caseConversion, int minMatches, int maxMatches) : base(caseConversion)
        {
            this._capturing = capturing;
            this._minMatches = minMatches;
            this._maxMatches = maxMatches;
            Regex.IsMatch("q", this.CapturingExpression);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override string CapturingExpression
        {
            get { return this._capturing; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsLiteral
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MaxMatches
        {
            get { return this._maxMatches; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinMatches
        {
            get { return this._minMatches; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void PatchMatches(int min, int max)
        {
            this._minMatches = min;
            this._maxMatches = max;
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
            if (elementValue.Length < this.MaxMatches)
            {
                int count = (elementValue.Length >= this.MinMatches) ? 1 : (this.MinMatches - elementValue.Length);
                return (elementValue + new string(blank, count));
            }
            return elementValue;
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
            if (elementValue.Length < this.MinMatches)
            {
                return (elementValue + new string(blank, this.MinMatches - elementValue.Length));
            }
            return elementValue;
        }
        #endregion
    }
}

