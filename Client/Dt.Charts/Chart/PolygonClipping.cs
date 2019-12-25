#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    internal class PolygonClipping
    {
        int ExtraVertices = 20;
        Point[] _inVertexArray;
        Point[] _outVertexArray;

        PolygonClipping()
        {
        }

        public Point[] ClipPolygonByRect(Point[] inArray, Rect clipRect)
        {
            Point[] clipBoundary = (Point[]) Array.CreateInstance( typeof(Point), new int[] { 2 });
            _outVertexArray = (Point[]) Array.CreateInstance( typeof(Point), new int[] { inArray.Length + ExtraVertices });
            _inVertexArray = (Point[]) Array.CreateInstance( typeof(Point), new int[] { inArray.Length + ExtraVertices });
            Array.Copy(inArray, _inVertexArray, inArray.Length);
            clipBoundary[0] = new Point(clipRect.Left, clipRect.Top);
            clipBoundary[1] = new Point(clipRect.Left, clipRect.Bottom);
            int length = inArray.Length;
            if (length == 0)
            {
                return null;
            }
            int outLength = SutherlandHodgmanPolygoClip(length, clipBoundary);
            OutputToInput(length, outLength);
            clipBoundary[0] = new Point(clipRect.Left, clipRect.Bottom);
            clipBoundary[1] = new Point(clipRect.Right, clipRect.Bottom);
            length = outLength;
            if (length == 0)
            {
                return null;
            }
            outLength = SutherlandHodgmanPolygoClip(length, clipBoundary);
            OutputToInput(length, outLength);
            clipBoundary[0] = new Point(clipRect.Right, clipRect.Bottom);
            clipBoundary[1] = new Point(clipRect.Right, clipRect.Top);
            length = outLength;
            if (length == 0)
            {
                return null;
            }
            outLength = SutherlandHodgmanPolygoClip(length, clipBoundary);
            OutputToInput(length, outLength);
            clipBoundary[0] = new Point(clipRect.Right, clipRect.Top);
            clipBoundary[1] = new Point(clipRect.Left, clipRect.Top);
            length = outLength;
            if (length == 0)
            {
                return null;
            }
            outLength = SutherlandHodgmanPolygoClip(length, clipBoundary);
            if (outLength == 0)
            {
                return null;
            }
            Point[] pointArray2 = (Point[]) Array.CreateInstance( typeof(Point), new int[] { outLength });
            for (int i = 0; i < outLength; i++)
            {
                pointArray2[i] = _outVertexArray[i];
            }
            return pointArray2;
        }

        bool Inside(Point testVertex, Point[] clipBoundary)
        {
            return (((clipBoundary[1].X > clipBoundary[0].X) && (testVertex.Y <= clipBoundary[0].Y)) || (((clipBoundary[1].X < clipBoundary[0].X) && (testVertex.Y >= clipBoundary[0].Y)) || (((clipBoundary[1].Y < clipBoundary[0].Y) && (testVertex.X <= clipBoundary[1].X)) || ((clipBoundary[1].Y > clipBoundary[0].Y) && (testVertex.X >= clipBoundary[1].X)))));
        }

        Point Intersect(Point first, Point second, Point[] clipBoundary)
        {
            Point point = new Point();
            if (clipBoundary[0].Y == clipBoundary[1].Y)
            {
                point.Y = clipBoundary[0].Y;
                point.X = first.X + (((clipBoundary[0].Y - first.Y) * (second.X - first.X)) / (second.Y - first.Y));
                return point;
            }
            point.X = clipBoundary[0].X;
            point.Y = first.Y + (((clipBoundary[0].X - first.X) * (second.Y - first.Y)) / (second.X - first.X));
            return point;
        }

        int Output(Point newVertex, int outLength)
        {
            outLength++;
            if (outLength >= _outVertexArray.Length)
            {
                Point[] pointArray = new Point[outLength + ExtraVertices];
                Array.Copy(_outVertexArray, pointArray, outLength - 1);
                _outVertexArray = pointArray;
            }
            _outVertexArray[outLength - 1] = newVertex;
            return outLength;
        }

        void OutputToInput(int inLength, int outLength)
        {
            if ((inLength == 2) && (outLength == 3))
            {
                _inVertexArray[0].X = _outVertexArray[0].X;
                _inVertexArray[0].Y = _outVertexArray[0].Y;
                if (_outVertexArray[0].X == _outVertexArray[1].X)
                {
                    _inVertexArray[1].X = _outVertexArray[2].X;
                    _inVertexArray[1].Y = _outVertexArray[2].Y;
                }
                else
                {
                    _inVertexArray[1].X = _outVertexArray[1].X;
                    _inVertexArray[1].Y = _outVertexArray[1].Y;
                }
            }
            else
            {
                if (outLength > _inVertexArray.Length)
                {
                    _inVertexArray = new Point[inLength + ExtraVertices];
                }
                Point[] inVertexArray = _inVertexArray;
                _inVertexArray = _outVertexArray;
                _outVertexArray = inVertexArray;
            }
        }

        public static Point[] sClipPolygonByRect(Point[] inArray, Rect clipRect)
        {
            if ((inArray == null) || (inArray.Length == 0))
            {
                return null;
            }
            PolygonClipping clipping = new PolygonClipping();
            return clipping.ClipPolygonByRect(inArray, clipRect);
        }

        int SutherlandHodgmanPolygoClip(int inLength, Point[] clipBoundary)
        {
            int outLength = 0;
            Point testVertex = _inVertexArray[inLength - 1];
            for (int i = 0; i < inLength; i++)
            {
                Point point3;
                Point point2 = _inVertexArray[i];
                if (Inside(point2, clipBoundary))
                {
                    if (Inside(testVertex, clipBoundary))
                    {
                        outLength = Output(point2, outLength);
                    }
                    else
                    {
                        point3 = Intersect(testVertex, point2, clipBoundary);
                        outLength = Output(point3, outLength);
                        outLength = Output(point2, outLength);
                    }
                }
                else if (Inside(testVertex, clipBoundary))
                {
                    point3 = Intersect(testVertex, point2, clipBoundary);
                    outLength = Output(point3, outLength);
                }
                testVertex = point2;
            }
            return outLength;
        }
    }
}

