#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 分组相关
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public static readonly DependencyProperty ShowGroupHeaderProperty = DependencyProperty.Register(
            "ShowGroupHeader",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(true, OnReload));

        public readonly static DependencyProperty GroupNameProperty = DependencyProperty.Register(
            "GroupName",
            typeof(string),
            typeof(Lv),
            new PropertyMetadata(null, OnDataViewPropertyChanged));

        public readonly static DependencyProperty GroupTemplateProperty = DependencyProperty.Register(
            "GroupTemplate",
            typeof(DataTemplate),
            typeof(Lv),
            new PropertyMetadata(null, OnReload));

        public readonly static DependencyProperty GroupContextProperty = DependencyProperty.Register(
            "GroupContext",
            typeof(Type),
            typeof(Lv),
            new PropertyMetadata(null, OnReload));
        #endregion

        /// <summary>
        /// 获取设置顶部是否显示分组导航，默认true
        /// </summary>
        public bool ShowGroupHeader
        {
            get { return (bool)GetValue(ShowGroupHeaderProperty); }
            set { SetValue(ShowGroupHeaderProperty, value); }
        }

        /// <summary>
        /// 获取设置分组列名
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// 获取设置分组模板，和GroupContext配合使用
        /// </summary>
        public DataTemplate GroupTemplate
        {
            get { return (DataTemplate)GetValue(GroupTemplateProperty); }
            set { SetValue(GroupTemplateProperty, value); }
        }

        /// <summary>
        /// 获取设置分组模板的数据上下文类型，和GroupTemplate配合使用，需继承自GroupContext
        /// </summary>
        public Type GroupContext
        {
            get { return (Type)GetValue(GroupContextProperty); }
            set { SetValue(GroupContextProperty, value); }
        }
    }
}