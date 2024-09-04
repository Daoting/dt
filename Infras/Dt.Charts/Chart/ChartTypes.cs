#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Charts
{
    internal static class ChartTypes
    {
        static readonly List<ChartTypeInfo> _gallery = new List<ChartTypeInfo>();

        static ChartTypes()
        {
            Init();
        }

        internal static ChartSubtype GetSubtype(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (name != "HighLowOpenClose")
            {
                for (int i = 3; i < name.Length; i++)
                {
                    if (char.IsUpper(name[i]) || char.IsNumber(name[i]))
                    {
                        name = name.Insert(i, ".");
                        break;
                    }
                }
            }
            string[] strArray = name.Split(new char[] { '.' });
            if ((strArray.Length < 0) || (strArray.Length > 2))
            {
                return null;
            }
            ChartTypeInfo info = null;
            ChartSubtype subtype = null;
            foreach (ChartTypeInfo info2 in _gallery)
            {
                if (info2.Name == strArray[0])
                {
                    info = info2;
                    break;
                }
            }
            if (info == null)
            {
                return subtype;
            }
            if (strArray.Length == 2)
            {
                return info[strArray[1]];
            }
            return info.Subtypes[0];
        }

        static void Init()
        {
            Bar bar = new Bar();
            DotSymbol symbol = new DotSymbol();
            Lines lines = new Lines();
            lines.StrokeThickness = 3.0;
            ChartTypeInfo info = new ChartTypeInfo("Column");
            ChartSubtype subtype = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None");
            ChartSubtype subtype2 = new ChartSubtype("Stacked", "Renderer2D", "Inverted=false;Stacked=Stacked");
            ChartSubtype subtype3 = new ChartSubtype("Stacked100pc", "Renderer2D", "Inverted=false;Stacked=Stacked100pc");
            subtype.Symbol = bar;
            subtype2.Symbol = bar;
            subtype3.Symbol = bar;
            info.Subtypes.Add(subtype);
            info.Subtypes.Add(subtype2);
            info.Subtypes.Add(subtype3);
            _gallery.Add(info);

            ChartTypeInfo info2 = new ChartTypeInfo("Bar");
            ChartSubtype subtype4 = new ChartSubtype("Default", "Renderer2D", "Inverted=true;Stacked=None");
            ChartSubtype subtype5 = new ChartSubtype("Stacked", "Renderer2D", "Inverted=true;Stacked=Stacked");
            ChartSubtype subtype6 = new ChartSubtype("Stacked100pc", "Renderer2D", "Inverted=true;Stacked=Stacked100pc");
            subtype4.Symbol = bar;
            subtype5.Symbol = bar;
            subtype6.Symbol = bar;
            info2.Subtypes.Add(subtype4);
            info2.Subtypes.Add(subtype5);
            info2.Subtypes.Add(subtype6);
            _gallery.Add(info2);

            PieSlice slice = new PieSlice();
            ChartTypeInfo info3 = new ChartTypeInfo("Pie");
            ChartSubtype subtype7 = new ChartSubtype("Default", "Pie", "Offset=0;InnerRadius=0");
            ChartSubtype subtype8 = new ChartSubtype("Exploded", "Pie", "Offset=0.2;InnerRadius=0");
            ChartSubtype subtype9 = new ChartSubtype("Doughnut", "Pie", "Offset=0;InnerRadius=0.4");
            ChartSubtype subtype10 = new ChartSubtype("ExplodedDoughnut", "Pie", "Offset=0.2;InnerRadius=0.4");
            ChartSubtype subtype11 = new ChartSubtype("Stacked", "Pie", "Offset=0;InnerRadius=0");
            subtype7.Symbol = slice.Clone();
            subtype8.Symbol = slice.Clone();
            subtype9.Symbol = slice.Clone();
            subtype10.Symbol = slice.Clone();
            subtype11.Symbol = slice.Clone();
            info3.Subtypes.Add(subtype7);
            info3.Subtypes.Add(subtype8);
            info3.Subtypes.Add(subtype9);
            info3.Subtypes.Add(subtype10);
            info3.Subtypes.Add(subtype11);
            _gallery.Add(info3);

            Lines lines2 = new Lines();
            lines2.StrokeThickness = 3.0;
            lines2.Smoothed = true;
            ChartTypeInfo info4 = new ChartTypeInfo("Line");
            ChartSubtype subtype12 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None");
            ChartSubtype subtype13 = new ChartSubtype("Stacked", "Renderer2D", "Inverted=false;Stacked=Stacked");
            ChartSubtype subtype14 = new ChartSubtype("Stacked100pc", "Renderer2D", "Inverted=false;Stacked=Stacked100pc");
            subtype12.Connection = lines;
            subtype13.Connection = lines;
            subtype14.Connection = lines;
            ChartSubtype subtype15 = new ChartSubtype("Smoothed", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = lines2
            };
            ChartSubtype subtype16 = new ChartSubtype("Symbols", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = lines,
                Symbol = symbol
            };
            ChartSubtype subtype17 = new ChartSubtype("SymbolsSmoothed", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = lines2,
                Symbol = symbol
            };
            ChartSubtype subtype18 = new ChartSubtype("SymbolsStacked", "Renderer2D", "Inverted=false;Stacked=Stacked")
            {
                Connection = lines,
                Symbol = symbol
            };
            ChartSubtype subtype19 = new ChartSubtype("SymbolsStacked100pc", "Renderer2D", "Inverted=false;Stacked=Stacked100pc")
            {
                Connection = lines,
                Symbol = symbol
            };
            info4.Subtypes.Add(subtype12);
            info4.Subtypes.Add(subtype13);
            info4.Subtypes.Add(subtype14);
            info4.Subtypes.Add(subtype15);
            info4.Subtypes.Add(subtype16);
            info4.Subtypes.Add(subtype17);
            info4.Subtypes.Add(subtype18);
            info4.Subtypes.Add(subtype19);
            _gallery.Add(info4);

            Area area = new Area();
            Area area2 = new Area
            {
                Smoothed = true
            };
            ChartTypeInfo info5 = new ChartTypeInfo("Area");
            ChartSubtype subtype20 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None");
            ChartSubtype subtype21 = new ChartSubtype("Stacked", "Renderer2D", "Inverted=false;Stacked=Stacked");
            ChartSubtype subtype22 = new ChartSubtype("Stacked100pc", "Renderer2D", "Inverted=false;Stacked=Stacked100pc");
            subtype20.Connection = area;
            subtype21.Connection = area;
            subtype22.Connection = area;
            ChartSubtype subtype23 = new ChartSubtype("Smoothed", "Renderer2D", "Inverted=false;Stacked=None");
            ChartSubtype subtype24 = new ChartSubtype("SmoothedStacked", "Renderer2D", "Inverted=false;Stacked=Stacked");
            ChartSubtype subtype25 = new ChartSubtype("SmoothedStacked100pc", "Renderer2D", "Inverted=false;Stacked=Stacked100pc");
            subtype23.Connection = area2;
            subtype24.Connection = area2;
            subtype25.Connection = area2;
            info5.Subtypes.Add(subtype20);
            info5.Subtypes.Add(subtype21);
            info5.Subtypes.Add(subtype22);
            info5.Subtypes.Add(subtype23);
            info5.Subtypes.Add(subtype24);
            info5.Subtypes.Add(subtype25);
            _gallery.Add(info5);

            ChartTypeInfo info6 = new ChartTypeInfo("XYPlot");
            ChartSubtype subtype26 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Symbol = symbol
            };
            info6.Subtypes.Add(subtype26);
            _gallery.Add(info6);

            ChartTypeInfo info7 = new ChartTypeInfo("Bubble");
            ChartSubtype subtype27 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Symbol = symbol
            };
            info7.Subtypes.Add(subtype27);
            _gallery.Add(info7);

            ChartTypeInfo info8 = new ChartTypeInfo("Radar");
            ChartSubtype subtype28 = new ChartSubtype("Default", "Radar", "Stacked=None;IsPolar=false")
            {
                Symbol = null,
                Connection = lines
            };
            info8.Subtypes.Add(subtype28);
            ChartSubtype subtype29 = new ChartSubtype("Symbols", "Radar", "Stacked=None;IsPolar=false")
            {
                Symbol = symbol,
                Connection = lines
            };
            info8.Subtypes.Add(subtype29);
            ChartSubtype subtype30 = new ChartSubtype("Filled", "Radar", "Stacked=None;IsPolar=false")
            {
                Symbol = null
            };
            Area area3 = new Area();
            subtype30.Connection = area3;
            info8.Subtypes.Add(subtype30);
            _gallery.Add(info8);

            ChartTypeInfo info9 = new ChartTypeInfo("HighLowOpenClose");
            ChartSubtype subtype31 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Symbol = new HLOC()
            };
            info9.Subtypes.Add(subtype31);
            _gallery.Add(info9);

            ChartTypeInfo info10 = new ChartTypeInfo("Candle");
            ChartSubtype subtype32 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None");
            HLOC hloc = new HLOC
            {
                Appearance = HLOCAppearance.Candle
            };
            subtype32.Symbol = hloc;
            info10.Subtypes.Add(subtype32);
            _gallery.Add(info10);

            ChartTypeInfo info11 = new ChartTypeInfo("Gantt");
            ChartSubtype subtype33 = new ChartSubtype("Default", "Renderer2D", "Inverted=true;Stacked=None")
            {
                Symbol = new HLBar()
            };
            info11.Subtypes.Add(subtype33);
            _gallery.Add(info11);

            ChartTypeInfo info12 = new ChartTypeInfo("Step");
            StepLines lines4 = new StepLines();
            lines4.StrokeThickness = 3.0;
            StepLines lines3 = lines4;
            ChartSubtype subtype34 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = lines3
            };
            info12.Subtypes.Add(subtype34);
            ChartSubtype subtype35 = new ChartSubtype("Symbols", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = lines3,
                Symbol = symbol
            };
            info12.Subtypes.Add(subtype35);
            ChartSubtype subtype36 = new ChartSubtype("Area", "Renderer2D", "Inverted=false;Stacked=None")
            {
                Connection = new StepArea()
            };
            info12.Subtypes.Add(subtype36);
            ChartSubtype subtype37 = new ChartSubtype("AreaStacked", "Renderer2D", "Inverted=false;Stacked=Stacked")
            {
                Connection = new StepArea()
            };
            info12.Subtypes.Add(subtype37);
            _gallery.Add(info12);

            ChartTypeInfo info13 = new ChartTypeInfo("Polar");
            ChartSubtype subtype38 = new ChartSubtype("Lines", "Radar", "Stacked=None;IsPolar=true")
            {
                Symbol = null,
                Connection = lines
            };
            info13.Subtypes.Add(subtype38);
            ChartSubtype subtype39 = new ChartSubtype("Symbols", "Radar", "Stacked=None;IsPolar=true")
            {
                Symbol = symbol,
                Connection = null
            };
            info13.Subtypes.Add(subtype39);
            ChartSubtype subtype40 = new ChartSubtype("LinesSymbols", "Radar", "Stacked=None;IsPolar=true")
            {
                Symbol = symbol,
                Connection = lines
            };
            info13.Subtypes.Add(subtype40);
            _gallery.Add(info13);

            ChartTypeInfo info14 = new ChartTypeInfo("Polygon");
            ChartSubtype subtype41 = new ChartSubtype("Default", "Renderer2D", "Inverted=false;Stacked=None");
            Lines lines5 = new Lines
            {
                IsClosed = true
            };
            subtype41.Connection = lines5;
            info14.Subtypes.Add(subtype41);
            ChartSubtype subtype42 = new ChartSubtype("Filled", "Renderer2D", "Inverted=false;Stacked=None");
            Lines lines6 = new Lines
            {
                IsClosed = true,
                IsFilled = true
            };
            subtype42.Connection = lines6;
            info14.Subtypes.Add(subtype42);
            _gallery.Add(info14);
        }
    }
}

