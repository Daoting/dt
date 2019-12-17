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
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JieHuMain : PageWin
    {

        public JieHuMain(long p_id)
        {
            InitializeComponent();
            Title = "胡志强";
            _img.Source = new BitmapImage(new Uri("ms-appx:///Bs.Kehu/Assets/header.png"));
        }

    }
}