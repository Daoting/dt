#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal class ScrollSelectionManager : IDisposable
    {
        double _max;
        double _min;
        double _mousePosition;
        bool _needIncrease;
        Action<bool> _tickAction;
        DispatcherTimer _timer;

        public ScrollSelectionManager(double min, double max, Action<bool> tickAction)
        {
            _tickAction = tickAction;
            _min = min;
            _max = max;
            _timer = new DispatcherTimer();
            DispatcherTimer timer = _timer;
            timer.Tick += TimerTick;
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= TimerTick;
                _timer = null;
            }
        }

        void TimerTick(object sender, object e)
        {
            _tickAction(_needIncrease);
        }

        public double MousePosition
        {
            get { return  _mousePosition; }
            set
            {
                if (_mousePosition != value)
                {
                    _mousePosition = value;
                    if ((_mousePosition > _min) && (_mousePosition < _max))
                    {
                        _timer.Stop();
                    }
                    else
                    {
                        double num = 0.0;
                        if (_mousePosition <= _min)
                        {
                            num = _min - _mousePosition;
                            _needIncrease = false;
                        }
                        else if (_mousePosition >= _max)
                        {
                            num = _mousePosition - _max;
                            _needIncrease = true;
                        }
                        if (num < 5.0)
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                        }
                        else if (num < 10.0)
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 150);
                        }
                        else if (num < 15.0)
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else if (num < 20.0)
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else if (num < 25.0)
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else
                        {
                            _timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        _timer.Start();
                    }
                }
            }
        }
    }
}

