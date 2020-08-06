#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class BorderLineStyleDatas
    {
        static BorderLineData _dashDot;
        static BorderLineData _dashDotDot;
        static BorderLineData _dashed;
        static BorderLineData _dotted;
        static BorderLineData _double;
        static BorderLineData _empty;
        static BorderLineData _gridLine;
        static BorderLineData _hair;
        static BorderLineData _medium;
        static BorderLineData _mediumDashDot;
        static BorderLineData _mediumDashDotDot;
        static BorderLineData _mediumDashed;
        static BorderLineData _noBorder;
        static BorderLineData _slantedDashDot;
        static BorderLineData _thick;
        static BorderLineData _thin;

        public static BorderLineData DashDot
        {
            get
            {
                if (_dashDot == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 8.0, 2.0, 2.0, 2.0 };
                    _dashDot = new BorderLineData(100, 0, null, 1, dashMiddle, 0, null);
                }
                return _dashDot;
            }
        }

        public static BorderLineData DashDotDot
        {
            get
            {
                if (_dashDotDot == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 9.0, 3.0, 3.0, 3.0, 3.0, 3.0 };
                    _dashDotDot = new BorderLineData(100, 0, null, 1, dashMiddle, 0, null);
                    _dashDotDot.StrokeDashOffset = 5;
                }
                return _dashDotDot;
            }
        }

        public static BorderLineData Dashed
        {
            get
            {
                if (_dashed == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 3.0, 1.0 };
                    _dashed = new BorderLineData(100, 0, null, 1, dashMiddle, 0, null);
                }
                return _dashed;
            }
        }

        public static BorderLineData Dotted
        {
            get
            {
                if (_dotted == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 2.0, 2.0 };
                    _dotted = new BorderLineData(100, 0, null, 1, dashMiddle, 0, null);
                    _dotted.StrokeDashOffset = 1;
                }
                return _dotted;
            }
        }

        public static BorderLineData Double
        {
            get
            {
                if (_double == null)
                {
                    _double = new BorderLineData(90, 1, null, 1, null, 1, null);
                    _double.Linkable = false;
                    _double.DrawingThickness = 3;
                }
                return _double;
            }
        }

        public static BorderLineData Empty
        {
            get
            {
                if (_empty == null)
                {
                    _empty = new BorderLineData(0, 1, null, 0, null, 0, null);
                }
                return _empty;
            }
        }

        internal static BorderLineData GridLine
        {
            get
            {
                if (_gridLine == null)
                {
                    _gridLine = new BorderLineData(-1, 0, null, 1, null, 0, null);
                }
                return _gridLine;
            }
        }

        public static BorderLineData Hair
        {
            get
            {
                if (_hair == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 1.0 };
                    _hair = new BorderLineData(100, 0, null, 1, dashMiddle, 0, null);
                }
                return _hair;
            }
        }

        public static BorderLineData Medium
        {
            get
            {
                if (_medium == null)
                {
                    _medium = new BorderLineData(0xc7, 0, null, 2, null, 0, null);
                    _medium.DrawingThickness = 2;
                }
                return _medium;
            }
        }

        public static BorderLineData MediumDashDot
        {
            get
            {
                if (_mediumDashDot == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 4.5, 1.5, 1.5, 1.5 };
                    _mediumDashDot = new BorderLineData(0xc6, 0, null, 2, dashMiddle, 0, null);
                    _mediumDashDot.DrawingThickness = 2;
                }
                return _mediumDashDot;
            }
        }

        public static BorderLineData MediumDashDotDot
        {
            get
            {
                if (_mediumDashDotDot == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 4.5, 1.5, 1.5, 1.5, 1.5, 1.5 };
                    _mediumDashDotDot = new BorderLineData(0xc6, 0, null, 2, dashMiddle, 0, null);
                    _mediumDashDotDot.StrokeDashOffset = 2;
                    _mediumDashDotDot.DrawingThickness = 2;
                }
                return _mediumDashDotDot;
            }
        }

        public static BorderLineData MediumDashed
        {
            get
            {
                if (_mediumDashed == null)
                {
                    DoubleCollection dashMiddle = new DoubleCollection { 4.5, 1.5 };
                    _mediumDashed = new BorderLineData(0xc6, 0, null, 2, dashMiddle, 0, null);
                    _mediumDashed.StrokeDashOffset = 3;
                    _mediumDashed.DrawingThickness = 2;
                }
                return _mediumDashed;
            }
        }

        internal static BorderLineData NoBorder
        {
            get
            {
                if (_noBorder == null)
                {
                    _noBorder = new BorderLineData(-2, 0, null, 1, null, 0, null);
                }
                return _noBorder;
            }
        }

        public static BorderLineData SlantedDashDot
        {
            get
            {
                if (_slantedDashDot == null)
                {
                    DoubleCollection dashFar = new DoubleCollection { 11.0, 1.0, 5.0, 1.0 };
                    DoubleCollection dashMiddle = new DoubleCollection { 10.0, 2.0, 4.0, 2.0 };
                    _slantedDashDot = new BorderLineData(0xc6, 1, dashFar, 1, dashMiddle, 0, null);
                    _slantedDashDot.DrawingThickness = 1;
                }
                return _slantedDashDot;
            }
        }

        public static BorderLineData Thick
        {
            get
            {
                if (_thick == null)
                {
                    _thick = new BorderLineData(300, 1, null, 1, null, 1, null);
                    _thick.DrawingThickness = 3;
                }
                return _thick;
            }
        }

        public static BorderLineData Thin
        {
            get
            {
                if (_thin == null)
                {
                    _thin = new BorderLineData(0x65, 0, null, 1, null, 0, null);
                }
                return _thin;
            }
        }
    }
}

