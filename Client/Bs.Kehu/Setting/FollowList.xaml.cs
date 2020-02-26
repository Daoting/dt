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
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FollowList : Win
    {

        public FollowList()
        {
            InitializeComponent();

            var tbl = new Table
            {
                { "AuthorIcon" },
                { "AuthorName" },
            };
            tbl.AddRow(new { AuthorIcon = "ms-appx:///Bs.Kehu/Assets/header.png", AuthorName = "胡志强" });
            tbl.AddRow(new { AuthorIcon = "ms-appx:///Bs.Kehu/Assets/u364.png", AuthorName = "李小琳" });
            _lv.Data = tbl;
        }

    }
}