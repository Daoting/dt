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
    public partial class AxisScrollBar : ContentControl, IAxisScrollBar
    {
        private Axis _axis;
        private ScrollBar _sb;
        private double _scale;
        private double _value;
        private bool notify;
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

        private double AxisScale
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

        private double AxisValue
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
            _scale = 1.0;
            notify = true;
            base.DefaultStyleKey = typeof(AxisScrollBar);
        }

        private void InitScrollBar()
        {
            if (_sb != null)
            {
                UpdateScale();
                UpdateValue();
                ScrollBar bar = _sb;
                bar.ValueChanged += (sender, e) =>
                {
                    OnAxisRangeChanged();
                };
            }
        }


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _sb = base.GetTemplateChild("ScrollBar") as ScrollBar;
            InitScrollBar();
        }


        private void OnAxisRangeChanged()
        {
            if (notify && (AxisRangeChanged != null))
            {
                AxisRangeChanged(this, new AxisRangeChangedEventArgs(AxisValue, AxisScale));
            }
        }


        private void UpdateScale()
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


        private void UpdateValue()
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

        private void _axis_PropertyChanged(object sender, PropertyChangedEventArgs e)
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