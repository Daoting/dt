#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using SkiaSharp;
using SkiaSharp.Skottie;
using SkiaSharp.Views.Windows;
using System.Diagnostics;
#endregion

namespace Dt.Sample
{
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

            Load("LottieLogo1.json", ref _animation1, _watch1, _timer1, _cvs1);
            Load("LightBulb.json", ref _animation2, _watch2, _timer2, _cvs2);
        }

        private void Load(string p_file, ref Animation p_animation, Stopwatch p_watch, DispatcherQueueTimer p_timer, SKXamlCanvas p_cvs)
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
            p_timer.Start();

            p_watch.Start();
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
}
