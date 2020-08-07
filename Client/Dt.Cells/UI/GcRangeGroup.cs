#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
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
        List<GroupButtonInfo> _groupButtonInfos;
        List<GroupDotInfo> _groupDotInfos;
        List<GroupLineInfo> _groupLineInfos;
        const double LINE_SIZE = 2.0;
        Brush paintedBrush;
        const double STARTLINE_SIZE = 6.0;

        public GcRangeGroup(SheetView sheetView)
            : base(sheetView)
        {
            _groupLineInfos = new List<GroupLineInfo>();
            _groupDotInfos = new List<GroupDotInfo>();
        }

        void ArrangeColumnGroups(Size finalSize)
        {
            if (GetMaxLevel(Orientation) != -1)
            {
                double num2 = CalcMinWidthOrHeight(finalSize, Orientation);
                if (num2 != 0.0)
                {
                    double x;
                    double num5;
                    double num3 = Math.Max((double)0.0, (double)((num2 - 6.0) / 2.0)) + 2.0;
                    base._sheetView.GetSheetLayout();
                    ColumnLayoutModel columnLayoutModel = base._sheetView.GetColumnLayoutModel(ViewportIndex, SheetArea.Cells);
                    foreach (GroupDotInfo info in _groupDotInfos)
                    {
                        ColumnLayout layout = columnLayoutModel.FindColumn(info.Index);
                        if ((layout != null) && (layout.Width >= 2.0))
                        {
                            x = layout.X + Math.Max((double)0.0, (double)((layout.Width - 2.0) / 2.0));
                            num5 = (base.Location.Y + (info.Level * num2)) + num3;
                            double width = 2.0;
                            double height = 2.0;
                            info.Dot.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(width, height)));
                        }
                    }
                    RangeGroupDirection direction = base._sheetView.ActiveSheet.ColumnRangeGroup.Direction;
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
                            num5 = (base.Location.Y + (info2.Level * num2)) + num3;
                            double num8 = Math.Max((double)0.0, (double)(((layout3.X + layout3.Width) - layout2.X) - 1.0));
                            double num9 = 2.0;
                            line.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(num8, num9)));
                            Rectangle startLine = info2.StartLine;
                            if (startLine != null)
                            {
                                double num10 = Math.Min((double)6.0, (double)(num2 - 2.0));
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
                        RangeGroupButtonPresenter button = info3.Button;
                        ColumnLayout layout4 = columnLayoutModel.FindColumn(button.Index);
                        if (layout4 != null)
                        {
                            double num11 = Math.Max((double)0.0, (double)((layout4.Width - num2) / 2.0));
                            x = layout4.X + num11;
                            num5 = (base.Location.Y + (button.Level * num2)) + 2.0;
                            double num12 = Math.Min(num2, layout4.Width);
                            double num13 = num2;
                            button.Arrange(new Rect(base.PointToClient(new Point(x, num5)), new Size(num12, num13)));
                            Rectangle rectangle3 = info3.Line;
                            if ((rectangle3 != null) && (num12 < layout4.Width))
                            {
                                x = layout4.X;
                                num5 = (base.Location.Y + (button.Level * num2)) + num3;
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
            }
        }

        void ArrangeGroups(Size finalSize)
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                ArrangeRowGroups(finalSize);
            }
            else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                ArrangeColumnGroups(finalSize);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeGroups(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void ArrangeRowGroups(Size finalSize)
        {
            if (GetMaxLevel(Orientation) != -1)
            {
                double num2 = CalcMinWidthOrHeight(finalSize, Orientation);
                if (num2 != 0.0)
                {
                    double num4;
                    double y;
                    double num3 = Math.Max((double)0.0, (double)((num2 - 6.0) / 2.0)) + 2.0;
                    base._sheetView.GetSheetLayout();
                    RowLayoutModel rowLayoutModel = base._sheetView.GetRowLayoutModel(ViewportIndex, SheetArea.Cells);
                    foreach (GroupDotInfo info in _groupDotInfos)
                    {
                        RowLayout layout = rowLayoutModel.Find(info.Index);
                        if ((layout != null) && (layout.Height >= 2.0))
                        {
                            num4 = (base.Location.X + (info.Level * num2)) + num3;
                            y = layout.Y + Math.Max((double)0.0, (double)((layout.Height - 2.0) / 2.0));
                            double width = 2.0;
                            double height = 2.0;
                            info.Dot.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(width, height)));
                        }
                    }
                    RangeGroupDirection direction = base._sheetView.ActiveSheet.RowRangeGroup.Direction;
                    foreach (GroupLineInfo info2 in _groupLineInfos)
                    {
                        RowLayout layout2 = rowLayoutModel.FindRow(info2.Start);
                        RowLayout layout3 = rowLayoutModel.FindRow(info2.End);
                        if ((layout2 != null) && (layout3 != null))
                        {
                            Rectangle line = info2.Line;
                            num4 = (base.Location.X + (info2.Level * num2)) + num3;
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
                                double num10 = Math.Min((double)6.0, (double)(num2 - 2.0));
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
                            RangeGroupButtonPresenter button = info3.Button;
                            double num11 = Math.Max((double)0.0, (double)((layout4.Height - num2) / 2.0));
                            num4 = (base.Location.X + (button.Level * num2)) + 2.0;
                            y = layout4.Y + num11;
                            double num12 = num2;
                            double num13 = Math.Min(num2, layout4.Height);
                            button.Arrange(new Rect(base.PointToClient(new Point(num4, y)), new Size(num12, num13)));
                            Rectangle rectangle3 = info3.Line;
                            if ((rectangle3 != null) && (num13 < layout4.Height))
                            {
                                num4 = (base.Location.X + (button.Level * num2)) + num3;
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
            }
        }

        RangeGroupDirection GetGroupDirection()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return base._sheetView.ActiveSheet.RowRangeGroup.Direction;
            }
            return base._sheetView.ActiveSheet.ColumnRangeGroup.Direction;
        }

        List<RangeGroupInfo> GetGroupsByLevel(int level)
        {
            List<RangeGroupInfo> list = new List<RangeGroupInfo>();
            int rowOrColumnStartIndex = GetRowOrColumnStartIndex();
            int rowOrColumnCount = GetRowOrColumnCount();
            while (rowOrColumnStartIndex < rowOrColumnCount)
            {
                RangeGroupInfo info = null;
                if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                {
                    info = base._sheetView.ActiveSheet.RowRangeGroup.Find(rowOrColumnStartIndex, level);
                }
                else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                {
                    info = base._sheetView.ActiveSheet.ColumnRangeGroup.Find(rowOrColumnStartIndex, level);
                }
                if (info != null)
                {
                    rowOrColumnStartIndex = info.End + 1;
                    list.Add(info);
                }
                rowOrColumnStartIndex++;
            }
            return list;
        }

        int GetRowOrColumnCount()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                int viewportBottomRow = base._sheetView.GetViewportBottomRow(ViewportIndex);
                return Math.Min(base._sheetView.ActiveSheet.RowCount, viewportBottomRow + 2);
            }
            int viewportRightColumn = base._sheetView.GetViewportRightColumn(ViewportIndex);
            return Math.Min(base._sheetView.ActiveSheet.ColumnCount, viewportRightColumn + 2);
        }

        int GetRowOrColumnStartIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                int viewportTopRow = base._sheetView.GetViewportTopRow(ViewportIndex);
                return Math.Max(0, viewportTopRow - 1);
            }
            int viewportLeftColumn = base._sheetView.GetViewportLeftColumn(ViewportIndex);
            return Math.Min(0, viewportLeftColumn - 1);
        }

        int GetViewportEndIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return base._sheetView.GetViewportBottomRow(ViewportIndex);
            }
            return base._sheetView.GetViewportRightColumn(ViewportIndex);
        }

        int GetViewportStartIndex()
        {
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                return base._sheetView.GetViewportTopRow(ViewportIndex);
            }
            return base._sheetView.GetViewportLeftColumn(ViewportIndex);
        }

        void GroupButton_Click(object sender, RoutedEventArgs e)
        {
            RangeGroupButtonPresenter presenter = sender as RangeGroupButtonPresenter;
            if ((presenter != null) && !base._sheetView.IsEditing)
            {
                var sheet = base._sheetView.ActiveSheet;
                int index = presenter.Index;
                if ((sheet != null) && (index >= 0))
                {
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        bool collapsed = sheet.RowRangeGroup.Data.GetCollapsed(index);
                        RowGroupExpandExtent rowExpandExtent = new RowGroupExpandExtent(index, presenter.Level, ViewportIndex, !collapsed);
                        RowGroupExpandUndoAction command = new RowGroupExpandUndoAction(sheet, rowExpandExtent);
                        int num2 = (base._sheetView.ActiveSheet.RowRangeGroup.Direction == RangeGroupDirection.Forward) ? (index - 1) : (index + 1);
                        base._sheetView.ActiveSheet.RowRangeGroup.Find(num2, presenter.Level);
                        if (!base._sheetView.RaiseRangeGroupStateChanging(true, num2, presenter.Level))
                        {
                            base._sheetView.DoCommand(command);
                            base._sheetView.RaiseRangeGroupStateChanged(true, num2, presenter.Level);
                        }
                    }
                    else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        bool flag3 = sheet.ColumnRangeGroup.Data.GetCollapsed(index);
                        ColumnGroupExpandExtent columnExpandExtent = new ColumnGroupExpandExtent(index, presenter.Level, ViewportIndex, !flag3);
                        ColumnGroupExpandUndoAction action2 = new ColumnGroupExpandUndoAction(sheet, columnExpandExtent);
                        int num3 = (base._sheetView.ActiveSheet.RowRangeGroup.Direction == RangeGroupDirection.Forward) ? (index - 1) : (index + 1);
                        if (!base._sheetView.RaiseRangeGroupStateChanging(false, num3, presenter.Level))
                        {
                            base._sheetView.DoCommand(action2);
                            base._sheetView.RaiseRangeGroupStateChanged(false, num3, presenter.Level);
                        }
                    }
                }
            }
        }

        void MeasureGroups(Size availableSize, RangeGroupInfo group, double buttonSize)
        {
            RangeGroupDirection groupDirection = GetGroupDirection();
            int viewportStartIndex = GetViewportStartIndex();
            int viewportEndIndex = GetViewportEndIndex();
            int start = group.Start;
            int end = group.End;
            if (group.State != GroupState.Expanded)
            {
                if (group.State == GroupState.Collapsed)
                {
                    RangeGroupButtonPresenter button = new RangeGroupButtonPresenter();
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
                                    base.Children.Add(button);
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
                                    base.Children.Add(button);
                                    button.Measure(new Size(buttonSize, buttonSize));
                                    button.Index = num14;
                                    info6.LineDirection = RangeGroupDirection.Backward;
                                    _groupButtonInfos.Add(info6);
                                }
                                break;
                            }
                    }
                }
            }
            else
            {
                bool flag = true;
                RangeGroupInfo parent = group.Parent;
                if ((parent != null) && (((groupDirection == RangeGroupDirection.Backward) && (group.Start == parent.Start)) || ((groupDirection == RangeGroupDirection.Forward) && (group.End == parent.End))))
                {
                    flag = false;
                }
                if (flag)
                {
                    RangeGroupButtonPresenter presenter = new RangeGroupButtonPresenter();
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
                                    base.Children.Add(presenter);
                                    presenter.Measure(new Size(buttonSize, buttonSize));
                                    base.Children.Add(rectangle);
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
                                    base.Children.Add(presenter);
                                    presenter.Measure(new Size(buttonSize, buttonSize));
                                    base.Children.Add(rectangle);
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
                        base.Children.Add(line);
                        line.Measure(new Size(2.0, availableSize.Height));
                        if (((groupDirection == RangeGroupDirection.Forward) && (num7 == start)) || ((groupDirection == RangeGroupDirection.Backward) && (num8 == end)))
                        {
                            Rectangle rectangle3 = new Rectangle();
                            rectangle3.Fill = PaintedBrush;
                            info3.StartLine = rectangle3;
                            base.Children.Add(rectangle3);
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
                            base.Children.Add(ellipse);
                            ellipse.Measure(new Size(availableSize.Width, availableSize.Height));
                            _groupDotInfos.Add(info5);
                        }
                    }
                }
            }
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
            base.Children.Clear();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureInitialization();
            if (GetMaxLevel(Orientation) != -1)
            {
                double buttonSize = CalcMinWidthOrHeight(availableSize, Orientation);
                foreach (RangeGroupInfo info in GetGroupsByLevel(0))
                {
                    MeasureGroups(availableSize, info, buttonSize);
                }
                switch (Orientation)
                {
                    case Windows.UI.Xaml.Controls.Orientation.Vertical:
                        base.MeasureBottomBorder(availableSize);
                        break;

                    case Windows.UI.Xaml.Controls.Orientation.Horizontal:
                        base.MeasureRightBorder(availableSize);
                        break;
                }
            }
            return base.MeasureOverride(availableSize);
        }

        public Windows.UI.Xaml.Controls.Orientation Orientation { get; set; }

        internal Brush PaintedBrush
        {
            get
            {
                if ((_sheetView != null) && (_sheetView.RangeGroupLineStroke != null))
                {
                    return _sheetView.RangeGroupLineStroke;
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
            public GroupButtonInfo(RangeGroupButtonPresenter button)
            {
                Button = button;
            }

            public RangeGroupButtonPresenter Button { get; private set; }

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

