#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.BaseObject;
using Dt.Pdf.Drawing;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// Pdf Graphics
    /// </summary>
    public class PdfGraphics
    {
        private GraphicsState curState;
        /// <summary>
        /// the container of this graphics. can be null.
        /// </summary>
        private IGraphicsStream gStream;
        private PdfStreamWriter psw;
        private PdfResources resources;
        private readonly Stack<GraphicsState> stateStack;
        /// <summary>
        /// uncolored graphics if true
        /// </summary>
        private bool stencil;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics" /> class.
        /// </summary>
        public PdfGraphics()
        {
            this.curState = new GraphicsState();
            this.stateStack = new Stack<GraphicsState>();
            this.psw = new PdfStreamWriter(new MemoryStream(), PdfASCIIEncoding.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics" /> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public PdfGraphics(Stream stream)
        {
            this.curState = new GraphicsState();
            this.stateStack = new Stack<GraphicsState>();
            if (stream == null)
            {
                throw new PdfArgumentNullException("stream");
            }
            this.psw = new PdfStreamWriter(stream, PdfASCIIEncoding.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics" /> class.
        /// </summary>
        /// <param name="psw">The PSW.</param>
        /// <param name="gStream">the container of this graphics. can be null.</param>
        internal PdfGraphics(PdfStreamWriter psw, IGraphicsStream gStream)
        {
            this.curState = new GraphicsState();
            this.stateStack = new Stack<GraphicsState>();
            this.psw = psw;
            this.gStream = gStream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfGraphics" /> class.
        /// </summary>
        /// <param name="psw">The PSW.</param>
        /// <param name="gStream">the container of this graphics. can be null.</param>
        /// <param name="stencil">if set to <c>true</c> [stencil].</param>
        internal PdfGraphics(PdfStreamWriter psw, IGraphicsStream gStream, bool stencil) : this(psw, gStream)
        {
            this.stencil = stencil;
        }

        /// <summary>
        /// Adds the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        /// <param name="f">The f.</param>
        public void AddImage(Image image, float a, float b, float c, float d, float e, float f)
        {
            this.W("q").WE();
            this.WS(a).WS(b).WS(c).WS(d).WS(e).WS(f).W("cm").WE();
            PdfName name = this.Resources.AddImage(image);
            this.WS(name).W("Do").WE();
            this.W("Q").WE();
        }

        /// <summary>
        /// Modify the current transformation matrix (CTM) by concatenating the 
        /// specified matrix. Although the operands specify a matrix, they are 
        /// written as six separate numbers, not as an array.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <param name="e">The e.</param>
        /// <param name="f">The f.</param>
        public void ApplyCTM(float a, float b, float c, float d, float e, float f)
        {
            this.WS(a).WS(b).WS(c).WS(d).WS(e).WS(f).W("cm").WE();
            this.curState.Matrix.Apply(a, b, c, d, e, f);
        }

        /// <summary>
        /// Draw Arc.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The end angle.</param>
        public void Arc(float x1, float y1, float x2, float y2, float startAngle, float stopAngle)
        {
            List<float[]> list = CreateBezier(x1, y1, x2, y2, startAngle, stopAngle);
            if (list.Count > 0)
            {
                this.MoveTo(list[0][0], list[0][1]);
                for (int i = 0; i < list.Count; i++)
                {
                    this.CurveTo(list[i][2], list[i][3], list[i][4], list[i][5], list[i][6], list[i][7]);
                }
            }
        }

        /// <summary>
        /// Draw Arc.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The end angle.</param>
        public void Arc2(float x1, float y1, float x2, float y2, float startAngle, float stopAngle)
        {
            List<float[]> list = CreateBezier(x1, y1, x2, y2, startAngle, stopAngle);
            if (list.Count > 0)
            {
                this.LineTo(list[0][0], list[0][1]);
                for (int i = 0; i < list.Count; i++)
                {
                    this.CurveTo(list[i][2], list[i][3], list[i][4], list[i][5], list[i][6], list[i][7]);
                }
            }
        }

        /// <summary>
        /// Begin a text object, initializing the text matrix, Tm , and the text 
        /// line matrix, Tlm , to the identity matrix. Text objects cannot be nested; 
        /// a second BT cannot appear before an ET.
        /// </summary>
        public void BeginText()
        {
            this.curState.TextState.X = 0f;
            this.curState.TextState.Y = 0f;
            this.W("BT").WE();
        }

        /// <summary>
        /// Draw Circles by the specified cx, cy and r.
        /// </summary>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="r">The r.</param>
        public void Circle(float cx, float cy, float r)
        {
            this.MoveTo(cx + r, cy);
            this.CurveTo(cx + r, cy + (r * 0.5523f), cx + (r * 0.5523f), cy + r, cx, cy + r);
            this.CurveTo(cx - (r * 0.5523f), cy + r, cx - r, cy + (r * 0.5523f), cx - r, cy);
            this.CurveTo(cx - r, cy - (r * 0.5523f), cx - (r * 0.5523f), cy - r, cx, cy - r);
            this.CurveTo(cx + (r * 0.5523f), cy - r, cx + r, cy - (r * 0.5523f), cx + r, cy);
        }

        /// <summary>
        /// Modify the current clipping path by intersecting it with the current
        /// path, using the nonzero winding number rule to determine which regions 
        /// lie inside the clipping path.
        /// </summary>
        public void Clip()
        {
            this.W("W").WE();
        }

        /// <summary>
        /// Modify the current clipping path by intersecting it with the current 
        /// path, using the even-odd rule to determine which regions lie inside 
        /// the clipping path.
        /// </summary>
        public void ClipEo()
        {
            this.W("W*").WE();
        }

        /// <summary>
        /// Close the current subpath by appending a straight line segment from the
        /// current point to the starting point of the subpath. If the current subpath 
        /// is already closed, h does nothing.
        /// This operator terminates the current subpath. Appending another segment 
        /// to the current path begins a new subpath, even if the new segment begins
        /// at the endpoint reached by the h operation.
        /// </summary>
        public void ClosePath()
        {
            this.W('h').WE();
        }

        /// <summary>
        /// Close, fill, and then stroke the path, using the nonzero winding number 
        /// rule to determine the region to fill.
        /// </summary>
        public void ClosePathFillStroke()
        {
            this.W('b').WE();
        }

        /// <summary>
        /// Close, fill, and then stroke the path, using the even-odd rule to 
        /// determine the region to fill.
        /// </summary>
        public void ClosePathFillStrokEo()
        {
            this.W("b*").WE();
        }

        /// <summary>
        /// Close and stroke the path.
        /// </summary>
        public void ClosePathStroke()
        {
            this.W('s').WE();
        }

        public void Comment(string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                this.W("%" + comment).WE();
            }
        }

        /// <summary>
        /// Creates the bezier.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="startAngle">The start angle.</param>
        /// <param name="stopAngle">The stop angle.</param>
        /// <returns></returns>
        public static List<float[]> CreateBezier(float x1, float y1, float x2, float y2, float startAngle, float stopAngle)
        {
            float num;
            int num2;
            if (Math.Abs(stopAngle) <= 90f)
            {
                num = stopAngle;
                num2 = 1;
            }
            else
            {
                num2 = (int) Math.Ceiling((double) (Math.Abs(stopAngle) / 90f));
                num = stopAngle / ((float) num2);
            }
            List<float[]> list = new List<float[]>();
            for (int i = 0; i < num2; i++)
            {
                float num6;
                float num4 = (float)((num * 3.1415926535897931) / 360.0);
                float num5 = (float) Math.Abs((double) ((1.3333333333333333 * (1.0 - Math.Cos((double) num4))) / Math.Sin((double) num4)));
                if (x1 > x2)
                {
                    num6 = x1;
                    x1 = x2;
                    x2 = num6;
                }
                if (y2 > y1)
                {
                    num6 = y1;
                    y1 = y2;
                    y2 = num6;
                }
                float num7 = (x1 + x2) / 2f;
                float num8 = (y1 + y2) / 2f;
                float num9 = (x2 - x1) / 2f;
                float num10 = (y2 - y1) / 2f;
                float num11 = (float)(((startAngle + (i * num)) * 3.1415926535897931) / 180.0);
                float num12 = (float)(((startAngle + ((i + 1) * num)) * 3.1415926535897931) / 180.0);
                float num13 = (float) Math.Cos((double) num11);
                float num14 = (float) Math.Cos((double) num12);
                float num15 = (float) Math.Sin((double) num11);
                float num16 = (float) Math.Sin((double) num12);
                if (num > 0f)
                {
                    list.Add(new float[] { num7 + (num9 * num13), num8 - (num10 * num15), num7 + (num9 * (num13 - (num5 * num15))), num8 - (num10 * (num15 + (num5 * num13))), num7 + (num9 * (num14 + (num5 * num16))), num8 - (num10 * (num16 - (num5 * num14))), num7 + (num9 * num14), num8 - (num10 * num16) });
                }
                else
                {
                    list.Add(new float[] { num7 + (num9 * num13), num8 - (num10 * num15), num7 + (num9 * (num13 + (num5 * num15))), num8 - (num10 * (num15 - (num5 * num13))), num7 + (num9 * (num14 - (num5 * num16))), num8 - (num10 * (num16 + (num5 * num14))), num7 + (num9 * num14), num8 - (num10 * num16) });
                }
            }
            return list;
        }

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve extends from
        /// the current point to the point (x3 , y3 ), using (x1 , y1 ) and (x3 , y3 )
        /// as the Bézier control points. The new current point is (x3 , y3 ).
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        public void CurveFromTo(float x1, float y1, float x3, float y3)
        {
            this.WS(x1).WS(y1).WS(x3).WS(y3).W('y').WE();
        }

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve extends from
        /// the current point to the point (x3 , y3 ), using the current point and
        /// (x2 , y2 ) as the Bézier control points. The new current point is (x3 , y3 ).
        /// </summary>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        public void CurveTo(float x2, float y2, float x3, float y3)
        {
            this.WS(x2).WS(y2).WS(x3).WS(y3).W('v').WE();
        }

        /// <summary>
        /// Append a cubic Bézier curve to the current path. The curve extends
        /// from the current point to the point (x3 , y3 ), using (x1 , y1 ) and
        /// (x2 , y2 ) as the Bézier control points. The new current point is (x3 , y3 ).
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x3">The x3.</param>
        /// <param name="y3">The y3.</param>
        public void CurveTo(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            this.WS(x1).WS(y1).WS(x2).WS(y2).WS(x3).WS(y3).W('c').WE();
        }

        /// <summary>
        /// Draw Ellipse
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        public void Ellipse(float x1, float y1, float x2, float y2)
        {
            this.Arc(x1, y1, x2, y2, 0f, 360f);
        }

        /// <summary>
        /// End a text object, discarding the text matrix.
        /// </summary>
        public void EndText()
        {
            this.W("ET").WE();
        }

        /// <summary>
        /// Fill the path, using the nonzero winding number rule to determine the 
        /// region to fill. Any subpaths that are open are implicitly closed before 
        /// being filled.
        /// </summary>
        public void Fill()
        {
            this.W('f').WE();
        }

        /// <summary>
        /// Fill the path, using the even-odd rule to determine the region to fill.
        /// </summary>
        public void FillEo()
        {
            this.W("f*").WE();
        }

        /// <summary>
        /// Fill and then stroke the path, using the nonzero winding number rule to 
        /// determine the region to fill.
        /// </summary>
        public void FillStroke()
        {
            this.W('B').WE();
        }

        /// <summary>
        /// Fill and then stroke the path, using the even-odd rule to determine the 
        /// region to fill.
        /// </summary>
        public void FillStrokEo()
        {
            this.W("B*").WE();
        }

        /// <summary>
        /// Append a straight line segment from the current point to the point
        /// (x, y). The new current point is (x, y).
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void LineTo(float x, float y)
        {
            this.WS(x).WS(y).W('l').WE();
        }

        /// <summary>
        /// Set the text matrix, Tm , and the text line matrix.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void MoveText(float x, float y)
        {
            TextState textState = this.curState.TextState;
            textState.X += x;
            TextState state2 = this.curState.TextState;
            state2.Y += y;
            this.WS(x).WS(y).W("Td").WE();
        }

        /// <summary>
        /// Begin a new subpath by moving the current point to coordinates (x, y),Moves to.
        /// omitting any connecting line segment. If the previous path
        /// construction operator in the current path was also m, the new 
        /// moverrides it; no vestige of the previous m operation remains in the
        /// path.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void MoveTo(float x, float y)
        {
            this.WS(x).WS(y).W('m').WE();
        }

        /// <summary>
        /// Move to the start of the next line.
        /// </summary>
        public void NewlineText()
        {
            TextState textState = this.curState.TextState;
            textState.Y -= this.curState.TextState.Leading;
            this.W("T*").WE();
        }

        /// <summary>
        /// End the path object without filling or stroking it.
        /// </summary>
        public void NewPath()
        {
            this.W('n').WE();
        }

        /// <summary>
        /// Paints the shading.
        /// </summary>
        /// <param name="shading">The shading.</param>
        public void PaintShading(PdfShading shading)
        {
            if (shading == null)
            {
                throw new PdfArgumentNullException("shading");
            }
            PdfName name = this.Resources.AddShading(shading);
            this.WS(name).W("sh").WE();
        }

        /// <summary>
        /// Append a rectangle to the current path as a complete subpath, with
        /// lower-left corner (x, y) and dimensions width and height in user space. 
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        public void Rectangle(float x, float y, float w, float h)
        {
            this.WS(x).WS(y).WS(w).WS(h).W("re").WE();
        }

        /// <summary>
        /// Resets the CMYK color fill.
        /// </summary>
        public void ResetCMYKColorFill()
        {
            this.W("0 0 0 1 k").WE();
        }

        /// <summary>
        /// Resets the CMYK color stroke.
        /// </summary>
        public void ResetCMYKColorStroke()
        {
            this.W("0 0 0 1 K").WE();
        }

        /// <summary>
        /// Resets the gray fill.
        /// </summary>
        public void ResetGrayFill()
        {
            this.W("0 g").WE();
        }

        /// <summary>
        /// Resets the RGB color fill.
        /// </summary>
        public void ResetRGBColorFill()
        {
            this.W("0 g").WE();
        }

        /// <summary>
        /// Resets the RGB color stroke.
        /// </summary>
        public void ResetRGBColorStroke()
        {
            this.W("0 G").WE();
        }

        /// <summary>
        /// Restore the graphics state by removing the most recently saved state 
        /// from the stack and making it the current state.
        /// </summary>
        public void RestoreState()
        {
            this.W("Q").WE();
            if (this.stateStack.Count <= 0)
            {
                throw new PdfUnbalancedStateOperatorException();
            }
            this.curState = this.stateStack.Pop();
        }

        /// <summary>
        /// Save the current graphics state on the graphics state stack.
        /// </summary>
        public void SaveState()
        {
            this.W("q").WE();
            this.stateStack.Push(new GraphicsState(this.curState));
        }

        /// <summary>
        /// Set the character spacing, Tc , to charSpace, which is a number expressed
        /// in unscaled text space units. Character spacing is used by the Tj, TJ, 
        /// and ' operators. Initial value: 0.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetCharacterSpacing(float value)
        {
            this.curState.TextState.CharSpace = value;
            this.WS(value).W("Tc").WE();
        }

        /// <summary>
        /// Sets the CMYK color fill F.
        /// </summary>
        /// <param name="cyan">The cyan.</param>
        /// <param name="magenta">The magenta.</param>
        /// <param name="yellow">The yellow.</param>
        /// <param name="black">The black.</param>
        public void SetCMYKColorFillF(float cyan, float magenta, float yellow, float black)
        {
            this.WCMYK(cyan, magenta, yellow, black).W(" k").WE();
        }

        /// <summary>
        /// Sets the CMYK color stroke F.
        /// </summary>
        /// <param name="cyan">The cyan.</param>
        /// <param name="magenta">The magenta.</param>
        /// <param name="yellow">The yellow.</param>
        /// <param name="black">The black.</param>
        public void SetCMYKColorStrokeF(float cyan, float magenta, float yellow, float black)
        {
            this.WCMYK(cyan, magenta, yellow, black).W(" K").WE();
        }

        public void SetExtGState(PdfExtGraphicState egs)
        {
            if (egs == null)
            {
                throw new PdfArgumentNullException("egs");
            }
            PdfName name = this.Resources.AddExtGState(egs);
            this.W(name).W(" gs").WE();
        }

        /// <summary>
        /// Sets the flatness.
        /// Flatness: sets the maximum permitted distance in device pixels between the
        /// mathematically correct path and an approximation constructed from straight line segments.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetFlatness(float value)
        {
            if ((value >= 0f) && (value <= 100f))
            {
                this.WS(value).W('i').WE();
            }
        }

        /// <summary>
        /// Set the text font, Tf , to font and the text font size, Tfs , to size.
        /// </summary>
        /// <param name="font">The font.</param>
        /// <param name="size">The size.</param>
        public void SetFontAndSize(BaseFont font, float size)
        {
            PdfFont font2;
            if (font == null)
            {
                throw new PdfArgumentNullException("font");
            }
            if ((size < 0.0001f) && (size > -0.0001f))
            {
                throw new PdfArgumentException("The size is too small: " + ((float) size));
            }
            PdfName name = this.Resources.AddFont(font, out font2);
            this.WS(name).WS(size).W("Tf").WE();
            this.curState.Font = font2;
            this.curState.FontSize = size;
        }

        /// <summary>
        /// Sets the gray fill.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetGrayFill(float value)
        {
            this.WS(value).W('g').WE();
        }

        /// <summary>
        /// Sets the gray stroke.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetGrayStroke(float value)
        {
            this.WS(value).W('G').WE();
        }

        /// <summary>
        /// Set the horizontal scaling, Th , to (scale/100). scale is a number 
        /// specifying the percentage of the normal width. Initial value: 100 (normal width).
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetHorizontalScaling(float value)
        {
            this.curState.TextState.HorizontalScale = value;
            this.WS(value).W("Tz").WE();
        }

        /// <summary>
        /// Set the text leading, Tl , to leading, which is a number expressed in 
        /// unscaled text space units. Text leading is used only by the T*, ', 
        /// and " operators. Initial value: 0.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLeading(float value)
        {
            this.curState.TextState.Leading = value;
            this.WS(value).W("TL").WE();
        }

        /// <summary>
        /// Set the line cap style in the graphics state.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetLineCap(LineCapType type)
        {
            this.SetLineCap((int) type);
        }

        /// <summary>
        /// Set the line cap style in the graphics state.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLineCap(int value)
        {
            if ((value >= 0) && (value <= 2))
            {
                this.WS(value).W('J').WE();
            }
        }

        /// <summary>
        /// Set the line dash pattern in the graphics state.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLineDash(float value)
        {
            this.WS("[]").WS(value).W('d').WE();
        }

        /// <summary>
        /// Set the line dash pattern in the graphics state.
        /// </summary>
        /// <param name="unitsOn">The units on.</param>
        /// <param name="phase">The phase.</param>
        public void SetLineDash(float unitsOn, float phase)
        {
            this.W('[').W(unitsOn).WS(']').WS(phase).W('d').WE();
        }

        /// <summary>
        /// Set the line dash pattern in the graphics state.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="phase">The phase.</param>
        public void SetLineDash(float[] array, float phase)
        {
            this.W('[');
            for (int i = 0; i < array.Length; i++)
            {
                this.WS(array[i]);
            }
            this.WS(']').WS(phase).W('d').WE();
        }

        /// <summary>
        /// Set the line dash pattern in the graphics state.
        /// </summary>
        /// <param name="unitsOn">The units on.</param>
        /// <param name="unitsOff">The units off.</param>
        /// <param name="phase">The phase.</param>
        public void SetLineDash(float unitsOn, float unitsOff, float phase)
        {
            this.W('[').WS(unitsOn).W(unitsOff).WS(']').WS(phase).W('d').WE();
        }

        /// <summary>
        /// Set the line join style in the graphics state.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetLineJoin(LineJoinType type)
        {
            this.SetLineJoin((int) type);
        }

        /// <summary>
        /// Set the line join style in the graphics state.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLineJoin(int value)
        {
            if ((value >= 0) && (value <= 2))
            {
                this.WS(value).W('j').WE();
            }
        }

        /// <summary>
        /// Set the line width in the graphics state.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetLineWidth(float value)
        {
            this.WS(value).W('w').WE();
        }

        /// <summary>
        /// Set the miter limit in the graphics state.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetMiterLimit(float value)
        {
            if (value > 1f)
            {
                this.WS(value).W('M').WE();
            }
        }

        /// <summary>
        /// Sets the pattern fill.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public void SetPatternFill(PdfTilingPattern pattern)
        {
            if (pattern == null)
            {
                throw new PdfArgumentNullException("pattern");
            }
            PdfName name = this.Resources.AddPattern(pattern);
            this.W(PdfName.Pattern).W(" cs").WE();
            this.W(name).W(" scn").WE();
        }

        /// <summary>
        /// Sets the pattern stroke.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public void SetPatternStroke(PdfTilingPattern pattern)
        {
            if (pattern == null)
            {
                throw new PdfArgumentNullException("pattern");
            }
            PdfName name = this.Resources.AddPattern(pattern);
            this.W(PdfName.Pattern).W(" CS").WE();
            this.W(name).W(" SCN").WE();
        }

        /// <summary>
        /// Sets the RGB color fill F.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public void SetRGBColorFillF(float red, float green, float blue)
        {
            this.WRGB(red, green, blue).W(" rg").WE();
        }

        /// <summary>
        /// Sets the RGB color stroke F.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        public void SetRGBColorStrokeF(float red, float green, float blue)
        {
            this.WRGB(red, green, blue).W(" RG").WE();
        }

        /// <summary>
        /// Sets the shading fill.
        /// </summary>
        /// <param name="shading">The shading.</param>
        public void SetShadingFill(PdfShading shading)
        {
            if (shading == null)
            {
                throw new PdfArgumentNullException("shading");
            }
            this.SetShadingPatternFill(new PdfShadingPattern(shading));
        }

        /// <summary>
        /// Sets the shading pattern fill.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public void SetShadingPatternFill(PdfShadingPattern pattern)
        {
            if (pattern == null)
            {
                throw new PdfArgumentNullException("pattern");
            }
            PdfName name = this.Resources.AddPattern(pattern);
            this.W(PdfName.Pattern).W(" cs").WE();
            this.W(name).W(" scn").WE();
        }

        /// <summary>
        /// Sets the shading pattern stroke.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public void SetShadingPatternStroke(PdfShadingPattern pattern)
        {
            if (pattern == null)
            {
                throw new PdfArgumentNullException("pattern");
            }
            PdfName name = this.Resources.AddPattern(pattern);
            this.W(PdfName.Pattern).W(" CS").WE();
            this.W(name).W(" SCN").WE();
        }

        /// <summary>
        /// Sets the shading stroke.
        /// </summary>
        /// <param name="shading">The shading.</param>
        public void SetShadingStroke(PdfShading shading)
        {
            if (shading == null)
            {
                throw new PdfArgumentNullException("shading");
            }
            this.SetShadingPatternStroke(new PdfShadingPattern(shading));
        }

        /// <summary>
        /// Sets the text matrix.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetTextMatrix(float x, float y)
        {
            this.SetTextMatrix(1f, 0f, 0f, 1f, x, y);
        }

        /// <summary>
        /// Sets the text matrix.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetTextMatrix(double a, double b, double c, double d, double x, double y)
        {
            this.SetTextMatrix((float) ((float) a), (float) ((float) b), (float) ((float) c), (float) ((float) d), (float) ((float) x), (float) ((float) y));
        }

        /// <summary>
        /// Sets the text matrix.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <param name="c">The c.</param>
        /// <param name="d">The d.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void SetTextMatrix(float a, float b, float c, float d, float x, float y)
        {
            this.curState.TextState.X = x;
            this.curState.TextState.Y = y;
            this.WS(a).WS(b).WS(c).WS(d).WS(x).WS(y).W("Tm").WE();
        }

        /// <summary>
        /// Set the text rendering mode, Tmode , to render, which is an integer.
        /// Initial value: 0.
        /// </summary>
        /// <param name="mode">The mode.</param>
        public void SetTextRenderingMode(TextRenderingMode mode)
        {
            this.SetTextRenderingMode((int) mode);
        }

        /// <summary>
        /// Set the text rendering mode, Tmode , to render, which is an integer. 
        /// Initial value: 0.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetTextRenderingMode(int value)
        {
            this.WS(value).W("Tr").WE();
        }

        /// <summary>
        /// Sets the text rise.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetTextRise(float value)
        {
            this.WS(value).W("Ts").WE();
        }

        /// <summary>
        /// Set the word spacing, Tw , to wordSpace, which is a number expressed 
        /// in unscaled text space units. Word spacing is used by the Tj, TJ, 
        /// and ' operators. Initial value: 0.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetWordSpacing(float value)
        {
            this.curState.TextState.WordSpace = value;
            this.WS(value).W("Tw").WE();
        }

        /// <summary>
        /// Shows the text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowText(string text)
        {
            this.WritePdfString(text);
            this.W("Tj").WE();
        }

        /// <summary>
        /// Shows the text in newline.
        /// </summary>
        /// <param name="text">The text.</param>
        public void ShowTextInNewline(string text)
        {
            TextState textState = this.curState.TextState;
            textState.Y -= this.curState.TextState.Leading;
            this.WritePdfString(text);
            this.W('\'').WE();
        }

        /// <summary>
        /// Shows the text in newline.
        /// </summary>
        /// <param name="wordSpacing">The word spacing.</param>
        /// <param name="charSpacing">The char spacing.</param>
        /// <param name="text">The text.</param>
        public void ShowTextInNewline(float wordSpacing, float charSpacing, string text)
        {
            TextState textState = this.curState.TextState;
            textState.Y -= this.curState.TextState.Leading;
            this.curState.TextState.CharSpace = charSpacing;
            this.curState.TextState.WordSpace = wordSpacing;
            this.WS(wordSpacing).WS(charSpacing);
            this.WritePdfString(text);
            this.WS('"').WE();
        }

        /// <summary>
        /// Stroke the path.
        /// </summary>
        public void Stroke()
        {
            this.W('S').WE();
        }

        /// <summary>
        /// Ws the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private PdfGraphics W(PdfName name)
        {
            name.WriteTo(this.Psw);
            return this;
        }

        /// <summary>
        /// Ws the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private PdfGraphics W(PdfString str)
        {
            str.WriteTo(this.Psw);
            return this;
        }

        /// <summary>
        /// Ws the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private PdfGraphics W(byte b)
        {
            this.Psw.WriteByte(b);
            return this;
        }

        /// <summary>
        /// Ws the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private PdfGraphics W(char c)
        {
            this.Psw.WriteChar(c);
            return this;
        }

        /// <summary>
        /// Ws the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics W(double value)
        {
            this.Psw.WriteDouble(value);
            return this;
        }

        /// <summary>
        /// Ws the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics W(int value)
        {
            this.Psw.WriteInt(value);
            return this;
        }

        /// <summary>
        /// Ws the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics W(float value)
        {
            this.Psw.WriteDouble((double) value);
            return this;
        }

        /// <summary>
        /// Ws the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private PdfGraphics W(string str)
        {
            this.Psw.WriteString(str);
            return this;
        }

        /// <summary>
        /// Ws the specified buf.
        /// </summary>
        /// <param name="buf">The buf.</param>
        /// <returns></returns>
        private PdfGraphics W(byte[] buf)
        {
            this.Psw.WriteBytes(buf);
            return this;
        }

        /// <summary>
        /// WCMYKs the specified cyan.
        /// </summary>
        /// <param name="cyan">The cyan.</param>
        /// <param name="magenta">The magenta.</param>
        /// <param name="yellow">The yellow.</param>
        /// <param name="black">The black.</param>
        /// <returns></returns>
        private PdfGraphics WCMYK(float cyan, float magenta, float yellow, float black)
        {
            cyan = (cyan < 0f) ? 0f : cyan;
            magenta = (magenta < 0f) ? 0f : magenta;
            yellow = (yellow < 0f) ? 0f : yellow;
            black = (black < 0f) ? 0f : black;
            cyan = (cyan > 1f) ? 1f : cyan;
            magenta = (magenta > 1f) ? 1f : magenta;
            yellow = (yellow > 1f) ? 1f : yellow;
            black = (black > 1f) ? 1f : black;
            return this.WS(cyan).WS(magenta).WS(yellow).W(black);
        }

        /// <summary>
        /// WEs this instance.
        /// </summary>
        /// <returns></returns>
        private PdfGraphics WE()
        {
            this.Psw.WriteLineEnd();
            return this;
        }

        /// <summary>
        /// WRGBs the specified red.
        /// </summary>
        /// <param name="red">The red.</param>
        /// <param name="green">The green.</param>
        /// <param name="blue">The blue.</param>
        /// <returns></returns>
        private PdfGraphics WRGB(float red, float green, float blue)
        {
            red = (red < 0f) ? 0f : red;
            green = (green < 0f) ? 0f : green;
            blue = (blue < 0f) ? 0f : blue;
            red = (red > 1f) ? 1f : red;
            green = (green > 1f) ? 1f : green;
            blue = (blue > 1f) ? 1f : blue;
            return this.WS(red).WS(green).W(blue);
        }

        /// <summary>
        /// Writes the PDF string.
        /// </summary>
        /// <param name="text">The text.</param>
        private void WritePdfString(string text)
        {
            if (this.curState.Font == null)
            {
                throw new PdfGraphicsException("Please set font and size first.");
            }
            byte[] showOperan = this.curState.Font.GetShowOperan(text);
            if (showOperan != null)
            {
                PdfString str = new PdfString(showOperan) {
                    IsHexMode = true
                };
                this.WS(str);
            }
            else
            {
                this.WS(PdfString.Empty);
            }
        }

        /// <summary>
        /// WSs this instance.
        /// </summary>
        /// <returns></returns>
        private PdfGraphics WS()
        {
            this.Psw.WriteSpace();
            return this;
        }

        /// <summary>
        /// WSs the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private PdfGraphics WS(PdfName name)
        {
            return this.W(name).WS();
        }

        /// <summary>
        /// WSs the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private PdfGraphics WS(PdfString str)
        {
            return this.W(str).WS();
        }

        /// <summary>
        /// WSs the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        private PdfGraphics WS(byte b)
        {
            return this.W(b).WS();
        }

        /// <summary>
        /// WSs the specified c.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        private PdfGraphics WS(char c)
        {
            return this.W(c).WS();
        }

        /// <summary>
        /// WSs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics WS(double value)
        {
            return this.W(value).WS();
        }

        /// <summary>
        /// WSs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics WS(int value)
        {
            return this.W(value).WS();
        }

        /// <summary>
        /// WSs the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private PdfGraphics WS(float value)
        {
            return this.W(value).WS();
        }

        /// <summary>
        /// WSs the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private PdfGraphics WS(string str)
        {
            return this.W(str).WS();
        }

        /// <summary>
        /// WSs the specified buf.
        /// </summary>
        /// <param name="buf">The buf.</param>
        /// <returns></returns>
        private PdfGraphics WS(byte[] buf)
        {
            return this.W(buf).WS();
        }

        /// <summary>
        /// Gets the character spacing.
        /// </summary>
        /// <value>The character spacing.</value>
        public float CharacterSpacing
        {
            get { return  this.curState.TextState.CharSpace; }
        }

        /// <summary>
        /// Gets the horizontal scaling.
        /// </summary>
        /// <value>The horizontal scaling.</value>
        public float HorizontalScaling
        {
            get { return  this.curState.TextState.HorizontalScale; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:PdfGraphics" /> is uncolored graphics.
        /// </summary>
        /// <value><c>true</c> if uncolored graphics; otherwise, <c>false</c>.</value>
        public bool IsStencil
        {
            get { return  this.stencil; }
        }

        /// <summary>
        /// Gets the leading.
        /// </summary>
        /// <value>The leading.</value>
        public float Leading
        {
            get { return  this.curState.TextState.Leading; }
        }

        /// <summary>
        /// Gets the PSW.
        /// </summary>
        /// <value>The PSW.</value>
        internal PdfStreamWriter Psw
        {
            get { return  this.psw; }
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>The resources.</value>
        public PdfResources Resources
        {
            get
            {
                if (this.resources == null)
                {
                    this.resources = (this.gStream != null) ? this.gStream.GetResources() : new PdfResources();
                }
                return this.resources;
            }
        }

        /// <summary>
        /// Gets the word spacing.
        /// </summary>
        /// <value>The word spacing.</value>
        public float WordSpacing
        {
            get { return  this.curState.TextState.WordSpace; }
        }

        /// <summary>
        /// Gets the X text line matrix.
        /// </summary>
        /// <value>The X text line matrix.</value>
        public float XTextLineMatrix
        {
            get { return  this.curState.TextState.X; }
        }

        /// <summary>
        /// Gets the Y text line matrix.
        /// </summary>
        /// <value>The Y text line matrix.</value>
        public float YTextLineMatrix
        {
            get { return  this.curState.TextState.Y; }
        }

        /// <summary>
        /// Graphics state
        /// </summary>
        public class GraphicsState
        {
            private PdfFont curFont;
            private float fontSize;
            private PdfMatrix matrix;
            private readonly PdfGraphics.TextState textState;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:PdfGraphics.GraphicsState" /> class.
            /// </summary>
            public GraphicsState()
            {
                this.textState = new PdfGraphics.TextState();
                this.matrix = new PdfMatrix();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:PdfGraphics.GraphicsState" /> class.
            /// </summary>
            /// <param name="gs">The gs.</param>
            public GraphicsState(PdfGraphics.GraphicsState gs)
            {
                this.textState = new PdfGraphics.TextState();
                this.matrix = new PdfMatrix();
                if (gs == null)
                {
                    throw new PdfArgumentNullException("gs");
                }
                this.textState = new PdfGraphics.TextState(gs.TextState);
                this.curFont = gs.curFont;
                this.fontSize = gs.fontSize;
                this.matrix.CopyFrom(gs.matrix);
            }

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            /// <value>The font.</value>
            public PdfFont Font
            {
                get { return  this.curFont; }
                set { this.curFont = value; }
            }

            /// <summary>
            /// Gets or sets the size of the font.
            /// </summary>
            /// <value>The size of the font.</value>
            public float FontSize
            {
                get { return  this.fontSize; }
                set { this.fontSize = value; }
            }

            /// <summary>
            /// Gets the matrix.
            /// </summary>
            /// <value>The matrix.</value>
            public PdfMatrix Matrix
            {
                get { return  this.matrix; }
            }

            /// <summary>
            /// Gets the state of the text.
            /// </summary>
            /// <value>The state of the text.</value>
            public PdfGraphics.TextState TextState
            {
                get { return  this.textState; }
            }
        }

        /// <summary>
        /// Line cap type
        /// </summary>
        public enum LineCapType
        {
            Butt,
            Round,
            Square
        }

        /// <summary>
        /// Line join type
        /// </summary>
        public enum LineJoinType
        {
            Miter,
            Round,
            Bevel
        }

        /// <summary>
        /// Text rendering modes
        /// </summary>
        public enum TextRenderingMode
        {
            Fill,
            Stroke,
            FillAndStroke,
            Invisible,
            FillAndAddToClip,
            StrokeAndAddToClip,
            FillAndStrokeAndAddToClip,
            AddToClip
        }

        /// <summary>
        /// The Text state
        /// </summary>
        public class TextState
        {
            private float charSpace;
            private PdfFont font;
            private float fontSize;
            private float horizontalScale;
            private bool knockout;
            private float leading;
            private int renderMode;
            private float rise;
            private float wordSpace;
            private float x;
            private float y;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:PdfGraphics.TextState" /> class.
            /// </summary>
            public TextState()
            {
                this.horizontalScale = 100f;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:PdfGraphics.TextState" /> class.
            /// </summary>
            /// <param name="ts">The ts.</param>
            public TextState(PdfGraphics.TextState ts)
            {
                this.horizontalScale = 100f;
                if (ts == null)
                {
                    throw new PdfArgumentNullException("ts");
                }
                this.charSpace = ts.charSpace;
                this.wordSpace = ts.wordSpace;
                this.horizontalScale = ts.horizontalScale;
                this.leading = ts.leading;
                this.font = ts.font;
                this.fontSize = ts.fontSize;
                this.renderMode = ts.renderMode;
                this.rise = ts.rise;
                this.knockout = ts.knockout;
                this.x = ts.x;
                this.y = ts.y;
            }

            /// <summary>
            /// Gets or sets the char space.
            /// </summary>
            /// <value>The char space.</value>
            public float CharSpace
            {
                get { return  this.charSpace; }
                internal set { this.charSpace = value; }
            }

            /// <summary>
            /// Gets or sets the font.
            /// </summary>
            /// <value>The font.</value>
            public PdfFont Font
            {
                get { return  this.font; }
                internal set { this.font = value; }
            }

            /// <summary>
            /// Gets or sets the size of the font.
            /// </summary>
            /// <value>The size of the font.</value>
            public float FontSize
            {
                get { return  this.fontSize; }
                internal set { this.fontSize = value; }
            }

            /// <summary>
            /// Gets or sets the horizontal scale.
            /// </summary>
            /// <value>The horizontal scale.</value>
            public float HorizontalScale
            {
                get { return  this.horizontalScale; }
                internal set { this.horizontalScale = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="T:PdfGraphics.TextState" /> is knockout.
            /// </summary>
            /// <value><c>true</c> if knockout; otherwise, <c>false</c>.</value>
            public bool Knockout
            {
                get { return  this.knockout; }
                internal set { this.knockout = value; }
            }

            /// <summary>
            /// Gets or sets the leading.
            /// </summary>
            /// <value>The leading.</value>
            public float Leading
            {
                get { return  this.leading; }
                internal set { this.leading = value; }
            }

            /// <summary>
            /// Gets or sets the render mode.
            /// </summary>
            /// <value>The render mode.</value>
            public int RenderMode
            {
                get { return  this.renderMode; }
                internal set { this.renderMode = value; }
            }

            /// <summary>
            /// Gets or sets the rise.
            /// </summary>
            /// <value>The rise.</value>
            public float Rise
            {
                get { return  this.rise; }
                internal set { this.rise = value; }
            }

            /// <summary>
            /// Gets or sets the word space.
            /// </summary>
            /// <value>The word space.</value>
            public float WordSpace
            {
                get { return  this.wordSpace; }
                internal set { this.wordSpace = value; }
            }

            /// <summary>
            /// Gets or sets the X.
            /// </summary>
            /// <value>The X.</value>
            public float X
            {
                get { return  this.x; }
                internal set { this.x = value; }
            }

            /// <summary>
            /// Gets or sets the Y.
            /// </summary>
            /// <value>The Y.</value>
            public float Y
            {
                get { return  this.y; }
                internal set { this.y = value; }
            }
        }
    }
}

