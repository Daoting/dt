#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FloatingObjectLayer : Panel
    {
        Dictionary<string, SpreadChartContainer> _cachedCharts = new Dictionary<string, SpreadChartContainer>();
        Dictionary<string, FloatingObjectContainer> _cachedFloatingObjects = new Dictionary<string, FloatingObjectContainer>();
        Dictionary<string, PictureContainer> _cachedPictures = new Dictionary<string, PictureContainer>();
        int _suspendFloatingObjectInvalidate;
        const int _CHART_MARGIN = 6;

        public FloatingObjectLayer(CellsPanel parentViewport)
        {
            ParentViewport = parentViewport;
        }

        void ArrangeCharts()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            foreach (SpreadChartContainer container in _cachedCharts.Values)
            {
                IFloatingObject floatingObject = container.FloatingObject;
                Point location = new Point(0.0, 0.0);
                Size size = new Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Point(((layout.X - Location.X) - num) - num2, ((layout.Y - Location.Y) - num) - num2);
                    size = new Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Rect(location, size));
            }
        }

        void ArrangeFloatingObjects()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            foreach (FloatingObjectContainer container in _cachedFloatingObjects.Values)
            {
                IFloatingObject floatingObject = container.FloatingObject;
                Point location = new Point(0.0, 0.0);
                Size size = new Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Point(((layout.X - Location.X) - num) - num2, ((layout.Y - Location.Y) - num) - num2);
                    size = new Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Rect(location, size));
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!IsSuspendFloatingObjectInvalidate)
            {
                ArrangeCharts();
                ArrangeFloatingObjects();
                ArrangePictures();
            }
            return finalSize;
        }

        void ArrangePictures()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            foreach (PictureContainer container in _cachedPictures.Values)
            {
                FloatingObject floatingObject = container.FloatingObject;
                Point location = new Point(0.0, 0.0);
                Size size = new Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Point(((layout.X - Location.X) - num) - num2, ((layout.Y - Location.Y) - num) - num2);
                    size = new Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Rect(location, size));
            }
        }

        List<SpreadChart> GetCharts()
        {
            List<SpreadChart> list = new List<SpreadChart>();
            if (ActiveSheet.Charts.Count > 0)
            {
                foreach (SpreadChart chart in ActiveSheet.Charts)
                {
                    list.Add(chart);
                }
            }
            return list;
        }

        internal FloatingObjectContainer GetFloatingObjectContainer(string name)
        {
            SpreadChartContainer container = null;
            if (_cachedCharts.TryGetValue(name, out container))
            {
                return container;
            }
            PictureContainer container2 = null;
            if (_cachedPictures.TryGetValue(name, out container2))
            {
                return container2;
            }
            FloatingObjectContainer container3 = null;
            if (_cachedFloatingObjects.TryGetValue(name, out container3))
            {
                return container3;
            }
            return null;
        }

        List<FloatingObject> GetFloatingObjects()
        {
            List<FloatingObject> list = new List<FloatingObject>();
            if (ActiveSheet.FloatingObjects.Count > 0)
            {
                foreach (FloatingObject obj2 in ActiveSheet.FloatingObjects)
                {
                    list.Add(obj2);
                }
            }
            return list;
        }

        public int GetFlotingObjectZIndex(string name)
        {
            FloatingObjectContainer floatingObjectContainer = GetFloatingObjectContainer(name);
            if (floatingObjectContainer != null)
            {
                return Canvas.GetZIndex(floatingObjectContainer);
            }
            return -1;
        }

        int GetMaxZIndex()
        {
            if (base.Children.Count == 0)
            {
                return 0;
            }
            int num = -2147483648;
            foreach(UIElement elem in base.Children)
            {
                int zIndex = Canvas.GetZIndex(elem);
                if (zIndex > num)
                {
                    num = zIndex;
                }
            }
            return num;
        }

        List<Picture> GetPictures()
        {
            List<Picture> list = new List<Picture>();
            if (ActiveSheet.Pictures.Count > 0)
            {
                foreach (Picture picture in ActiveSheet.Pictures)
                {
                    list.Add(picture);
                }
            }
            return list;
        }

        void MeasureCharts()
        {
            List<SpreadChart> charts = GetCharts();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < charts.Count; i++)
            {
                SpreadChartContainer container;
                SpreadChart spreadChart = charts[i];
                if (!_cachedCharts.TryGetValue(spreadChart.Name, out container))
                {
                    container = new SpreadChartContainer(spreadChart, new Chart(), ParentViewport);
                    int maxZIndex = GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    _cachedCharts.Add(spreadChart.Name, container);
                }
                if (container != null)
                {
                    Size empty = Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(spreadChart.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) _cachedCharts.Keys))
            {
                if (ParentViewport.Excel.ActiveSheet.FindChart(str) == null)
                {
                    base.Children.Remove(_cachedCharts[str]);
                    _cachedCharts.Remove(str);
                }
            }
        }

        void MeasureFloatingObjects()
        {
            List<FloatingObject> floatingObjects = GetFloatingObjects();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < floatingObjects.Count; i++)
            {
                FloatingObjectContainer container;
                FloatingObject floatingObject = floatingObjects[i];
                if (!_cachedFloatingObjects.TryGetValue(floatingObject.Name, out container))
                {
                    FrameworkElement content = null;
                    if (floatingObject is CustomFloatingObject)
                    {
                        content = (floatingObject as CustomFloatingObject).Content;
                    }
                    container = new FloatingObjectContainer(floatingObject, ParentViewport);
                    if (content != null)
                    {
                        container.Content = content;
                    }
                    int maxZIndex = GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    _cachedFloatingObjects.Add(floatingObject.Name, container);
                }
                if (container != null)
                {
                    Size empty = Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) _cachedFloatingObjects.Keys))
            {
                if (ParentViewport.Excel.ActiveSheet.FindFloatingObject(str) == null)
                {
                    base.Children.Remove(_cachedFloatingObjects[str]);
                    _cachedFloatingObjects.Remove(str);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!IsSuspendFloatingObjectInvalidate)
            {
                MeasureCharts();
                MeasureFloatingObjects();
                MeasurePictures();
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        void MeasurePictures()
        {
            List<Picture> pictures = GetPictures();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < pictures.Count; i++)
            {
                PictureContainer container;
                Picture picture = pictures[i];
                if (!_cachedPictures.TryGetValue(picture.Name, out container))
                {
                    container = new PictureContainer(picture, ParentViewport);
                    int maxZIndex = GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    _cachedPictures.Add(picture.Name, container);
                }
                if (container != null)
                {
                    Size empty = Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(picture.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) _cachedPictures.Keys))
            {
                if (ParentViewport.Excel.ActiveSheet.FindPicture(str) == null)
                {
                    base.Children.Remove(_cachedPictures[str]);
                    _cachedPictures.Remove(str);
                }
            }
        }

        internal void Refresh()
        {
            using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = _cachedCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Refresh(null);
                }
            }
            using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator2 = _cachedFloatingObjects.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Value.Refresh(null);
                }
            }
            using (Dictionary<string, PictureContainer>.Enumerator enumerator3 = _cachedPictures.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Value.Refresh(null);
                }
            }
        }

        internal void Refresh(object parameter)
        {
            if (!IsSuspendFloatingObjectInvalidate)
            {
                FloatingObject chart = null;
                if (parameter is ChartChangedEventArgs)
                {
                    chart = (parameter as ChartChangedEventArgs).Chart;
                }
                else if (parameter is FloatingObjectChangedEventArgs)
                {
                    chart = (parameter as FloatingObjectChangedEventArgs).FloatingObject;
                }
                else if (parameter is PictureChangedEventArgs)
                {
                    chart = (parameter as PictureChangedEventArgs).Picture;
                }
                if (chart != null)
                {
                    FloatingObjectContainer floatingObjectContainer = GetFloatingObjectContainer(chart.Name);
                    if (floatingObjectContainer != null)
                    {
                        floatingObjectContainer.Refresh(parameter);
                    }
                }
                else
                {
                    using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = _cachedCharts.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.Value.Refresh(parameter);
                        }
                    }
                    using (Dictionary<string, PictureContainer>.Enumerator enumerator2 = _cachedPictures.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            enumerator2.Current.Value.Refresh(parameter);
                        }
                    }
                    using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator3 = _cachedFloatingObjects.GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            enumerator3.Current.Value.Refresh(parameter);
                        }
                    }
                }
            }
        }

        internal void RefreshContainerIsSelected()
        {
            using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = _cachedCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.RefreshIsSelected();
                }
            }
            using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator2 = _cachedFloatingObjects.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Value.RefreshIsSelected();
                }
            }
            using (Dictionary<string, PictureContainer>.Enumerator enumerator3 = _cachedPictures.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Value.RefreshIsSelected();
                }
            }
        }

        internal void RefreshContainerIsSelected(FloatingObject floatingObject)
        {
            SpreadChartContainer container = null;
            if (_cachedCharts.TryGetValue(floatingObject.Name, out container))
            {
                container.RefreshIsSelected();
            }
            else
            {
                PictureContainer container2 = null;
                if (_cachedPictures.TryGetValue(floatingObject.Name, out container2))
                {
                    container2.RefreshIsSelected();
                }
                else
                {
                    FloatingObjectContainer container3 = null;
                    if (_cachedFloatingObjects.TryGetValue(floatingObject.Name, out container3))
                    {
                        container3.RefreshIsSelected();
                    }
                }
            }
        }

        internal void ResumeFloatingObjectInvalidate(bool forceInvalidate)
        {
            _suspendFloatingObjectInvalidate--;
            if (_suspendFloatingObjectInvalidate < 0)
            {
                _suspendFloatingObjectInvalidate = 0;
            }
            if (forceInvalidate)
            {
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        public void SetFlotingObjectZIndex(string name, int zIndex)
        {
            FloatingObjectContainer floatingObjectContainer = GetFloatingObjectContainer(name);
            if (floatingObjectContainer != null)
            {
                Canvas.SetZIndex(floatingObjectContainer, zIndex);
            }
        }

        internal void SuspendFloatingObjectInvalidate()
        {
            _suspendFloatingObjectInvalidate++;
        }

        internal void SyncChartShapeTheme()
        {
            using (Dictionary<string, SpreadChartContainer>.ValueCollection.Enumerator enumerator = _cachedCharts.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Theme = ActiveSheet.Workbook.CurrentTheme;
                }
            }
        }

        public Worksheet ActiveSheet
        {
            get { return  ParentViewport.Excel.ActiveSheet; }
        }

        int ColumnViewportIndex
        {
            get { return  ParentViewport.ColumnViewportIndex; }
        }

        bool IsSuspendFloatingObjectInvalidate
        {
            get { return  (_suspendFloatingObjectInvalidate > 0); }
        }

        public Point Location
        {
            get { return  ParentViewport.Location; }
        }

        public CellsPanel ParentViewport { get; set; }

        int RowViewportIndex
        {
            get { return  ParentViewport.RowViewportIndex; }
        }
    }
}

