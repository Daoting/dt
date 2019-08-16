#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Windows.Input;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 回调命令类，封装对外部回调方法的调用，与DataCmd配合使用
    /// </summary>
    public class RelayCommand : ICommand
    {
        readonly DataCmd _viewModel;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_model"></param>
        public RelayCommand(DataCmd p_model)
        {
            _viewModel = p_model;
        }

        /// <summary>
        /// 获取设置命令的回调方法
        /// </summary>
        public Action<DataCmd> Callback { get; set; }

        /// <summary>
        /// 获取设置判断命令是否可执行的回调方法
        /// </summary>
        public Func<bool> CanExec { get; set; }

        /// <summary>
        /// 触发CanExecuteChanged事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand
        /// <summary>
        /// CanExecute更改时发生
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 定义用于确定此命令是否可以在其当前状态下执行的方法
        /// </summary>
        /// <param name="parameter">此命令使用的数据</param>
        /// <returns>如果可以执行此命令，则为 true；否则为 false</returns>
        public bool CanExecute(object parameter)
        {
            return CanExec == null ? true : CanExec();
        }

        /// <summary>
        /// 调用此命令时调用的方法
        /// </summary>
        /// <param name="parameter">命令参数</param>
        public void Execute(object parameter)
        {
            if (CanExecute(null) && Callback != null)
            {
                Callback(_viewModel);
            }
        }
        #endregion
    }
}
