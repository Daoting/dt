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

namespace Dt.Base.Report
{
    /// <summary>
    /// 命令的撤消与重做管理类
    /// </summary>
    internal class RptCmdHistory
    {
        const int _depth = 1000;
        readonly Stack<HistoryCmdAction> _redoStack = new Stack<HistoryCmdAction>();
        readonly Stack<HistoryCmdAction> _undoStack = new Stack<HistoryCmdAction>();

        /// <summary>
        /// 数据源修改状态变化事件
        /// </summary>
        public event EventHandler<bool> DirtyChanged;

        /// <summary>
        /// 添加可撤消的动作
        /// </summary>
        /// <param name="p_action"></param>
        public void RecordAction(HistoryCmdAction p_action)
        {
            if (_undoStack.Count > _depth)
                _undoStack.Pop();
            _undoStack.Push(p_action);
            _redoStack.Clear();
            if (_undoStack.Count == 1 && DirtyChanged != null)
                DirtyChanged(this, true);
        }

        /// <summary>
        /// 执行撤消操作
        /// </summary>
        public void Undo()
        {
            if (CanUndo)
            {
                HistoryCmdAction action = _undoStack.Pop();
                action.Undo();
                if (_redoStack.Count < _depth)
                    _redoStack.Push(action);
                if (_undoStack.Count == 0 && DirtyChanged != null)
                    DirtyChanged(this, false);
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
                HistoryCmdAction action = _redoStack.Pop();
                action.Redo();
                if (_undoStack.Count < _depth)
                    _undoStack.Push(action);
                if (_undoStack.Count == 1 && DirtyChanged != null)
                    DirtyChanged(this, true);
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
            bool canUndo = CanUndo;
            _undoStack.Clear();
            _redoStack.Clear();
            if (canUndo && DirtyChanged != null)
                DirtyChanged(this, false);
        }

        public bool CanRedo
        {
            get { return _redoStack.Count > 0; }
        }

        public bool CanUndo
        {
            get { return _undoStack.Count > 0; }
        }
    }

    /// <summary>
    /// 可撤消、重做的动作描述类
    /// </summary>
    internal class HistoryCmdAction
    {
        RptCmdBase _cmd;
        object _args;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_cmd"></param>
        /// <param name="p_args"></param>
        public HistoryCmdAction(RptCmdBase p_cmd, object p_args)
        {
            _cmd = p_cmd;
            _args = p_args;
        }

        /// <summary>
        /// 撤消
        /// </summary>
        public void Undo()
        {
            _cmd.Undo(_args);
        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            _cmd.Execute(_args);
        }
    }
}
