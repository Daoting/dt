#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Demo.UI
{
    public partial class ImgExcel : Win
    {
        public ImgExcel()
        {
            InitializeComponent();
        }

        async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = Kit.GetFileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            StorageFile file = await filePicker.PickSingleFileAsync();
            if (file == null)
                return;

            int startRow = 0;
            int startColumn = 0;
            Worksheet sheet = _excel.ActiveSheet;
            int selectCount = sheet.Selections.Count;
            if (selectCount >= 1)
            {
                CellRange cellRange = sheet.Selections[selectCount - 1];
                startRow = cellRange.Row;
                startColumn = cellRange.Column;
            }
            try
            {
                _excel.SuspendEvent();
                var stream = await file.OpenStreamForReadAsync();
                sheet.AddPicture(CreatePictureName(), stream);
                stream.Dispose();
            }
            finally
            {
                _excel.ResumeEvent();
                _excel.RefreshPictures();
            }
        }

        string CreatePictureName()
        {
            SpreadPictures picutres = _excel.ActiveSheet.Pictures;
            return "Picture" + (picutres.Count > 0 ? Int32.Parse(picutres[picutres.Count - 1].Name.Substring(7)) + 1 : 1);
        }
    }
}