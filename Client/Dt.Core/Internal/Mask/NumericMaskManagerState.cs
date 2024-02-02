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
    public class NumericMaskManagerState : RegExpMaskManagerState
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        public static readonly NumericMaskManagerState NullInstance = new NumericMaskManagerState();
        #endregion

        #region 成员变量
        readonly bool _fIsNegative;
        readonly bool _fIsNull;
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public bool IsNegative
        {
            get { return this._fIsNegative; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsNull
        {
            get { return this._fIsNull; }
        }
        #endregion 

        #region 外部方法
        /// <summary>
        /// 数字型掩码状态
        /// </summary>
        /// <param name="editText">掩码表达式</param>
        /// <param name="cursorPosition">区域信息</param>
        /// <param name="selectionAnchor"></param>
        /// <param name="isNegative">是否被拒绝</param>
        public NumericMaskManagerState(string editText, int cursorPosition, int selectionAnchor, bool isNegative)
            : base(editText, cursorPosition, selectionAnchor)
        {
            this._fIsNegative = isNegative;
            this._fIsNull = false;
        }
        #endregion

        #region 内部方法
        NumericMaskManagerState()
            : base(string.Empty, 0, 0)
        {
            this._fIsNegative = false;
            this._fIsNull = true;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 比较掩码状态是否和当前状态相同
        /// </summary>
        /// <param name="comparedState">掩码状态实例</param>
        /// <returns></returns>
        public override bool IsSame(MaskManagerState comparedState)
        {
            return ((base.IsSame(comparedState) && (this.IsNegative == ((NumericMaskManagerState)comparedState).IsNegative)) && (this.IsNull == ((NumericMaskManagerState)comparedState).IsNull));
        }
        #endregion        
    }
}
