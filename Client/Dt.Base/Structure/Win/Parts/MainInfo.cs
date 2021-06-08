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
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 窗口主区内容描述
    /// </summary>
    public class MainInfo
    {
        public MainInfo()
        { }

        /// <summary>
        /// 窗口主区
        /// </summary>
        /// <param name="p_icon">图标</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_winType">窗口类型</param>
        /// <param name="p_desc">描述信息</param>
        public MainInfo(Icons p_icon, string p_title, Type p_winType, string p_desc = null)
        {
            Icon = p_icon;
            Title = p_title;
            Type = p_winType;
            Desc = p_desc;
        }

        /// <summary>
        /// 窗口主区
        /// </summary>
        /// <param name="p_icon">图标</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_callback">外部回调方法</param>
        /// <param name="p_desc">描述信息</param>
        public MainInfo(Icons p_icon, string p_title, Action p_callback, string p_desc = null)
        {
            Icon = p_icon;
            Title = p_title;
            Callback = p_callback;
            Desc = p_desc;
        }

        /// <summary>
        /// 获取设置图标
        /// </summary>
        public Icons Icon { get; set; }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置描述信息
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 获取设置主区内容类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取设置外部回调方法
        /// </summary>
        public Action Callback { get; set; }

        object _obj;
        internal object GetCenter()
        {
            if (_obj != null)
                return _obj;

            if (Type != null)
                _obj = Activator.CreateInstance(Type);
            return _obj;
        }
    }
}
