using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells.Data;
using System;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;

namespace Dt.Shell
{
    public sealed partial class MutiTabPage : Page
    {

        public MutiTabPage()
        {
            this.InitializeComponent();

            using (_excel.Defer())
            {
                InitData();
            }
        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        void InitData()
        {
            // Range group
            Worksheet sheet = _excel.Sheets[0];

            sheet.ColumnCount = 7;
            sheet.RowCount = 34;
            sheet.Columns[0].Width = sheet.Columns[1].Width = 80;
            sheet.Columns[2].Width = 110;
            sheet.AddSelection(0, 0, 1, 1);
            // set value
            var t = new object[,]
                             {
                               {"= Eastern ==========", "", "", 0, 0, "", 0},
                               {"Eastern", "Atlantic", "Celtics", 57, 19, "-", 0.750},
                               {"Eastern", "Atlantic", "76ers", 38, 35, 17.5, 0.521},
                               {"Eastern", "Atlantic", "Nets", 31, 44, 25.5, 0.413},
                               {"Eastern", "Atlantic", "Raptors", 29, 45, 27, 0.392},
                               {"Eastern", "Atlantic", "Knicks", 29, 46, 27.5, 0.387},
                               {"Eastern", "Central", "Cavaliers", 61, 13, "-", 0.824},
                               {"Eastern", "Central", "Pistons", 36, 39, 25.5, 0.480},
                               {"Eastern", "Central", "Bulls", 36, 40, 26, 0.474},
                               {"Eastern", "Central", "Pacers", 32, 43, 29.5, 0.427},
                               {"Eastern", "Central", "Bucks", 32, 44, 30, 0.421},
                               {"Eastern", "Southeast", "Magic", 55, 19, "-", 0.743},
                               {"Eastern", "Southeast", "Hawks", 43, 32, 12.5, 0.573},
                               {"Eastern", "Southeast", "Heat", 39, 36, 16.5, 0.520},
                               {"Eastern", "Southeast", "Bobcats", 34, 41, 21.5, 0.453},
                               {"Eastern", "Southeast", "Wizards", 17, 59, 39, 0.224},
                               {"= Total ==========", "", "", 0, 0, "", 0},
                               {"= Western ==========", "", "", 0, 0, "", 0},
                               {"Western", "Northwest", "Nuggets", 49, 26, "-", 0.653},
                               {"Western", "Northwest", "Trail Blazers", 47, 27, 1.5, 0.635},
                               {"Western", "Northwest", "Jazz", 46, 28, 2.5, 0.622},
                               {"Western", "Northwest", "Thunder", 21, 53, 27.5, 0.284},
                               {"Western", "Northwest", "Timberwolves", 21, 54, 28, 0.280},
                               {"Western", "Pacific", "Lakers", 59, 16, "-", 0.787},
                               {"Western", "Pacific", "Suns", 41, 34, 18, 0.547},
                               {"Western", "Pacific", "Warriors", 26, 49, 33, 0.347},
                               {"Western", "Pacific", "Clippers", 18, 57, 41, 0.240},
                               {"Western", "Pacific", "Kings", 16, 58, 42.5, 0.216},
                               {"Western", "Southwest", "Spurs", 48, 29, "-", 0.649},
                               {"Western", "Southwest", "Rockets", 48, 27, 0.5, 0.640},
                               {"Western", "Southwest", "Hornets", 47, 27, 1, 0.635},
                               {"Western", "Southwest", "Mavericks", 45, 30, 3.5, 0.600},
                               {"Western", "Southwest", "Grizzlies", 20, 54, 28, 0.270},
                               {"= Total ==========", "", "", 0, 0, "", 0},
                             };

            for (int r = 0; r <= t.GetUpperBound(0); r++)
            {
                for (int c = 0; c <= t.GetUpperBound(1); c++)
                {
                    sheet.SetValue(r, c, t[r, c]);
                }
            }
            sheet.Cells[0, 0].ColumnSpan = 7;
            sheet.Cells[16, 0].ColumnSpan = 3;
            sheet.Cells[17, 0].ColumnSpan = 7;
            sheet.Cells[33, 0].ColumnSpan = 3;
            sheet.ColumnHeader.RowCount = 2;
            sheet.ColumnHeader.AutoTextIndex = 1;
            sheet.ColumnHeader.Cells[0, 0].Value = "2008-09 NBA Regular Season Standings";
            sheet.ColumnHeader.Cells[0, 0].ColumnSpan = 7;
            sheet.ColumnHeader.Cells[0, 0].FontFamily = new FontFamily("Arial");
            sheet.ColumnHeader.Cells[0, 0].FontSize = 14;
            sheet.ColumnHeader.Cells[0, 0].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet.ColumnHeader.Cells[0, 0].VerticalAlignment = CellVerticalAlignment.Center;
            sheet.ColumnHeader.Cells[0, 0].Foreground = new SolidColorBrush(Colors.Gray);
            sheet.ColumnHeader.Rows[0].Height = 30;

            sheet.Columns[2].Foreground = new SolidColorBrush(Colors.Blue);
            sheet.Cells[19, 2].Foreground = new SolidColorBrush(Colors.Blue);
            sheet.Columns[0].Label = "Conference";
            sheet.Columns[1].Label = "Standing";
            sheet.Columns[2].Label = "Team";
            sheet.Columns[3].Label = "W";
            sheet.Columns[4].Label = "L";
            sheet.Columns[5].Label = "GB";
            sheet.Columns[6].Label = "PCT";

            // set row range group
            sheet.RowRangeGroup.Group(1, 15); // eastern
            sheet.RowRangeGroup.Group(1, 4);
            sheet.RowRangeGroup.Group(6, 4);
            sheet.RowRangeGroup.Group(11, 4);
            sheet.RowRangeGroup.Group(18, 15); // western
            sheet.RowRangeGroup.Group(18, 4);
            sheet.RowRangeGroup.Group(23, 4);
            sheet.RowRangeGroup.Group(28, 4);
            // sheet.RowRangeGroup.Expand(1, false);
        }

        void OnPrintExcel(object sender, RoutedEventArgs e)
        {
            _excel.Print();
        }
    }
}
