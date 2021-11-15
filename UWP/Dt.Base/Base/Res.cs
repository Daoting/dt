#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Globalization;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 资源管理类
    /// </summary>
    public static class Res
    {
        // 全局资源字典
        readonly static ResourceDictionary _dict = Application.Current.Resources;

        #region 标准颜色画刷
        static SolidColorBrush _transparentBrush;
        static SolidColorBrush _blackBrush;
        static SolidColorBrush _blueBrush;
        static SolidColorBrush _brownBrush;
        static SolidColorBrush _cyanBrush;
        static SolidColorBrush _darkGrayBrush;
        static SolidColorBrush _grayBrush;
        static SolidColorBrush _greenBrush;
        static SolidColorBrush _lightGrayBrush;
        static SolidColorBrush _magentaBrush;
        static SolidColorBrush _orangeBrush;
        static SolidColorBrush _purpleBrush;
        static SolidColorBrush _redBrush;
        static SolidColorBrush _whiteBrush;
        static SolidColorBrush _yellowBrush;

        /// <summary>
        /// 透明色
        /// </summary>
        public static SolidColorBrush TransparentBrush
        {
            get
            {
                if (_transparentBrush == null)
                    _transparentBrush = new SolidColorBrush(Colors.Transparent);
                return _transparentBrush;
            }
        }

        /// <summary>
        /// 黑色
        /// </summary>
        public static SolidColorBrush BlackBrush
        {
            get
            {
                if (_blackBrush == null)
                    _blackBrush = new SolidColorBrush(Colors.Black);
                return _blackBrush;
            }
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        public static SolidColorBrush BlueBrush
        {
            get
            {
                if (_blueBrush == null)
                    _blueBrush = new SolidColorBrush(Colors.Blue);
                return _blueBrush;
            }
        }

        /// <summary>
        /// 深褐色
        /// </summary>
        public static SolidColorBrush BrownBrush
        {
            get
            {
                if (_brownBrush == null)
                    _brownBrush = new SolidColorBrush(Colors.Brown);
                return _brownBrush;
            }
        }

        /// <summary>
        /// 青色
        /// </summary>
        public static SolidColorBrush CyanBrush
        {
            get
            {
                if (_cyanBrush == null)
                    _cyanBrush = new SolidColorBrush(Colors.Cyan);
                return _cyanBrush;
            }
        }

        /// <summary>
        /// 深灰
        /// </summary>
        public static SolidColorBrush DarkGrayBrush
        {
            get
            {
                if (_darkGrayBrush == null)
                    _darkGrayBrush = new SolidColorBrush(Colors.DarkGray);
                return _darkGrayBrush;
            }
        }

        /// <summary>
        /// 灰色
        /// </summary>
        public static SolidColorBrush GrayBrush
        {
            get
            {
                if (_grayBrush == null)
                    _grayBrush = new SolidColorBrush(Colors.Gray);
                return _grayBrush;
            }
        }

        /// <summary>
        /// 绿色
        /// </summary>
        public static SolidColorBrush GreenBrush
        {
            get
            {
                if (_greenBrush == null)
                    _greenBrush = new SolidColorBrush(Colors.Green);
                return _greenBrush;
            }
        }

        /// <summary>
        /// 浅灰1
        /// </summary>
        public static SolidColorBrush LightGrayBrush
        {
            get
            {
                if (_lightGrayBrush == null)
                    _lightGrayBrush = new SolidColorBrush(Colors.LightGray);
                return _lightGrayBrush;
            }
        }

        /// <summary>
        /// 品红
        /// </summary>
        public static SolidColorBrush MagentaBrush
        {
            get
            {
                if (_magentaBrush == null)
                    _magentaBrush = new SolidColorBrush(Colors.Magenta);
                return _magentaBrush;
            }
        }

        /// <summary>
        /// 桔色
        /// </summary>
        public static SolidColorBrush OrangeBrush
        {
            get
            {
                if (_orangeBrush == null)
                    _orangeBrush = new SolidColorBrush(Colors.Orange);
                return _orangeBrush;
            }
        }

        /// <summary>
        /// 紫色
        /// </summary>
        public static SolidColorBrush PurpleBrush
        {
            get
            {
                if (_purpleBrush == null)
                    _purpleBrush = new SolidColorBrush(Colors.Purple);
                return _purpleBrush;
            }
        }

        /// <summary>
        /// 红色
        /// </summary>
        public static SolidColorBrush RedBrush
        {
            get
            {
                if (_redBrush == null)
                    _redBrush = new SolidColorBrush(Colors.Red);
                return _redBrush;
            }
        }

        /// <summary>
        /// 白色
        /// </summary>
        public static SolidColorBrush WhiteBrush
        {
            get
            {
                if (_whiteBrush == null)
                    _whiteBrush = new SolidColorBrush(Colors.White);
                return _whiteBrush;
            }
        }

        /// <summary>
        /// 黄色
        /// </summary>
        public static SolidColorBrush YellowBrush
        {
            get
            {
                if (_yellowBrush == null)
                    _yellowBrush = new SolidColorBrush(Colors.Yellow);
                return _yellowBrush;
            }
        }

        /// <summary>
        /// 根据标准颜色的名称获取画刷
        /// </summary>
        /// <param name="p_colorName">颜色的名称</param>
        /// <returns>返回画刷</returns>
        public static SolidColorBrush GetBrushByName(string p_colorName)
        {
            if (string.IsNullOrEmpty(p_colorName))
                return BlackBrush;

            switch (p_colorName.ToLower())
            {
                case "black":
                    return BlackBrush;
                case "blue":
                    return BlueBrush;
                case "brown":
                    return BrownBrush;
                case "cyan":
                    return CyanBrush;
                case "darkGray":
                    return DarkGrayBrush;
                case "gray":
                    return GrayBrush;
                case "green":
                    return GreenBrush;
                case "lightGray":
                    return LightGrayBrush;
                case "magenta":
                    return MagentaBrush;
                case "orange":
                    return OrangeBrush;
                case "purple":
                    return PurpleBrush;
                case "red":
                    return RedBrush;
                case "white":
                    return WhiteBrush;
                case "yellow":
                    return YellowBrush;

            }
            return BlackBrush;
        }
        #endregion

        #region 系统画刷
        public static SolidColorBrush 主蓝 => (SolidColorBrush)_dict["主蓝"];
        public static SolidColorBrush 默认背景 => (SolidColorBrush)_dict["默认背景"];
        public static SolidColorBrush 默认前景 => (SolidColorBrush)_dict["默认前景"];
        public static SolidColorBrush 暗遮罩 => (SolidColorBrush)_dict["暗遮罩"];
        public static SolidColorBrush 深暗遮罩 => (SolidColorBrush)_dict["深暗遮罩"];
        public static SolidColorBrush 亮遮罩 => (SolidColorBrush)_dict["亮遮罩"];
        public static SolidColorBrush 深亮遮罩 => (SolidColorBrush)_dict["深亮遮罩"];
        public static SolidColorBrush 黄遮罩 => (SolidColorBrush)_dict["黄遮罩"];
        public static SolidColorBrush 深黄遮罩 => (SolidColorBrush)_dict["深黄遮罩"];
        public static SolidColorBrush 中灰1 => (SolidColorBrush)_dict["中灰1"];
        public static SolidColorBrush 中灰2 => (SolidColorBrush)_dict["中灰2"];
        public static SolidColorBrush 浅灰1 => (SolidColorBrush)_dict["浅灰1"];
        public static SolidColorBrush 浅灰2 => (SolidColorBrush)_dict["浅灰2"];
        public static SolidColorBrush 深灰1 => (SolidColorBrush)_dict["深灰1"];
        public static SolidColorBrush 深灰2 => (SolidColorBrush)_dict["深灰2"];
        public static SolidColorBrush 中黄 => (SolidColorBrush)_dict["中黄"];
        public static SolidColorBrush 浅黄 => (SolidColorBrush)_dict["浅黄"];
        public static SolidColorBrush 中绿 => (SolidColorBrush)_dict["中绿"];
        public static SolidColorBrush 浅绿 => (SolidColorBrush)_dict["浅绿"];
        public static SolidColorBrush 湖蓝 => (SolidColorBrush)_dict["湖蓝"];
        public static SolidColorBrush 浅蓝 => (SolidColorBrush)_dict["浅蓝"];
        public static SolidColorBrush 深蓝 => (SolidColorBrush)_dict["深蓝"];
        public static SolidColorBrush 亮蓝 => (SolidColorBrush)_dict["亮蓝"];
        public static SolidColorBrush 亮红 => (SolidColorBrush)_dict["亮红"];
        #endregion

        #region 系统资源
        /// <summary>
        /// 字符按钮样式
        /// </summary>
        public static Style 字符按钮 => (Style)_dict["字符按钮"];

        /// <summary>
        /// 浅色按钮样式
        /// </summary>
        public static Style 浅色按钮 => (Style)_dict["浅色按钮"];

        /// <summary>
        /// Phone模式的搜索栏按钮
        /// </summary>
        public static Style PhonSearchButton => (Style)_dict["PhonSearchButton"];

        /// <summary>
        /// 默认字体大小
        /// </summary>
        public static double DefaultFontSize => (double)_dict["ControlContentThemeFontSize"];

        /// <summary>
        /// 图标字体
        /// </summary>
        public static FontFamily IconFont => (FontFamily)_dict["IconFont"];

        /// <summary>
        /// Lv中默认文本
        /// </summary>
        public static Style LvTextBlock => (Style)_dict["LvTextBlock"];

        /// <summary>
        /// Fv中的文本编辑框
        /// </summary>
        public static Style FvTextBox => (Style)_dict["FvTextBox"];

        /// <summary>
        /// 小一，32px
        /// </summary>
        public static double 特大字 => (double)_dict["特大字"];

        /// <summary>
        /// 小二，24px
        /// </summary>
        public static double 大字 => (double)_dict["大字"];

        /// <summary>
        /// 小三，20px
        /// </summary>
        public static double 标题字 => (double)_dict["标题字"];

        /// <summary>
        /// 小四，16px
        /// </summary>
        public static double 默认字 => (double)_dict["默认字"];

        /// <summary>
        /// 五号，14px
        /// </summary>
        public static double 小字 => (double)_dict["小字"];

        /// <summary>
        /// 小五，12px
        /// </summary>
        public static double 特小字 => (double)_dict["特小字"];

        /// <summary>
        /// 默认行外高，含边框1
        /// </summary>
        public static double RowOuterHeight => (double)_dict["RowOuterHeight"];

        /// <summary>
        /// 默认行内容高度
        /// </summary>
        public static double RowInnerHeight => (double)_dict["RowInnerHeight"];
        #endregion

        #region 动画
        static TransitionCollection _naviTransition;
        static TransitionCollection _forwardTransition;
        static TransitionCollection _backTransition;
        static TransitionCollection _addDeleteTransition;

        /// <summary>
        /// 页面导航动画
        /// </summary>
        public static TransitionCollection NaviTransition
        {
            get
            {
                if (_naviTransition == null)
                {
                    _naviTransition = new TransitionCollection();
                    NavigationThemeTransition tran = new NavigationThemeTransition();
                    tran.DefaultNavigationTransitionInfo = new DrillInNavigationTransitionInfo();
                    _naviTransition.Add(tran);
                }
                return _naviTransition;
            }
        }

        /// <summary>
        /// 前进效果动画
        /// </summary>
        public static TransitionCollection ForwardTransition
        {
            get
            {
                if (_forwardTransition == null)
                {
                    _forwardTransition = new TransitionCollection();
                    PaneThemeTransition tran = new PaneThemeTransition();
                    tran.Edge = EdgeTransitionLocation.Right;
                    _forwardTransition.Add(tran);
                }
                return _forwardTransition;
            }
        }

        /// <summary>
        /// 返回效果动画
        /// </summary>
        public static TransitionCollection BackTransition
        {
            get
            {
                if (_backTransition == null)
                {
                    _backTransition = new TransitionCollection();
                    PaneThemeTransition tran = new PaneThemeTransition();
                    tran.Edge = EdgeTransitionLocation.Left;
                    _backTransition.Add(tran);
                }
                return _backTransition;
            }
        }

        /// <summary>
        /// 返回效果动画
        /// </summary>
        public static TransitionCollection AddDeleteTransition
        {
            get
            {
                if (_addDeleteTransition == null)
                {
                    _addDeleteTransition = new TransitionCollection();
                    _addDeleteTransition.Add(new AddDeleteThemeTransition());
                }
                return _addDeleteTransition;
            }
        }
        #endregion

        #region 图标
        /// <summary>
        /// 根据图标类型生成图标文字
        /// </summary>
        /// <param name="p_icon">图标资源名称</param>
        /// <param name="p_size">大小</param>
        /// <param name="p_brush">画刷颜色</param>
        /// <returns>图标文字</returns>
        public static TextBlock GetIcon(Icons p_icon, int p_size = 20, Brush p_brush = null)
        {
            TextBlock tb = new TextBlock();
            tb.FontFamily = IconFont;
            tb.FontSize = p_size;
            tb.Text = GetIconChar(p_icon);
            if (p_brush != null)
                tb.Foreground = p_brush;
            return tb;
        }

        /// <summary>
        /// 获取图标的unicode字符
        /// </summary>
        /// <param name="p_icon"></param>
        /// <returns>unicode字符</returns>
        public static string GetIconChar(Icons p_icon)
        {
            if (p_icon == Icons.None)
                return "";

            // 自定义Icon字库在 E000 ~ FFFF 之间
            // 枚举类型定义了在字库中的偏移量
            int index = 0xE000 + (int)p_icon;
            return Encoding.Unicode.GetChars(BitConverter.GetBytes(index))[0].ToString();
        }

        /// <summary>
        /// 根据图标名称获取图标枚举类型
        /// </summary>
        /// <param name="p_icon">图标名称</param>
        /// <returns>图标枚举类型</returns>
        public static Icons ParseIcon(string p_icon)
        {
            if (string.IsNullOrEmpty(p_icon))
                return Icons.None;
            Icons icon;
            if (Enum.TryParse<Icons>(p_icon, out icon))
                return icon;
            return Icons.None;
        }

        /// <summary>
        /// 根据图标名称获取unicode字符
        /// </summary>
        /// <param name="p_icon">图标名称</param>
        /// <returns>unicode字符</returns>
        public static string ParseIconChar(string p_icon)
        {
            return GetIconChar(ParseIcon(p_icon));
        }

        /// <summary>
        /// 解析Path.Data字符串内容，返回Geometry
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns>返回Geometry对象</returns>
        public static PathGeometry ParseGeometry(string p_data)
        {
            p_data = p_data.ToUpper();
            PathGeometry pg = new PathGeometry();
            PathFigureCollection pfc = new PathFigureCollection();
            pg.Figures = pfc;
            PathFigure pf = new PathFigure();
            pf.IsFilled = true;
            pf.IsClosed = true;
            pfc.Add(pf);

            // 去掉头
            p_data = p_data.TrimStart('M', ' ');
            // 开始节点
            string first = p_data.Substring(0, GetCharIdx(p_data));
            string[] point = first.Split(',');
            pf.StartPoint = new Point(double.Parse(point[0]), double.Parse(point[1]));
            p_data = p_data.Substring(first.Length).TrimStart();
            // 处理以下数据
            bool isOver = false;
            while (!isOver)
            {
                int pos = GetCharIdx(p_data);
                int next = 0;
                if (pos == 0)
                {
                    first = p_data.Substring(0, 1);
                    p_data = p_data.Substring(1);
                    string data;

                    switch (first)
                    {
                        case "L":
                            next = GetCharIdx(p_data);
                            data = p_data.Substring(0, next);
                            p_data = p_data.Substring(next).Trim();
                            point = data.Split(',');
                            LineSegment ls = new LineSegment();
                            ls.Point = new Point(double.Parse(point[0]), double.Parse(point[1]));
                            pf.Segments.Add(ls);
                            break;
                        case "C":
                            BezierSegment bs = new BezierSegment();
                            next = GetCharIdx(p_data);
                            data = p_data.Substring(0, next);
                            p_data = p_data.Substring(next).Trim();
                            point = data.Split(',');
                            bs.Point1 = new Point(double.Parse(point[0]), double.Parse(point[1]));
                            next = GetCharIdx(p_data);
                            data = p_data.Substring(0, next);
                            p_data = p_data.Substring(next).Trim();
                            point = data.Split(',');
                            bs.Point2 = new Point(double.Parse(point[0]), double.Parse(point[1]));
                            next = GetCharIdx(p_data);
                            data = p_data.Substring(0, next);
                            p_data = p_data.Substring(next).Trim();
                            point = data.Split(',');
                            bs.Point3 = new Point(double.Parse(point[0]), double.Parse(point[1]));
                            pf.Segments.Add(bs);
                            break;
                        case "Z":
                            isOver = true;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    // 无字母表示，默认为时Line标志，省略了L字母
                    next = GetCharIdx(p_data);
                    string data = p_data.Substring(0, next);
                    p_data = p_data.Substring(next).Trim();
                    point = data.Split(',');
                    LineSegment ls = new LineSegment();
                    ls.Point = new Point(double.Parse(point[0]), double.Parse(point[1]));
                    pf.Segments.Add(ls);
                }

            }

            return pg;
        }

        /// <summary>
        /// 根据path的定义字符串形成path
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static Path ParsePath(string p_data)
        {
            Path path = new Path();
            path.Data = ParseGeometry(p_data);
            return path;
        }

        /// <summary>
        /// 深度克隆PathGeometry
        /// </summary>
        /// <param name="p_pathGeometry"></param>
        /// <returns></returns>
        public static PathGeometry CloneGeometry(PathGeometry p_pathGeometry)
        {
            PathFigureCollection figures = new PathFigureCollection();
            foreach (PathFigure figure in p_pathGeometry.Figures)
            {
                PathFigure fig = new PathFigure();
                fig.IsClosed = figure.IsClosed;
                fig.IsFilled = figure.IsFilled;
                fig.Segments = CloneSegments(figure.Segments);
                fig.StartPoint = figure.StartPoint;
                figures.Add(fig);
            }
            PathGeometry geometry = new PathGeometry();
            geometry.Figures = figures;
            geometry.FillRule = p_pathGeometry.FillRule;
            geometry.Transform = p_pathGeometry.Transform;
            return geometry;
        }

        /// <summary>
        /// 深度克隆PathSegment集合
        /// </summary>
        /// <param name="p_pathSegColl"></param>
        /// <returns></returns>
        static PathSegmentCollection CloneSegments(PathSegmentCollection p_pathSegColl)
        {
            PathSegmentCollection segments = new PathSegmentCollection();
            foreach (PathSegment segment in p_pathSegColl)
            {
                PathSegment seg = null;
                if (segment is LineSegment)
                {
                    LineSegment lineSrc = segment as LineSegment;
                    LineSegment lineTgt = new LineSegment();
                    lineTgt.Point = lineSrc.Point;
                    seg = lineTgt;
                }
                else if (segment is PolyLineSegment)
                {
                    PolyLineSegment polySrc = segment as PolyLineSegment;
                    PolyLineSegment polyTgt = new PolyLineSegment();
                    polyTgt.Points = ClonePoints(polySrc.Points);
                    seg = polyTgt;
                }
                else if (segment is BezierSegment)
                {
                    BezierSegment bezierSrc = segment as BezierSegment;
                    BezierSegment bezierTgt = new BezierSegment();
                    bezierTgt.Point1 = bezierSrc.Point1;
                    bezierTgt.Point2 = bezierSrc.Point2;
                    bezierTgt.Point3 = bezierSrc.Point3;
                    seg = bezierTgt;
                }
                else if (segment is PolyBezierSegment)
                {
                    PolyBezierSegment polyBezSrc = segment as PolyBezierSegment;
                    PolyBezierSegment polyBezTgt = new PolyBezierSegment();
                    polyBezTgt.Points = ClonePoints(polyBezSrc.Points);
                    seg = polyBezTgt;
                }
                else if (segment is PolyQuadraticBezierSegment)
                {
                    PolyQuadraticBezierSegment polyQuaSrc = segment as PolyQuadraticBezierSegment;
                    PolyQuadraticBezierSegment polyQuaTgt = new PolyQuadraticBezierSegment();
                    polyQuaTgt.Points = ClonePoints(polyQuaSrc.Points);
                    seg = polyQuaTgt;
                }
                else if (segment is QuadraticBezierSegment)
                {
                    QuadraticBezierSegment quadSrc = segment as QuadraticBezierSegment;
                    QuadraticBezierSegment quadTgt = new QuadraticBezierSegment();
                    quadTgt.Point1 = quadSrc.Point1;
                    quadTgt.Point2 = quadSrc.Point2;
                    seg = quadTgt;
                }
                else if (segment is ArcSegment)
                {
                    ArcSegment arcSrc = segment as ArcSegment;
                    ArcSegment arcTgt = new ArcSegment();
                    arcTgt.IsLargeArc = arcSrc.IsLargeArc;
                    arcTgt.Point = arcSrc.Point;
                    arcTgt.RotationAngle = arcSrc.RotationAngle;
                    arcTgt.Size = arcSrc.Size;
                    arcTgt.SweepDirection = arcSrc.SweepDirection;
                    seg = arcTgt;
                }
                segments.Add(seg);
            }
            return segments;
        }

        /// <summary>
        /// 复制点集合
        /// </summary>
        /// <param name="p_pts"></param>
        /// <returns></returns>
        static PointCollection ClonePoints(PointCollection p_pts)
        {
            PointCollection points = new PointCollection();
            foreach (Point point in p_pts)
            {
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// 取得字符串中第一个不是数字和点的字符的位置。
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        static int GetCharIdx(string p_str)
        {
            string num = ",-1234567890.";
            for (int i = 0; i < p_str.Length; i++)
            {
                if (num.IndexOf(p_str[i]) < 0)
                    return i;
            }

            return -1;
        }
        #endregion

        #region FileItem资源
        /// <summary>
        /// FileItem默认模板
        /// </summary>
        internal static ControlTemplate VirFileTemplate => (ControlTemplate)_dict["VirFileTemplate"];

        /// <summary>
        /// FileItem图片模板
        /// </summary>
        internal static ControlTemplate VirImageTemplate => (ControlTemplate)_dict["VirImageTemplate"];

        /// <summary>
        /// FileItem视频模板
        /// </summary>
        internal static ControlTemplate VirVideoTemplate => (ControlTemplate)_dict["VirVideoTemplate"];
        #endregion

        #region 颜色
        /// <summary>
        /// 由标准Color形式字符串转换成Color对象，
        /// </summary>
        /// <param name="p_hexColor">color 的字符串表示</param>
        /// <returns></returns>
        public static Color HexStringToColor(string p_hexColor)
        {
            Color color = new Color();
            if (string.IsNullOrEmpty(p_hexColor))
                return color;
            p_hexColor = p_hexColor.Trim('#');
            if (p_hexColor.Length != 8)
                return color;
            try
            {
                color = Color.FromArgb(byte.Parse(p_hexColor.Substring(0, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(2, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(4, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(6, 2), NumberStyles.HexNumber));
            }
            catch
            {
                return color;
            }
            return color;
        }
        #endregion
    }
}
