#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询面板，面板内的所有固定按钮、搜索框、搜索历史统一触发Search事件
    /// 固定按钮Click事件传递格式为 "#按钮名称"，#前缀用于区别普通搜索
    /// 搜索历史管理
    /// </summary>
    public partial class SearchFv : Fv
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(SearchFv),
            new PropertyMetadata("搜索"));

        #region 构造方法
        public SearchFv()
        {
            DefaultStyleKey = typeof(SearchFv);
            Loaded += OnLoaded;
        }
        #endregion

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<string> Search;

        /// <summary>
        /// 获取设置查询框提示内容
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var btn = (Button)GetTemplateChild("CloseButton");
            if (AtSys.IsPhoneUI)
                btn.Click += OnCloseClick;
            else
                btn.Visibility = Visibility.Collapsed;

            var tb = (TextBox)GetTemplateChild("SearchBox");
            tb.KeyDown += OnTextKeyDown;

            if (!AtSys.IsPhoneUI)
            {
                // 显示上边框
                var panel = (FormPanel)GetTemplateChild("FormPanel");
                panel.Margin = new Thickness(-1, 0, 0, 0);
            }
        }

        protected override void LoadAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                AddItem(item, i);
                foreach (var btn in item.FindChildrenByType<Button>())
                {
                    btn.Click += OnFixedBtnClick;
                }
            }

        }

        void OnFixedBtnClick(object sender, RoutedEventArgs e)
        {
            string txt = ((Button)sender).Content as string;
            if (!string.IsNullOrEmpty(txt))
                OnSearch("#" + txt);
        }

        void OnTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                string txt = ((TextBox)sender).Text.Trim();
                if (txt != string.Empty)
                    OnSearch(txt);
            }
        }

        void OnCloseClick(object sender, RoutedEventArgs e)
        {
            OnSearch(null);
        }

        void OnSearch(string p_text)
        {
            Search?.Invoke(this, p_text);
            if (!string.IsNullOrEmpty(p_text) && !p_text.StartsWith("#"))
                SaveCookie(p_text);
        }

        void SaveCookie(string p_text)
        {

        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

        }
    }
}