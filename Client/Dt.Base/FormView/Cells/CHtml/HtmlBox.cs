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
    /// Html编辑器
    /// </summary>
    public partial class HtmlBox : WebView2
    {
        #region 静态成员
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
           "IsReadOnly",
           typeof(bool),
           typeof(HtmlBox),
           new PropertyMetadata(false, OnIsReadOnlyChanged));

        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HtmlBox)d).ApplyNavigate(true);
        }

        const string _folderPath = "Assets/Html";
        #endregion

        #region 变量
        bool _isInited;
        bool _inNaviCompleted;
        string _html;
        #endregion

        public HtmlBox()
        {
            EnsureView2();
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
        /// 获取html内容
        /// </summary>
        public async Task<string> GetHtml()
        {
            if (_inNaviCompleted)
            {
                _html = await this.InvokeScriptAsync("getHtml", null);
            }
            return _html;
        }

        /// <summary>
        /// 设置html内容
        /// </summary>
        /// <param name="p_html"></param>
        public void SetHtml(string p_html)
        {
            _html = p_html;
            ApplyHtml();
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="p_img"></param>
        /// <returns></returns>
        public async Task InsertImg(string p_img)
        {
            if (_inNaviCompleted && !string.IsNullOrEmpty(p_img))
            {
                await this.InvokeScriptAsync("insertImage", new string[] { p_img });
            }
        }

        /// <summary>
        /// 插入视频
        /// </summary>
        /// <param name="p_video"></param>
        /// <returns></returns>
        public async Task InsertVideo(string p_video)
        {
            if (_inNaviCompleted && !string.IsNullOrEmpty(p_video))
            {
                await this.InvokeScriptAsync("insertVideo", new string[] { p_video });
            }
        }

        #region 内部方法
        async void EnsureView2()
        {
            await EnsureCoreWebView2Async();

            var settings = CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.IsScriptEnabled = true;
            settings.AreDevToolsEnabled = false;

            CoreWebView2.SetVirtualHostNameToFolderMapping("html", _folderPath, CoreWebView2HostResourceAccessKind.Allow);
            CoreWebView2.NavigationCompleted += OnNavigationCompleted;
            _isInited = true;
            ApplyNavigate(false);
        }

        void OnNavigationCompleted(CoreWebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            _inNaviCompleted = true;
            ApplyHtml();
        }

        async void ApplyNavigate(bool p_update)
        {
            if (_isInited)
            {
                _inNaviCompleted = false;
                if (p_update && CoreWebView2.Source == "https://html/editor.html")
                    _html = await this.InvokeScriptAsync("getHtml", null);
                CoreWebView2.Navigate(IsReadOnly ? "https://html/viewer.html" : "https://html/editor.html");
            }
        }

        async void ApplyHtml()
        {
            if (_inNaviCompleted)
            {
                string val = (_html != null) ? _html.ToString() : "";
                await this.InvokeScriptAsync("setHtml", new string[] { val });
            }
        }
        #endregion
    }
}