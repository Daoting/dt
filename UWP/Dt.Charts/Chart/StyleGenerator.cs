#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    internal class StyleGenerator
    {
        Dt.Charts.Palette _colorGen;
        Brush[] _customBrushes;
        List<ShapeStyle> _list = new List<ShapeStyle>();
        List<ShapeStyle> _list2 = new List<ShapeStyle>();
        public Converter<Brush, Brush> CustomBrushConverter;

        Brush GetCustomBrush(int i)
        {
            if (CustomBrushes == null)
            {
                return null;
            }
            Brush input = CustomBrushes[i % CustomBrushes.Length];
            if (CustomBrushConverter != null)
            {
                input = CustomBrushConverter(input);
            }
            return input;
        }

        public ShapeStyle GetStyle(int i)
        {
            if (i < _list.Count)
            {
                return _list[i];
            }
            return Next();
        }

        public ShapeStyle GetStyle2(int i)
        {
            if (i >= _list2.Count)
            {
                Next();
            }
            return _list2[i];
        }

        static void InitStyle(ShapeStyle ss, Windows.UI.Color clr, bool dark)
        {
            SolidColorBrush fill = ss.Fill as SolidColorBrush;
            if (fill == null)
            {
                try
                {
                    ss.FillAuto = ss.Fill = new SolidColorBrush(clr);
                }
                catch { }
            }
            else
            {
                fill.Color = clr;
            }
            fill = ss.Stroke as SolidColorBrush;
            Windows.UI.Color color = dark ? ColorPalette.Darken(clr) : clr;
            if (fill == null)
            {
                ss.StrokeAuto = ss.Stroke = new SolidColorBrush(color);
            }
            else
            {
                fill.Color = color;
            }
        }

        static void InitStyle(ShapeStyle ss, Brush brush, bool dark)
        {
            ss.FillAuto = ss.Fill = brush;
            Windows.UI.Color clr = Colors.Gray;
            SolidColorBrush brush2 = brush as SolidColorBrush;
            if (brush2 != null)
            {
                clr = brush2.Color;
            }
            else
            {
                GradientBrush brush3 = brush as GradientBrush;
                if (((brush3 != null) && (brush3.GradientStops != null)) && (brush3.GradientStops.Count > 0))
                {
                    clr = brush3.GradientStops[0].Color;
                }
            }
            Windows.UI.Color color2 = dark ? ColorPalette.Darken(clr) : clr;
            ss.StrokeAuto = ss.Stroke = new SolidColorBrush(color2);
        }

        public ShapeStyle Next()
        {
            return Next(true);
        }

        public ShapeStyle Next(bool dark)
        {
            ShapeStyle style2;
            ShapeStyle ss = new ShapeStyle();
            if ((CustomBrushes != null) && (_colorGen == Dt.Charts.Palette.Default))
            {
                InitStyle(ss, GetCustomBrush(_list.Count), dark);
            }
            else
            {
                Dt.Charts.Palette colorGenTheme = (_colorGen == Dt.Charts.Palette.Default) ? Dt.Charts.Palette.Apex : _colorGen;
                Windows.UI.Color clr = ColorPalette.GetOfficePalette(colorGenTheme).GetColor(_list.Count);
                InitStyle(ss, clr, dark);
            }
            ss.DarkOutline = dark;
            ss.StrokeThickness = 2.0;
            _list.Add(ss);
            style2 = new ShapeStyle {
                FillAuto = ss.FillAuto,
                Fill = ss.FillAuto,
                StrokeAuto = ss.StrokeAuto,
                Stroke = ss.StrokeAuto,
                StrokeThickness = 2.0
            };
            _list2.Add(style2);
            return ss;
        }

        public void Reset()
        {
            _list.Clear();
            _list2.Clear();
        }

        void SetColors(Dt.Charts.Palette cg)
        {
            int num = _list.Count;
            if (cg == Dt.Charts.Palette.Default)
            {
                if (CustomBrushes != null)
                {
                    for (int j = 0; j < num; j++)
                    {
                        Brush customBrush = GetCustomBrush(j);
                        InitStyle(_list[j], customBrush, _list[j].DarkOutline);
                        SyncStyle(j);
                    }
                    return;
                }
                cg = Dt.Charts.Palette.Apex;
            }
            for (int i = 0; i < num; i++)
            {
                Windows.UI.Color clr = ColorPalette.GetOfficePalette(cg).GetColor(i);
                InitStyle(_list[i], clr, _list[i].DarkOutline);
                SyncStyle(i);
            }
        }

        void SyncStyle(int i)
        {
            _list2[i].Fill = _list2[i].FillAuto = _list[i].FillAuto;
            _list2[i].Stroke = _list2[i].StrokeAuto = _list[i].StrokeAuto;
        }

        public Brush[] CustomBrushes
        {
            get { return  _customBrushes; }
            set
            {
                if (_customBrushes != value)
                {
                    _customBrushes = value;
                    if ((_customBrushes != null) && (Palette == Dt.Charts.Palette.Default))
                    {
                        int num = _list.Count;
                        for (int i = 0; i < num; i++)
                        {
                            Brush customBrush = GetCustomBrush(i);
                            InitStyle(_list[i], customBrush, _list[i].DarkOutline);
                            SyncStyle(i);
                        }
                    }
                    else
                    {
                        SetColors(Palette);
                    }
                }
            }
        }

        internal List<ShapeStyle> List
        {
            get { return  _list; }
        }

        internal List<ShapeStyle> List2
        {
            get { return  _list2; }
        }

        public Dt.Charts.Palette Palette
        {
            get { return  _colorGen; }
            set
            {
                if (_colorGen != value)
                {
                    _colorGen = value;
                    SetColors(_colorGen);
                }
            }
        }
    }
}

