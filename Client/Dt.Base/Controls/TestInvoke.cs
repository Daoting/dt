﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 测试不同平台主事件的调用顺序
    /// </summary>
    [ContentProperty(Name = nameof(Child))]
    public partial class TestInvoke : Control
    {
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(TestInvoke),
            new PropertyMetadata(""));

        public readonly static DependencyProperty ChildProperty = DependencyProperty.Register(
            "Child",
            typeof(TestInvoke),
            typeof(TestInvoke),
            new PropertyMetadata(null));

        public TestInvoke()
        {
            DefaultStyleKey = typeof(TestInvoke);
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public TestInvoke Child
        {
            get { return (TestInvoke)GetValue(ChildProperty); }
            set { SetValue(ChildProperty, value); }
        }

        public StringBuilder Output { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            OnOutput("OnApplyTemplate");
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            OnOutput("MeasureOverride");
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            OnOutput("ArrangeOverride");
            return base.ArrangeOverride(finalSize);
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            OnOutput("Loaded");
            OnOutput("OnFirstLoaded");
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnOutput("SizeChanged");
        }

        void OnOutput(string p_invoke)
        {
            Log.Debug(Title + p_invoke);
            if (Output != null)
            {
                if (Output.Length > 0)
                    Output.Append($" -> {Title}{p_invoke}");
                else
                    Output.Append($"{Title}{p_invoke}");
            }
        }
    }
}
