#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    public partial class FileCellDemo : Win
    {
        public FileCellDemo()
        {
            InitializeComponent();
            _fv.CellClick += (s, e) => CellDemoKit.OnCellClick(e, _pbCell);
            _fv.Changed += (s, e) => CellDemoKit.OnChanged(_fv, e);
            _pbFv.Data = _fv;
            LoadData();
        }

        void LoadData()
        {
            var tbl = new Table
            {
                { "txt" },
                { "file0" },
                { "file1" },
                { "file2" },
                { "file3" },
                { "file4" },
                { "img1" },
                { "img2" },
                { "img3" },
            };

            _fv.Data = tbl.AddRow(new
            {
                file1 = "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]",
                file2 = "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"],[\"photo/文本文档.txt\",\"文本文档\",\"txt文件\",8,\"daoting\",\"2020-03-13 10:37\"]]",
                file3 = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/Logon.wav\",\"Logon\",\"00:04\",384496,\"daoting\",\"2020-03-13 10:37\"],[\"photo/mov.mp4\",\"mov\",\"00:00:10 (320 x 176)\",788493,\"daoting\",\"2020-03-13 10:37\"],[\"photo/文本文档.txt\",\"文本文档\",\"txt文件\",8,\"daoting\",\"2020-03-13 10:37\"],[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]",
                file4 = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"],[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/mov1.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null],[\"photo/banner.png\", \"头像\", \".png\", 1140, \"hdt\", null],[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]",
                img1 = "[[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null]]",
                img3 = "[[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null]]",
            });
        }

        void OnChanged(object sender, object e)
        {
            Kit.Msg("文件列表变化");
        }
    }
}