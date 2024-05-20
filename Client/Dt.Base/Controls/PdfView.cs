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
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Pdf浏览器
    /// </summary>
    public partial class PdfView : WebView2
    {
        #region 静态成员
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(
           "FileName",
           typeof(string),
           typeof(PdfView),
           new PropertyMetadata(null));

        internal static string FolderPath = "web";
        #endregion

        #region 变量
        bool _inNaviCompleted;
        #endregion

        public PdfView()
        {
            CoreWebView2Initialized += OnCoreWebView2Initialized;
            EnsureView2();
        }

        /// <summary>
        /// 准备就绪事件
        /// </summary>
        public event Action Ready;

        /// <summary>
        /// 获取设置文件名，下载或打印时用到
        /// </summary>
        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        /// <summary>
        /// 打开文件，文件内容以base64编码传递给js
        /// </summary>
        /// <param name="p_file"></param>
        public async void Open(StorageFile p_file)
        {
            Throw.If(p_file == null, "未选择要打开的文件！");

            FileName = p_file.Name;
            Stream stream = await p_file.OpenStreamForReadAsync();
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            var asBase64 = Convert.ToBase64String(buffer);
            _ = RunScript(GetOpenScript(asBase64));
        }

        /// <summary>
        /// 直接调用js打开pdf文件
        /// </summary>
        public void Open()
        {
            _ = RunScript("const fileInput = PDFViewerApplication.appConfig.openFileInput; fileInput.click();");
        }

        /// <summary>
        /// 打开内容，内容以base64编码传递给js
        /// </summary>
        /// <param name="p_data"></param>
        /// <param name="p_fileName"></param>
        public void Open(byte[] p_data, string p_fileName = null)
        {
            if (p_data == null || p_data.Length == 0)
                Throw.Msg("pdf内容不可为空！");

            FileName = p_fileName;
            var asBase64 = Convert.ToBase64String(p_data);
            _ = RunScript(GetOpenScript(asBase64));
        }

        /// <summary>
        /// 清除pdf内容
        /// </summary>
        public void Clear()
        {
            _ = RunScript("PDFViewerApplication.close();");
        }
        
        /// <summary>
        /// 打印，必须弹框
        /// </summary>
        public void Print()
        {
            _ = RunScript("PDFViewerApplication.triggerPrinting();");
        }

        /// <summary>
        /// 下载
        /// </summary>
        public void Download()
        {
            _ = RunScript("PDFViewerApplication.downloadOrSave();");
        }

        /// <summary>
        /// 首页
        /// </summary>
        public void FirstPage()
        {
            _ = RunScript("PDFViewerApplication.page = 1;");
        }

        /// <summary>
        /// 末页
        /// </summary>
        public void LastPage()
        {
            _ = RunScript("PDFViewerApplication.page = PDFViewerApplication.pagesCount;");
        }

        /// <summary>
        /// 下页
        /// </summary>
        public void NextPage()
        {
            _ = RunScript("PDFViewerApplication.pdfViewer.nextPage();");
        }

        /// <summary>
        /// 上页
        /// </summary>
        public void PreviousPage()
        {
            _ = RunScript("PDFViewerApplication.pdfViewer.previousPage();");
        }

        /// <summary>
        /// 转到某页
        /// </summary>
        /// <param name="p_no"></param>
        public void GotoPage(int p_no)
        {
            _ = RunScript($"PDFViewerApplication.page = {p_no};");
        }

        #region 内部方法
        async void EnsureView2()
        {
            await EnsureCoreWebView2Async();
        }

        void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            var settings = CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.IsScriptEnabled = true;
            settings.AreDevToolsEnabled = false;

            CoreWebView2.SetVirtualHostNameToFolderMapping("pdf", FolderPath, CoreWebView2HostResourceAccessKind.Allow);
            CoreWebView2.NavigationCompleted += (s, e) =>
            {
                _inNaviCompleted = true;
                Ready?.Invoke();
            };
            Source = new("https://pdf/web/viewer.html");
        }

        string GetOpenScript(string p_base64)
        {
            if (string.IsNullOrEmpty(p_base64))
                return null;

            // 文件名随内容一起提交
            string name = string.IsNullOrEmpty(FileName) ? DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".pdf" : FileName;
            return
$"PDFViewerApplication._contentDispositionFilename = '{name}';" +
"var binary_string = window.atob('" + p_base64 + @"');
var len = binary_string.length;
var bytes = new Uint8Array(len);
for (var i = 0; i < len; i++) {
	bytes[i] = binary_string.charCodeAt(i);
}
PDFViewerApplication.open({ data: bytes });
PDFViewerApplication.update();";
        }

        async Task<string> RunScript(string p_script)
        {
            try
            {
                if (string.IsNullOrEmpty(p_script))
                    return null;

                while (!_inNaviCompleted)
                {
                    await Task.Delay(200);
                }

                return await this.ExecuteScriptAsync(p_script);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Pdf脚本运行出错");
            }
            return null;
        }
        #endregion
    }
}