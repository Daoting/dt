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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class DataExcel : Win
    {
        public DataExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitializeSpread();
            }
        }

        void InitializeSpread()
        {
            _excel.ValueChanged += gcSpreadSheet1_ValueChanged;
            _excel.TabStripVisibility = Visibility.Collapsed;
            _excel.AutoClipboard = false;
            _excel.CanCellOverflow = false;
            _excel.CanUserDragFill = false;
            _excel.CanUserDragDrop = false;
            _excel.ColumnSplitBoxPolicy = SplitBoxPolicy.Never;
            _excel.RowSplitBoxPolicy = SplitBoxPolicy.Never;
            var sheet = _excel.ActiveSheet;
            sheet.SelectionPolicy = SelectionPolicy.Single;
            //sheet.SelectionUnit = SelectionUnit.Row;
            sheet.RowFilter = new HideRowFilter(new CellRange(-1, -1, -1, -1));
            //sheet.Protect = true;

            //sheet.DataSource = GetDataSource();
            //sheet.AddSelection(0, 0, 1, 1);
            //sheet.Columns[0].Locked = false;
            //sheet.Columns[1].Locked = false;
            //sheet.Columns[2].Locked = false;
            //sheet.Columns[3].Locked = false;
            //sheet.Columns[4].Locked = false;
            //sheet.Columns[0].Width = 100;
            //sheet.Columns[1].Width = 100;
            //sheet.Columns[2].Width = 200;
            //sheet.Columns[3].Width = 100;
            //sheet.Columns[4].Width = 300;

            sheet.DataSource = SampleData.CreatePersonsList(5000);
            sheet.Columns[0].Width = 150;
            sheet.Columns[1].Width = 100;
            sheet.Columns[2].Width = 60;
            sheet.Columns[3].Width = 100;
            sheet.Columns[4].Width = 200;
            sheet.Columns[5].Width = 60;
            sheet.Columns[6].Width = 60;
            sheet.Columns[7].Width = 80;
            sheet.Columns[8].Width = 80;
            sheet.Columns[9].Width = 150;
            sheet.Columns[10].Width = 100;
        }

        void gcSpreadSheet1_ValueChanged(object sender, CellEventArgs e)
        {
            if (_excel.ActiveSheet.Rows[e.Row].Tag != null) return;
            _excel.ActiveSheet.Rows[e.Row].Background = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255));
            _excel.ActiveSheet.Rows[e.Row].Tag = "Edit";
            _btnUpdate.IsEnabled = true;
        }

        Employee[] GetDataSource()
        {
            return new Employee[] {
                new Employee(){ LastName="Freehafer",   FirstName="Nancy",  Title="Sales Representative", Phone="(123)555-0100", Email="nancy@northwindtraders.com"},
                new Employee(){ LastName="Cencini", FirstName="Andrew", Title="Vice President, Sales", Phone="(123)555-0100", Email="andrew@northwindtraders.com"},
                new Employee(){ LastName="Kotas",   FirstName="Jan",    Title="Sales Representative", Phone="(123)555-0100", Email="jan@northwindtraders.com"},
                new Employee(){ LastName="Sergienko",   FirstName="Mariya", Title="Sales Representative", Phone="(123)555-0100", Email="mariya@northwindtraders.com"},
                new Employee(){ LastName="Thorpe",  FirstName="Steven", Title="Sales Manager", Phone="(123)555-0100", Email="steven@northwindtraders.com"},
                new Employee(){ LastName="Neipper", FirstName="Michael",    Title="Sales Representative", Phone="(123)555-0100", Email="michael@northwindtraders.com"},
                new Employee(){ LastName="Zare",    FirstName="Robert", Title="Sales Representative", Phone="(123)555-0100", Email="robert@northwindtraders.com"},
                new Employee(){ LastName="Giussani",    FirstName="Laura",  Title="Sales Coordinator", Phone="(123)555-0100", Email="laura@northwindtraders.com"},
                new Employee(){ LastName="Hellung-Larsen",  FirstName="Anne",   Title="Sales Representative", Phone="(123)555-0100", Email="anne@northwindtraders.com"},
            };
        }

        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_tbSearch.Text))
            {
                var sheet = _excel.ActiveSheet;
                sheet.ConditionalFormats.ClearRule();
                sheet.ConditionalFormats.AddSpecificTextRule(TextComparisonOperator.Contains, _tbSearch.Text,
                    new StyleInfo() { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold },
                    new CellRange(0, 0, sheet.RowCount, sheet.ColumnCount));
            }
        }

        void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                btnSearch_Click(null, null);
        }

        void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            _tbSearch.Text = "";
            var sheet = _excel.ActiveSheet;
            sheet.ConditionalFormats.ClearRule();
        }

        void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            sheet.RowCount = sheet.RowCount + 1;
            sheet.Rows[sheet.RowCount - 1].Background = new SolidColorBrush(Color.FromArgb(32, 27, 161, 226));
            sheet.Rows[sheet.RowCount - 1].Tag = "New";
            sheet.AddSelection(sheet.RowCount - 1, 0, 1, 1);
            _btnUpdate.IsEnabled = true;
        }

        void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            for (int i = sheet.RowCount - 1; i >= 0; i--)
            {
                if (sheet.Rows[i].Tag != null)
                {
                    if (sheet.Rows[i].Tag.ToString() == "Delete")
                    {
                        sheet.RemoveRows(i, 1);
                        continue;
                    }
                    else
                    {
                        sheet.Rows[i].ResetBackground();
                        sheet.Rows[i].Tag = null;
                    }
                }
            }
            if (sheet.ActiveRowIndex == -1 && sheet.RowCount > 0) sheet.AddSelection(sheet.RowCount - 1, 0, 1, 1);
            _btnUpdate.IsEnabled = false;
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            if (sheet.ActiveRowIndex == -1) return;
            sheet.Rows[sheet.ActiveRowIndex].Background = new SolidColorBrush(Color.FromArgb(30, 255, 0, 0));
            sheet.Rows[sheet.ActiveRowIndex].Tag = "Delete";
            _btnUpdate.IsEnabled = true;
        }

        async void SaveExcelFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
            filePicker.FileTypeChoices.Add("Excel 97-2003 Files", new List<string>(new string[] { ".xls" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                var fileName = storageFile.FileType.ToUpperInvariant();
                var fileFormat = ExcelFileFormat.XLS;
                if (fileName.EndsWith(".XLSX"))
                    fileFormat = ExcelFileFormat.XLSX;
                else
                    fileFormat = ExcelFileFormat.XLS;
                await _excel.SaveExcel(stream, fileFormat, ExcelSaveFlags.NoFlagsSet);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SavePDFFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SavePdf(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SaveXmlFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Xml文件", new List<string>(new string[] { ".xml" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveXmlAsync(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }
    }

    public class Employee
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}