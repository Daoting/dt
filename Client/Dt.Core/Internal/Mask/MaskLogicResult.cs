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
    public class MaskLogicResult
    {
        #region 成员变量
        int _cursorPosition;
        string _editText;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="editText"></param>
        /// <param name="cursorPosition"></param>
        public MaskLogicResult(string editText, int cursorPosition)
        {
            this._editText = editText;
            this._cursorPosition = cursorPosition;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int CursorPosition
        {
            get { return this._cursorPosition; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string EditText
        {
            get { return this._editText; }
        }
        #endregion
    }
}

