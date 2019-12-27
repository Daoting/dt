#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.ObjectModel;
#endregion

namespace Dt.Charts
{
    public class AxisCollection : Collection<Axis>
    {
        ChartView _view;

        protected override void ClearItems()
        {
            while (base.Count > 2)
            {
                RemoveItem(base.Count - 1);
            }
        }

        string GetUniqueName(string prefix)
        {
            string str = "";
            int num = 1;
            while (true)
            {
                str = prefix + ((int) num);
                if (this[str] == null)
                {
                    return str;
                }
                num++;
            }
        }

        protected override void InsertItem(int index, Axis item)
        {
            base.InsertItem(index, item);
            _view.AddAxis(item);
            ChartViewport2D viewElement = _view.ViewPort as ChartViewport2D;
            if (viewElement != null)
            {
                viewElement.AddAxis(item);
            }
            UpdateChart();
        }

        protected override void RemoveItem(int index)
        {
            if (index >= 2)
            {
                ChartViewport2D viewElement = _view.ViewPort as ChartViewport2D;
                if (viewElement != null)
                {
                    Axis ax = base[index];
                    viewElement.RemoveAxis(ax);
                }
                _view.RemoveAxis(base[index]);
                base.RemoveItem(index);
                UpdateChart();
            }
        }

        void UpdateChart()
        {
            if (_view.Chart != null)
            {
                _view.Chart._forceRebuild = true;
                _view.Chart.InvalidateChart();
            }
        }

        public Axis this[string name]
        {
            get
            {
                foreach (Axis axis in this)
                {
                    if (axis.Name == name)
                    {
                        return axis;
                    }
                }
                return null;
            }
        }

        internal ChartView View
        {
            set { _view = value; }
        }
    }
}

