#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
#if !WIN
using Microsoft.Toolkit.Uwp.UI.Lottie;
#endif
#endregion

namespace Dt.Sample
{
    public sealed partial class LottieDemo : Win
    {
        string[] _names = new string[]
        {
            "AlBoardman",
            "BeatingHeart",
            "BeloFoggy",
            "Camera",
            "Countdown",
            "Dash",
            "EmptyState",
            "FavoriteStar",
            "Gears",
            "HamburgerArrow",
            "Loading1",
            "Loading2",
            "Loading3",
            "Loading4",
            "LottieLogo1",
            "LottieLogo2",
            "MailSent",
            "MotionCorpse-Jrcanest",
            "Name",
            "Octopus",
            "PinJump",
            "PlayLike",
            "Postcard",
            "Preloader",
            "ProgressSuccess",
            "Retweet",
            "Shock",
            "Tongue",
            "TouchID",
            "TwitterHeart",
            "VideoCamera",
            "WeAccept",
            "Wink",
        };

        int _cur = 0;

        public LottieDemo()
        {
            InitializeComponent();

            LoadLottie();
        }

        void OnPreview(object sender, RoutedEventArgs e)
        {
            if (_cur > 0)
                _cur--;
            else
                _cur = _names.Length - 1;

            LoadLottie();
        }

        void OnNext(object sender, RoutedEventArgs e)
        {
            if (_cur < _names.Length - 1)
                _cur++;
            else
                _cur = 0;

            LoadLottie();
        }

        void OnPause(object sender, RoutedEventArgs e)
        {
            if (_player.IsPlaying)
            {
                _player.Pause();
                _btnPlay.Content = "\uE02D";
            }
            else
            {
                _player.Resume();
                _btnPlay.Content = "\uE02E";
            }
        }

#if WIN
        void LoadLottie()
        {
        }
#else
        void LoadLottie()
        {
            // uno 支持嵌入资源文件，但winui不支持
            //Uri uri = new Uri("embedded://Dt.Sample/Dt.Sample.Resource.LottieLogo1.json");
            Uri uri = new Uri($"ms-appx:///Assets/Lottie/{_names[_cur]}.json");

            // 切换动画时改变_player.Source无效，只能重置UriSource！
            if (_player.Source == null)
                _player.Source = new LottieVisualSource { UriSource = uri };
            else
                ((LottieVisualSource)_player.Source).UriSource = uri;

            _tb.Text = _names[_cur] + ".json";
        }
#endif
    }


    // Skottie https://github.com/unoplatform/Uno.Samples/blob/master/UI/SkottieSample/SkottieSample/SkottieSample.Shared/MainPage.xaml.cs
    /*
    public sealed partial class LottieDemo : Win
    {
        Stopwatch _watch1 = new Stopwatch();
        Animation _animation1;
        DispatcherQueueTimer _timer1;

        Stopwatch _watch2 = new Stopwatch();
        Animation _animation2;
        DispatcherQueueTimer _timer2;


        public LottieDemo()
        {
            InitializeComponent();

            Load("LottieLogo1.json", ref _animation1, _watch1, ref _timer1, _cvs1);
            Load("LightBulb.json", ref _animation2, _watch2, ref _timer2, _cvs2);
            _grid.Loaded += OnLoaded;
            _grid.Unloaded += OnUnloaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            _timer1?.Start();
            _timer2?.Start();
            _watch1?.Start();
            _watch2?.Start();
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _timer1?.Stop();
            _timer2?.Stop();
            _watch1?.Stop();
            _watch2?.Stop();
        }

        void Load(string p_file, ref Animation p_animation, Stopwatch p_watch, ref DispatcherQueueTimer p_timer, SKXamlCanvas p_cvs)
        {
            using var stream = ResKit.GetResource(p_file);
            using var fileStream = new SKManagedStream(stream);

            if (SkiaSharp.Skottie.Animation.TryCreate(fileStream, out p_animation))
            {
                p_animation.Seek(0, null);

                Console.WriteLine($"动画: Version:{p_animation.Version} Duration:{p_animation.Duration} Fps:{p_animation.Fps} InPoint:{p_animation.InPoint} OutPoint:{p_animation.OutPoint}");
            }
            else
            {
                Console.WriteLine($"加载动画失败");
            }

            p_timer = DispatcherQueue.CreateTimer();
            p_timer.Interval = TimeSpan.FromSeconds(Math.Max(1 / 60.0, 1 / p_animation.Fps));
            p_timer.Tick += (s, e) => p_cvs.Invalidate();
        }

        private void OnPaint1(object sender, SKPaintSurfaceEventArgs e)
        {
            DoPaint(e, _animation1, _watch1);
        }

        private void OnPaint2(object sender, SKPaintSurfaceEventArgs e)
        {
            DoPaint(e, _animation2, _watch2);
        }

        private void DoPaint(SKPaintSurfaceEventArgs e, Animation animation, Stopwatch watch)
        {
            if (animation != null)
            {
                animation.SeekFrameTime((float)watch.Elapsed.TotalSeconds, null);

                if (watch.Elapsed.TotalSeconds > animation.Duration.TotalSeconds)
                {
                    watch.Restart();
                }

                animation.Render(e.Surface.Canvas, new SKRect(0, 0, animation.Size.Width, animation.Size.Height));
            }
        }
    }
    */
}
