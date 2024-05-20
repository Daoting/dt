using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace Dt.UIDemo
{
    /// <summary>
    /// 显示数字变化情况
    /// </summary>
    public partial class NumericTicker : UserControl
    {
        #region 静态内容
        static Brush _brNegative = new SolidColorBrush(Colors.Red);
        static Brush _brPositive = new SolidColorBrush(Colors.Green);
        static Color _clrNegative = Color.FromArgb(100, 0xff, 0, 0);
        static Color _clrPositive = Color.FromArgb(100, 0, 0xff, 0);

        #endregion

        const double _norm = 1.74;
        string _format = "n2";

        public NumericTicker()
        {
            InitializeComponent();
        }

        public void Apply(double p_curValue)
        {
            if (double.IsNaN(p_curValue))
            {
                _arrow.Fill = null;
                _txtChange.Foreground = _txtValue.Foreground;
                return;
            }

            var change = _norm == 0 || double.IsNaN(_norm) ? 0 : (p_curValue - _norm) / _norm;

            _txtValue.Text = p_curValue.ToString(_format);
            _txtChange.Text = string.Format("({0:0.0}%)", change * 100);

            if (change == 0)
            {
                _arrow.Fill = null;
                _txtChange.Foreground = _txtValue.Foreground;
            }
            else if (change < 0)
            {
                _stArrow.ScaleY = -1;
                _txtChange.Foreground = _arrow.Fill = _brNegative;
            }
            else
            {
                _stArrow.ScaleY = +1;
                _txtChange.Foreground = _arrow.Fill = _brPositive;
            }

            var points = _sparkLine.Points;
            points.Clear();
            points.Add(new Point(0, _norm));
            points.Add(new Point(1, p_curValue));
        }
    }
}
