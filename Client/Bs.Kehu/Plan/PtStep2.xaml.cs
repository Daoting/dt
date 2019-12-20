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
using Windows.Media.Playback;
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
    public partial class PtStep2 : PageWin
    {

        public PtStep2()
        {
            InitializeComponent();
            _player.MediaPlayer.MediaEnded += OnMediaEnded;
        }

        void OnMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            AtApp.GoBackToHome();
            AtApp.OpenWin(typeof(PtStep3));
        }
    }
}