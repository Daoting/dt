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
using Windows.Media.Core;
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
    public partial class PlanTest2 : Win
    {

        public PlanTest2()
        {
            InitializeComponent();

            //Windows.Media.Playback.MediaPlayer _mediaPlayer = new Windows.Media.Playback.MediaPlayer();
            //_mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Bs.Kehu/Assets/mov.mp4"));
            //_mediaPlayer.MediaEnded += OnMediaEnded;
            //_player.SetMediaPlayer(_mediaPlayer);
            //_player.MediaPlayer.MediaEnded += OnMediaEnded;
            
            _player.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri("http://mapp.wicp.net/oa/mov.mp4"));
            _player.MediaPlayer.MediaEnded += OnMediaEnded;
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            _player.MediaPlayer.Play();
        }

        void OnMediaEnded(Windows.Media.Playback.MediaPlayer sender, object args)
        {
            AtApp.OpenWin(typeof(PlanTest3));
        }
    }
}