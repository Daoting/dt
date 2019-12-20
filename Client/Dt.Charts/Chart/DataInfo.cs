#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DataInfo
    {
        public int nser;
        public int npts;
        public int ndim;
        public double[] MaxVals;
        public double[] MinVals;
        public double[] Sums;
        public double[] SumsAbs;
        public StackedSum Stacked;
        public double MinX;
        public double MaxX;
        public double MinY;
        public double MaxY;
        public double MinZ;
        public double MaxZ;
        public double DeltaX;
        public Size SymbolSize;
        public Size SymbolMinSize;
        public Size SymbolMaxSize;
        public bool incX;
        public bool hasNaN;
        public void ClearLimits()
        {
            MinX = double.NaN;
            MaxX = double.NaN;
            MinY = double.NaN;
            MaxY = double.NaN;
            MinZ = double.NaN;
            MaxZ = double.NaN;
        }

        public void UpdateLimits(ValueCoordinate coord, double min, double max)
        {
            switch (coord)
            {
                case ValueCoordinate.X:
                    if ((max > MaxX) || double.IsNaN(MaxX))
                    {
                        MaxX = max;
                    }
                    if ((min >= MinX) && !double.IsNaN(MinX))
                    {
                        break;
                    }
                    MinX = min;
                    return;

                case ValueCoordinate.Y:
                    if ((max > MaxY) || double.IsNaN(MaxY))
                    {
                        MaxY = max;
                    }
                    if ((min >= MinY) && !double.IsNaN(MinY))
                    {
                        break;
                    }
                    MinY = min;
                    return;

                case ValueCoordinate.Z:
                    if ((max > MaxZ) || double.IsNaN(MaxZ))
                    {
                        MaxZ = max;
                    }
                    if ((min < MinZ) || double.IsNaN(MinZ))
                    {
                        MinZ = min;
                    }
                    break;

                default:
                    return;
            }
        }
    }
}

