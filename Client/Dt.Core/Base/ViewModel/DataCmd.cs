#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 支持命令模式的ViewModel基类
    /// </summary>
    public class DataCmd : ViewModel
    {
        #region 成员变量
        RelayCommand _command;
        string _title;
        object _icon;
        string _note;
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public DataCmd()
        {
            _command = new RelayCommand(this);
        }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        
        /// <summary>
        /// 获取设置图标(可以为ImageSource, Segoe UI Symbol)
        /// </summary>
        public object Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value); }
        }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Note
        {
            get { return _note; }
            set { SetProperty(ref _note, value); }
        }

        /// <summary>
        /// 获取设置命令的回调方法
        /// </summary>
        public Action<DataCmd> Callback
        {
            get { return _command.Callback; }
            set { _command.Callback = value; }
        }

        /// <summary>
        /// 获取设置判断命令是否可执行的回调方法
        /// </summary>
        public Func<bool> CanExec
        {
            get { return _command.CanExec; }
            set { _command.CanExec = value; }
        }

        /// <summary>
        /// 获取设置要触发执行的命令
        /// </summary>
        public RelayCommand Command
        {
            get { return _command; }
        }

        /// <summary>
        /// 获取设置自定义对象
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 执行命令回调
        /// </summary>
        public void Exec()
        {
            _command.Execute(null);
        }
    }
}
