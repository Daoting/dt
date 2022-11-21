#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表中行模板的占位格
    /// </summary>
    public partial class Dot : ContentPresenter
    {
        #region 静态内容
        const double _defaultFontSize = 16;
        public static readonly DependencyProperty CallProperty = DependencyProperty.Register(
            "Call",
            typeof(string),
            typeof(Dot),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(Dot),
            new PropertyMetadata(null));

        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            "AutoHide",
            typeof(bool),
            typeof(Dot),
            new PropertyMetadata(true));
        #endregion

        #region 成员变量
        bool _isInit;
        Action<CallArgs> _set;
        #endregion

        #region 构造方法
        public Dot()
        {
            // 系统默认大小14，uwp初次测量结果偏小
            FontSize = _defaultFontSize;
            DataContextChanged += OnDataContextChanged;
        }
        #endregion

        /// <summary>
        /// 获取设置Dot内容对应的数据对象的属性名，null或空时对应数据对象本身
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置自定义单元格UI的方法名，多个方法名用逗号隔开，形如：Def.Icon,Def.小灰
        /// </summary>
        public string Call
        {
            get { return (string)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// 获取设置格式串，null或空时按默认显示，如：时间格式、小数位格式、枚举类型名称
        /// <para>也是自定义单元格UI方法的参数</para>
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取设置内容为空时是否自动隐藏Dot，默认true
        /// <para>隐藏时Padding 或 Margin 不再占用位置！</para>
        /// <para>若false，内容为空时仍然占位</para>
        /// <para>未处理Table模式的Dot，因其负责画右下边线！</para>
        /// </summary>
        public bool AutoHide
        {
            get { return (bool)GetValue(AutoHideProperty); }
            set { SetValue(AutoHideProperty, value); }
        }

        /// <summary>
        /// 切换Dot显示隐藏
        /// </summary>
        /// <param name="p_isEmpty"></param>
        public void ToggleVisible(bool p_isEmpty)
        {
            if (!AutoHide)
                return;

            if (p_isEmpty)
            {
                if (Visibility == Visibility.Visible)
                    Visibility = Visibility.Collapsed;
            }
            else if (Visibility == Visibility.Collapsed)
            {
                // 数据变化时重新可见
                Visibility = Visibility.Visible;
            }
        }

        void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            ViewItem vi = e.NewValue as ViewItem;
            if (vi == null)
                return;

            if (!_isInit)
            {
                _isInit = true;

                /*****************************************************************************************/
                // 只在初次触发 DataContextChanged 构造内部元素！
                // 初次触发发生在加载 DataTemplate 后设置 DataContext 时，还未在可视树上！
                // 若在 OnApplyTemplate 或 Loaded 中绑定在uno上已晚！
                // Dot及内部元素的只有以下5种样式采用OneTime绑定，其余依靠切换 DataContext 更新Dot！！！
                /*****************************************************************************************/

                // 优先级：直接设置 > ViewItem属性，未直接设置的绑定ViewItem中行样式
                if (ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                    SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
                    SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                    SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                    SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle"), Mode = BindingMode.OneTime });
                if (FontSize == _defaultFontSize)
                    SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize"), Mode = BindingMode.OneTime });

                // 构造内部元素
                Content = GetCellUI(vi);
            }
            _set?.Invoke(new CallArgs(vi, this));
        }
    }
}
