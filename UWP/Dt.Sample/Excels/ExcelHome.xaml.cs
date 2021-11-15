#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class ExcelHome : Win
    {
        public ExcelHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("基本操作", typeof(BaseExcel), Icons.汉堡) { Desc = "Excel基本编辑功能及设置" },
                new Nav("用例", typeof(UseCase), Icons.分组) { Desc = "常见使用用例" },
                new Nav("数据操作", typeof(DataExcel), Icons.全选) { Desc = "Excel数据源" },
                new Nav("图表", typeof(ChartExcel), Icons.日历) { Desc = "内嵌图表" },
                new Nav("迷你图", typeof(Sparkline), Icons.修改),                        
                new Nav("过滤", typeof(FilterExcel), Icons.划卡),
                new Nav("分组排序", typeof(RangeGroup), Icons.排列),
                new Nav("浮动对象", typeof(FloatingObject), Icons.录音),
                new Nav("触摸时选择", typeof(TouchExcel), Icons.点击),
                new Nav("格式化", typeof(Formatter), Icons.全选),
                new Nav("样式", typeof(ExcelTheme), Icons.日历),
                new Nav("图片", typeof(ImgExcel), Icons.修改),
                new Nav("图表图片", typeof(ChartPicture), Icons.划卡),
                new Nav("打印", typeof(PrintExcel), Icons.打印) { Desc = "只支持Windows" },
            };
        }
    }
}
