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
            _lv.Data = new List<CenterInfo>
            {
                new CenterInfo(Icons.分组, "格", typeof(CellsDemo), "基础编辑器，数据和界面的桥梁，通过设置属性指定交互行为和编辑方法，支持自定义"),
                new CenterInfo(Icons.详细, "数据操作", typeof(FvData), "对二维表格数据的增删改或对普通对象的修改"),
                new CenterInfo(Icons.小图标, "属性编辑器", typeof(FvObjData), "对控件或普通对象的属性进行编辑"),
                new CenterInfo(Icons.排列, "自动生成格", typeof(FvAutoCell), "根据数据源列类型生成对应格"),
                new CenterInfo(Icons.数据库, "多类型数据源", typeof(FvDataSource), "支持二维数据行和对普通对象作为数据源"),
                new CenterInfo(Icons.芯片, "自定义内容", typeof(CustomCell), "自定义格内容"),
                new CenterInfo(Icons.芯片, "查询面板", typeof(SearchFvDemo), "手机样式查询面板"),
                new CenterInfo(Icons.排列, "自动布局", typeof(FvLayout), "面板布局及格的样式"),
            };
        }
    }
}
