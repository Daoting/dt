#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal static class FillEffects
    {
        static Brush _Black = null;
        static readonly object _lock = new object();
        static Brush _Red = null;
        static Brush _White = null;
        static Brush _Yellow = null;

        static Brush EnsureBrush(ref Brush field, Windows.UI.Color defValue)
        {
            if (field == null)
            {
                lock (_lock)
                {
                    if (field == null)
                        field = new SolidColorBrush(defValue);
                }
            }
            return field;
        }

        /// <summary>
        /// Converts the effect to a color.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>The color.</returns>
        public static Windows.UI.Color ToColor(GradientBrush effect)
        {
            Windows.UI.Color color;
            if ((effect.GradientStops != null) && (effect.GradientStops.Count > 0))
            {
                color = effect.GradientStops[0].Color;
            }
            else
            {
                color = Colors.Transparent;
            }
            return color;
        }

        public static Brush Black
        {
            get { return  EnsureBrush(ref _Black, Colors.Black); }
        }

        public static Brush Red
        {
            get { return  EnsureBrush(ref _Red, Colors.Red); }
        }

        public static Brush White
        {
            get { return  EnsureBrush(ref _White, Colors.White); }
        }

        public static Brush Yellow
        {
            get { return  EnsureBrush(ref _Yellow, Colors.Yellow); }
        }
    }
}

