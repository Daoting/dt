#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-06-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表项按钮
    /// </summary>
    public partial class BtnItem : Button
    {
        #region 静态内容
        /// <summary>
        /// 按钮图标
        /// </summary>
        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(BtnItem),
            new PropertyMetadata(default(Icons)));

        /// <summary>
        /// 按钮标题
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(BtnItem),
            null);

        /// <summary>
        /// 按钮描述信息
        /// </summary>
        public readonly static DependencyProperty DescProperty = DependencyProperty.Register(
            "Desc",
            typeof(string),
            typeof(BtnItem),
            null);
        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public BtnItem()
        {
            DefaultStyleKey = typeof(BtnItem);
        }

        /// <summary>
        /// 获取设置按钮图标
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置按钮标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置按钮描述信息
        /// </summary>
        public string Desc
        {
            get { return (string)GetValue(DescProperty); }
            set { SetValue(DescProperty, value); }
        }
    }
}
