#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using System;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            Log.Debug("Before new TestDemo1()");
            InitializeComponent();
            Log.Debug("new TestDemo1()");
        }

        void OnTest1(object sender, RoutedEventArgs e)
        {
            ConvertRgbToArgb(0xff, 0xE4, 0x66, 0x33);
            //ConvertRgbToArgb(0xff, 0xff, 204, 0x33);

            //ConvertArgbToRgb(0x19, 0xff, 0xff, 0);
            //ConvertArgbToRgb(0x33, 0xff, 0xff, 0);
            ConvertArgbToRgb(0x33, 0xfa, 0xd0, 0);
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            Kit.Toast("标题", DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });
        }

        void ConvertRgbToArgb(byte p_r, byte p_g, byte p_b, byte p_tgtAlpha)
        {
            // 背景为白色
            var alpha = p_tgtAlpha / 255.0;
            byte r = (byte)(Math.Abs((p_r - 255) / alpha + 255));
            byte g = (byte)(Math.Abs((p_g - 255) / alpha + 255));
            byte b = (byte)(Math.Abs((int)((p_b - 255) / alpha) % 255 + 255));
            Console.WriteLine(r);
            Console.WriteLine(g);
            Console.WriteLine(b);
        }

        void ConvertArgbToRgb(byte p_ahpha, byte p_r, byte p_g, byte p_b)
        {
            var alpha = p_ahpha / 255.0;
            var diff = 1.0 - alpha;
            byte r = (byte)(p_r * alpha + 255 * diff);
            byte g = (byte)(p_g * alpha + 255 * diff);
            byte b = (byte)(p_b * alpha + 255 * diff);
            Console.WriteLine(r);
            Console.WriteLine(g);
            Console.WriteLine(b);
        }
    }
}