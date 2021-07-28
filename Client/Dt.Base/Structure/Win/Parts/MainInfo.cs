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
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 窗口主区内容描述
    /// </summary>
    public class MainInfo : INotifyPropertyChanged
    {
        #region 成员变量
        Icons _icon;
        string _title;
        string _desc;
        #endregion

        #region 构造方法
        public MainInfo()
        { }

        /// <summary>
        /// 窗口主区
        /// </summary>
        /// <param name="p_icon">图标</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_winType">窗口类型</param>
        /// <param name="p_desc">描述信息</param>
        /// <param name="p_cache">是否缓存主区内容</param>
        public MainInfo(Icons p_icon, string p_title, Type p_winType, string p_desc = null, bool p_cache = true)
        {
            _icon = p_icon;
            _title = p_title;
            _desc = p_desc;
            Type = p_winType;
            Cache = p_cache;
        }

        /// <summary>
        /// 窗口主区
        /// </summary>
        /// <param name="p_icon">图标</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_callback">外部回调方法</param>
        /// <param name="p_desc">描述信息</param>
        /// <param name="p_cache">是否缓存主区内容</param>
        public MainInfo(Icons p_icon, string p_title, Action p_callback, string p_desc = null, bool p_cache = true)
        {
            _icon = p_icon;
            _title = p_title;
            _desc = p_desc;
            Callback = p_callback;
            Cache = p_cache;
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
        /// 获取设置主区内容类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取设置外部回调方法
        /// </summary>
        public Action Callback { get; set; }

        /// <summary>
        /// 获取设置是否缓存主区内容
        /// </summary>
        public bool Cache { get; set; }

        object _obj;
        internal object GetCenter()
        {
            if (!Cache)
                return Activator.CreateInstance(Type);

            if (_obj != null)
                return _obj;

            if (Type != null)
                _obj = Activator.CreateInstance(Type);
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
