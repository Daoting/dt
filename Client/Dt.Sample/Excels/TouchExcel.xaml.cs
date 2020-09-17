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
    public partial class TouchExcel : Win
    {
        public TouchExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                _excel.ActiveSheet.SetValue(2, 1, "Goods");
                _excel.ActiveSheet.AddTable("sampleTable1", 3, 0, 5, 4, TableStyles.Medium3);
            }
        }

        void OnAutoFill(object sender, RoutedEventArgs e)
        {
            _excel.ShowAutoFillIndicator();
        }
    }
}