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
    internal class StackedSum
    {
        Dictionary<double, double>[] _dicts;
        int _nsg;

        public StackedSum(int nsg)
        {
            _nsg = nsg;
            _dicts = new Dictionary<double, double>[nsg];
            for (int i = 0; i < nsg; i++)
            {
                _dicts[i] = new Dictionary<double, double>();
            }
        }

        public void Add(int sg, double x, double y)
        {
            Check(sg, false);
            Dictionary<double, double> dictionary = _dicts[sg];
            if (dictionary.ContainsKey(x))
            {
                Dictionary<double, double> dictionary2;
                double num;
                (dictionary2 = dictionary)[num = x] = dictionary2[num] + y;
            }
            else
            {
                dictionary[x] = y;
            }
        }

        bool Check(int sg, bool throwException = false)
        {
            if ((sg >= 0) && (sg < _nsg))
            {
                return true;
            }
            if (throwException)
            {
                throw new ArgumentOutOfRangeException();
            }
            return false;
        }

        public double[] GetYs(int sg)
        {
            Check(sg, false);
            Dictionary<double, double> dictionary = _dicts[sg];
            double[] numArray = new double[dictionary.Values.Count];
            dictionary.Values.CopyTo(numArray, 0);
            return numArray;
        }

        public double this[int sg, double x]
        {
            get
            {
                Check(sg, false);
                Dictionary<double, double> dictionary = _dicts[sg];
                if (dictionary.ContainsKey(x))
                {
                    return dictionary[x];
                }
                return 0.0;
            }
            set
            {
                Check(sg, false);
                _dicts[sg][x] = value;
            }
        }
    }
}

