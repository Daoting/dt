#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 掩码员抽象基类
    /// 子类有 MaskManagerStated, DateTimeMaskManager
    /// </summary>
    public abstract class MaskManager
    {
        #region 事件
        /// <summary>
        /// 文本变化事件
        /// </summary>
        public event EventHandler EditTextChanged;

        /// <summary>
        ///准备文本变化事件
        /// </summary>
        public event MaskChangingEventHandler EditTextChanging;

        /// <summary>
        /// 编辑事件(虽未修改)
        /// </summary>
        public event CancelEventHandler LocalEditAction;
        #endregion

        #region 属性
        /// <summary>
        /// 是否可撤消
        /// </summary>
        public abstract bool CanUndo { get; }

        /// <summary>
        /// 当前光标位置
        /// </summary>
        public abstract int DisplayCursorPosition { get; }

        /// <summary>
        /// 选择区域的开始位置（在开头或结尾）
        /// </summary>
        public abstract int DisplaySelectionAnchor { get; }

        /// <summary>
        /// 选择区域的起始位置
        /// </summary>
        public int DisplaySelectionStart
        {
            get
            {
                if (DisplayCursorPosition >= DisplaySelectionAnchor)
                {
                    return DisplaySelectionAnchor;
                }
                return DisplayCursorPosition;
            }
        }

        /// <summary>
        /// 选择区域的结束位置
        /// </summary>
        public int DisplaySelectionEnd
        {
            get
            {
                if (DisplayCursorPosition <= DisplaySelectionAnchor)
                {
                    return DisplaySelectionAnchor;
                }
                return DisplayCursorPosition;
            }
        }

        /// <summary>
        /// 选中区域长度
        /// </summary>
        public int DisplaySelectionLength
        {
            get
            {
                if (DisplayCursorPosition <= DisplaySelectionAnchor)
                {
                    return (DisplaySelectionAnchor - DisplayCursorPosition);
                }
                return (DisplayCursorPosition - DisplaySelectionAnchor);
            }
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        public abstract string DisplayText { get; }

        /// <summary>
        /// 值与文本是否相同
        /// </summary>
        public abstract bool IsEditValueDifferFromEditText { get; }

        /// <summary>
        /// 光标是否在最后
        /// </summary>
        public virtual bool IsFinal
        {
            get { return false; }
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        public virtual bool IsMatch
        {
            get { return true; }
        }

        /// <summary>
        /// 是否为纯文本
        /// </summary>
        public abstract bool IsPlainTextLike { get; }

        /// <summary>
        /// 是否存在选中内容
        /// </summary>
        public bool IsSelection
        {
            get { return (DisplayCursorPosition != DisplaySelectionAnchor); }
        }
        #endregion

        #region 常用操作
        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <param name="initialEditValue"></param>
        public abstract void SetInitialEditValue(object initialEditValue);

        /// <summary>
        /// 设置初始文本
        /// </summary>
        /// <param name="initialEditText"></param>
        public abstract void SetInitialEditText(string initialEditText);

        /// <summary>
        /// 获取掩码员的当前值
        /// </summary>
        /// <returns></returns>
        public abstract object GetCurrentEditValue();

        /// <summary>
        /// 获取当前文本
        /// </summary>
        /// <returns></returns>
        public abstract string GetCurrentEditText();

        /// <summary>
        /// 新输入
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public abstract bool Insert(string insertion);

        /// <summary>
        /// 全选清除
        /// </summary>
        public virtual void ClearAfterSelectAll()
        {
            PrepareForInsertAfterSelectAll();
            if (IsSelection)
            {
                Delete();
            }
        }

        /// <summary>
        /// 退格操作
        /// </summary>
        /// <returns></returns>
        public abstract bool Backspace();

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <returns></returns>
        public abstract bool Delete();

        /// <summary>
        /// 刷新未提交的操作内容
        /// </summary>
        /// <returns></returns>
        public virtual bool FlushPendingEditActions()
        {
            return false;
        }

        /// <summary>
        /// 下调
        /// </summary>
        /// <returns></returns>
        public abstract bool SpinDown();

        /// <summary>
        /// 上调
        /// </summary>
        /// <returns></returns>
        public abstract bool SpinUp();

        /// <summary>
        /// 撤消操作
        /// </summary>
        /// <returns></returns>
        public abstract bool Undo();

        /// <summary>
        /// 全选准备插入
        /// </summary>
        public virtual void PrepareForInsertAfterSelectAll()
        {
            PrepareForCursorMoveAfterSelectAll();
        }

        #endregion

        #region 光标操作
        /// <summary>
        /// 光标置最后
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public abstract bool CursorEnd(bool forceSelection);

        /// <summary>
        /// 光标置最前
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public abstract bool CursorHome(bool forceSelection);

        /// <summary>
        /// 左移光标
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public bool CursorLeft(bool forceSelection)
        {
            return CursorLeft(forceSelection, false);
        }

        /// <summary>
        /// 左移光标
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public abstract bool CursorLeft(bool forceSelection, bool isNeededKeyCheck);

        /// <summary>
        /// 右移光标
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public bool CursorRight(bool forceSelection)
        {
            return CursorRight(forceSelection, false);
        }

        /// <summary>
        /// 右移光标
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public abstract bool CursorRight(bool forceSelection, bool isNeededKeyCheck);

        /// <summary>
        /// 选中所有
        /// </summary>
        public virtual void PrepareForCursorMoveAfterSelectAll()
        {
            CursorToDisplayPosition(DisplayText.Length, false);
            CursorToDisplayPosition(0, true);
        }

        /// <summary>
        /// 移动光标位置，并可在原位置和当前位置之间置选中状态
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public abstract bool CursorToDisplayPosition(int newPosition, bool forceSelection);

        #endregion

        #region 内部方法
        /// <summary>
        /// 引发编辑文本变化事件
        /// </summary>
        /// <param name="newEditValue"></param>
        /// <returns></returns>
        protected virtual bool RaiseEditTextChanging(object newEditValue)
        {
            if (EditTextChanging != null)
            {
                MaskChangingEventArgs e = new MaskChangingEventArgs(GetCurrentEditValue(), newEditValue);
                EditTextChanging(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// 触发文本变化事件
        /// </summary>
        protected void RaiseEditTextChanged()
        {
            EditTextChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 引发编辑事件，提供给外部决定是否可编辑，目前只提供给DatTimeMaskManager使用
        /// </summary>
        /// <returns></returns>
        protected bool RaiseModifyWithoutEditValueChange()
        {
            if (LocalEditAction != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                LocalEditAction(this, e);
                if (e.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MaskChangingEventHandler(object sender, MaskChangingEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class MaskChangingEventArgs : CancelEventArgs
    {
        #region 成员变量
        object _newValue;
        object _oldValue;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public MaskChangingEventArgs(object oldValue, object newValue)
            : this(oldValue, newValue, false)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="cancel"></param>
        public MaskChangingEventArgs(object oldValue, object newValue, bool cancel)
            : base(cancel)
        {
            this._oldValue = oldValue;
            this._newValue = newValue;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public object NewValue
        {
            get { return this._newValue; }
            set { this._newValue = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public object OldValue
        {
            get { return this._oldValue; }
        }
        #endregion
    }
}