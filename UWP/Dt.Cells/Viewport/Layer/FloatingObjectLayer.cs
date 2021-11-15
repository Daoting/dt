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
        static Size _szEmpty = new Size();
        Dictionary<string, SpreadChartContainer> _cachedCharts = new Dictionary<string, SpreadChartContainer>();
        Dictionary<string, FloatingObjectContainer> _cachedFloatingObjects = new Dictionary<string, FloatingObjectContainer>();
        Dictionary<string, PictureContainer> _cachedPictures = new Dictionary<string, PictureContainer>();
        int _suspendFloatingObjectInvalidate;
        const int _CHART_MARGIN = 6;

        public FloatingObjectLayer(CellsPanel parentViewport)
        {
            ParentViewport = parentViewport;
        }

        #region 测量
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!IsSuspendFloatingObjectInvalidate)
            {
                MeasureCharts();
                MeasureFloatingObjects();
                MeasurePictures();
            }
            return availableSize;
        }

        void MeasureCharts()
        {
            var charts = ActiveSheet.Charts;
            if (charts.Count == 0)
            {
                if (_cachedCharts.Count > 0)
                    _cachedCharts.Clear();
                return;
            }

            var oldCharts = _cachedCharts;
            _cachedCharts = new Dictionary<string, SpreadChartContainer>();
            FloatingObjectLayoutModel layoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < charts.Count; i++)
            {
                SpreadChartContainer container;
                SpreadChart chart = charts[i];
                if (!oldCharts.TryGetValue(chart.Name, out container))
                {
                    container = new SpreadChartContainer(chart, new Chart(), ParentViewport);
                    int maxZIndex = GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    Children.Add(container);
                }
                else
                {
                    oldCharts.Remove(chart.Name);
                }
                _cachedCharts.Add(chart.Name, container);

                Size size = _szEmpty;
                FloatingObjectLayout layout = layoutModel.Find(chart.Name);
                if (layout != null)
                {
                    double num3 = 7.0;
                    size = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                }
#if !IOS
                container.InvalidateMeasure();
#endif
                container.Measure(size);
            }

            if (oldCharts.Count > 0)
            {
                foreach (var item in oldCharts)
                {
                    Children.Remove(item.Value);
                }
            }
        }

        void MeasureFloatingObjects()
        {
            var floatingObjects = ActiveSheet.FloatingObjects;
            if (floatingObjects.Count == 0)
            {
                if (_cachedFloatingObjects.Count > 0)
                    _cachedFloatingObjects.Clear();
                return;
            }

            var oldObjects = _cachedFloatingObjects;
            _cachedFloatingObjects = new Dictionary<string, FloatingObjectContainer>();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < floatingObjects.Count; i++)
            {
                FloatingObjectContainer container;
                FloatingObject floatingObject = floatingObjects[i];
                if (!oldObjects.TryGetValue(floatingObject.Name, out container))
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
                    Children.Add(container);
                }
                else
                {
                    oldObjects.Remove(floatingObject.Name);
                }
                _cachedFloatingObjects.Add(floatingObject.Name, container);

                Size empty = _szEmpty;
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num3 = 7.0;
                    empty = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                }
#if !IOS
                container.InvalidateMeasure();
#endif
                container.Measure(empty);
            }

            if (oldObjects.Count > 0)
            {
                foreach (var item in oldObjects)
                {
                    Children.Remove(item.Value);
                }
            }
        }

        void MeasurePictures()
        {
            var pictures = ActiveSheet.Pictures;
            if (pictures.Count == 0)
            {
                if (_cachedPictures.Count > 0)
                    _cachedPictures.Clear();
                return;
            }

            var oldPics = _cachedPictures;
            _cachedPictures = new Dictionary<string, PictureContainer>();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = ParentViewport.Excel.GetViewportFloatingObjectLayoutModel(RowViewportIndex, ColumnViewportIndex);
            for (int i = 0; i < pictures.Count; i++)
            {
                PictureContainer container;
                Picture picture = pictures[i];
                if (!oldPics.TryGetValue(picture.Name, out container))
                {
                    container = new PictureContainer(picture, ParentViewport);
                    int maxZIndex = GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    Children.Add(container);
                }
                else
                {
                    oldPics.Remove(picture.Name);
                }
                _cachedPictures.Add(picture.Name, container);

                Size empty = _szEmpty;
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(picture.Name);
                if (layout != null)
                {
                    double num3 = 7.0;
                    empty = new Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                }

#if !IOS
                container.InvalidateMeasure();
#endif
                container.Measure(empty);
            }

            if (oldPics.Count > 0)
            {
                foreach (var item in oldPics)
                {
                    Children.Remove(item.Value);
                }
            }
        }
        #endregion

        #region 布局
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

        void ArrangeCharts()
        {
            if (_cachedCharts.Count == 0)
                return;

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
#if !IOS
                container.InvalidateArrange();
#endif
                container.Arrange(new Rect(location, size));
            }
        }

        void ArrangeFloatingObjects()
        {
            if (_cachedFloatingObjects.Count == 0)
                return;

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
#if !IOS
                container.InvalidateArrange();
#endif
                container.Arrange(new Rect(location, size));
            }
        }

        void ArrangePictures()
        {
            if (_cachedPictures.Count == 0)
                return;

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
#if !IOS
                container.InvalidateArrange();
#endif
                container.Arrange(new Rect(location, size));
            }
        }
        #endregion

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
            foreach (UIElement elem in base.Children)
            {
                int zIndex = Canvas.GetZIndex(elem);
                if (zIndex > num)
                {
                    num = zIndex;
                }
            }
            return num;
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
                InvalidateMeasure();
                InvalidateArrange();
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
            get { return ParentViewport.Excel.ActiveSheet; }
        }

        int ColumnViewportIndex
        {
            get { return ParentViewport.ColumnViewportIndex; }
        }

        bool IsSuspendFloatingObjectInvalidate
        {
            get { return (_suspendFloatingObjectInvalidate > 0); }
        }

        public Point Location
        {
            get { return ParentViewport.Location; }
        }

        public CellsPanel ParentViewport { get; set; }

        int RowViewportIndex
        {
            get { return ParentViewport.RowViewportIndex; }
        }
    }
}

