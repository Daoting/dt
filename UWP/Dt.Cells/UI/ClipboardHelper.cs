#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.ApplicationModel.DataTransfer;
#endregion

namespace Dt.Cells.UI
{
    internal static class ClipboardHelper
    {
        public static void ClearClipboard()
        {
            try
            {
                Clipboard.Clear();
            }
            catch
            {
            }
            ClearSpreadXClipboard();
        }

        public static void ClearSpreadXClipboard()
        {
            SpreadXClipboard.Worksheet = null;
            SpreadXClipboard.Range = null;
            SpreadXClipboard.IsCutting = false;
            SpreadXClipboard.FloatingObjects = null;
        }

        public static string GetClipboardData()
        {
            try
            {
                return WindowsRuntimeSystemExtensions.AsTask<string>(Clipboard.GetContent().GetTextAsync()).Result;
            }
            catch
            {
                return null;
            }
        }

        public static void SetClipboardData(string text)
        {
            try
            {
                DataPackage package2 = new DataPackage();
                package2.RequestedOperation = DataPackageOperation.Copy;
                DataPackage content = package2;
                if (text == string.Empty)
                {
                    Clipboard.SetContent(null);
                }
                else
                {
                    content.SetText(text);
                    Clipboard.SetContent(content);
                }
            }
            catch
            {
            }
        }
    }
}

