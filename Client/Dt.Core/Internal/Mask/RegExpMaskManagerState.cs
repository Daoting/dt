#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpMaskManagerState : MaskManagerState
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        public static RegExpMaskManagerState Empty = new RegExpMaskManagerState(string.Empty, 0, 0);
        #endregion

        #region 成员变量
        int _cursorPosition;
        string _editText;
        int _selectionAnchor;
        #endregion

        #region 构造方法
        /// <summary>
        /// RegExp掩码状态管理者
        /// </summary>
        /// <param name="editText">掩码表达式</param>
        /// <param name="cursorPosition">区域信息</param>
        /// <param name="selectionAnchor"></param>
        public RegExpMaskManagerState(string editText, int cursorPosition, int selectionAnchor)
        {
            _editText = editText;
            _cursorPosition = cursorPosition;
            _selectionAnchor = selectionAnchor;
        }
        #endregion
        
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int CursorPosition
        {
            get { return _cursorPosition; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string EditText
        {
            get { return _editText; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SelectionAnchor
        {
            get { return _selectionAnchor; }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparedState"></param>
        /// <returns></returns>
        public override bool IsSame(MaskManagerState comparedState)
        {
            if (comparedState == null)
            {
                return false;
            }
            if (base.GetType() != comparedState.GetType())
            {
                throw new NotImplementedException("Internal error");
            }
            RegExpMaskManagerState state = (RegExpMaskManagerState) comparedState;
            if (EditText != state.EditText)
            {
                return false;
            }
            if (CursorPosition != state.CursorPosition)
            {
                return false;
            }
            if (SelectionAnchor != state.SelectionAnchor)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}

