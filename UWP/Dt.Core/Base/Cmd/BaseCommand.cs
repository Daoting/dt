#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Windows.Input;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 普通命令基类，实现ICommand接口
    /// </summary>
    public class BaseCommand : ICommand
    {
        bool _allowExecute;
        Action<object> _executeCallback;
        CmdHistory _history;

        /// <summary>
        /// 构造方法
        /// </summary>
        public BaseCommand()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_history">历史命令</param>
        public BaseCommand(CmdHistory p_history)
        {
            _history = p_history;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_executeCallback">命令回调方法</param>
        /// <param name="p_allowExecute">初始状态</param>
        public BaseCommand(Action<object> p_executeCallback, bool p_allowExecute = true)
        {
            _executeCallback = p_executeCallback;
            _allowExecute = p_allowExecute;
        }

        /// <summary>
        /// 获取设置当前状态下是否可执行命令
        /// </summary>
        public bool AllowExecute
        {
            get { return _allowExecute; }
            set
            {
                if (_allowExecute != value)
                {
                    _allowExecute = value;
                    OnCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="p_parameter"></param>
        protected virtual void DoExecute(object p_parameter)
        {
        }

        /// <summary>
        /// 执行撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        protected virtual void DoUndo(object p_parameter)
        {
        }

        #region ICommand 成员
        /// <summary>
        /// 当出现影响是否应执行该命令的更改时发生
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 定义用于确定此命令是否可以在其当前状态下执行的方法
        /// </summary>
        /// <param name="p_parameter">执行参数</param>
        /// <returns>是否允许执行</returns>
        public bool CanExecute(object p_parameter = null)
        {
            return _allowExecute;
        }

        /// <summary>
        /// 定义在调用此命令时调用的方法
        /// </summary>
        /// <param name="p_parameter">执行参数</param>
        public void Execute(object p_parameter = null)
        {
            if (_allowExecute)
            {
                if (_executeCallback != null)
                    _executeCallback(p_parameter);
                else
                    DoExecute(p_parameter);

                if (_history != null)
                    _history.RecordAction(new CmdAction(this, p_parameter));
            }
        }

        #endregion

        /// <summary>
        /// 撤消
        /// </summary>
        /// <param name="p_parameter"></param>
        public void Undo(object p_parameter = null)
        {
            if (_allowExecute)
                DoUndo(p_parameter);
        }

        /// <summary>
        /// 重做
        /// </summary>
        /// <param name="p_parameter"></param>
        public void Redo(object p_parameter = null)
        {
            if (_allowExecute)
            {
                if (_executeCallback != null)
                    _executeCallback(p_parameter);
                else
                    DoExecute(p_parameter);
            }
        }

        /// <summary>
        /// 触发CanExecuteChanged事件
        /// </summary>
        void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

