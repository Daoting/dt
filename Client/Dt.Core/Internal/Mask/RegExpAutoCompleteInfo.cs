#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpAutoCompleteInfo
    {
        #region 成员变量
        readonly char _autoCompleteChar;
        readonly DfaAutoCompleteType _autoCompleteType;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public char AutoCompleteChar
        {
            get { return this._autoCompleteChar; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DfaAutoCompleteType DfaAutoCompleteType
        {
            get { return this._autoCompleteType; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="autoCompleteType"></param>
        /// <param name="autoCompleteChar"></param>
        public RegExpAutoCompleteInfo(DfaAutoCompleteType autoCompleteType, char autoCompleteChar)
        {
            this._autoCompleteType = autoCompleteType;
            this._autoCompleteChar = autoCompleteChar;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public enum DfaAutoCompleteType
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        ExactChar,

        /// <summary>
        /// 
        /// </summary>
        Final,

        /// <summary>
        /// 
        /// </summary>
        FinalOrExactBeforeNone,

        /// <summary>
        /// 
        /// </summary>
        FinalOrExactBeforeFinal,

        /// <summary>
        /// 
        /// </summary>
        FinalOrExactBeforeFinalOrNone
    }

}

