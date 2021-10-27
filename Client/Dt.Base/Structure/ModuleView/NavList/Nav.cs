#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.ComponentModel;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 新窗口或主区内容的信息
    /// </summary>
    public class Nav : INotifyPropertyChanged
    {
        #region 成员变量
        Icons _icon;
        string _title;
        string _desc;
        string _warning;
        #endregion

        #region 构造方法
        public Nav()
        { }

        /// <summary>
        /// 新窗口或主区内容的信息
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_type">新窗口或主区内容的类型</param>
        /// <param name="p_icon">图标</param>
        public Nav(string p_title, Type p_type = null, Icons p_icon = Icons.None)
        {
            _title = p_title;
            Type = p_type;
            _icon = p_icon;
        }
        #endregion

        /// <summary>
        /// 获取设置图标
        /// </summary>
        public Icons Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged("Icon");
                }
            }
        }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Desc
        {
            get { return _desc; }
            set
            {
                if (_desc != value)
                {
                    _desc = value;
                    OnPropertyChanged("Desc");
                }
            }
        }

        /// <summary>
        /// 醒目提示的数字
        /// </summary>
        public string Warning
        {
            get { return _warning; }
            set
            {
                if (_warning != value)
                {
                    _warning = value;
                    OnPropertyChanged("Warning");
                }
            }
        }

        /// <summary>
        /// 获取设置内容类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取设置外部回调方法
        /// </summary>
        public Action<Win, Nav> Callback { get; set; }

        /// <summary>
        /// 新窗口或主区内容的类型的构造方法的参数
        /// </summary>
        public object Params { get; set; }

        /// <summary>
        /// 加载内容的目标位置，优先级高于NavList，null时采用 NavList.To 的设置
        /// </summary>
        public NavTarget? To { get; set; }

        /// <summary>
        /// 获取设置是否缓存主区内容
        /// </summary>
        public bool Cache { get; set; } = true;

        object _obj;
        internal object GetCenter(object p_params = null)
        {
            if (!Cache)
            {
                if (p_params == null)
                    return Activator.CreateInstance(Type);
                return Activator.CreateInstance(Type, p_params);
            }

            if (_obj != null)
                return _obj;

            if (Type != null)
                _obj = (p_params == null) ? Activator.CreateInstance(Type) : Activator.CreateInstance(Type, p_params);
            return _obj;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性变化事件
        /// </summary>
        /// <param name="propertyName">通知更改时的属性名称</param>
        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
