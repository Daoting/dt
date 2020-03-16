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
                { "file1" },

                { "img1" },
            };

            _fv.Data = tbl.AddRow(new
            {
                file1 = "[[\"photo/项目文档.docx\",\"项目文档\",\"docx文件\",13071,\"daoting\",\"2020-03-13 10:37\"]]",

                img1 = "[[\"photo/profilephoto.jpg\", \"头像\", \".jpg\", 1140, \"hdt\", null]]",
            });
        }
    }
}