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
    internal class Statistics
    {
        int _cnt;
        double _max;
        double _min;
        double _sum;
        double _sum2;

        public Statistics(IList<double> list)
        {
            int num = list.Count;
            for (int i = 0; i < num; i++)
            {
                if (IsValid(list[i]))
                {
                    double num3 = list[i];
                    if (_cnt++ == 0)
                    {
                        _min = _max = num3;
                    }
                    else if (num3 < _min)
                    {
                        _min = num3;
                    }
                    else if (num3 > _max)
                    {
                        _max = num3;
                    }
                    _sum += num3;
                    _sum2 += num3 * num3;
                }
            }
        }

        static bool IsValid(double val)
        {
            return (!double.IsInfinity(val) && !double.IsNaN(val));
        }

        public double Avg
        {
            get
            {
                if (_cnt <= 0)
                {
                    return 0.0;
                }
                return (_sum / ((double) _cnt));
            }
        }

        public int Count
        {
            get { return  _cnt; }
        }

        public double Max
        {
            get { return  _max; }
        }

        public double Min
        {
            get { return  _min; }
        }

        public double StandardDeviation
        {
            get { return  Math.Sqrt(Variance); }
        }

        public double StandardDeviationPop
        {
            get { return  Math.Sqrt(VariancePop); }
        }

        public double Sum
        {
            get { return  _sum; }
        }

        public double Sum2
        {
            get { return  _sum2; }
        }

        public double Variance
        {
            get
            {
                if (_cnt <= 1)
                {
                    return 0.0;
                }
                return ((VariancePop * _cnt) / ((double) (_cnt - 1)));
            }
        }

        public double VariancePop
        {
            get
            {
                if (_cnt > 1)
                {
                    double num = _sum / ((double) _cnt);
                    return ((_sum2 / ((double) _cnt)) - (num * num));
                }
                return 0.0;
            }
        }
    }
}

