#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvCellUI : Win
    {
        public LvCellUI()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            Table tbl = new Table
            {
                { "scale", typeof(double) },
                { "date", typeof(DateTime) },
                { "Icon", typeof(int) },
                { "IconName", typeof(Icons) },
                { "CheckBox", typeof(bool) },
                { "Image" },
                { "File" },
                { "Enum1", typeof(byte) },
                { "Enum2", typeof(HostOS) },
                { "AutoDate", typeof(DateTime) },
                { "Warning", typeof(int) },
            };

            Random rand = new Random();
            DateTime birth = Kit.Now;
            for (int i = 0; i < 50; i++)
            {
                tbl.AddRow(new
                {
                    scale = rand.NextDouble(),
                    date = birth.AddMonths(rand.Next(100)),
                    Icon = i,
                    IconName = (Icons)i,
                    CheckBox = (i % 2 == 0),
                    Image = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]",
                    File = (i % 2 == 0) ? "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]" : "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/mov1.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]",
                    Enum1 = rand.Next(0, 6),
                    Enum2 = (HostOS)rand.Next(0, 6),
                    AutoDate = birth.AddHours(-rand.Next(100)),
                    Warning = rand.Next(200),
                });
            }
            _lv.Data = tbl;
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["GridView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }
    }
}