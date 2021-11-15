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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class FileLvDemo : Win
    {
        public FileLvDemo()
        {
            InitializeComponent();
            _lv.View = Resources["TableView"];
            _lv.ViewMode = ViewMode.Table;
            LoadData();
        }

        void LoadData()
        {
            var tbl = new Table
            {
                { "fs" },
            };
            tbl.AddRow(new { fs = "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]" });
            tbl.AddRow(new { fs = "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"],[\"photo/文本文档.txt\",\"文本文档\",\"txt文件\",8,\"daoting\",\"2020-03-13 10:37\"]]" });
            tbl.AddRow(new { fs = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/Logon.wav\",\"Logon\",\"00:04\",384496,\"daoting\",\"2020-03-13 10:37\"],[\"photo/mov.mp4\",\"mov\",\"00:00:10 (320 x 176)\",788493,\"daoting\",\"2020-03-13 10:37\"],[\"photo/文本文档.txt\",\"文本文档\",\"txt文件\",8,\"daoting\",\"2020-03-13 10:37\"],[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]" });
            tbl.AddRow(new { fs = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/mov1.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]" });
            tbl.AddRow(new { fs = "[[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null]]" });
            tbl.AddRow(new { fs = "" });
            _lv.Data = tbl;
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }
    }
}