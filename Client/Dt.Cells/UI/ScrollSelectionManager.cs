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
        private double _max;
        private double _min;
        private double _mousePosition;
        private bool _needIncrease;
        private Action<bool> _tickAction;
        private DispatcherTimer _timer;

        public ScrollSelectionManager(double min, double max, Action<bool> tickAction)
        {
            this._tickAction = tickAction;
            this._min = min;
            this._max = max;
            this._timer = new DispatcherTimer();
            DispatcherTimer timer = this._timer;
            timer.Tick += TimerTick;
        }

        public void Dispose()
        {
            if (this._timer != null)
            {
                this._timer.Stop();
                _timer.Tick -= TimerTick;
                this._timer = null;
            }
        }

        private void TimerTick(object sender, object e)
        {
            this._tickAction(this._needIncrease);
        }

        public double MousePosition
        {
            get { return  this._mousePosition; }
            set
            {
                if (this._mousePosition != value)
                {
                    this._mousePosition = value;
                    if ((this._mousePosition > this._min) && (this._mousePosition < this._max))
                    {
                        this._timer.Stop();
                    }
                    else
                    {
                        double num = 0.0;
                        if (this._mousePosition <= this._min)
                        {
                            num = this._min - this._mousePosition;
                            this._needIncrease = false;
                        }
                        else if (this._mousePosition >= this._max)
                        {
                            num = this._mousePosition - this._max;
                            this._needIncrease = true;
                        }
                        if (num < 5.0)
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                        }
                        else if (num < 10.0)
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 150);
                        }
                        else if (num < 15.0)
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else if (num < 20.0)
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else if (num < 25.0)
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        else
                        {
                            this._timer.Interval = new TimeSpan(0, 0, 0, 0, 0x7d);
                        }
                        this._timer.Start();
                    }
                }
            }
        }
    }
}

