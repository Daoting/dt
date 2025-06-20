#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class PrintPdfDemo : Win
    {
        public PrintPdfDemo()
        {
            InitializeComponent();
            
        }

        void OnPrintFile(object sender, RoutedEventArgs e)
        {
            Kit.PrintPdf("ms-appx:///Demo.UI/Assets/dt.pdf");
        }

        void OnPrintLocal(object sender, RoutedEventArgs e)
        {
            Kit.PrintPdf("D:\\Dt\\Master\\Demo\\Demo.UI\\Assets\\dt.pdf");
        }

        void OnPrintLocal2(object sender, RoutedEventArgs e)
        {
            Kit.PrintPdf(Path.Combine(Kit.CachePath, "dt.pdf"));
        }
        
        void OnPrintStream(object sender, RoutedEventArgs e)
        {
            //MemoryStream ms = new();
            //new LvDocument(new Lv()).GeneratePdf(ms);
            //Kit.PrintPdf(ms, "lv.pdf");
        }

        async void OnDirectPrint(object sender, RoutedEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Demo.UI/Assets/dt.pdf"));
            Stream stream = await file.OpenStreamForReadAsync();
            PdfPrinter printer = new PdfPrinter();
            printer.Print(stream, file.Name);
        }
    }
}