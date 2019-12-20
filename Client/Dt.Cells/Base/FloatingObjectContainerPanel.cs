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
    internal partial class FloatingObjectContainerPanel : Panel
    {
        private Dictionary<string, SpreadChartContainer> _cachedCharts = new Dictionary<string, SpreadChartContainer>();
        private Dictionary<string, FloatingObjectContainer> _cachedFloatingObjects = new Dictionary<string, FloatingObjectContainer>();
        private Dictionary<string, PictureContainer> _cachedPictures = new Dictionary<string, PictureContainer>();
        private int _suspendFloatingObjectInvalidate;
        private const int _CHART_MARGIN = 6;

        public FloatingObjectContainerPanel(GcViewport parentViewport)
        {
            this.ParentViewport = parentViewport;
        }

        private void ArrangeCharts()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            foreach (SpreadChartContainer container in this._cachedCharts.Values)
            {
                IFloatingObject floatingObject = container.FloatingObject;
                Windows.Foundation.Point location = new Windows.Foundation.Point(0.0, 0.0);
                Windows.Foundation.Size size = new Windows.Foundation.Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Windows.Foundation.Point(((layout.X - this.Location.X) - num) - num2, ((layout.Y - this.Location.Y) - num) - num2);
                    size = new Windows.Foundation.Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Windows.Foundation.Rect(location, size));
            }
        }

        private void ArrangeFloatingObjects()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            foreach (FloatingObjectContainer container in this._cachedFloatingObjects.Values)
            {
                IFloatingObject floatingObject = container.FloatingObject;
                Windows.Foundation.Point location = new Windows.Foundation.Point(0.0, 0.0);
                Windows.Foundation.Size size = new Windows.Foundation.Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Windows.Foundation.Point(((layout.X - this.Location.X) - num) - num2, ((layout.Y - this.Location.Y) - num) - num2);
                    size = new Windows.Foundation.Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Windows.Foundation.Rect(location, size));
            }
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (!this.IsSuspendFloatingObjectInvalidate)
            {
                this.ArrangeCharts();
                this.ArrangeFloatingObjects();
                this.ArrangePictures();
            }
            return finalSize;
        }

        private void ArrangePictures()
        {
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            foreach (PictureContainer container in this._cachedPictures.Values)
            {
                FloatingObject floatingObject = container.FloatingObject;
                Windows.Foundation.Point location = new Windows.Foundation.Point(0.0, 0.0);
                Windows.Foundation.Size size = new Windows.Foundation.Size(0.0, 0.0);
                FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                if (layout != null)
                {
                    double num = 7.0;
                    double num2 = 1.0;
                    location = new Windows.Foundation.Point(((layout.X - this.Location.X) - num) - num2, ((layout.Y - this.Location.Y) - num) - num2);
                    size = new Windows.Foundation.Size(layout.Width + (2.0 * num), layout.Height + (2.0 * num));
                }
                container.InvalidateArrange();
                container.Arrange(new Windows.Foundation.Rect(location, size));
            }
        }

        private List<SpreadChart> GetCharts()
        {
            List<SpreadChart> list = new List<SpreadChart>();
            if (this.ActiveSheet.Charts.Count > 0)
            {
                foreach (SpreadChart chart in this.ActiveSheet.Charts)
                {
                    list.Add(chart);
                }
            }
            return list;
        }

        internal FloatingObjectContainer GetFloatingObjectContainer(string name)
        {
            SpreadChartContainer container = null;
            if (this._cachedCharts.TryGetValue(name, out container))
            {
                return container;
            }
            PictureContainer container2 = null;
            if (this._cachedPictures.TryGetValue(name, out container2))
            {
                return container2;
            }
            FloatingObjectContainer container3 = null;
            if (this._cachedFloatingObjects.TryGetValue(name, out container3))
            {
                return container3;
            }
            return null;
        }

        private List<FloatingObject> GetFloatingObjects()
        {
            List<FloatingObject> list = new List<FloatingObject>();
            if (this.ActiveSheet.FloatingObjects.Count > 0)
            {
                foreach (FloatingObject obj2 in this.ActiveSheet.FloatingObjects)
                {
                    list.Add(obj2);
                }
            }
            return list;
        }

        public int GetFlotingObjectZIndex(string name)
        {
            FloatingObjectContainer floatingObjectContainer = this.GetFloatingObjectContainer(name);
            if (floatingObjectContainer != null)
            {
                return Canvas.GetZIndex(floatingObjectContainer);
            }
            return -1;
        }

        private int GetMaxZIndex()
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

        private List<Picture> GetPictures()
        {
            List<Picture> list = new List<Picture>();
            if (this.ActiveSheet.Pictures.Count > 0)
            {
                foreach (Picture picture in this.ActiveSheet.Pictures)
                {
                    list.Add(picture);
                }
            }
            return list;
        }

        private void MeasureCharts()
        {
            List<SpreadChart> charts = this.GetCharts();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            for (int i = 0; i < charts.Count; i++)
            {
                SpreadChartContainer container;
                SpreadChart spreadChart = charts[i];
                if (!this._cachedCharts.TryGetValue(spreadChart.Name, out container))
                {
                    container = new SpreadChartContainer(spreadChart, new Chart(), this.ParentViewport);
                    int maxZIndex = this.GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    this._cachedCharts.Add(spreadChart.Name, container);
                }
                if (container != null)
                {
                    Windows.Foundation.Size empty = Windows.Foundation.Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(spreadChart.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Windows.Foundation.Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) this._cachedCharts.Keys))
            {
                if (this.ParentViewport.Sheet.Worksheet.FindChart(str) == null)
                {
                    base.Children.Remove(this._cachedCharts[str]);
                    this._cachedCharts.Remove(str);
                }
            }
        }

        private void MeasureFloatingObjects()
        {
            List<FloatingObject> floatingObjects = this.GetFloatingObjects();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            for (int i = 0; i < floatingObjects.Count; i++)
            {
                FloatingObjectContainer container;
                FloatingObject floatingObject = floatingObjects[i];
                if (!this._cachedFloatingObjects.TryGetValue(floatingObject.Name, out container))
                {
                    FrameworkElement content = null;
                    if (floatingObject is CustomFloatingObject)
                    {
                        content = (floatingObject as CustomFloatingObject).Content;
                    }
                    container = new FloatingObjectContainer(floatingObject, this.ParentViewport);
                    if (content != null)
                    {
                        container.Content = content;
                    }
                    int maxZIndex = this.GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    this._cachedFloatingObjects.Add(floatingObject.Name, container);
                }
                if (container != null)
                {
                    Windows.Foundation.Size empty = Windows.Foundation.Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(floatingObject.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Windows.Foundation.Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) this._cachedFloatingObjects.Keys))
            {
                if (this.ParentViewport.Sheet.Worksheet.FindFloatingObject(str) == null)
                {
                    base.Children.Remove(this._cachedFloatingObjects[str]);
                    this._cachedFloatingObjects.Remove(str);
                }
            }
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (!this.IsSuspendFloatingObjectInvalidate)
            {
                this.MeasureCharts();
                this.MeasureFloatingObjects();
                this.MeasurePictures();
            }
            return this.ParentViewport.GetViewportSize(availableSize);
        }

        private void MeasurePictures()
        {
            List<Picture> pictures = this.GetPictures();
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = this.ParentViewport.Sheet.GetViewportFloatingObjectLayoutModel(this.RowViewportIndex, this.ColumnViewportIndex);
            for (int i = 0; i < pictures.Count; i++)
            {
                PictureContainer container;
                Picture picture = pictures[i];
                if (!this._cachedPictures.TryGetValue(picture.Name, out container))
                {
                    container = new PictureContainer(picture, this.ParentViewport);
                    int maxZIndex = this.GetMaxZIndex();
                    Canvas.SetZIndex(container, maxZIndex + 1);
                    base.Children.Add(container);
                    this._cachedPictures.Add(picture.Name, container);
                }
                if (container != null)
                {
                    Windows.Foundation.Size empty = Windows.Foundation.Size.Empty;
                    FloatingObjectLayout layout = viewportFloatingObjectLayoutModel.Find(picture.Name);
                    if (layout != null)
                    {
                        double num3 = 7.0;
                        empty = new Windows.Foundation.Size(layout.Width + (2.0 * num3), layout.Height + (2.0 * num3));
                    }
                    container.InvalidateMeasure();
                    container.Measure(empty);
                }
            }
            foreach (string str in Enumerable.ToArray<string>((IEnumerable<string>) this._cachedPictures.Keys))
            {
                if (this.ParentViewport.Sheet.Worksheet.FindPicture(str) == null)
                {
                    base.Children.Remove(this._cachedPictures[str]);
                    this._cachedPictures.Remove(str);
                }
            }
        }

        internal void Refresh()
        {
            using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = this._cachedCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.Refresh(null);
                }
            }
            using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator2 = this._cachedFloatingObjects.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Value.Refresh(null);
                }
            }
            using (Dictionary<string, PictureContainer>.Enumerator enumerator3 = this._cachedPictures.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Value.Refresh(null);
                }
            }
        }

        internal void Refresh(object parameter)
        {
            if (!this.IsSuspendFloatingObjectInvalidate)
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
                    FloatingObjectContainer floatingObjectContainer = this.GetFloatingObjectContainer(chart.Name);
                    if (floatingObjectContainer != null)
                    {
                        floatingObjectContainer.Refresh(parameter);
                    }
                }
                else
                {
                    using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = this._cachedCharts.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.Value.Refresh(parameter);
                        }
                    }
                    using (Dictionary<string, PictureContainer>.Enumerator enumerator2 = this._cachedPictures.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            enumerator2.Current.Value.Refresh(parameter);
                        }
                    }
                    using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator3 = this._cachedFloatingObjects.GetEnumerator())
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
            using (Dictionary<string, SpreadChartContainer>.Enumerator enumerator = this._cachedCharts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.RefreshIsSelected();
                }
            }
            using (Dictionary<string, FloatingObjectContainer>.Enumerator enumerator2 = this._cachedFloatingObjects.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Value.RefreshIsSelected();
                }
            }
            using (Dictionary<string, PictureContainer>.Enumerator enumerator3 = this._cachedPictures.GetEnumerator())
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
            if (this._cachedCharts.TryGetValue(floatingObject.Name, out container))
            {
                container.RefreshIsSelected();
            }
            else
            {
                PictureContainer container2 = null;
                if (this._cachedPictures.TryGetValue(floatingObject.Name, out container2))
                {
                    container2.RefreshIsSelected();
                }
                else
                {
                    FloatingObjectContainer container3 = null;
                    if (this._cachedFloatingObjects.TryGetValue(floatingObject.Name, out container3))
                    {
                        container3.RefreshIsSelected();
                    }
                }
            }
        }

        internal void ResumeFloatingObjectInvalidate(bool forceInvalidate)
        {
            this._suspendFloatingObjectInvalidate--;
            if (this._suspendFloatingObjectInvalidate < 0)
            {
                this._suspendFloatingObjectInvalidate = 0;
            }
            if (forceInvalidate)
            {
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        public void SetFlotingObjectZIndex(string name, int zIndex)
        {
            FloatingObjectContainer floatingObjectContainer = this.GetFloatingObjectContainer(name);
            if (floatingObjectContainer != null)
            {
                Canvas.SetZIndex(floatingObjectContainer, zIndex);
            }
        }

        internal void SuspendFloatingObjectInvalidate()
        {
            this._suspendFloatingObjectInvalidate++;
        }

        internal void SyncChartShapeTheme()
        {
            using (Dictionary<string, SpreadChartContainer>.ValueCollection.Enumerator enumerator = this._cachedCharts.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Theme = this.ActiveSheet.Workbook.CurrentTheme;
                }
            }
        }

        public Worksheet ActiveSheet
        {
            get { return  this.ParentViewport.Sheet.Worksheet; }
        }

        private int ColumnViewportIndex
        {
            get { return  this.ParentViewport.ColumnViewportIndex; }
        }

        private bool IsSuspendFloatingObjectInvalidate
        {
            get { return  (this._suspendFloatingObjectInvalidate > 0); }
        }

        public Windows.Foundation.Point Location
        {
            get { return  this.ParentViewport.Location; }
        }

        public GcViewport ParentViewport { get; set; }

        private int RowViewportIndex
        {
            get { return  this.ParentViewport.RowViewportIndex; }
        }
    }
}

