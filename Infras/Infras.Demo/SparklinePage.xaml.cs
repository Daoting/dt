using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells.Data;
using System;

namespace Infras.Demo
{
    public sealed partial class SparklinePage : Page
    {
        public SparklinePage()
        {
            this.InitializeComponent();

            using (_excel.Defer())
            {
                InitializeSample();
            }
        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        void InitializeSample()
        {
            Worksheet sheet = _excel.ActiveSheet;
            _excel.CanCellOverflow = true;

            sheet.SetValue(0, 0, "Data Range is A2-A9");
            sheet.SetValue(1, 0, 1);
            sheet.SetValue(2, 0, -2);
            sheet.SetValue(3, 0, -1);
            sheet.SetValue(4, 0, 6);
            sheet.SetValue(5, 0, 4);
            sheet.SetValue(6, 0, -4);
            sheet.SetValue(7, 0, 3);
            sheet.SetValue(8, 0, 8);


            sheet.SetValue(0, 2, "Date axis range is C2-C9");
            sheet.SetValue(1, 2, new DateTime(2011, 1, 5));
            sheet.SetValue(2, 2, new DateTime(2011, 1, 1));
            sheet.SetValue(3, 2, new DateTime(2011, 2, 11));
            sheet.SetValue(4, 2, new DateTime(2011, 3, 1));
            sheet.SetValue(5, 2, new DateTime(2011, 2, 1));
            sheet.SetValue(6, 2, new DateTime(2011, 2, 3));
            sheet.SetValue(7, 2, new DateTime(2011, 3, 6));
            sheet.SetValue(8, 2, new DateTime(2011, 2, 19));

            CellRange data = new CellRange(1, 0, 8, 1);
            CellRange dateAxis = new CellRange(1, 2, 8, 1);


            sheet.Cells["A12"].Text = "Sparkline without dateAxis:";

            sheet.Cells["A13"].Text = "(1) Line";
            sheet.Cells["d13"].Text = "(2)Column";
            sheet.Cells["g13"].Text = "(3)Winloss";

            //line
            sheet.Cells["A14"].ColumnSpan = 3;
            sheet.Cells["A14"].RowSpan = 4;
            sheet.SetSparkline(
                13,
                0,
                data,
                DataOrientation.Vertical
                , SparklineType.Line
                , new SparklineSetting()
                {
                    ShowMarkers = true,
                    LineWeight = 3,
                    DisplayXAxis = true
                ,
                    ShowFirst = true
                ,
                    ShowLast = true
                ,
                    ShowLow = true
                ,
                    ShowHigh = true
                ,
                    ShowNegative = true
                }
                );

            //column
            sheet.Cells["d14"].ColumnSpan = 3;
            sheet.Cells["d14"].RowSpan = 4;
            sheet.SetSparkline(13, 3, data
                , DataOrientation.Vertical
                , SparklineType.Column
                , new SparklineSetting()
                {
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //winloss
            sheet.Cells["g14"].ColumnSpan = 3;
            sheet.Cells["g14"].RowSpan = 4;
            sheet.SetSparkline(13, 6, data
                , DataOrientation.Vertical
                , SparklineType.Winloss
                , new SparklineSetting()
                {
                    DisplayXAxis = true,
                    ShowNegative = true
                    //,ShowFirst= true
                    //,ShowLast = true
                    //,ShowLow= true
                    //,ShowHigh = true
                }
                );



            //////////////////////////////////////////////
            sheet.Cells["A18"].Text = "Sparkline with dateAxis:";

            sheet.Cells["A19"].Text = "(1) Line";
            sheet.Cells["d19"].Text = "(2)Column";
            sheet.Cells["g19"].Text = "(3)Winloss";

            //line
            sheet.Cells["A20"].ColumnSpan = 3;
            sheet.Cells["A20"].RowSpan = 4;
            sheet.SetSparkline(19, 0, data
                , DataOrientation.Vertical
                , SparklineType.Line
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    ShowMarkers = true,
                    LineWeight = 3,
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //column
            sheet.Cells["d20"].ColumnSpan = 3;
            sheet.Cells["d20"].RowSpan = 4;
            sheet.SetSparkline(19, 3, data
                , DataOrientation.Vertical
                , SparklineType.Column
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //winloss
            sheet.Cells["g20"].ColumnSpan = 3;
            sheet.Cells["g20"].RowSpan = 4;
            sheet.SetSparkline(19, 6, data
                , DataOrientation.Vertical
                , SparklineType.Winloss
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    ShowNegative = true,
                    DisplayXAxis = true
                    //,ShowFirst = true
                    //,ShowLast = true
                    //,ShowLow = true
                    //,ShowHigh = true
                }
                );

            _excel.ActiveSheet.AddSelection(0, 0, 1, 1);
        }
    }
}
