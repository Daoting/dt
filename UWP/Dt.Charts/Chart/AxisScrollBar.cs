#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Charts
{
    public partial class AxisScrollBar : UnoControl, IAxisScrollBar
    {
        Axis _axis;
        ScrollBar _sb;
        double _scale = 1.0;
        double _value;
        bool notify = true;
        internal const string ScrollBarElementName = "ScrollBar";

        public Axis Axis
        {
            get { return  _axis; }
            set
            {
                if (_axis != value)
                {
                    if (_axis != null)
                    {
                        _axis.PropertyChanged -= new PropertyChangedEventHandler(_axis_PropertyChanged);
                    }
                    _axis = value;
                    if (_axis != null)
                    {
                        _axis.PropertyChanged += new PropertyChangedEventHandler(_axis_PropertyChanged);
                        notify = false;
                        AxisScale = _axis.Scale;
                        AxisValue = _axis.Value;
                        notify = true;
                    }
                }
            }
        }

        double AxisScale
        {
            get { return  _scale; }
            set
            {
                _scale = value;
                if (_sb != null)
                {
                    UpdateScale();
                }
                OnAxisRangeChanged();
            }
        }

        double AxisValue
        {
            get
            {
                if (_sb == null)
                {
                    return _value;
                }
                if ((Axis != null) && (Axis.AxisType == AxisType.Y))
                {
                    return (1.0 - _sb.Value);
                }
                return _sb.Value;
            }
            set
            {
                _value = value;
                if (_sb != null)
                {
                    UpdateValue();
                }
            }
        }

        public virtual Thickness ScrollBarMargin
        {
            get { return  new Thickness(0.0); }
        }

        public AxisScrollBarPosition ScrollBarPosition { get; set; }

        public event AxisRangeChangedEventHandler AxisRangeChanged;


        public AxisScrollBar()
        {
            DefaultStyleKey = typeof(AxisScrollBar);
        }

        void InitScrollBar()
        {
            if (_sb != null)
            {
                UpdateScale();
                UpdateValue();
                _sb.ValueChanged += (sender, e) =>
                {
                    OnAxisRangeChanged();
                };
            }
        }


        protected override void OnLoadTemplate()
        {
            _sb = GetTemplateChild("ScrollBar") as ScrollBar;
            InitScrollBar();
        }


        void OnAxisRangeChanged()
        {
            if (notify && (AxisRangeChanged != null))
            {
                AxisRangeChanged(this, new AxisRangeChangedEventArgs(AxisValue, AxisScale));
            }
        }


        void UpdateScale()
        {
            if (_scale == 1.0)
            {
                _sb.ViewportSize = 10000.0;
                _sb.LargeChange = 1.0;
                _sb.SmallChange = 1.0;
            }
            else
            {
                _sb.ViewportSize = (1.0 / (1.0 - _scale)) - 1.0;
                _sb.LargeChange = _sb.ViewportSize;
                _sb.SmallChange = 0.5 * _sb.ViewportSize;
            }
        }


        void UpdateValue()
        {
            if ((Axis != null) && (Axis.AxisType == AxisType.Y))
            {
                _sb.Value = 1.0 - _value;
            }
            else
            {
                _sb.Value = _value;
            }
        }

        void _axis_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            notify = false;
            try
            {
                if (e.PropertyName == "Value")
                {
                    AxisValue = Axis.Value;
                }
                else if (e.PropertyName == "Scale")
                {
                    AxisScale = Axis.Scale;
                }
            }
            finally
            {
                notify = true;
            }
        }
    }
}