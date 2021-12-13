#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Charts
{
    public class BasePieRenderer : BaseRenderer
    {
        double _innerRadius;
        double _offset;
        double _radius = 0.8;

        internal BasePieRenderer()
        {
            base.ColorScheme = ColorScheme.Point;
        }

        internal override string GetValue(string name)
        {
            switch (name)
            {
                case "InnerRadius":
                    return ((double) InnerRadius).ToString((IFormatProvider) CultureInfo.InvariantCulture);

                case "Offset":
                    return ((double) Offset).ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
            return "";
        }

        protected override void InitOptions()
        {
            base.OptionsBag.Add("InnerRadius");
            base.OptionsBag.Add("Offset");
        }

        internal override void SetValue(string name, string value)
        {
            string str = name;
            if (str != null)
            {
                if (str != "InnerRadius")
                {
                    if (str != "Offset")
                    {
                        return;
                    }
                }
                else
                {
                    InnerRadius = double.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
                    return;
                }
                Offset = double.Parse(value, (IFormatProvider) CultureInfo.InvariantCulture);
            }
        }

        public double InnerRadius
        {
            get { return  _innerRadius; }
            set
            {
                _innerRadius = value;
                base.FireChanged(this, EventArgs.Empty);
            }
        }

        public double Offset
        {
            get { return  _offset; }
            set
            {
                _offset = value;
                base.FireChanged(this, EventArgs.Empty);
            }
        }

        public double Radius
        {
            get { return  _radius; }
            set
            {
                _radius = value;
                base.FireChanged(this, EventArgs.Empty);
            }
        }
    }
}

