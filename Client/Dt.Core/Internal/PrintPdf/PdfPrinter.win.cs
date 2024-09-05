#if WIN
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Printing;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.Storage.Streams;
#endregion

namespace Dt.Core;

/// <summary>
/// Pdf打印类
/// 0. PrintManagerInterop.ShowPrintUIForWindowAsync()显示打印对话框
/// 1. PrintTaskRequested设置打印参数和内容
/// 2. PrintDocument.Paginate生成预览页面，设置页数
/// 3. PrintDocument.GetPreviewPage显示特定预览页面
/// 4. 按下打印PrintDocument.AddPages输出最终打印页集合
/// </summary>
public partial class PdfPrinter
{
    #region 变量
    readonly List<Image> _previewPages = new();
    readonly List<InMemoryRandomAccessStream> _pagesStream = new();
    PdfDocument _pdfDoc;
    PrintDocument _printDoc;
    Size _printPageSize;
    double _pdfPageRatio;
    string _fileName;
    nint _hWnd;
    #endregion

    public PdfPrinter()
    {
        _hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Kit.MainWin);
        // 打印请求
        var printMan = PrintManagerInterop.GetForWindow(_hWnd);
        printMan.PrintTaskRequested += OnPrintTaskRequested;

        _printDoc = new PrintDocument();
        // 打印参数变化时生成预览页面
        _printDoc.Paginate += OnPaginate;
        // 显示指定预览页面
        _printDoc.GetPreviewPage += OnGetPreviewPage;
        // 最终打印页集合
        _printDoc.AddPages += OnAddPages;
    }

    /// <summary>
    /// 打印，始终显示打印对话框，无法静默打印
    /// </summary>
    /// <param name="p_inputStream"></param>
    /// <param name="p_fileName"></param>
    public async void Print(Stream p_inputStream, string p_fileName)
    {
        _fileName = string.IsNullOrEmpty(p_fileName) ? "文档.pdf" : p_fileName;

        var ras = await ConvertToRandomAccessStream(p_inputStream);
        _pdfDoc = await PdfDocument.LoadFromStreamAsync(ras);

        _pagesStream.Clear();
        for (var i = 0; i < _pdfDoc.PageCount; i++)
        {
            using var pdfPage = _pdfDoc.GetPage((uint)i);

            var ms = new InMemoryRandomAccessStream();
            // 按实际尺寸绘制
            await pdfPage.RenderToStreamAsync(ms);
            _pagesStream.Add(ms);

            // 记录页面宽高比例
            _pdfPageRatio = pdfPage.Size.Width / pdfPage.Size.Height;
        }

        await PrintManagerInterop.ShowPrintUIForWindowAsync(_hWnd);
    }

    Image CreatePage(int p_index)
    {
        var img = new Image { Stretch = Stretch.Uniform };
        var src = new BitmapImage();
        var ras = _pagesStream[p_index];
        ras.Seek(0);
        src.SetSource(ras);
        img.Source = src;

        // 等比例缩放
        if (_pdfPageRatio > _printPageSize.Width / _printPageSize.Height)
        {
            img.Width = _printPageSize.Width;
        }
        else
        {
            img.Height = _printPageSize.Height;
        }
        return img;
    }

    #region 事件处理
    /// <summary>
    /// 处理打印请求
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
    {
        sender.PrintTaskRequested -= OnPrintTaskRequested;
        PrintTask printTask = null;
        printTask = e.Request.CreatePrintTask(_fileName, src => Kit.RunSync(() =>
        {
            //var deferral = src.GetDeferral();
            src.SetSource(_printDoc.DocumentSource);
            //deferral.Complete();
        }));
        printTask.Completed += OnPrintTaskCompleted;
    }

    /// <summary>
    /// 打印完成
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    void OnPrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
    {
        sender.Completed -= OnPrintTaskCompleted;
        try
        {
            foreach (var ps in _pagesStream)
            {
                ps.Dispose();
            }
        }
        catch { }

        if (args.Completion == PrintTaskCompletion.Failed)
        {
            Kit.Warn("打印失败！");
        }
        else
        {
            Kit.Msg("打印结束！");
        }
    }

    /// <summary>
    /// 打印参数变化时生成预览页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnPaginate(object sender, PaginateEventArgs e)
    {
        if (_dtSize.TryGetValue(e.PrintTaskOptions.MediaSize, out var sz))
        {
            _printPageSize = sz;
        }
        else
        {
            // 默认A4
            _printPageSize = new Size(793.681884765625, 1122.48193359375);
        }

        _previewPages.Clear();
        for (var i = 0; i < _pdfDoc.PageCount; i++)
        {
            var page = CreatePage(i);
            _previewPages.Add(page);
        }

        _printDoc.SetPreviewPageCount((int)_pdfDoc.PageCount, PreviewPageCountType.Final);
    }

    /// <summary>
    /// 显示指定预览页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnGetPreviewPage(object sender, GetPreviewPageEventArgs e)
    {
        _printDoc.SetPreviewPage(e.PageNumber, _previewPages[e.PageNumber - 1]);
    }

    /// <summary>
    /// 发送最终打印页集合
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void OnAddPages(object sender, AddPagesEventArgs e)
    {
        try
        {
            for (var i = 0; i < _pdfDoc.PageCount; i++)
            {
                //ApplicationLanguages.PrimaryLanguageOverride = CultureInfo.InvariantCulture.TwoLetterISOLanguageName;
                var page = CreatePage(i);
                _printDoc.AddPage(page);
            }
            _printDoc.AddPagesComplete();
        }
        catch
        {
            _printDoc.InvalidatePreview();
        }
    }
    #endregion

    #region 静态
    static async Task<IRandomAccessStream> ConvertToRandomAccessStream(Stream p_inputStream)
    {
        p_inputStream.Position = 0;
        using var contentStream = new MemoryStream();
        await p_inputStream.CopyToAsync(contentStream);

        var randomAccessStream = new InMemoryRandomAccessStream();
        using var outputStream = randomAccessStream.GetOutputStreamAt(0);
        using var dw = new DataWriter(outputStream);
        dw.WriteBytes(contentStream.ToArray());

        await dw.StoreAsync();
        await outputStream.FlushAsync();
        await dw.FlushAsync();

        outputStream.Dispose();
        dw.DetachStream();

        return randomAccessStream;
    }

    static readonly Dictionary<PrintMediaSize, Size> _dtSize;
    static PdfPrinter()
    {
        Dictionary<PrintMediaSize, Size> dict = new Dictionary<PrintMediaSize, Size>();
        dict.Add(PrintMediaSize.PrinterCustom, Size.Empty);
        dict.Add(PrintMediaSize.BusinessCard, new Size(10, 20));
        dict.Add(PrintMediaSize.IsoA2, new Size(1587.35998535156, 2244.9599609375));
        dict.Add(PrintMediaSize.IsoA3, new Size(1122.48193359375, 1587.35998535156));
        dict.Add(PrintMediaSize.IsoA3Extra, new Size(1216.95874023438, 1681.84069824219));
        dict.Add(PrintMediaSize.IsoA3Rotated, new Size(1587.35998535156, 1122.48193359375));
        dict.Add(PrintMediaSize.IsoA4, new Size(793.681884765625, 1122.48193359375));
        dict.Add(PrintMediaSize.IsoA4Extra, new Size(889.681884765625, 1218.08129882813));
        dict.Add(PrintMediaSize.IsoA4Rotated, new Size(1122.48193359375, 793.681884765625));
        dict.Add(PrintMediaSize.IsoA5, new Size(559.358764648438, 793.681884765625));
        dict.Add(PrintMediaSize.IsoA5Extra, new Size(657.600036621094, 888.158752441406));
        dict.Add(PrintMediaSize.IsoA5Rotated, new Size(793.681884765625, 559.358764648438));
        dict.Add(PrintMediaSize.IsoA6, new Size(396.80126953125, 559.358764648438));
        dict.Add(PrintMediaSize.IsoA6Rotated, new Size(559.358764648438, 396.80126953125));
        dict.Add(PrintMediaSize.IsoB4, new Size(944.881896972656, 1334.16186523438));
        dict.Add(PrintMediaSize.IsoB4Envelope, new Size(944.881896972656, 1334.16186523438));
        dict.Add(PrintMediaSize.IsoB5Envelope, new Size(665.121276855469, 944.881896972656));
        dict.Add(PrintMediaSize.IsoB5Extra, new Size(759.681274414063, 1043.11938476563));
        dict.Add(PrintMediaSize.IsoC3Envelope, new Size(1224.55944824219, 1730.95935058594));
        dict.Add(PrintMediaSize.IsoC4Envelope, new Size(865.440002441406, 1224.55944824219));
        dict.Add(PrintMediaSize.IsoC5Envelope, new Size(612.241882324219, 865.440002441406));
        dict.Add(PrintMediaSize.IsoC6C5Envelope, new Size(430.801910400391, 865.440002441406));
        dict.Add(PrintMediaSize.IsoC6Envelope, new Size(430.801910400391, 612.241882324219));
        dict.Add(PrintMediaSize.IsoDLEnvelope, new Size(415.679992675781, 831.439392089844));
        dict.Add(PrintMediaSize.JapanChou3Envelope, new Size(453.520629882813, 888.158752441406));
        dict.Add(PrintMediaSize.JapanChou3EnvelopeRotated, new Size(888.158752441406, 453.520629882813));
        dict.Add(PrintMediaSize.JapanChou4Envelope, new Size(340.081909179688, 774.799377441406));
        dict.Add(PrintMediaSize.JapanChou4EnvelopeRotated, new Size(774.799377441406, 340.081909179688));
        dict.Add(PrintMediaSize.JapanDoubleHagakiPostcard, new Size(755.841247558594, 559.358764648438));
        dict.Add(PrintMediaSize.JapanDoubleHagakiPostcardRotated, new Size(559.358764648438, 755.841247558594));
        dict.Add(PrintMediaSize.JapanHagakiPostcard, new Size(377.918731689453, 559.358764648438));
        dict.Add(PrintMediaSize.JapanHagakiPostcardRotated, new Size(559.358764648438, 377.918731689453));
        dict.Add(PrintMediaSize.JapanKaku2Envelope, new Size(907.041259765625, 1254.79943847656));
        dict.Add(PrintMediaSize.JapanKaku2EnvelopeRotated, new Size(1254.79943847656, 907.041259765625));
        dict.Add(PrintMediaSize.JapanKaku3Envelope, new Size(816.3212890625, 1046.88000488281));
        dict.Add(PrintMediaSize.JapanKaku3EnvelopeRotated, new Size(1046.88000488281, 816.3212890625));
        dict.Add(PrintMediaSize.JapanYou4Envelope, new Size(396.80126953125, 888.158752441406));
        dict.Add(PrintMediaSize.JapanYou4EnvelopeRotated, new Size(888.158752441406, 396.80126953125));
        dict.Add(PrintMediaSize.JisB4, new Size(971.281921386719, 1375.68005371094));
        dict.Add(PrintMediaSize.JisB4Rotated, new Size(1375.68005371094, 971.281921386719));
        dict.Add(PrintMediaSize.JisB5, new Size(687.840026855469, 971.281921386719));
        dict.Add(PrintMediaSize.JisB5Rotated, new Size(971.281921386719, 687.840026855469));
        dict.Add(PrintMediaSize.JisB6, new Size(483.760650634766, 687.840026855469));
        dict.Add(PrintMediaSize.JisB6Rotated, new Size(687.840026855469, 483.760650634766));
        dict.Add(PrintMediaSize.NorthAmerica10x11, new Size(960, 1056));
        dict.Add(PrintMediaSize.NorthAmerica10x14, new Size(960, 1344));
        dict.Add(PrintMediaSize.NorthAmerica11x17, new Size(1056, 1632));
        dict.Add(PrintMediaSize.NorthAmerica9x11, new Size(864, 1056));
        dict.Add(PrintMediaSize.NorthAmericaCSheet, new Size(1632, 2112));
        dict.Add(PrintMediaSize.NorthAmericaDSheet, new Size(2112, 3264));
        dict.Add(PrintMediaSize.NorthAmericaESheet, new Size(3264, 4224));
        dict.Add(PrintMediaSize.NorthAmericaExecutive, new Size(695.761901855469, 1008));
        dict.Add(PrintMediaSize.NorthAmericaGermanLegalFanfold, new Size(816, 1248));
        dict.Add(PrintMediaSize.NorthAmericaGermanStandardFanfold, new Size(816, 1152));
        dict.Add(PrintMediaSize.NorthAmericaLegal, new Size(816, 1344));
        dict.Add(PrintMediaSize.NorthAmericaLegalExtra, new Size(912, 1440));
        dict.Add(PrintMediaSize.NorthAmericaLetter, new Size(816, 1056));
        dict.Add(PrintMediaSize.NorthAmericaLetterExtra, new Size(912, 1152));
        dict.Add(PrintMediaSize.NorthAmericaLetterPlus, new Size(816, 1218.08129882813));
        dict.Add(PrintMediaSize.NorthAmericaLetterRotated, new Size(1056, 816));
        dict.Add(PrintMediaSize.NorthAmericaMonarchEnvelope, new Size(371.841278076172, 720));
        dict.Add(PrintMediaSize.NorthAmericaNote, new Size(816, 1056));
        dict.Add(PrintMediaSize.NorthAmericaNumber10Envelope, new Size(395.678741455078, 912));
        dict.Add(PrintMediaSize.NorthAmericaNumber11Envelope, new Size(432, 995.841247558594));
        dict.Add(PrintMediaSize.NorthAmericaNumber12Envelope, new Size(455.761901855469, 1056));
        dict.Add(PrintMediaSize.NorthAmericaNumber14Envelope, new Size(480, 1104));
        dict.Add(PrintMediaSize.NorthAmericaNumber9Envelope, new Size(371.841278076172, 851.841247558594));
        dict.Add(PrintMediaSize.NorthAmericaPersonalEnvelope, new Size(347.678741455078, 624));
        dict.Add(PrintMediaSize.NorthAmericaQuarto, new Size(812.560668945313, 1039.35876464844));
        dict.Add(PrintMediaSize.NorthAmericaStatement, new Size(528, 816));
        dict.Add(PrintMediaSize.NorthAmericaSuperA, new Size(857.918762207031, 1345.44006347656));
        dict.Add(PrintMediaSize.NorthAmericaSuperB, new Size(1152.72192382813, 1840.56188964844));
        dict.Add(PrintMediaSize.NorthAmericaTabloid, new Size(1056, 1632));
        dict.Add(PrintMediaSize.OtherMetricA4Plus, new Size(793.681884765625, 1247.19873046875));
        dict.Add(PrintMediaSize.OtherMetricFolio, new Size(816, 1248));
        dict.Add(PrintMediaSize.OtherMetricInviteEnvelope, new Size(831.439392089844, 831.439392089844));
        dict.Add(PrintMediaSize.OtherMetricItalianEnvelope, new Size(415.679992675781, 869.280029296875));
        dict.Add(PrintMediaSize.Prc10Envelope, new Size(1224.55944824219, 1730.95935058594));
        dict.Add(PrintMediaSize.Prc1Envelope, new Size(385.440002441406, 623.599365234375));
        dict.Add(PrintMediaSize.Prc1EnvelopeRotated, new Size(623.599365234375, 385.440002441406));
        dict.Add(PrintMediaSize.Prc3Envelope, new Size(472.399383544922, 665.121276855469));
        dict.Add(PrintMediaSize.Prc3EnvelopeRotated, new Size(665.121276855469, 472.399383544922));
        dict.Add(PrintMediaSize.Prc4Envelope, new Size(415.679992675781, 786.081298828125));
        dict.Add(PrintMediaSize.Prc4EnvelopeRotated, new Size(786.081298828125, 415.679992675781));
        dict.Add(PrintMediaSize.Prc5Envelope, new Size(415.679992675781, 831.439392089844));
        dict.Add(PrintMediaSize.Prc5EnvelopeRotated, new Size(831.439392089844, 415.679992675781));
        dict.Add(PrintMediaSize.Prc6Envelope, new Size(453.520629882813, 869.280029296875));
        dict.Add(PrintMediaSize.Prc6EnvelopeRotated, new Size(869.280029296875, 453.520629882813));
        dict.Add(PrintMediaSize.Prc7Envelope, new Size(604.720642089844, 869.280029296875));
        dict.Add(PrintMediaSize.Prc7EnvelopeRotated, new Size(869.280029296875, 604.720642089844));
        dict.Add(PrintMediaSize.Prc8Envelope, new Size(453.520629882813, 1167.83996582031));
        dict.Add(PrintMediaSize.Prc8EnvelopeRotated, new Size(1167.83996582031, 453.520629882813));
        dict.Add(PrintMediaSize.Prc9Envelope, new Size(865.440002441406, 1224.55944824219));
        dict.Add(PrintMediaSize.Prc9EnvelopeRotated, new Size(1224.55944824219, 865.440002441406));
        _dtSize = dict;
    }
    #endregion
}
#endif