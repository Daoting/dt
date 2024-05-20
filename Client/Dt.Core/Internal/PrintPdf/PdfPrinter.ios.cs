/*
#if __IOS__ || MACCATALYST

#region 引用命名
using Foundation;
using UIKit;
#endregion

namespace Dt.Core;

/// <summary>
/// Pdf打印类
/// </summary>
public partial class PdfPrinter
{
    public void Print(Stream inputStream, string fileName)
    {
        var printInfo = UIPrintInfo.PrintInfo;
        printInfo.OutputType = UIPrintInfoOutputType.General;
        printInfo.JobName = "Print PDF";
 
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var library = Path.Combine(documents, "..", "Library");
        var filepath = Path.Combine(library, fileName);
 
        using var tempStream = new MemoryStream();

        inputStream.Position = 0;
        inputStream.CopyTo(tempStream);
        File.WriteAllBytes(filepath, tempStream.ToArray());

        var printer = UIPrintInteractionController.SharedPrintController;
        printInfo.OutputType = UIPrintInfoOutputType.General;
 
        printer.PrintingItem = NSUrl.FromFilename(filepath);
        printer.PrintInfo = printInfo;
 
#pragma warning disable CA1422
        printer.ShowsPageRange = true;
#pragma warning restore CA1422
 
        printer.Present(true, (handler, completed, err) => {
            if (!completed)
            {
                Console.WriteLine("error");
            }
        });
    }
}
#endif
*/