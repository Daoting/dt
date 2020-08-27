#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
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

            _excel.SuspendEvent();
            var sheet = _excel.ActiveSheet;
            sheet.AddSpanCell(0, 0, 3, 4);
            sheet.SetValue(0, 0, "啊手动阀");
            sheet[0, 0, 2, 3].Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            
        }
    }
}
