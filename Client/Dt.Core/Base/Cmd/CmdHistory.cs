#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 命令的撤消与重做管理类
    /// </summary>
    public class CmdHistory
    {
        const int _depth = 1000;
        readonly Stack<CmdAction> _redoStack = new Stack<CmdAction>();
        readonly Stack<CmdAction> _undoStack = new Stack<CmdAction>();

        /// <summary>
        /// 命令变化事件
        /// </summary>
        public event EventHandler CmdChanged;

        /// <summary>
        /// 添加可撤消的动作
        /// </summary>
        /// <param name="p_action"></param>
        public void RecordAction(CmdAction p_action)
        {
            if (_undoStack.Count > _depth)
                _undoStack.Pop();
            _undoStack.Push(p_action);
            _redoStack.Clear();
            OnCmdChanged();
        }

        /// <summary>
        /// 执行撤消操作
        /// </summary>
        public void Undo()
        {
            if (CanUndo)
            {
                CmdAction action = _undoStack.Pop();
                action.Undo();
                if (_redoStack.Count < _depth)
                    _redoStack.Push(action);
                OnCmdChanged();
            }
            else
            {
                Kit.Msg("没有可撤消的操作！");
            }
        }

        /// <summary>
        /// 执行重做操作
        /// </summary>
        public void Redo()
        {
            if (CanRedo)
            {
                CmdAction action = _redoStack.Pop();
                action.Redo();
                if (_undoStack.Count < _depth)
                    _undoStack.Push(action);
                OnCmdChanged();
            }
            else
            {
                Kit.Msg("无操作可重做！");
            }
        }

        /// <summary>
        /// 清空所有撤消、重做的动作
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            OnCmdChanged();
        }

        /// <summary>
        /// 是否可重做
        /// </summary>
        public bool CanRedo
        {
            get { return _redoStack.Count > 0; }
        }

        /// <summary>
        /// 是否可撤消
        /// </summary>
        public bool CanUndo
        {
            get { return _undoStack.Count > 0; }
        }

        void OnCmdChanged()
        {
            CmdChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
