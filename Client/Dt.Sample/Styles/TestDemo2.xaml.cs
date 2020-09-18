#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo2 : Win
    {
        public TestDemo2()
        {
            InitializeComponent();

            //_excel.ActiveSheet.RowCount = 3;
            //_excel.ActiveSheet.ColumnCount = 2;

            //var sheet = _excel.ActiveSheet;
            //sheet.SetValue(0, 0, "System.Random r = new System.Random();");
        }

        void FillSampleData(Worksheet sheet, CellRange range)
        {
            System.Random r = new System.Random();
            for (int i = 0; i < range.RowCount; i++)
            {
                for (int j = 0; j < range.ColumnCount; j++)
                {
                    sheet.SetValue(range.Row + i, range.Column + j, r.Next(50, 300));
                }
            }
        }

        void OnTest(object sender, RoutedEventArgs e)
        {
            _excel.ShowAutoFillIndicator();
            //_excel.InvalidateMeasure();
        }
    }
}
