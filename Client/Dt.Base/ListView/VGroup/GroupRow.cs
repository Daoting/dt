#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 分组行
    /// </summary>
    public partial class GroupRow : DtControl
    {
        #region 静态内容
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(GroupRow),
            new PropertyMetadata(null));

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(GroupRow),
            new PropertyMetadata(null));

        public readonly static DependencyProperty IsFirstProperty = DependencyProperty.Register(
            "IsFirst",
            typeof(bool),
            typeof(GroupRow),
            new PropertyMetadata(false));
        #endregion

        #region 构造方法
        public GroupRow(Lv p_owner, IList p_group)
        {
            DefaultStyleKey = typeof(GroupRow);

            Data = p_group;
            Title = p_group.ToString();

            if (p_owner.GroupTemplate != null)
            {
                Content = p_owner.GroupTemplate.LoadContent();
                Type tp = p_owner.GroupContext;
                if (tp != null && tp.IsSubclassOf(typeof(GroupContext)))
                {
                    // 设置分组模板的数据上下文
                    var context = (GroupContext)Activator.CreateInstance(tp);
                    context.Rows = p_group;
                    DataContext = context;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取设置分组标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置分组行内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为第一行
        /// </summary>
        public bool IsFirst
        {
            get { return (bool)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, value); }
        }

        internal IList Data { get; }

        /// <summary>
        /// 在面板上的垂直位置
        /// </summary>
#if ANDROID
        new
#endif
        internal double Top { get; set; }

        protected override void OnLoadTemplate()
        {
            if (IsFirst)
                ((Rectangle)GetTemplateChild("Rect")).Visibility = Visibility.Collapsed;
        }
    }
}
