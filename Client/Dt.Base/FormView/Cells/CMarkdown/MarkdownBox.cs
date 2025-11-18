#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Markdown编辑器
    /// </summary>
    public partial class MarkdownBox : WebView2
    {
        #region 静态成员
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
           "IsReadOnly",
           typeof(bool),
           typeof(MarkdownBox),
           new PropertyMetadata(false, OnIsReadOnlyChanged));

        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MarkdownBox)d).ApplyNavigate(true);
        }

        const string _folderPath = "Assets/Markdown";
        #endregion

        #region 变量
        bool _isInited;
        bool _inNaviCompleted;
        string _txt;
        #endregion

        public MarkdownBox()
        {
            EnsureView2();
            SizeChanged += OnSizeChanged;
        }

        /// <summary>
        /// 获取设置是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 获取Markdown内容
        /// </summary>
        public async Task<string> GetText()
        {
            if (_inNaviCompleted)
            {
                _txt = await this.InvokeScriptAsync("getText", null);
            }
            return _txt;
        }

        /// <summary>
        /// 设置Markdown内容
        /// </summary>
        /// <param name="p_html"></param>
        public void SetText(string p_html)
        {
            _txt = p_html;
            ApplyMarkdown();
        }

        #region 内部方法
        async void EnsureView2()
        {
            await EnsureCoreWebView2Async();

            var settings = CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.IsScriptEnabled = true;
            settings.AreDevToolsEnabled = false;

            CoreWebView2.SetVirtualHostNameToFolderMapping("md", _folderPath, CoreWebView2HostResourceAccessKind.Allow);
            CoreWebView2.NavigationCompleted += OnNavigationCompleted;
            _isInited = true;
            ApplyNavigate(false);
            
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_inNaviCompleted)
            {
                ApplyHeight(e.NewSize.Height);
            }
        }
        
        void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _inNaviCompleted = true;
            ApplyHeight(ActualHeight);
            ApplyMarkdown();
        }

        async void ApplyNavigate(bool p_update)
        {
            if (_isInited)
            {
                _inNaviCompleted = false;
                if (p_update && CoreWebView2.Source == "https://md/editor.html")
                    _txt = await this.InvokeScriptAsync("getText", null);
                CoreWebView2.Navigate(IsReadOnly ? "https://md/viewer.html" : "https://md/editor.html");
            }
        }

        async void ApplyMarkdown()
        {
            if (_inNaviCompleted)
            {
                string val = (_txt != null) ? _txt.ToString() : "";
                await this.InvokeScriptAsync("setText", new string[] { val });
            }
        }

        async void ApplyHeight(double p_height)
        {
            double height = Math.Max(300, Math.Floor(p_height - 20));
            await this.InvokeScriptAsync("setHeight", new string[] { height + "px" });
        }
        #endregion
    }
}