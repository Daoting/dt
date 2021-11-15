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
    public sealed partial class FvHome : Win
    {
        public FvHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础格", typeof(BaseCellDemo), Icons.词典) { Desc = "基本格、日期格、布尔格、链接格，可通过属性编辑器动态设置格属" },
                new Nav("选择格", typeof(SelectionCellDemo), Icons.分组) { Desc = "列表格、树形格、图标格、颜色格" },
                new Nav("文件格", typeof(FileCellDemo), Icons.图片) { Desc = "文件格、图像格" },
                new Nav("数据操作", typeof(FvData), Icons.全选) { Desc = "对二维表格数据的增删改或对普通对象的修改" },
                new Nav("属性编辑器", typeof(FvObjData), Icons.小图标) { Desc = "对控件或普通对象的属性进行编辑" },
                new Nav("自动生成格", typeof(FvAutoCell), Icons.排列) { Desc = "根据数据源列类型生成对应格" },
                new Nav("多类型数据源", typeof(FvDataSource), Icons.数据库) { Desc = "支持二维数据行和对普通对象作为数据源" },
                new Nav("自定义内容", typeof(CustomCell), Icons.芯片) { Desc = "自定义格内容" },
                new Nav("自动布局", typeof(FvLayout), Icons.排列) { Desc = "面板布局及格的样式" },
                new Nav("外部ScrollViewer", typeof(FvInScrollViewer), Icons.乐谱) { Desc = "外部嵌套ScrollViewer，和其他元素一起滚动" },
                new Nav("内嵌Lv", typeof(LvInFv), Icons.乐谱) { Desc = "Lv作为单元格嵌套在内部" },
            };
        }
    }
}
