#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 视图项基类，是数据和UI的中间对象
    /// 继承DependencyObject为节省资源，实现INotifyPropertyChanged作为DataContext能更新
    /// </summary>
    public abstract partial class ViewItem : DependencyObject, INotifyPropertyChanged
    {
        #region 静态内容
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(SolidColorBrush),
            typeof(ViewItem),
            new PropertyMetadata(Res.默认前景, OnForegroundChanged));

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(SolidColorBrush),
            typeof(ViewItem),
            new PropertyMetadata(Res.TransparentBrush, OnBackgroundChanged));

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight",
            typeof(FontWeight),
            typeof(ViewItem),
            new PropertyMetadata(FontWeights.Normal, OnFontWeightChanged));

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            "FontStyle",
            typeof(FontStyle),
            typeof(ViewItem),
            new PropertyMetadata(FontStyle.Normal, OnFontStyleChanged));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize",
            typeof(double),
            typeof(ViewItem),
            new PropertyMetadata(16d, OnFontSizeChanged));

        static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("Foreground");
        }

        static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("Background");
        }

        static void OnFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontWeight");
        }

        static void OnFontStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontStyle");
        }

        static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewItem)d).OnPropertyChanged("FontSize");
        }
        #endregion

        #region 成员变量
        readonly object _data;
        bool _isInit;
        #endregion

        #region 构造方法
        public ViewItem(object p_data)
        {
            _data = p_data ?? throw new Exception("视图项数据不可为空！");
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取视图项数据
        /// </summary>
        public object Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row
        {
            get { return _data as Row; }
        }

        /// <summary>
        /// 获取设置行前景画刷
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// 获取设置行背景画刷
        /// </summary>
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// 获取设置行字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// 获取设置行文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置行文本大小
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 获取单元格值，外部绑定用
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <returns></returns>
        public object this[string p_colName]
        {
            get
            {
                object val = null;
                if (_data is Row dr && dr.Contains(p_colName))
                {
                    // 从Row取
                    val = dr[p_colName];
                }
                else
                {
                    // 对象属性
                    var pi = _data.GetType().GetProperty(p_colName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                        val = pi.GetValue(_data);
                }
                return val;
            }
        }

        /// <summary>
        /// 值变化时的回调
        /// </summary>
        internal Action ValueChanged { get; set; }

        /// <summary>
        /// 宿主
        /// </summary>
        protected abstract IViewItemHost Host { get; }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        protected abstract bool EnableCache { get; }
        #endregion

        #region 方法
        /// <summary>
        /// 获取当前行某列的值
        /// </summary>
        /// <typeparam name="T">将值转换为指定的类型</typeparam>
        /// <param name="p_id">列名</param>
        /// <returns>指定类型的值</returns>
        public T GetVal<T>(string p_id)
        {
            return ObjectEx.To<T>(this[p_id]);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化视图行，包括调用外部 CellEx.SetStyle 设置行样式、附加值变化事件、初始化缓存字典等
        /// </summary>
        internal void Init()
        {
            if (_isInit)
                return;

            _isInit = true;
            Host.SetItemStyle(this);
            if (_data is Row row)
                row.Changed += (s, e) => OnValueChanged();
            else if (_data is INotifyPropertyChanged pro)
                pro.PropertyChanged += (s, e) => OnValueChanged();
        }

        /// <summary>
        /// 值变化
        /// </summary>
        void OnValueChanged()
        {
            // 重新设置行/项目样式
            Host.SetItemStyle(this);
            ValueChanged?.Invoke();
        }
        #endregion

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