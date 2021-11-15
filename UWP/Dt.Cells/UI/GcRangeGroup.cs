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
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcRangeGroup : GcGroupBase
    {
        static Size _dotSize = new Size(2.0, 2.0);
        List<GroupButtonInfo> _groupButtonInfos;
        List<GroupDotInfo> _groupDotInfos;
        List<GroupLineInfo> _groupLineInfos;
        Brush paintedBrush;
        int _maxLevel;
        int _rowOrColStartIndex = -1;
        int _rowOrColCount = -1;

        public GcRangeGroup(Excel p_excel)
            : base(p_excel)
        {
            _groupLineInfos = new List<GroupLineInfo>();
            _groupDotInfos = new List<GroupDotInfo>();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _maxLevel = GetMaxLevel(Orientation);
            if (_maxLevel == -1)
                return availableSize;

            int rowOrColStartIndex = GetRowOrColumnStartIndex();
            int rowOrColCount = GetRowOrColumnCount();

            // 区域无变化
            if (rowOrColStartIndex == _rowOrColStartIndex && rowOrColCount == _rowOrColCount)
            {
                foreach (UIElement elem in Children)
                {
                    elem.Measure(availableSize);
                }
                return availableSize;
            }

            _rowOrColStartIndex = rowOrColStartIndex;
            _rowOrColCount = rowOrColCount;
            MeasureInitialization();
            double buttonSize = CalcMinWidthOrHeight(availableSize, Orientation);

            while (rowOrColStartIndex < rowOrColCount)
            {
                RangeGroupInfo info = null;
                if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    info = _excel.ActiveSheet.RowRangeGroup.Find(rowOrColStartIndex, 0);
                }
                else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                {
                    info = _excel.ActiveSheet.ColumnRangeGroup.Find(rowOrColStartIndex, 0);
                }

                if (info != null)
                {
                    rowOrColStartIndex = info.End + 1;
                    MeasureGroups(availableSize, info, buttonSize);
                }
                rowOrColStartIndex++;
            }

            switch (Orientation)
            {
                case Windows.UI.Xaml.Controls.Orientation.Vertical:
                    MeasureBottomBorder(availableSize);
                    break;

                case Windows.UI.Xaml.Controls.Orientation.Horizontal:
                    MeasureRightBorder(availableSize);
                    break;
            }
            return availableSize;
        }

        void MeasureInitialization()
        {
            _groupLineInfos = new List<GroupLineInfo>();
            _groupDotInfos = new List<GroupDotInfo>();
            if ((_groupButtonInfos != null) && (_groupButtonInfos.Count > 0))
            {
                foreach (GroupButtonInfo info in _groupButtonInfos)
                {
                    info.Button.Click -= GroupButton_Click;
                }
            }
            _groupButtonInfos = new List<GroupButtonInfo>();
            Children.Clear();
        }

        void MeasureGroups(Size availableSize, RangeGroupInfo group, double buttonSize)
        {
            RangeGroupDirection groupDirection = GetGroupDirection();
            int viewportStartIndex = GetViewportStartIndex();
            int viewportEndIndex = GetViewportEndIndex();
            int start = group.Start;
            int end = group.End;

            if (group.State == GroupState.Collapsed)
            {
                // 折叠状态
                GroupButton button = new GroupButton();
                button.Click += GroupButton_Click;
                button.IsExpanded = false;
                button.Level = group.Level;
                GroupButtonInfo info6 = new GroupButtonInfo(button);

                switch (groupDirection)
                {
                    case RangeGroupDirection.Forward:
                        {
                            int num13 = end + 1;
                            if ((num13 >= viewportStartIndex) && (num13 <= viewportEndIndex))
                            {
                                Children.Add(button);
                                button.Measure(new Size(buttonSize, buttonSize));
                                button.Index = num13;
                                info6.LineDirection = RangeGroupDirection.Forward;
                                _groupButtonInfos.Add(info6);
                                return;
                            }
                            break;
                        }
                    case RangeGroupDirection.Backward:
                        {
                            int num14 = start - 1;
                            if ((num14 >= viewportStartIndex) && (num14 <= viewportEndIndex))
                            {
                                Children.Add(button);
                                button.Measure(new Size(buttonSize, buttonSize));
                                button.Index = num14;
                                info6.LineDirection = RangeGroupDirection.Backward;
                                _groupButtonInfos.Add(info6);
                            }
                            break;
                        }
                }
            }
            else
            {
                // 展开状态
                bool flag = true;
                RangeGroupInfo parent = group.Parent;
                if ((parent != null) && (((groupDirection == RangeGroupDirection.Backward) && (group.Start == parent.Start)) || ((groupDirection == RangeGroupDirection.Forward) && (group.End == parent.End))))
                {
                    flag = false;
                }
                if (flag)
                {
                    GroupButton presenter = new GroupButton();
                    presenter.Click += GroupButton_Click;
                    presenter.IsExpanded = true;
                    presenter.Level = group.Level;
                    GroupButtonInfo info2 = new GroupButtonInfo(presenter);
                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = PaintedBrush;
                    info2.Line = rectangle;
                    switch (groupDirection)
                    {
                        case RangeGroupDirection.Forward:
                            {
                                int num5 = end + 1;
                                if ((num5 >= viewportStartIndex) && (num5 <= viewportEndIndex))
                                {
                                    presenter.Index = num5;
                                    info2.LineDirection = RangeGroupDirection.Forward;
                                    Children.Add(presenter);
                                    presenter.Measure(new Size(buttonSize, buttonSize));
                                    Children.Add(rectangle);
                                    rectangle.Measure(new Size(availableSize.Width, availableSize.Height));
                                    _groupButtonInfos.Add(info2);
                                }
                                break;
                            }
                        case RangeGroupDirection.Backward:
                            {
                                int num6 = start - 1;
                                if ((num6 >= viewportStartIndex) && (num6 <= viewportEndIndex))
                                {
                                    presenter.Index = num6;
                                    info2.LineDirection = RangeGroupDirection.Backward;
                                    Children.Add(presenter);
                                    presenter.Measure(new Size(buttonSize, buttonSize));
                                    Children.Add(rectangle);
                                    rectangle.Measure(new Size(availableSize.Width, availableSize.Height));
                                    _groupButtonInfos.Add(info2);
                                }
                                break;
                            }
                    }
                }
                if ((start <= viewportEndIndex) && (end >= viewportStartIndex))
                {
                    int num7 = Math.Max(viewportStartIndex, start);
                    int num8 = Math.Min(viewportEndIndex, end);
                    if (flag)
                    {
                        Rectangle line = new Rectangle();
                        line.Fill = PaintedBrush;
                        GroupLineInfo info3 = new GroupLineInfo(line)
                        {
                            Start = num7,
                            End = num8,
                            Level = group.Level
                        };
                        Children.Add(line);
                        line.Measure(new Size(2.0, availableSize.Height));
                        if (((groupDirection == RangeGroupDirection.Forward) && (num7 == start)) || ((groupDirection == RangeGroupDirection.Backward) && (num8 == end)))
                        {
                            Rectangle rectangle3 = new Rectangle();
                            rectangle3.Fill = PaintedBrush;
                            info3.StartLine = rectangle3;
                            Children.Add(rectangle3);
                            rectangle3.Measure(new Size(availableSize.Width, availableSize.Height));
                        }
                        _groupLineInfos.Add(info3);
                    }
                    List<int> list = new List<int>();
                    for (int i = num7; i <= num8; i++)
                    {
                        list.Add(i);
                    }
                    foreach (RangeGroupInfo info4 in group.Children)
                    {
                        if (info4.State == GroupState.Collapsed)
                        {
                            for (int j = info4.Start; j <= info4.End; j++)
                            {
                                list.Remove(j);
                            }
                        }
                        switch (groupDirection)
                        {
                            case RangeGroupDirection.Forward:
                                list.Remove(info4.End + 1);
                                break;

                            case RangeGroupDirection.Backward:
                                list.Remove(info4.Start - 1);
                                break;
                        }
                        MeasureGroups(availableSize, info4, buttonSize);
                        if (((groupDirection == RangeGroupDirection.Backward) && (group.Start == info4.Start)) || ((groupDirection == RangeGroupDirection.Forward) && (group.End == info4.End)))
                        {
                            for (int k = info4.Start; k <= info4.End; k++)
                            {
                                list.Remove(k);
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        foreach (int num12 in list)
                        {
                            Ellipse ellipse = new Ellipse();
                            ellipse.Fill = PaintedBrush;
                            GroupDotInfo info5 = new GroupDotInfo(ellipse)
                            {
                                Index = num12,
                                Level = group.Level + 1
                            };
                            Children.Add(ellipse);
                            ellipse.Measure(_dotSize);
                            _groupDotInfos.Add(info5);
                        }
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double buttonSize = CalcMinWidthOrHeight(finalSize, Orientation);
            if (_maxLevel == -1 || buttonSize == 0.0)
                return finalSize;

            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                ArrangeRowGroups(buttonSize);
            }
            else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                ArrangeColumnGroups(buttonSize);
            }
            return finalSize;
        }

        void ArrangeRowGroups(double buttonSize)
        {
            double num4;
            double y;
            double num3 = Math.Max((double)0.0, (double)((buttonSize - 6.0) / 2.0)) + 2.0;
            _excel.GetSheetLayout();
            
            RowLayoutModel rowLayoutModel = _excel.GetRowLayoutModel(ViewportIndex, SheetArea.Cells);
            foreach (GroupDotInfo info in _groupDotInfos)
            {
                RowLayout layout = rowLayoutModel.Find(info.Index);
                if ((layout != null) && (layout.Height >= 2.0))
                {
                    num4 = (base.Location.X + (info.Level * buttonSize)) + num3;
                    y = layout.Y + Math.Max((double)0.0, (double)((layout.Height - 2.0) / 2.0));
                    info.Dot.Arrange(new Rect(base.PointToClient(new Point(num4, y)), _dotSize));
                }
            }
            RangeGroupDirection direction = _excel.ActiveSheet.RowRangeGroup.Direction;
            foreach (GroupLineInfo info2 in _groupLineInfos)
            {
                RowLayout layout2 = rowLayoutModel.FindRow(info2.Start);
                RowLayout layout3 = rowLayoutModel.FindRow(info2.End);
                if ((layout2 != null) && (layout3 != null))
                {
                    Rectangle line = info2.Line;
                    num4 = (base.Location.X + (info2.Level * buttonSize)) + num3;
                    y = layout2.Y;
                    switch (direction)
                    {
                        case RangeGroupDirection.Forward:
                            y++;
                            break;

                        case RangeGroupDirection.Backward:
                            y--;
                            break;
                    }
                    double num8 = 2.0;
                    double num9 = Math.Max((double)0.0, (double)(((layout3.Y + layout3.Height) - layout2.Y) - 1.0));
                    line.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(num8, num9)));
                    Rectangle startLine = info2.StartLine;
                    if (startLine != null)
                    {
                        double num10 = Math.Min((double)6.0, (double)(buttonSize - 2.0));
                        if (num10 > 0.0)
                        {
                            if (direction == RangeGroupDirection.Backward)
                            {
                                y = (y + num9) - 2.0;
                            }
                            if ((y >= layout2.Y) && (y < (layout3.Y + layout3.Height)))
                            {
                                startLine.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(num10, 2.0)));
                            }
                        }
                    }
                }
            }
            foreach (GroupButtonInfo info3 in _groupButtonInfos)
            {
                RowLayout layout4 = rowLayoutModel.FindRow(info3.Button.Index);
                if (layout4 != null)
                {
                    GroupButton button = info3.Button;
                    double num11 = Math.Max((double)0.0, (double)((layout4.Height - buttonSize) / 2.0));
                    num4 = (base.Location.X + (button.Level * buttonSize)) + 2.0;
                    y = layout4.Y + num11;
                    double num12 = buttonSize;
                    double num13 = Math.Min(buttonSize, layout4.Height);
                    button.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(num12, num13)));
                    Rectangle rectangle3 = info3.Line;
                    if ((rectangle3 != null) && (num13 < layout4.Height))
                    {
                        num4 = (base.Location.X + (button.Level * buttonSize)) + num3;
                        y = layout4.Y;
                        double num14 = 2.0;
                        double num15 = num11;
                        if (info3.LineDirection == RangeGroupDirection.Backward)
                        {
                            y += num11 + num13;
                            num15 = (layout4.Height - num13) - num11;
                        }
                        rectangle3.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(num14, num15)));
                    }
                }
            }
        }

        void ArrangeColumnGroups(double buttonSize)
        {
            double x;
            double num5;
            double num3 = Math.Max((double)0.0, (double)((buttonSize - 6.0) / 2.0)) + 2.0;
            ColumnLayoutModel columnLayoutModel = _excel.GetColumnLayoutModel(ViewportIndex, SheetArea.Cells);

            foreach (GroupDotInfo info in _groupDotInfos)
            {
                ColumnLayout layout = columnLayoutModel.FindColumn(info.Index);
                if ((layout != null) && (layout.Width >= 2.0))
                {
                    x = layout.X + Math.Max((double)0.0, (double)((layout.Width - 2.0) / 2.0));
                    num5 = (Location.Y + (info.Level * buttonSize)) + num3;
                    info.Dot.Arrange(new Rect(PointToClient(new Point(x, num5)), _dotSize));
                }
            }

            RangeGroupDirection direction = _excel.ActiveSheet.ColumnRangeGroup.Direction;
            foreach (GroupLineInfo info2 in _groupLineInfos)
            {
                ColumnLayout layout2 = columnLayoutModel.FindColumn(info2.Start);
                ColumnLayout layout3 = columnLayoutModel.FindColumn(info2.End);
                if ((layout2 != null) && (layout3 != null))
                {
                    Rectangle line = info2.Line;
                    x = layout2.X;
                    switch (direction)
                    {
                        case RangeGroupDirection.Forward:
                            x++;
                            break;

                        case RangeGroupDirection.Backward:
                            x--;
                            break;
                    }
                    num5 = (base.Location.Y + (info2.Level * buttonSize)) + num3;
                    double num8 = Math.Max((double)0.0, (double)(((layout3.X + layout3.Width) - layout2.X) - 1.0));
                    double num9 = 2.0;
                    line.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(num8, num9)));
                    Rectangle startLine = info2.StartLine;
                    if (startLine != null)
                    {
                        double num10 = Math.Min((double)6.0, (double)(buttonSize - 2.0));
                        if (num10 > 0.0)
                        {
                            if (direction == RangeGroupDirection.Backward)
                            {
                                x = (x + num8) - 2.0;
                            }
                            if ((x >= layout2.X) && (x < (layout3.X + layout3.Width)))
                            {
                                startLine.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(2.0, num10)));
                            }
                        }
                    }
                }
            }

            foreach (GroupButtonInfo info3 in _groupButtonInfos)
            {
                GroupButton button = info3.Button;
                ColumnLayout layout4 = columnLayoutModel.FindColumn(button.Index);
                if (layout4 != null)
                {
                    double num11 = Math.Max((double)0.0, (double)((layout4.Width - buttonSize) / 2.0));
                    x = layout4.X + num11;
                    num5 = (base.Location.Y + (button.Level * buttonSize)) + 2.0;
                    double num12 = Math.Min(buttonSize, layout4.Width);
                    double num13 = buttonSize;
                    button.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(num12, num13)));
                    Rectangle rectangle3 = info3.Line;
                    if ((rectangle3 != null) && (num12 < layout4.Width))
                    {
                        x = layout4.X;
                        num5 = (base.Location.Y + (button.Level * buttonSize)) + num3;
                        double num14 = num11;
                        double num15 = 2.0;
                        if (info3.LineDirection == RangeGroupDirection.Backward)
                        {
                            x += num11 + num12;
                            num14 = (layout4.Width - num12) - num11;
                        }
                        rectangle3.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(num14, num15)));
                    }
                }
            }
        }

        RangeGroupDirection GetGroupDirection()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return _excel.ActiveSheet.RowRangeGroup.Direction;
            }
            return _excel.ActiveSheet.ColumnRangeGroup.Direction;
        }

        int GetRowOrColumnCount()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                int viewportBottomRow = _excel.GetViewportBottomRow(ViewportIndex);
                return Math.Min(_excel.ActiveSheet.RowCount, viewportBottomRow + 2);
            }
            int viewportRightColumn = _excel.GetViewportRightColumn(ViewportIndex);
            return Math.Min(_excel.ActiveSheet.ColumnCount, viewportRightColumn + 2);
        }

        int GetRowOrColumnStartIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                int viewportTopRow = _excel.GetViewportTopRow(ViewportIndex);
                return Math.Max(0, viewportTopRow - 1);
            }
            int viewportLeftColumn = _excel.GetViewportLeftColumn(ViewportIndex);
            return Math.Min(0, viewportLeftColumn - 1);
        }

        int GetViewportEndIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return _excel.GetViewportBottomRow(ViewportIndex);
            }
            return _excel.GetViewportRightColumn(ViewportIndex);
        }

        int GetViewportStartIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return _excel.GetViewportTopRow(ViewportIndex);
            }
            return _excel.GetViewportLeftColumn(ViewportIndex);
        }

        void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            GroupButton presenter = sender as GroupButton;
            if ((presenter != null) && !_excel.IsEditing)
            {
                var sheet = _excel.ActiveSheet;
                int index = presenter.Index;
                if ((sheet != null) && (index >= 0))
                {
                    _rowOrColStartIndex = -1;
                    _rowOrColCount = -1;
                    InvalidateMeasure();
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        bool collapsed = sheet.RowRangeGroup.Data.GetCollapsed(index);
                        RowGroupExpandExtent rowExpandExtent = new RowGroupExpandExtent(index, presenter.Level, ViewportIndex, !collapsed);
                        RowGroupExpandUndoAction command = new RowGroupExpandUndoAction(sheet, rowExpandExtent);
                        int num2 = (_excel.ActiveSheet.RowRangeGroup.Direction == RangeGroupDirection.Forward) ? (index - 1) : (index + 1);
                        _excel.ActiveSheet.RowRangeGroup.Find(num2, presenter.Level);
                        if (!_excel.RaiseRangeGroupStateChanging(true, num2, presenter.Level))
                        {
                            _excel.DoCommand(command);
                            _excel.RaiseRangeGroupStateChanged(true, num2, presenter.Level);
                        }
                    }
                    else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        bool flag3 = sheet.ColumnRangeGroup.Data.GetCollapsed(index);
                        ColumnGroupExpandExtent columnExpandExtent = new ColumnGroupExpandExtent(index, presenter.Level, ViewportIndex, !flag3);
                        ColumnGroupExpandUndoAction action2 = new ColumnGroupExpandUndoAction(sheet, columnExpandExtent);
                        int num3 = (_excel.ActiveSheet.RowRangeGroup.Direction == RangeGroupDirection.Forward) ? (index - 1) : (index + 1);
                        if (!_excel.RaiseRangeGroupStateChanging(false, num3, presenter.Level))
                        {
                            _excel.DoCommand(action2);
                            _excel.RaiseRangeGroupStateChanged(false, num3, presenter.Level);
                        }
                    }
                }
            }
        }

        public Windows.UI.Xaml.Controls.Orientation Orientation { get; set; }

        internal Brush PaintedBrush
        {
            get
            {
                if ((_excel != null) && (_excel.RangeGroupLineStroke != null))
                {
                    return _excel.RangeGroupLineStroke;
                }
                if (paintedBrush == null)
                {
                    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        paintedBrush = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        paintedBrush = new SolidColorBrush(Colors.Black);
                    }
                }
                return paintedBrush;
            }
        }

        public int ViewportIndex { get; set; }

        class GroupButtonInfo
        {
            public GroupButtonInfo(GroupButton button)
            {
                Button = button;
            }

            public GroupButton Button { get; private set; }

            public Rectangle Line { get; set; }

            public RangeGroupDirection LineDirection { get; set; }
        }

        class GroupDotInfo
        {
            public GroupDotInfo(Ellipse ellipse)
            {
                Dot = ellipse;
            }

            public Ellipse Dot { get; private set; }

            public int Index { get; set; }

            public int Level { get; set; }
        }

        class GroupLineInfo
        {
            public GroupLineInfo(Rectangle line)
            {
                Line = line;
            }

            public int End { get; set; }

            public int Level { get; set; }

            public Rectangle Line { get; private set; }

            public int Start { get; set; }

            public Rectangle StartLine { get; set; }
        }
    }
}

