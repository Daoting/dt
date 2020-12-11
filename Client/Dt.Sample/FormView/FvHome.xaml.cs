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
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo(Icons.词典, "基础格", typeof(BaseCellDemo), "基本格、日期格、布尔格、链接格，可通过属性编辑器动态设置格属"),
                new MainInfo(Icons.分组, "选择格", typeof(SelectionCellDemo), "列表格、树形格、图标格、颜色格"),
                new MainInfo(Icons.图片, "文件格", typeof(FileCellDemo), "文件格、图像格"),
                new MainInfo(Icons.详细, "数据操作", typeof(FvData), "对二维表格数据的增删改或对普通对象的修改"),
                new MainInfo(Icons.小图标, "属性编辑器", typeof(FvObjData), "对控件或普通对象的属性进行编辑"),
                new MainInfo(Icons.排列, "自动生成格", typeof(FvAutoCell), "根据数据源列类型生成对应格"),
                new MainInfo(Icons.数据库, "多类型数据源", typeof(FvDataSource), "支持二维数据行和对普通对象作为数据源"),
                new MainInfo(Icons.芯片, "自定义内容", typeof(CustomCell), "自定义格内容"),
                new MainInfo(Icons.搜索, "查询面板", typeof(SearchFvDemo), "手机样式查询面板"),
                new MainInfo(Icons.排列, "自动布局", typeof(FvLayout), "面板布局及格的样式"),
                new MainInfo(Icons.乐谱, "外部ScrollViewer", typeof(FvInScrollViewer), "外部嵌套ScrollViewer，和其他元素一起滚动"),
                new MainInfo(Icons.乐谱, "内嵌Lv", typeof(LvInFv), "Lv作为单元格嵌套在内部"),
            };
        }
    }
}
