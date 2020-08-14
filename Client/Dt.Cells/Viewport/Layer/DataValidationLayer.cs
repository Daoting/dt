#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class DataValidationLayer : Panel
    {
        DataValidationListButton _dataValidationListButton;
        DataValidationListButtonInfo _dataValidationListButtonInfo;
        DataValidationInputMessagePopUp _inputMessagePopUp;
        Dictionary<int, Dictionary<int, InvalidDataPresenter>> _presenters;
        DataValidator _validator;
        internal const double DATAVALIDATIONLISTBUTTONWIDTH = 16.0;

        public DataValidationLayer(CellsPanel parent)
        {
            ParentViewport = parent;
            IsHitTestVisible = false;
        }

        public void AddDataValidationListButtonInfo(DataValidationListButtonInfo info)
        {
            if (info != null)
            {
                _dataValidationListButtonInfo = info;
                if (_dataValidationListButton == null)
                {
                    _dataValidationListButton = new DataValidationListButton();
                    base.Children.Add(_dataValidationListButton);
                }
            }
        }

        public void AddInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            if (_presenters == null)
            {
                _presenters = new Dictionary<int, Dictionary<int, InvalidDataPresenter>>();
            }
            Dictionary<int, InvalidDataPresenter> dictionary = null;
            if (!_presenters.TryGetValue(info.Row, out dictionary))
            {
                dictionary = new Dictionary<int, InvalidDataPresenter>();
                _presenters[info.Row] = dictionary;
            }
            if (!dictionary.ContainsKey(info.Column))
            {
                InvalidDataPresenter presenter = new InvalidDataPresenter();
                base.Children.Add(presenter);
                dictionary[info.Column] = presenter;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((ParentViewport._activeRow >= 0) && (ParentViewport._activeCol >= 0))
            {
                Rect rangeBounds = ParentViewport.GetRangeBounds(new CellRange(ParentViewport._activeRow, ParentViewport._activeCol, 1, 1));
                double height = rangeBounds.Height;
                double width = rangeBounds.Width;
                double x = rangeBounds.X;
                double y = rangeBounds.Y;
                if (double.IsInfinity(rangeBounds.Height))
                {
                    height = 0.0;
                    y = 0.0;
                }
                if (double.IsInfinity(rangeBounds.Width))
                {
                    width = 0.0;
                    x = 0.0;
                }
                if ((_dataValidationListButton != null) && (_dataValidationListButtonInfo != null))
                {
                    _dataValidationListButton.VerticalAlignment = VerticalAlignment.Bottom;
                    if (_dataValidationListButtonInfo.Column == _dataValidationListButtonInfo.DisplayColumn)
                    {
                        _dataValidationListButton.HorizontalAlignment = HorizontalAlignment.Right;
                        Rect rect2 = new Rect(x - 1.0, y - 1.0, width, height);
                        _dataValidationListButton.Arrange(rect2);
                    }
                    else
                    {
                        _dataValidationListButton.HorizontalAlignment = HorizontalAlignment.Left;
                        Rect rect3 = new Rect((x + width) + 1.0, y - 1.0, 16.0, height);
                        _dataValidationListButton.Arrange(rect3);
                    }
                }
                if (_inputMessagePopUp != null)
                {
                    if (!double.IsInfinity(rangeBounds.Height))
                    {
                        y += rangeBounds.Height + 5.0;
                    }
                    if (!double.IsInfinity(rangeBounds.Width))
                    {
                        x += rangeBounds.Width / 2.0;
                    }
                    Size size = (_inputMessagePopUp.Content as Grid).DesiredSize;
                    Rect rect4 = new Rect(x, y, ((size.Width + (2.0 * (_inputMessagePopUp.Padding.Left + _inputMessagePopUp.Padding.Right))) + _inputMessagePopUp.BorderThickness.Left) + _inputMessagePopUp.BorderThickness.Right, ((size.Height + (2.0 * (_inputMessagePopUp.Padding.Bottom + _inputMessagePopUp.Padding.Top))) + _inputMessagePopUp.BorderThickness.Top) + _inputMessagePopUp.BorderThickness.Bottom);
                    _inputMessagePopUp.Arrange(rect4);
                }
                if (_presenters != null)
                {
                    foreach (KeyValuePair<int, Dictionary<int, InvalidDataPresenter>> pair in _presenters)
                    {
                        foreach (KeyValuePair<int, InvalidDataPresenter> pair2 in pair.Value)
                        {
                            if ((pair2.Value != null) && (ParentViewport.GetCellLayoutModel() != null))
                            {
                                InvalidDataPresenter presenter = pair2.Value;
                                rangeBounds = ParentViewport.GetRangeBounds(new CellRange(pair.Key, pair2.Key, 1, 1));
                                height = rangeBounds.Height;
                                width = rangeBounds.Width;
                                x = rangeBounds.X;
                                y = rangeBounds.Y;
                                if (double.IsInfinity(rangeBounds.Height))
                                {
                                    height = 0.0;
                                    y = 0.0;
                                }
                                if (double.IsInfinity(rangeBounds.Width))
                                {
                                    width = 0.0;
                                    x = 0.0;
                                }
                                presenter.Height = height;
                                presenter.Width = width;
                                Rect rect5 = new Rect(x - 1.0, y - 1.0, width, height);
                                presenter.Arrange(rect5);
                            }
                        }
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        public void CloseInputMessageToolTip()
        {
            if ((_inputMessagePopUp != null) && base.Children.Contains(_inputMessagePopUp))
            {
                base.Children.Remove(_inputMessagePopUp);
                _inputMessagePopUp = null;
                _validator = null;
                base.InvalidateMeasure();
            }
        }

        Grid CreateDataValidationInputMessage(DataValidator validator)
        {
            Grid grid = new Grid
            {
                RowDefinitions = { new RowDefinition() }
            };
            TextBlock block4 = new TextBlock();
            block4.Text = validator.InputMessage;
            TextBlock element = block4;
            element.TextWrapping = TextWrapping.Wrap;
            element.MaxWidth = 240.0;
            grid.Children.Add(element);
            if (!string.IsNullOrEmpty(validator.InputTitle))
            {
                grid.RowDefinitions.Add(new RowDefinition());
                TextBlock block3 = new TextBlock();
                block3.Text = validator.InputTitle;
                block3.FontWeight = FontWeights.Bold;
                TextBlock block2 = block3;
                grid.Children.Add(block2);
                Grid.SetRow(element, 1);
            }
            return grid;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_presenters != null)
            {
                if (ParentViewport.Sheet.HighlightInvalidData)
                {
                    using (Dictionary<int, Dictionary<int, InvalidDataPresenter>>.Enumerator enumerator = _presenters.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            using (Dictionary<int, InvalidDataPresenter>.Enumerator enumerator2 = enumerator.Current.Value.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    enumerator2.Current.Value.Measure(availableSize);
                                }
                                continue;
                            }
                        }
                        goto Label_010D;
                    }
                }
                using (Dictionary<int, Dictionary<int, InvalidDataPresenter>>.Enumerator enumerator3 = _presenters.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        foreach (KeyValuePair<int, InvalidDataPresenter> pair4 in enumerator3.Current.Value)
                        {
                            base.Children.Remove(pair4.Value);
                        }
                    }
                }
                _presenters.Clear();
            }
        Label_010D:
            if (((_inputMessagePopUp != null) && (_inputMessagePopUp.Content != null)) && (_validator != null))
            {
                _inputMessagePopUp.Measure(availableSize);
            }
            if ((_dataValidationListButton != null) && (_dataValidationListButtonInfo != null))
            {
                Rect rangeBounds = ParentViewport.GetRangeBounds(new CellRange(ParentViewport._activeRow, ParentViewport._activeCol, 1, 1));
                double height = rangeBounds.Height;
                double width = rangeBounds.Width;
                if (double.IsInfinity(rangeBounds.Height))
                {
                    height = 0.0;
                }
                if (double.IsInfinity(rangeBounds.Width))
                {
                    width = 0.0;
                }
                Size size = new Size(width, height);
                if (_dataValidationListButtonInfo.Column != _dataValidationListButtonInfo.DisplayColumn)
                {
                    size = new Size(16.0, height);
                }
                _dataValidationListButton.Measure(size);
            }
            return base.MeasureOverride(availableSize);
        }

        public void RemoveDataValidationListButtonInfo()
        {
            if ((_dataValidationListButton != null) && base.Children.Contains(_dataValidationListButton))
            {
                base.Children.Remove(_dataValidationListButton);
            }
            _dataValidationListButton = null;
            _dataValidationListButtonInfo = null;
        }

        public void RemoveInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            if ((_presenters != null) && _presenters.ContainsKey(info.Row))
            {
                Dictionary<int, InvalidDataPresenter> dictionary = _presenters[info.Row];
                if ((dictionary != null) && dictionary.ContainsKey(info.Column))
                {
                    InvalidDataPresenter presenter = dictionary[info.Column];
                    if (dictionary.Remove(info.Column))
                    {
                        base.Children.Remove(presenter);
                    }
                }
            }
        }

        public void ShowInputMessageToolTip(DataValidator validator)
        {
            if ((validator != null) && !string.IsNullOrEmpty(validator.InputMessage))
            {
                _validator = validator;
                _inputMessagePopUp = new DataValidationInputMessagePopUp();
                _inputMessagePopUp.Content = CreateDataValidationInputMessage(validator);
                base.Children.Add(_inputMessagePopUp);
                base.InvalidateMeasure();
            }
        }

        internal CellsPanel ParentViewport { get; set; }
    }
}

