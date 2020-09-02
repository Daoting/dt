#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Drawing;
using Dt.Pdf.Object;
using Dt.Pdf.Object.Filter;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// Graphics
    /// </summary>
    internal class Graphics
    {
        readonly GcReportContext context;
        BaseFont defaultChineseFont;
        BaseFont defaultFont;
        BaseFont defaultJapanFont;
        BaseFont defaultKoreaFont;
        readonly PdfExporter exporter;
        float fillAlpha;
        readonly Stack<float> fillAlphas;
        bool grayMode;
        MatrixMock matrix;
        readonly Stack<MatrixMock> matrixs;
        readonly PdfGraphics pdfGraphics;
        float strokeAlpha;
        readonly Stack<float> strokeAlphas;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Graphics" /> class.
        /// </summary>
        public Graphics()
        {
            this.matrixs = new Stack<MatrixMock>();
            this.strokeAlpha = 1f;
            this.fillAlpha = 1f;
            this.strokeAlphas = new Stack<float>();
            this.fillAlphas = new Stack<float>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Graphics" /> class.
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="exporter">The exporter</param>
        /// <param name="pdfGraphics">The PDF graphics</param>
        internal Graphics(GcReportContext context, PdfExporter exporter, PdfGraphics pdfGraphics)
        {
            this.matrixs = new Stack<MatrixMock>();
            this.strokeAlpha = 1f;
            this.fillAlpha = 1f;
            this.strokeAlphas = new Stack<float>();
            this.fillAlphas = new Stack<float>();
            this.context = context;
            this.exporter = exporter;
            this.pdfGraphics = pdfGraphics;
        }

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void AddImage(Image image, double x, double y, double width, double height)
        {
            this.pdfGraphics.AddImage(image, (float) width, 0f, 0f, -((float) height), (float) x, (float) (y + height));
        }

        /// <summary>
        /// Applies the fill effect.
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="isStroke">If set to <c>true</c> [is stroke].</param>
        /// <param name="isFill">If set to <c>true</c> [is fill].</param>
        public void ApplyFillEffect(Brush effect, Windows.Foundation.Rect rect, bool isStroke, bool isFill)
        {
            if (Utilities.HasFillEffect(effect))
            {
                if (effect is SolidColorBrush)
                {
                    SolidColorBrush solid = effect as SolidColorBrush;
                    if (isStroke)
                    {
                        this.SetRGBStroke(solid.Color);
                    }
                    if (isFill)
                    {
                        this.SetRGBFill(solid.Color);
                    }
                }
                if (effect is ImageBrush)
                {
                    ImageBrush imageEF = (ImageBrush) effect;
                    Image image = null;
                    ImageSource imageSource = null;
                    imageSource = imageEF.ImageSource;
                    image = this.exporter.GetImage(imageSource);
                    if (image != null)
                    {
                        if ((rect.Width <= 0.0) || (rect.Height <= 0.0))
                        {
                            return;
                        }
                        PdfTilingPattern p = new PdfTilingPattern(this.exporter.CurrentDocument);
                        p.Filters.Enqueue(PdfFilter.FlateFilter);
                        PdfGraphics g = p.Graphics;
                        float width = (float) rect.Width;
                        float height = (float) rect.Height;
                        float imageWidth = image.Width;
                        float imageHeight = image.Height;
                        float offsetX = 0f;
                        float offsetY = 0f;

                        switch (imageEF.Stretch)
                        {
                            case Stretch.None:
                                offsetX = 0f;
                                switch (imageEF.AlignmentX)
                                {
                                    case AlignmentX.Center:
                                        offsetX = (width - imageWidth) / 2f;
                                        goto Label_0092;

                                    case AlignmentX.Right:
                                        offsetX = width - imageWidth;
                                        goto Label_0092;
                                }
                                throw new ArgumentOutOfRangeException();

                            case Stretch.Fill:
                                width += 0.5f;
                                height += 0.5f;
                                g.AddImage(image, width, 0f, 0f, -height, 0f, height);
                                p.BBox = new PdfRectangle(0f, 0f, width, height);
                                p.XStep = width;
                                p.YStep = height;
                                return;

                            case Stretch.Uniform:
                            case Stretch.UniformToFill:
                                {
                                    width += 0.5f;
                                    height += 0.5f;
                                    bool flag = imageEF.Stretch == Stretch.UniformToFill;
                                    float num = imageWidth / imageHeight;
                                    offsetX = 0f;
                                    offsetY = 0f;
                                    if ((!flag && ((width / num) <= height)) || (flag && ((width / num) >= height)))
                                    {
                                        imageWidth = width;
                                        imageHeight = imageWidth / num;
                                        switch (imageEF.AlignmentY)
                                        {
                                            case AlignmentY.Top:
                                                goto Label_039E;

                                            case AlignmentY.Center:
                                                offsetY = (height - imageHeight) / 2f;
                                                goto Label_039E;

                                            case AlignmentY.Bottom:
                                                offsetY = height - imageHeight;
                                                goto Label_039E;
                                        }
                                        throw new ArgumentOutOfRangeException();
                                    }
                                    imageHeight = height;
                                    imageWidth = imageHeight * num;
                                    switch (imageEF.AlignmentX)
                                    {
                                        case AlignmentX.Left:
                                            goto Label_039E;

                                        case AlignmentX.Center:
                                            offsetX = (width - imageWidth) / 2f;
                                            goto Label_039E;

                                        case AlignmentX.Right:
                                            offsetX = width - imageWidth;
                                            goto Label_039E;
                                    }
                                    throw new ArgumentOutOfRangeException();
                                }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    Label_0092:
                        offsetY = 0f;
                        switch (imageEF.AlignmentY)
                        {
                            case AlignmentY.Top:
                                break;

                            case AlignmentY.Center:
                                offsetY = (height - imageHeight) / 2f;
                                break;

                            case AlignmentY.Bottom:
                                offsetY = height - imageHeight;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        g.AddImage(image, imageWidth, 0f, 0f, -imageHeight, offsetX, imageHeight + offsetY);
                        p.BBox = new PdfRectangle(0f, 0f, width, height);
                        p.XStep = width;
                        p.YStep = height;
                        return;
                    Label_039E:
                        g.AddImage(image, imageWidth, 0f, 0f, -imageHeight, offsetX, offsetY + imageHeight);
                        p.BBox = new PdfRectangle(0f, 0f, width, height);
                        p.XStep = width;
                        p.YStep = height;

                        p.Properties.Add(PdfName.Matrix, new PdfMatrix(this.matrix.M11, this.matrix.M12, this.matrix.M21, this.matrix.M22, this.matrix.OffsetX, this.matrix.OffsetY));
                        if (isStroke)
                        {
                            SetStrokeAlpha((float)imageEF.Opacity);
                            this.pdfGraphics.SetPatternStroke(p);
                        }
                        if (isFill)
                        {
                            SetFillAlpha((float)imageEF.Opacity);
                            this.pdfGraphics.SetPatternFill(p);
                        }
                    }
                }
                if (effect is GradientBrush)
                {
                    GradientBrush brush = effect as GradientBrush;
                    List<GradientStop> gradientSections = GetGradientSections(brush);
                    GradientStyle gradientStyle = GetGradinetStyle(brush);
                    if ((gradientSections == null) || (gradientSections.Count <= 1))
                    {
                        if (isStroke)
                        {
                            this.SetRGBStroke(FillEffects.ToColor(brush));
                        }
                        if (isFill)
                        {
                            this.SetRGBFill(FillEffects.ToColor(brush));
                        }
                    }
                    else
                    {
                        Windows.Foundation.Point point;
                        Windows.Foundation.Point point2;
                        PdfShading shading;
                        List<PdfColor> colors = new List<PdfColor>();
                        List<float> offsets = new List<float>();
                        using (List<GradientStop>.Enumerator enumerator = gradientSections.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                GradientStop tmpSection = enumerator.Current;
                                Windows.UI.Color tmpColor = new Windows.UI.Color();
                                tmpColor = this.GrayMode ? Utilities.GetGrayColor(tmpSection.Color) : tmpSection.Color;
                                if (gradientStyle == GradientStyle.RadialIntoCenter)
                                {
                                    colors.Insert(0, new PdfColor(tmpColor.R, tmpColor.G, tmpColor.B));
                                    offsets.Insert(0, 1f - (((float)tmpSection.Offset) / 100f));
                                }
                                else
                                {
                                    colors.Add(new PdfColor(tmpColor.R, tmpColor.G, tmpColor.B));
                                    offsets.Add((float)tmpSection.Offset);
                                }
                            }
                        }
                        bool flag = true;
                        switch (gradientStyle)
                        {
                            case GradientStyle.LinearTopDown:
                                point = new Windows.Foundation.Point(rect.X + (rect.Width / 2.0), rect.Y);
                                point2 = new Windows.Foundation.Point(rect.X + (rect.Width / 2.0), rect.Y + rect.Height);
                                break;

                            case GradientStyle.LinearBottomUp:
                                point2 = new Windows.Foundation.Point(rect.X + (rect.Width / 2.0), rect.Y);
                                point = new Windows.Foundation.Point(rect.X + (rect.Width / 2.0), rect.Y + rect.Height);
                                break;

                            case GradientStyle.LinearLeftRight:
                                point = new Windows.Foundation.Point(rect.X, rect.Y + (rect.Height / 2.0));
                                point2 = new Windows.Foundation.Point(rect.X + rect.Width, rect.Y + (rect.Height / 2.0));
                                break;

                            case GradientStyle.LinearRightLeft:
                                point2 = new Windows.Foundation.Point(rect.X, rect.Y + (rect.Height / 2.0));
                                point = new Windows.Foundation.Point(rect.X + rect.Width, rect.Y + (rect.Height / 2.0));
                                break;

                            case GradientStyle.LinearTopRightBottomLeft:
                                point = new Windows.Foundation.Point(rect.X + rect.Width, rect.Y);
                                point2 = new Windows.Foundation.Point(rect.X, rect.Y + rect.Height);
                                break;

                            case GradientStyle.LinearTopLeftBottomRight:
                                point = new Windows.Foundation.Point(rect.X, rect.Y);
                                point2 = new Windows.Foundation.Point(rect.X + rect.Width, rect.Y + rect.Height);
                                break;

                            case GradientStyle.RadialFromCenter:
                            case GradientStyle.RadialIntoCenter:
                                flag = false;
                                point = new Windows.Foundation.Point(rect.X + (rect.Width / 2.0), rect.Y + (rect.Height / 2.0));
                                point2 = new Windows.Foundation.Point(rect.X, rect.Y + rect.Height);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        point = GetPoint(point.X, point.Y, this.matrix);
                        point2 = GetPoint(point2.X, point2.Y, this.matrix);
                        if (flag)
                        {
                            shading = PdfAxialShading.Create((float) point.X, (float) point.Y, (float) point2.X, (float) point2.Y, colors, offsets);
                        }
                        else
                        {
                            double introduced20 = Math.Pow(point.X - point2.X, 2.0);
                            shading = PdfRadialShading.Create((float) point.X, (float) point.Y, 0f, (float) point.X, (float) point.Y, (float) Math.Sqrt(introduced20 + Math.Pow(point.Y - point2.Y, 2.0)), colors, offsets);
                        }
                        float alpha = ((float) FillEffects.ToColor(brush).A) / 255f;
                        if (isStroke)
                        {
                            this.SetStrokeAlpha(alpha);
                            this.pdfGraphics.SetShadingStroke(shading);
                        }
                        if (isFill)
                        {
                            this.SetFillAlpha(alpha);
                            this.pdfGraphics.SetShadingFill(shading);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies the matrix.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">The b</param>
        /// <param name="c">The c</param>
        /// <param name="d">The d</param>
        /// <param name="x">The x</param>
        /// <param name="y">The y</param>
        void ApplyMatrix(double a, double b, double c, double d, double x, double y)
        {
            this.pdfGraphics.ApplyCTM((float) a, (float) b, (float) c, (float) d, (float) x, (float) y);
        }

        /// <summary>
        /// Applies the text matrix.
        /// </summary>
        /// <param name="a">The a value.</param>
        /// <param name="b">The b value.</param>
        /// <param name="c">The c value.</param>
        /// <param name="d">The d value.</param>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public void ApplyTextMatrix(double a, double b, double c, double d, double x, double y)
        {
            this.pdfGraphics.SetTextMatrix((float) ((float) a), (float) ((float) b), (float) ((float) c), (float) ((float) d), (float) ((float) x), (float) ((float) y));
        }

        /// <summary>
        /// Creates an arc of the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The stop angle.</param>
        public void Arc(Windows.Foundation.Rect rect, double startAngle, double stopAngle)
        {
        }

        /// <summary>
        /// Creates an arc with the specified x1 point.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The stop angle.</param>
        public void Arc(double x1, double y1, double x2, double y2, double startAngle, double stopAngle)
        {
            this.pdfGraphics.Arc((float) x1, (float) y1, (float) x2, (float) y2, (float) startAngle, (float) stopAngle);
        }

        /// <summary>
        /// Creates an arc with the specified x1 point.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The stop angle.</param>
        public void Arc2(double x1, double y1, double x2, double y2, double startAngle, double stopAngle)
        {
            this.pdfGraphics.Arc2((float) x1, (float) y1, (float) x2, (float) y2, (float) startAngle, (float) stopAngle);
        }

        /// <summary>
        /// Begins the text.
        /// </summary>
        public void BeginText()
        {
            this.pdfGraphics.BeginText();
        }

        /// <summary>
        /// Clips this instance.
        /// </summary>
        public void Clip()
        {
            this.pdfGraphics.Clip();
            this.pdfGraphics.NewPath();
        }

        /// <summary>
        /// Closes the path.
        /// </summary>
        public void ClosePath()
        {
            this.pdfGraphics.ClosePath();
        }

        /// <summary>
        /// Comments the specified comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        public void Comment(string comment)
        {
            this.pdfGraphics.Comment(comment);
        }

        /// <summary>
        /// Converts to string format.
        /// </summary>
        /// <param name="align">The alignment</param>
        /// <returns></returns>
        static StringFormat ConvertToStringFormat(ContentAlignment align)
        {
            StringFormat format = new StringFormat();
            if (align != null)
            {
                switch (align.HorizontalAlignment)
                {
                    case TextHorizontalAlignment.General:
                    case TextHorizontalAlignment.Justify:
                        break;

                    case TextHorizontalAlignment.Left:
                        format.Alignment = StringAlignment.Near;
                        break;

                    case TextHorizontalAlignment.Center:
                        format.Alignment = StringAlignment.Center;
                        break;

                    case TextHorizontalAlignment.Right:
                        format.Alignment = StringAlignment.Far;
                        break;

                    case TextHorizontalAlignment.Distributed:
                        format.Alignment = StringAlignment.Center;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                switch (align.VerticalAlignment)
                {
                    case TextVerticalAlignment.General:
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case TextVerticalAlignment.Top:
                        format.LineAlignment = StringAlignment.Near;
                        break;

                    case TextVerticalAlignment.Center:
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    case TextVerticalAlignment.Bottom:
                        format.LineAlignment = StringAlignment.Far;
                        break;

                    case TextVerticalAlignment.Justify:
                        break;

                    case TextVerticalAlignment.Distributed:
                        format.LineAlignment = StringAlignment.Center;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                format.TextWrapping = align.WordWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
            return format;
        }

        /// <summary>
        /// Creates curves from one point to another.
        /// </summary>
        /// <param name="p1">The p1 point.</param>
        /// <param name="p3">The p3 point.</param>
        public void CurveFromTo(Windows.Foundation.Point p1, Windows.Foundation.Point p3)
        {
            this.CurveFromTo(p1.X, p1.Y, p3.X, p3.Y);
        }

        /// <summary>
        /// Creates curves from one location to another.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x3">The x3 value.</param>
        /// <param name="y3">The y3 value.</param>
        public void CurveFromTo(double x1, double y1, double x3, double y3)
        {
            this.pdfGraphics.CurveFromTo((float) x1, (float) y1, (float) x3, (float) y3);
        }

        /// <summary>
        /// Creates curves to the specified points.
        /// </summary>
        /// <param name="p2">The p2 point.</param>
        /// <param name="p3">The p3 point.</param>
        public void CurveTo(Windows.Foundation.Point p2, Windows.Foundation.Point p3)
        {
            this.CurveTo(p2.X, p2.Y, p3.X, p3.Y);
        }

        /// <summary>
        /// Creates curves to the specified points.
        /// </summary>
        /// <param name="p1">The p1 point.</param>
        /// <param name="p2">The p2 point.</param>
        /// <param name="p3">The p3 point.</param>
        public void CurveTo(Windows.Foundation.Point p1, Windows.Foundation.Point p2, Windows.Foundation.Point p3)
        {
            this.CurveTo(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
        }

        /// <summary>
        /// Creates curves to locations.
        /// </summary>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="x3">The x3 value.</param>
        /// <param name="y3">The y3 value.</param>
        public void CurveTo(double x2, double y2, double x3, double y3)
        {
            this.pdfGraphics.CurveTo((float) x2, (float) y2, (float) x3, (float) y3);
        }

        /// <summary>
        /// Creates curves to locations.
        /// </summary>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="x2">The x2 value.</param>
        /// <param name="y2">The y2 value.</param>
        /// <param name="x3">The x3 value.</param>
        /// <param name="y3">The y3 value.</param>
        public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            this.pdfGraphics.CurveTo((float) x1, (float) y1, (float) x2, (float) y2, (float) x3, (float) y3);
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="x0">The x0 value.</param>
        /// <param name="y0">The y0 value.</param>
        /// <param name="x1">The x1 value.</param>
        /// <param name="y1">The y1 value.</param>
        /// <param name="width">The width.</param>
        /// <param name="effect">The effect.</param>
        public void DrawLine(double x0, double y0, double x1, double y1, double width, Brush effect)
        {
            if (Utilities.HasFillEffect(effect))
            {
                this.NewPath();
                this.pdfGraphics.SetLineWidth((float) width);
                this.MoveTo(x0, y0);
                this.LineTo(x1, y1);
                Windows.Foundation.Rect rect = new Windows.Foundation.Rect(Math.Min(x0, x1), Math.Min(y0, y1), Math.Abs((double) (x0 - x1)), Math.Abs((double) (y0 - y1)));
                if (rect.Width <= 0.0)
                {
                    rect.Width = width;
                    rect.X -= width / 2.0;
                }
                if (rect.Height <= 0.0)
                {
                    rect.Height = width;
                    rect.Y -= width / 2.0;
                }
                this.ApplyFillEffect(effect, rect, true, false);
                this.Stroke();
            }
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="effect">The effect.</param>
        public void DrawRectangle(Windows.Foundation.Rect rect, Brush effect)
        {
            this.DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, effect);
        }

        /// <summary>
        /// Draws the rectangle.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="effect">The effect.</param>
        public void DrawRectangle(double x, double y, double width, double height, Brush effect)
        {
            if (((width > 0.0) && (height > 0.0)) && Utilities.HasFillEffect(effect))
            {
                this.NewPath();
                this.ApplyFillEffect(effect, new Windows.Foundation.Rect(x, y, width, height), true, false);
                this.Rectangle(x, y, width, height);
                this.Stroke();
            }
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="effect">The effect.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="format">The format.</param>
        public void DrawString(string str, Font font, Brush effect, Windows.Foundation.Rect rect, StringFormat format)
        {
            this.DrawString(str, font, effect, rect, format, false);
        }

        /// <summary>
        /// Draws the string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="effect">The effect.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="format">The format.</param>
        /// <param name="fitRect">if set to <c>true</c> [fit rect].</param>
        public void DrawString(string str, Font font, Brush effect, Windows.Foundation.Rect rect, StringFormat format, bool fitRect)
        {
            if (((((rect.Width > 0.0) && (rect.Height > 0.0)) && (effect != null)) && !string.IsNullOrEmpty(str)) && Utilities.HasFillEffect(effect))
            {
                font = font ?? this.context.DefaultFont;
                BaseFont font2 = this.exporter.GetFont(font);
                float fontSize = font.GetFontSize(UnitType.CentileInch, this.Dpi);
                float width = (font2.GetStringWidth(str) / 1000f) * fontSize;
                float num3 = (font2.GetFontHeight() / 1000f) * fontSize;
                float num4 = (font2.GetAscent() / 1000f) * fontSize;
                float num5 = (font2.GetDescent() / 1000f) * fontSize;
                MatrixMock mock = new MatrixMock(1.0, 0.0, 0.0, -1.0, rect.X, rect.Y);
                bool isStroke = false;
                if (font.Italic)
                {
                    mock.SkewPrepend(15.0, 0.0);
                }
                if (font.Bold)
                {
                    isStroke = true;
                    this.pdfGraphics.SetLineWidth(1f);
                    this.pdfGraphics.SetTextRenderingMode(2);
                }
                if (fitRect)
                {
                    double num6 = rect.Width / ((double) width);
                    double num7 = rect.Height / ((double) num3);
                    double scaleX = (num6 < num7) ? num6 : num7;
                    mock.Scale(scaleX, scaleX);
                    width = (float) rect.Width;
                    num3 = (float) rect.Height;
                    num5 *= (float) scaleX;
                }
                if (format != null)
                {
                    if (format.Alignment.Equals(StringAlignment.Far))
                    {
                        mock.Translate((rect.Width - width) - 1.0, 0.0);
                    }
                    else if (format.Alignment.Equals(StringAlignment.Center))
                    {
                        mock.Translate((rect.Width - width) / 2.0, 0.0);
                    }
                }
                if (!fitRect && ((width >= rect.Width) || (num3 >= rect.Height)))
                {
                    if ((format != null) && format.LineAlignment.Equals(StringAlignment.Center))
                    {
                        mock.Translate(0.0, -(rect.Height - num3) / 2.0);
                    }
                    this.SaveState();
                }
                double offsetY = (num3 - (((double) (num3 - (num4 - num5))) / 2.0)) + num5;
                mock.Translate(0.0, offsetY);
                this.ApplyFillEffect(effect, new Windows.Foundation.Rect(mock.OffsetX, mock.OffsetY, (double) width, (double) num3), isStroke, true);
                this.BeginText();
                this.ApplyTextMatrix(mock.M11, mock.M12, mock.M21, mock.M22, mock.OffsetX, mock.OffsetY);
                List<DrawStringInfo> list = this.SplitString(str, font2);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!string.IsNullOrEmpty(list[i].str) && (list[i].font != null))
                    {
                        this.pdfGraphics.SetFontAndSize(list[i].font, fontSize);
                        this.pdfGraphics.ShowText(list[i].str);
                    }
                }
                this.EndText();
                double height = ((double) num3) / 10.0;
                double y = mock.OffsetY + (num3 / 5f);
                double num13 = mock.OffsetY - (num3 / 3f);
                if (font2 is OpenTypeFont)
                {
                    OpenTypeFont font3 = font2 as OpenTypeFont;
                    if ((font3.OS_2Table != null) && (font3.FontHeaderTable != null))
                    {
                        height = (font3.OS_2Table.YStrikeoutSize * fontSize) / ((float) font3.FontHeaderTable.UnitsPerEm);
                        y = mock.OffsetY - ((font3.GetDescent() * fontSize) / (font3.GetFontHeight() * 2f));
                        num13 = mock.OffsetY - ((font3.OS_2Table.YStrikeoutPosition * fontSize) / ((float) font3.FontHeaderTable.UnitsPerEm));
                    }
                }
                bool flag2 = false;
                if (font.Underline == UnderlineType.Single)
                {
                    this.Rectangle(mock.OffsetX, y, (double) width, height);
                    flag2 = true;
                }
                if (font.Strikeout)
                {
                    this.Rectangle(mock.OffsetX, num13, (double) width, height);
                    flag2 = true;
                }
                if (flag2)
                {
                    this.Fill();
                }
                if (!fitRect && ((width >= rect.Width) || (num3 >= rect.Height)))
                {
                    this.RestoreState();
                }
            }
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="effect">The effect.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="fitRect">Decide wheather fit rect.</param>
        public void DrawText(string text, Font font, ContentAlignment alignment, Brush effect, Windows.Foundation.Rect rect, bool fitRect)
        {
            double width;
            StringAlignment lineAlignment;
            if ((string.IsNullOrEmpty(text) || !Utilities.HasFillEffect(effect)) || ((rect.Width == 0.0) || (rect.Height == 0.0)))
            {
                return;
            }
            if (alignment == null)
            {
                alignment = new ContentAlignment();
            }
            StringFormat format = ConvertToStringFormat(alignment);
            this.SaveState();
            this.Rectangle(rect);
            this.Clip();
            this.Translate(rect.X, rect.Y);
            rect.X = 0.0;
            rect.Y = 0.0;
            switch (alignment.TextOrientation)
            {
                case TextOrientation.TextHorizontal:
                    goto Label_0411;

                case TextOrientation.TextHorizontalFlipped:
                    this.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    this.Rotate(180.0);
                    this.Translate(-rect.Width / 2.0, -rect.Height / 2.0);
                    switch (format.Alignment)
                    {
                        case StringAlignment.Near:
                            format.Alignment = StringAlignment.Far;
                            goto Label_0155;

                        case StringAlignment.Far:
                            format.Alignment = StringAlignment.Near;
                            goto Label_0155;
                    }
                    throw new ArgumentOutOfRangeException();

                case TextOrientation.TextVertical:
                case TextOrientation.TextTopDown:
                case TextOrientation.TextTopDownRTL:
                    this.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    this.Rotate(90.0);
                    width = rect.Width;
                    rect.Width = rect.Height;
                    rect.Height = width;
                    this.Translate(-rect.Width / 2.0, -rect.Height / 2.0);
                    lineAlignment = format.LineAlignment;
                    switch (format.Alignment)
                    {
                        case StringAlignment.Near:
                            format.LineAlignment = StringAlignment.Far;
                            goto Label_0254;

                        case StringAlignment.Center:
                            format.LineAlignment = StringAlignment.Center;
                            goto Label_0254;

                        case StringAlignment.Far:
                            format.LineAlignment = StringAlignment.Near;
                            goto Label_0254;
                    }
                    throw new ArgumentOutOfRangeException();

                case TextOrientation.TextVerticalFlipped:
                    this.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    this.Rotate(-90.0);
                    width = rect.Width;
                    rect.Width = rect.Height;
                    rect.Height = width;
                    this.Translate(-rect.Width / 2.0, -rect.Height / 2.0);
                    lineAlignment = format.LineAlignment;
                    switch (format.Alignment)
                    {
                        case StringAlignment.Near:
                            format.LineAlignment = StringAlignment.Near;
                            goto Label_035A;

                        case StringAlignment.Center:
                            format.LineAlignment = StringAlignment.Center;
                            goto Label_035A;

                        case StringAlignment.Far:
                            format.LineAlignment = StringAlignment.Far;
                            goto Label_035A;
                    }
                    throw new ArgumentOutOfRangeException();

                case TextOrientation.TextRotateCustom:
                    if (alignment.TextRotationAngle != 0.0)
                    {
                        this.Translate(rect.Width / 2.0, rect.Height / 2.0);
                        this.Rotate(-alignment.TextRotationAngle);
                        this.Translate(-rect.Width / 2.0, -rect.Height / 2.0);
                    }
                    goto Label_0411;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        Label_0155:
            switch (format.LineAlignment)
            {
                case StringAlignment.Near:
                    format.LineAlignment = StringAlignment.Far;
                    goto Label_0411;

                case StringAlignment.Center:
                    goto Label_0411;

                case StringAlignment.Far:
                    format.LineAlignment = StringAlignment.Near;
                    goto Label_0411;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        Label_0254:
            switch (lineAlignment)
            {
                case StringAlignment.Near:
                    format.Alignment = StringAlignment.Near;
                    goto Label_0411;

                case StringAlignment.Center:
                    format.Alignment = StringAlignment.Center;
                    goto Label_0411;

                case StringAlignment.Far:
                    format.Alignment = StringAlignment.Far;
                    goto Label_0411;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        Label_035A:
            switch (lineAlignment)
            {
                case StringAlignment.Near:
                    format.Alignment = StringAlignment.Far;
                    break;

                case StringAlignment.Center:
                    format.Alignment = StringAlignment.Center;
                    break;

                case StringAlignment.Far:
                    format.Alignment = StringAlignment.Near;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        Label_0411:
            if ((alignment.TextIndent != 0) && (alignment.HorizontalAlignment != TextHorizontalAlignment.Center))
            {
                if (format.Alignment != StringAlignment.Far)
                {
                    rect.X += alignment.TextIndent;
                }
                rect.Width = Math.Max((double) 0.0, (double) (rect.Width - alignment.TextIndent));
            }
            font = font ?? this.context.DefaultFont;
            bool allowWrap = format.TextWrapping == TextWrapping.Wrap;
            BaseFont font2 = this.exporter.GetFont(font);
            float fontSize = font.GetFontSize(UnitType.CentileInch, this.Dpi);
            List<string> list = Utilities.GetLines(text, font, allowWrap, rect.Width - UnitManager.ConvertTo(6.0, UnitType.Pixel, UnitType.CentileInch, (float) this.Dpi), this.context);
            float num3 = (font2.GetFontHeight() / 1000f) * fontSize;
            switch (format.LineAlignment)
            {
                case StringAlignment.Center:
                    this.Translate(0.0, (rect.Height - (num3 * list.Count)) / 2.0);
                    break;

                case StringAlignment.Far:
                    this.Translate(0.0, rect.Height - (num3 * list.Count));
                    break;
            }
            for (int i = 0; i < list.Count; i++)
            {
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(rect.X, rect.Y + (i * num3), rect.Width, (double) num3);
                if (IsIntersect(rect, rect2))
                {
                    this.DrawString(list[i], font, effect, rect2, format, fitRect);
                }
            }
            this.RestoreState();
        }

        /// <summary>
        /// Ends the text.
        /// </summary>
        public void EndText()
        {
            this.pdfGraphics.EndText();
        }

        /// <summary>
        /// Fills this instance.
        /// </summary>
        public void Fill()
        {
            this.pdfGraphics.Fill();
        }

        /// <summary>
        /// Fills the eo.
        /// </summary>
        public void FillEo()
        {
            this.pdfGraphics.FillEo();
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="effect">The effect.</param>
        public void FillRectangle(Windows.Foundation.Rect rect, Brush effect)
        {
            if ((rect.Width > 0.0) && (rect.Height > 0.0))
            {
                this.FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, effect);
            }
        }

        /// <summary>
        /// Fills the rectangle.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="effect">The effect.</param>
        public void FillRectangle(double x, double y, double width, double height, Brush effect)
        {
            if (((width > 0.0) && (height > 0.0)) && Utilities.HasFillEffect(effect))
            {
                this.NewPath();
                this.ApplyFillEffect(effect, new Windows.Foundation.Rect(x, y, width, height), false, true);
                this.Rectangle(x, y, width, height);
                this.Fill();
            }
        }

        /// <summary>
        /// Fills the stroke.
        /// </summary>
        public void FillStroke()
        {
            this.pdfGraphics.FillStroke();
        }

        /// <summary>
        /// Fills the stroke eo.
        /// </summary>
        public void FillStrokeEo()
        {
            this.pdfGraphics.FillStrokEo();
        }

        BaseFont FindSupportFont(string str, BaseFont font)
        {
            if (this.IsSupportFont(str, font))
            {
                return font;
            }
            if (this.IsSupportFont(str, this.DeafultChineseFont))
            {
                return this.DeafultChineseFont;
            }
            if (this.IsSupportFont(str, this.DefaultJapanFont))
            {
                return this.DefaultJapanFont;
            }
            if (this.IsSupportFont(str, this.DefaultKoreaFont))
            {
                return this.DefaultKoreaFont;
            }
            return this.DefaultFont;
        }

        /// <summary>
        /// Gets the gradient sections.
        /// </summary>
        /// <param name="effect">The effect</param>
        /// <returns></returns>
        static List<GradientStop> GetGradientSections(GradientBrush effect)
        {
            List<GradientStop> sections = new List<GradientStop>((IEnumerable<GradientStop>)effect.GradientStops);
            GradientStop stop = null;
            for (int k = 1; k < sections.Count; k++)
            {
                for (int i = sections.Count - 1; i >= k; i--)
                {
                    if (sections[i].Offset < sections[i - 1].Offset)
                    {
                        stop = sections[i - 1];
                        sections[i - 1] = sections[i];
                        sections[i] = stop;
                    }
                }
            }
            return sections;
        }

        static GradientStyle GetGradinetStyle(GradientBrush gradientBrush)
        {
            // uno 取消注释
            if (gradientBrush is LinearGradientBrush linearGradientBrush)
            {
                Windows.Foundation.Point startPoint = linearGradientBrush.StartPoint;
                Windows.Foundation.Point endPoint = linearGradientBrush.EndPoint;
                double offsetx = endPoint.X - startPoint.X;
                if (Math.Abs((double)(offsetx - 0.0)) < 1E-07)
                {
                    offsetx = 0.0;
                }
                double offsety = endPoint.Y - startPoint.Y;
                double num4 = UpdateAngle(Math.Asin(offsety / Math.Sqrt(Math.Pow(offsetx, 2.0) + Math.Pow(offsety, 2.0))), offsetx, offsety);
                if (Math.Abs(num4) < 0.78539816339744828)
                {
                    return GradientStyle.LinearLeftRight;
                }
                if (num4 == 0.78539816339744828)
                {
                    return GradientStyle.LinearTopLeftBottomRight;
                }
                if ((0.78539816339744828 < num4) && (num4 < 1.5707963267948966))
                {
                    return GradientStyle.LinearTopLeftBottomRight;
                }
                if (num4 == 1.5707963267948966)
                {
                    return GradientStyle.LinearTopDown;
                }
                if ((1.5707963267948966 < num4) && (num4 < 2.3561944901923448))
                {
                    return GradientStyle.LinearTopRightBottomLeft;
                }
                if (num4 == 2.3561944901923448)
                {
                    return GradientStyle.LinearTopRightBottomLeft;
                }
                if ((num4 > 2.3561944901923448) && (num4 <= 3.1415926535897931))
                {
                    return GradientStyle.LinearRightLeft;
                }
                if ((num4 < -2.3561944901923448) && (num4 >= -3.1415926535897931))
                {
                    return GradientStyle.LinearRightLeft;
                }
                if ((num4 <= -0.78539816339744828) && (num4 >= -2.3561944901923448))
                {
                    return GradientStyle.LinearBottomUp;
                }
            }
            return GradientStyle.None;
        }

        /// <summary>
        /// Gets the original point.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="m">The matrix.</param>
        /// <returns></returns>
        public static Windows.Foundation.Point GetOriginalPoint(double x, double y, MatrixMock m)
        {
            Windows.Foundation.Point point = new Windows.Foundation.Point();
            double num = m.M12 / m.M11;
            point.Y = (((y - m.OffsetY) - (num * x)) + (num * m.OffsetX)) / (m.M22 - (num * m.M21));
            point.X = ((x - m.OffsetX) - (m.M21 * point.Y)) / m.M11;
            return point;
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <returns></returns>
        public Windows.Foundation.Point GetPoint(double x, double y)
        {
            return GetPoint(x, y, this.matrix);
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="m">The matrix.</param>
        /// <returns></returns>
        public static Windows.Foundation.Point GetPoint(double x, double y, MatrixMock m)
        {
            return new Windows.Foundation.Point { X = ((x * m.M11) + (y * m.M21)) + m.OffsetX, Y = ((x * m.M12) + (y * m.M22)) + m.OffsetY };
        }

        static bool IsIntersect(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2)
        {
            return ((((rect2.X < (rect1.X + rect1.Width)) && (rect1.X < (rect2.X + rect2.Width))) && (rect2.Y < (rect1.Y + rect1.Height))) && (rect1.Y < (rect2.Y + rect2.Height)));
        }

        bool IsSupportFont(string str, BaseFont font)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }
            byte[] showOperan = PdfFont.Create(font).GetShowOperan(str);
            return ((showOperan != null) && (showOperan.Length > 0));
        }

        /// <summary>
        /// Creates a line to the specified point.
        /// </summary>
        /// <param name="p">The point.</param>
        public void LineTo(Windows.Foundation.Point p)
        {
            this.LineTo(p.X, p.Y);
        }

        /// <summary>
        /// Creates a line segment.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public void LineTo(double x, double y)
        {
            this.pdfGraphics.LineTo((float) x, (float) y);
        }

        /// <summary>
        /// Moves to the specified point.
        /// </summary>
        /// <param name="p">The point.</param>
        public void MoveTo(Windows.Foundation.Point p)
        {
            this.MoveTo(p.X, p.Y);
        }

        /// <summary>
        /// Moves to the specified location.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public void MoveTo(double x, double y)
        {
            this.pdfGraphics.MoveTo((float) x, (float) y);
        }

        /// <summary>
        /// Creates a new path.
        /// </summary>
        public void NewPath()
        {
            this.pdfGraphics.NewPath();
        }

        /// <summary>
        /// Puts a rectangle around the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        public void Rectangle(Windows.Foundation.Rect rect)
        {
            this.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Creates a rectangle for the specified x location.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void Rectangle(double x, double y, double width, double height)
        {
            this.pdfGraphics.Rectangle((float) x, (float) y, (float) width, (float) height);
        }

        /// <summary>
        /// Restores the state.
        /// </summary>
        public void RestoreState()
        {
            this.pdfGraphics.RestoreState();
            this.matrix = this.matrixs.Pop();
            this.strokeAlpha = this.strokeAlphas.Pop();
            this.fillAlpha = this.fillAlphas.Pop();
        }

        /// <summary>
        /// Rotates the specified angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void Rotate(double angle)
        {
            double d = (angle * 3.1415926535897931) / 180.0;
            this.ApplyMatrix(Math.Cos(d), Math.Sin(d), -Math.Sin(d), Math.Cos(d), 0.0, 0.0);
            this.matrix.RotatePrepend(angle);
        }

        /// <summary>
        /// Saves the state.
        /// </summary>
        public void SaveState()
        {
            this.pdfGraphics.SaveState();
            this.matrixs.Push(this.matrix);
            this.strokeAlphas.Push(this.strokeAlpha);
            this.fillAlphas.Push(this.fillAlpha);
        }

        /// <summary>
        /// Scales the specified X scale.
        /// </summary>
        /// <param name="scaleX">The X scale.</param>
        /// <param name="scaleY">The Y scale.</param>
        public void Scale(double scaleX, double scaleY)
        {
            this.ApplyMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0);
            this.matrix.ScalePrepend(scaleX, scaleY);
        }

        public void SetFillAlpha(float alpha)
        {
            if (alpha != this.fillAlpha)
            {
                PdfExtGraphicState egs = new PdfExtGraphicState {
                    FillAlpha = alpha
                };
                this.pdfGraphics.SetExtGState(egs);
                this.fillAlpha = alpha;
            }
        }

        /// <summary>
        /// Sets the line cap.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetLineCap(PdfGraphics.LineCapType type)
        {
            this.pdfGraphics.SetLineCap(type);
        }

        /// <summary>
        /// Sets the line dash.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="phase">The phase.</param>
        public void SetLineDash(float[] array, float phase)
        {
            this.pdfGraphics.SetLineDash(array, phase);
        }

        /// <summary>
        /// Sets the line dash.
        /// </summary>
        /// <param name="dashStyle">The dash style.</param>
        /// <param name="lineWidth">Width of the line.</param>
        /// <param name="dashPattern">The dash pattern.</param>
        public void SetLineDash(DashStyle dashStyle, double lineWidth, float[] dashPattern)
        {
            if (lineWidth > 0.0)
            {
                float[] array = null;
                float num = (float) lineWidth;
                switch (dashStyle)
                {
                    case DashStyle.Solid:
                        break;

                    case DashStyle.Dash:
                        array = new float[] { 3f * num, num };
                        break;

                    case DashStyle.Dot:
                        array = new float[] { num };
                        break;

                    case DashStyle.DashDot:
                        array = new float[] { 3f * num, num, num, num };
                        break;

                    case DashStyle.DashDotDot:
                        array = new float[] { 3f * num, num, num, num, num, num };
                        break;

                    case DashStyle.Custom:
                        if ((dashPattern != null) && (dashPattern.Length > 0))
                        {
                            array = new float[dashPattern.Length];
                            for (int i = 0; i < dashPattern.Length; i++)
                            {
                                array[i] = dashPattern[i] * num;
                            }
                            break;
                        }
                        return;

                    default:
                        throw new ArgumentOutOfRangeException("dashStyle");
                }
                if (array == null)
                {
                    this.pdfGraphics.SetLineDash(0f);
                }
                else
                {
                    this.pdfGraphics.SetLineDash(array, 0f);
                }
            }
        }

        /// <summary>
        /// Sets the line dash.
        /// </summary>
        /// <param name="lineDashType">Type of the line dash</param>
        /// <param name="lineWidth">Width of the line</param>
        /// <param name="dashPattern">The dash pattern</param>
        internal void SetLineDash(LineDashType lineDashType, double lineWidth, float[] dashPattern)
        {
            DashStyle solid;
            switch (lineDashType)
            {
                case LineDashType.Solid:
                    solid = DashStyle.Solid;
                    break;

                case LineDashType.Dot:
                    solid = DashStyle.Dot;
                    break;

                case LineDashType.Dash:
                    solid = DashStyle.Dash;
                    break;

                case LineDashType.DashDot:
                    solid = DashStyle.DashDot;
                    break;

                case LineDashType.DashDotDot:
                    solid = DashStyle.DashDotDot;
                    break;

                case LineDashType.DashStyleCustom:
                    solid = DashStyle.Custom;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("lineDashType");
            }
            this.SetLineDash(solid, lineWidth, dashPattern);
        }

        /// <summary>
        /// Sets the line join style.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetLineJoin(PdfGraphics.LineJoinType type)
        {
            this.pdfGraphics.SetLineJoin(type);
        }

        /// <summary>
        /// Sets the width of the line.
        /// </summary>
        /// <param name="w">The width.</param>
        public void SetLineWidth(double w)
        {
            this.pdfGraphics.SetLineWidth((float) w);
        }

        /// <summary>
        /// Sets the RGB fill.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetRGBFill(Windows.UI.Color color)
        {
            this.SetFillAlpha(((float) color.A) / 255f);
            if (this.GrayMode)
            {
                color = Utilities.GetGrayColor(color);
            }
            this.pdfGraphics.SetRGBColorFillF(((float) color.R) / 255f, ((float) color.G) / 255f, ((float) color.B) / 255f);
        }

        /// <summary>
        /// Sets the RGB fill.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="g">The graphics object.</param>
        public static void SetRGBFill(Windows.UI.Color color, PdfGraphics g)
        {
            g.SetRGBColorFillF(((float) color.R) / 255f, ((float) color.G) / 255f, ((float) color.B) / 255f);
        }

        /// <summary>
        /// Sets the RGB stroke.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetRGBStroke(Windows.UI.Color color)
        {
            this.SetStrokeAlpha(((float) color.A) / 255f);
            if (this.GrayMode)
            {
                color = Utilities.GetGrayColor(color);
            }
            this.pdfGraphics.SetRGBColorStrokeF(((float) color.R) / 255f, ((float) color.G) / 255f, ((float) color.B) / 255f);
        }

        /// <summary>
        /// Sets the RGB stroke.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="g">The graphics object.</param>
        public static void SetRGBStroke(Windows.UI.Color color, PdfGraphics g)
        {
            g.SetRGBColorStrokeF(((float) color.R) / 255f, ((float) color.G) / 255f, ((float) color.B) / 255f);
        }

        public void SetStrokeAlpha(float alpha)
        {
            if (alpha != this.strokeAlpha)
            {
                PdfExtGraphicState egs = new PdfExtGraphicState {
                    StrokAlpha = alpha
                };
                this.pdfGraphics.SetExtGState(egs);
                this.strokeAlpha = alpha;
            }
        }

        /// <summary>
        /// Sets the text rendering mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void SetTextRenderingMode(PdfGraphics.TextRenderingMode mode)
        {
            this.pdfGraphics.SetTextRenderingMode(mode);
        }

        /// <summary>
        /// Skews the specified X skew.
        /// </summary>
        /// <param name="skewX">The X skew.</param>
        /// <param name="skewY">The Y skew.</param>
        public void Skew(double skewX, double skewY)
        {
            this.ApplyMatrix(1.0, Math.Tan(skewX), Math.Tan(skewY), 1.0, 0.0, 0.0);
            this.matrix.SkewPrepend(skewX, skewY);
        }

        public List<DrawStringInfo> SplitString(string str, BaseFont font)
        {
            BaseFont font2 = font;
            StringBuilder builder = new StringBuilder();
            List<DrawStringInfo> list = new List<DrawStringInfo>();
            for (int i = 0; i < str.Length; i++)
            {
                string str2 = str.Substring(i, 1);
                BaseFont font3 = this.FindSupportFont(str2, font);
                if (!font3.Equals(font2) && !string.IsNullOrEmpty(builder.ToString()))
                {
                    DrawStringInfo info = new DrawStringInfo {
                        str = builder.ToString(),
                        font = font2
                    };
                    list.Add(info);
                    builder = new StringBuilder();
                }
                font2 = font3;
                builder.Append(str2);
            }
            if (builder != null)
            {
                DrawStringInfo info2 = new DrawStringInfo {
                    str = builder.ToString(),
                    font = font2
                };
                list.Add(info2);
            }
            return list;
        }

        /// <summary>
        /// Strokes this instance.
        /// </summary>
        public void Stroke()
        {
            this.pdfGraphics.Stroke();
        }

        /// <summary>
        /// Translates the specified X offset.
        /// </summary>
        /// <param name="offsetX">The X offset.</param>
        /// <param name="offsetY">The Y offset.</param>
        public void Translate(double offsetX, double offsetY)
        {
            this.ApplyMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
            this.matrix.TranslatePrepend(offsetX, offsetY);
        }

        static double UpdateAngle(double angle, double offsetx, double offsety)
        {
            if (offsetx <= 0.0)
            {
                if ((angle == 0.0) && (offsetx < 0.0))
                {
                    return -3.1415926535897931;
                }
                if ((offsetx < 0.0) && (offsety < 0.0))
                {
                    return (-3.1415926535897931 - angle);
                }
                if ((offsetx < 0.0) && (offsety > 0.0))
                {
                    return (3.1415926535897931 - angle);
                }
            }
            return angle;
        }

        public BaseFont DeafultChineseFont
        {
            get
            {
                if (this.defaultChineseFont == null)
                {
                    this.defaultChineseFont = this.exporter.GetFont(new Font("SimSun", DefaultStyleCollection.DefaultFontSize));
                }
                return this.defaultChineseFont;
            }
        }

        public BaseFont DefaultFont
        {
            get
            {
                if (this.defaultFont == null)
                {
                    this.defaultFont = this.exporter.GetFont(new Font("Times New Roman", DefaultStyleCollection.DefaultFontSize));
                }
                return this.defaultFont;
            }
        }

        public BaseFont DefaultJapanFont
        {
            get
            {
                if (this.defaultJapanFont == null)
                {
                    this.defaultJapanFont = this.exporter.GetFont(new Font("MS Mincho", DefaultStyleCollection.DefaultFontSize));
                }
                return this.defaultJapanFont;
            }
        }

        public BaseFont DefaultKoreaFont
        {
            get
            {
                if (this.defaultKoreaFont == null)
                {
                    this.defaultKoreaFont = this.exporter.GetFont(new Font("Batang", DefaultStyleCollection.DefaultFontSize));
                }
                return this.defaultKoreaFont;
            }
        }

        public int Dpi
        {
            get { return  this.exporter.Dpi; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether [gray mode] is used.
        /// </summary>
        /// <value><c>true</c> if [gray mode]; otherwise, <c>false</c>.</value>
        public bool GrayMode
        {
            get { return  this.grayMode; }
            set { this.grayMode = value; }
        }
    }
}

