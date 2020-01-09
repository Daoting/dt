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
    public partial class MyInfoEdit : PageWin
    {

        public MyInfoEdit()
        {
            InitializeComponent();

            var tbl = new Table
            {
                { "头像",typeof(Icons) },
                { "名字" },
                { "性别" },
                { "年龄" },
                { "真实姓名" },
            };
            _fv.Data = tbl.AddRow(new { 头像 = Icons.个人信息, 名字 = "李世杰", 性别 = "男" });
        }

        void OnNext(object sender, RoutedEventArgs e)
        {
            
        }
    }
}