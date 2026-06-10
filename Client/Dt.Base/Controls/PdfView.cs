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

namespace Dt.Base;

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
    #endregion

    #region 变量
    bool _inNaviCompleted;
    #endregion

    public PdfView()
    {
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
    /// 打开本地pdf文件，虚拟主机方式
    /// </summary>
    /// <param name="p_file"></param>
    public async void Open(StorageFile p_file)
    {
        Throw.If(p_file == null, "未选择要打开的文件！");

        // 等待View2准备就绪
        while (!_inNaviCompleted)
        {
            await Task.Delay(200);
        }

        FileName = p_file.Name;

#if WIN
        CoreWebView2.ClearVirtualHostNameToFolderMapping("pdf");
        var folder = await p_file.GetParentAsync();
        CoreWebView2.SetVirtualHostNameToFolderMapping("pdf", folder.Path, CoreWebView2HostResourceAccessKind.Allow);
        CoreWebView2.Navigate($"https://pdf/{p_file.Name}");
#else

#endif
    }

    /// <summary>
    /// 打开内容，内容以base64编码生成html
    /// </summary>
    /// <param name="p_data"></param>
    /// <param name="p_fileName"></param>
    public void Open(byte[] p_data, string p_fileName = null)
    {
        if (p_data == null || p_data.Length == 0)
            Throw.Msg("pdf内容不可为空！");

        FileName = p_fileName;
        _ = NavigateToPdf(p_data);
    }

    /// <summary>
    /// 选择pdf文件并打开
    /// </summary>
    public async void OpenPdfFile()
    {
        var picker = Kit.GetFileOpenPicker();
        picker.FileTypeFilter.Add(".pdf");
        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            Open(file);
        }
    }
    
    /// <summary>
    /// 清除pdf内容
    /// </summary>
    public void Clear()
    {
        Source = null;
    }
    
    #region 内部方法
    async void EnsureView2()
    {
        await EnsureCoreWebView2Async();

        var settings = CoreWebView2.Settings;
        settings.AreDefaultContextMenusEnabled = false;
        settings.IsScriptEnabled = true;
        settings.AreDevToolsEnabled = false;

        _inNaviCompleted = true;
        Ready?.Invoke();
    }
    
    async Task NavigateToPdf(byte[] p_data)
    {
        try
        {
            if (p_data == null || p_data.Length == 0)
                return;

            while (!_inNaviCompleted)
            {
                await Task.Delay(200);
            }

            var asBase64 = Convert.ToBase64String(p_data);
            string html = $@"
<html style='margin:0;padding:0;height:100%;'>
<body style='margin:0;height:100%;'>
  <object type='application/pdf' width='100%' height='100%' data='data:application/pdf;base64,{asBase64}'></object>
</body>
</html>";
            NavigateToString(html);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "打开Pdf出错！");
        }
    }
    #endregion
}