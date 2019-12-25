#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Charts
{
    public class AxisPoint
    {
        Dt.Charts.Axis _axis;
        object _item;
        double _value;

        internal AxisPoint(Dt.Charts.Axis axis, double value, object item)
        {
            _axis = axis;
            _value = value;
            _item = item;
        }

        public override string ToString()
        {
            if (_item is string)
            {
                return (string) ((string) _item);
            }
            return FormattedValue;
        }

        public Dt.Charts.Axis Axis
        {
            get
            {
                if (_axis == null)
                {
                    return null;
                }
                return _axis;
            }
        }

        public object DataObject
        {
            get { return  _item; }
        }

        public string FormattedLogBase
        {
            get
            {
                if (_axis == null)
                {
                    return "";
                }
                if (_axis.LogBase == 2.7182818284590451)
                {
                    return "e";
                }
                if (double.IsNaN(_axis.LogBase))
                {
                    return "";
                }
                return ((double) _axis.LogBase).ToString();
            }
        }

        public string FormattedValue
        {
            get
            {
                if (_axis != null)
                {
                    return _axis.Format((double) Value);
                }
                return ((double) Value).ToString();
            }
        }

        public double LogBase
        {
            get
            {
                if (_axis != null)
                {
                    return _axis.LogBase;
                }
                return double.NaN;
            }
        }

        public double LogValue
        {
            get
            {
                if (double.IsNaN(LogBase))
                {
                    return _value;
                }
                return Math.Log(_value, LogBase);
            }
        }

        public double Value
        {
            get { return  _value; }
        }
    }
}

