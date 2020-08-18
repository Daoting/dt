#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        class ActiveCellChangingEventArgs : CancelEventArgs
        {
            public ActiveCellChangingEventArgs(int row, int column)
            {
                Row = row;
                Column = column;
            }

            public int Column { get; private set; }

            public int Row { get; private set; }
        }

        internal class ColoredText
        {
            public ColoredText(string text, Windows.UI.Color color)
            {
                Text = text;
                Color = color;
            }

            public Windows.UI.Color Color { get; set; }

            public string Text { get; set; }
        }

        internal enum DragFillDirection
        {
            Left,
            Right,
            Up,
            Down,
            LeftClear,
            UpClear
        }

        internal class EditorManager : IFormulaEditor
        {
            TextBox _editorTextBox;
            string _footer;
            Excel.FormulaSelectionFeature _formulaSelectionFeature;
            string _header;
            bool _isMouseLeftButtonDown;
            string _oldText;
            bool _selectionChanged;
            bool _textChanged;
            DispatcherTimer _timer;

            public EditorManager(Excel.FormulaSelectionFeature formulaSelectionFeature)
            {
                _formulaSelectionFeature = formulaSelectionFeature;
                _formulaSelectionFeature.Excel.EditStarting += new EventHandler<EditCellStartingEventArgs>(OnSheetViewEditStarting);
                _formulaSelectionFeature.Excel.EditEnd += new EventHandler<EditCellEventArgs>(OnSheetViewEditEnd);
                _formulaSelectionFeature.FormulaEditorConnector.FormulaChangedByUI += new EventHandler(OnEditorConnectorFormulaChangedByUI);
            }

            void OnEditorConnectorFormulaChangedByUI(object sender, EventArgs e)
            {
                if ((_editorTextBox != null) && (_formulaSelectionFeature.FormulaEditorConnector.Editor == this))
                {
                    _isMouseLeftButtonDown = false;
                    UpdateBlocks();
                }
            }

            void OnEditorTextBoxGotFocus(object sender, RoutedEventArgs e)
            {
                _formulaSelectionFeature.FormulaEditorConnector.Editor = this;
                _textChanged = true;
                _selectionChanged = true;
                OnTimerTick(this, EventArgs.Empty);
            }

            void OnEditorTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
            {
                if (((e.Key == VirtualKey.F4) && (Excel != null)) && (Excel.EditorConnector.Editor == this))
                {
                    Excel.EditorConnector.ChangeRelative();
                }
            }

            void OnEditorTextBoxSelectionChanged(object sender, RoutedEventArgs e)
            {
                if ((_editorTextBox != null) && _isMouseLeftButtonDown)
                {
                    _formulaSelectionFeature.FormulaEditorConnector.Editor = this;
                    _selectionChanged = true;
                    StartTimer();
                }
            }

            void OnEditorTextBoxTextChanged(object sender, TextChangedEventArgs e)
            {
                if ((_editorTextBox != null) && (_editorTextBox.Text != _oldText))
                {
                    _oldText = _editorTextBox.Text;
                    _textChanged = true;
                    _selectionChanged = true;
                    StartTimer();
                }
            }

            void OnPointerPressed(object sender, PointerRoutedEventArgs e)
            {
                bool isLeftButtonPressed = true;
                if (e.Pointer.PointerDeviceType != PointerDeviceType.Touch)
                {
                    isLeftButtonPressed = e.GetCurrentPoint(_editorTextBox).Properties.IsLeftButtonPressed;
                }
                if (isLeftButtonPressed)
                {
                    ProcessEditorLeftMouseDown();
                }
            }

            void OnSheetViewEditEnd(object sender, EditCellEventArgs e)
            {
                if (_editorTextBox != null)
                {
                    _editorTextBox.KeyDown -= OnEditorTextBoxKeyDown;
                    _editorTextBox.RemoveHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed));
                    _editorTextBox.LostFocus -= OnEditorTextBoxGotFocus;
                    UnWireEvents();
                    _editorTextBox = null;
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer = null;
                    }
                    if (!_formulaSelectionFeature.FormulaEditorConnector.IsInOtherSheet)
                    {
                        _formulaSelectionFeature.EndFormulaSelection();
                    }
                    _formulaSelectionFeature.Items.Clear();
                }
            }

            void OnSheetViewEditStarting(object sender, EditCellStartingEventArgs e)
            {
                if (Excel.CanUserEditFormula)
                {
                    _editorTextBox = _formulaSelectionFeature.Excel.CellEditor as TextBox;
                    if (_editorTextBox != null)
                    {
                        _editorTextBox.KeyDown += OnEditorTextBoxKeyDown;
                        _editorTextBox.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(OnPointerPressed), true);
                        _editorTextBox.LostFocus += OnEditorTextBoxGotFocus;
                        _formulaSelectionFeature.FormulaEditorConnector.Editor = this;
                        WireEvents();
                        if (_formulaSelectionFeature.FormulaEditorConnector.IsInOtherSheet)
                        {
                            OnEditorConnectorFormulaChangedByUI(this, EventArgs.Empty);
                            _formulaSelectionFeature.FormulaEditorConnector.IsInOtherSheet = false;
                        }
                        else
                        {
                            _isMouseLeftButtonDown = true;
                        }
                        _textChanged = true;
                        _selectionChanged = true;
                        OnTimerTick(this, EventArgs.Empty);
                    }
                }
            }

            void OnTimerTick(object sender, object e)
            {
                if (((Excel != null) && (Excel.EditorConnector != null)) && ((Excel.EditorConnector.Editor == this) && (_editorTextBox != null)))
                {
                    string text = _editorTextBox.Text;
                    Match match = new Regex(@"(^\s*=\s*)(.*?)(\s*$)").Match(text);
                    if (!match.Success)
                    {
                        _textChanged = false;
                        _selectionChanged = false;
                        if (_timer != null)
                        {
                            _timer.Stop();
                        }
                        Excel.EndFormulaSelection();
                        return;
                    }
                    if (_textChanged)
                    {
                        Excel.BeginFormulaSelection(null);
                        _header = match.Groups[1].Value;
                        _footer = match.Groups[3].Value;
                        Excel.EditorConnector.OnFormulaTextChanged(match.Groups[2].Value);
                    }
                    if (_selectionChanged)
                    {
                        _selectionChanged = false;
                        int selectionStart = _editorTextBox.SelectionStart;
                        int end = selectionStart + _editorTextBox.SelectionLength;
                        if (_header != null)
                        {
                            selectionStart -= _header.Length;
                            end -= _header.Length;
                        }
                        Excel.EditorConnector.OnCursorPositionChanged(selectionStart, end);
                    }
                    if (_textChanged)
                    {
                        _textChanged = false;
                        UpdateBlocks();
                    }
                }
                if (_timer != null)
                {
                    _timer.Stop();
                }
            }

            void ProcessEditorLeftMouseDown()
            {
                _isMouseLeftButtonDown = true;
                _selectionChanged = true;
                _formulaSelectionFeature.FormulaEditorConnector.Editor = this;
                StartTimer();
            }

            void StartTimer()
            {
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                    _timer.Tick += OnTimerTick;
                }
                _timer.Stop();
                _timer.Start();
            }

            void UnWireEvents()
            {
                if (_editorTextBox != null)
                {
                    _editorTextBox.TextChanged -= OnEditorTextBoxTextChanged;
                    _editorTextBox.SelectionChanged -= OnEditorTextBoxSelectionChanged;
                }
            }

            void UpdateBlocks()
            {
                UnWireEvents();
                try
                {
                    IList<Excel.ColoredText> coloredText = Excel.EditorConnector.GetColoredText(false);
                    StringBuilder builder = new StringBuilder();
                    if (string.IsNullOrEmpty(_header) && (coloredText.Count > 0))
                    {
                        _header = "=";
                    }
                    if (!string.IsNullOrEmpty(_header))
                    {
                        builder.Append(_header);
                    }
                    foreach (Excel.ColoredText text in coloredText)
                    {
                        builder.Append(text.Text);
                    }
                    if (!string.IsNullOrEmpty(_footer))
                    {
                        builder.Append(_footer);
                    }
                    _editorTextBox.Text = builder.ToString();
                    _oldText = _editorTextBox.Text;
                    int cursorPositionStart = Excel.EditorConnector.GetCursorPositionStart();
                    if (_header != null)
                    {
                        cursorPositionStart += _header.Length;
                    }
                    _editorTextBox.Select(cursorPositionStart, 0);
                }
                finally
                {
                    WireEvents();
                }
            }

            void WireEvents()
            {
                if (_editorTextBox != null)
                {
                    _editorTextBox.TextChanged -= OnEditorTextBoxTextChanged;
                    _editorTextBox.SelectionChanged -= OnEditorTextBoxSelectionChanged;
                    _editorTextBox.TextChanged += OnEditorTextBoxTextChanged;
                    _editorTextBox.SelectionChanged += OnEditorTextBoxSelectionChanged;
                }
            }

            public bool IsAbsolute { get; set; }

            Excel Excel
            {
                get { return _formulaSelectionFeature.Excel; }
            }
        }

        internal class FormulaEditorConnector
        {
            bool _activateEditor = true;
            int _colorIndex = -1;
            Windows.UI.Color[] _colors = new Windows.UI.Color[] { Windows.UI.Color.FromArgb(0xff, 0, 0, 0xff), Windows.UI.Color.FromArgb(0xff, 0, 0x80, 0), Windows.UI.Color.FromArgb(0xff, 0x99, 0, 0xcc), Windows.UI.Color.FromArgb(0xff, 0x80, 0, 0), Windows.UI.Color.FromArgb(0xff, 0, 0xcc, 0x33), Windows.UI.Color.FromArgb(0xff, 0xff, 0x66, 0), Windows.UI.Color.FromArgb(0xff, 0xcc, 0, 0x99) };
            int _columnIndex;
            int _cursorPositionEnd;
            int _cursorPositionStart;
            IFormulaEditor _editor;
            IList<Excel.FormulaExpression> _footer;
            IList<Excel.FormulaExpression> _formulaExpressions;
            Excel.FormulaSelectionFeature _formulaSelectionFeature;
            IList<Excel.FormulaExpression> _header;
            IList<Excel.FormulaExpression> _middle;
            int _rowIndex;
            int _sheetIndex;
            bool _splited;

            public event EventHandler FormulaChangedByUI;

            public FormulaEditorConnector(Excel.FormulaSelectionFeature formulaSelectionFeature)
            {
                _formulaSelectionFeature = formulaSelectionFeature;
                _formulaSelectionFeature.ItemAdded += new EventHandler<FormulaSelectionItemEventArgs>(OnFormulaSelectionFeatureItemAdded);
                _formulaSelectionFeature.ItemRemoved += new EventHandler<FormulaSelectionItemEventArgs>(OnFormulaSelectionFeatureItemRemoved);
            }

            bool CanSelectFormulaByUI(IList<Excel.FormulaExpression> expressionList, int cursorStart, int cursorEnd)
            {
                if (((cursorStart < 0) || (cursorEnd < 0)) || (expressionList == null))
                {
                    return false;
                }
                int num = 0;
                for (int i = 0; i < expressionList.Count; i++)
                {
                    Excel.FormulaExpression expression = expressionList[i];
                    int num3 = num + expression.Text.Length;
                    if (((cursorStart > num) && (cursorStart <= num3)) && ((expression.Range != null) || (expression.Text.EndsWith(")") && (cursorStart >= num3))))
                    {
                        return false;
                    }
                    num = num3;
                }
                return true;
            }

            public void ChangeRelative()
            {
                if (_splited)
                {
                    if ((_middle != null) && (_middle.Count > 0))
                    {
                        if (_middle[0].StartColumnRelative && _middle[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression in _middle)
                            {
                                expression.StartColumnRelative = expression.EndColumnRelative = false;
                                expression.StartRowRelative = expression.EndRowRelative = false;
                                expression.UpdateText();
                            }
                        }
                        else if (!_middle[0].StartColumnRelative && !_middle[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression2 in _middle)
                            {
                                expression2.StartColumnRelative = expression2.EndColumnRelative = true;
                                expression2.StartRowRelative = expression2.EndRowRelative = false;
                                expression2.UpdateText();
                            }
                        }
                        else if (_middle[0].StartColumnRelative && !_middle[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression3 in _middle)
                            {
                                expression3.StartColumnRelative = expression3.EndColumnRelative = false;
                                expression3.StartRowRelative = expression3.EndRowRelative = true;
                                expression3.UpdateText();
                            }
                        }
                        else if (!_middle[0].StartColumnRelative && _middle[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression4 in _middle)
                            {
                                expression4.StartColumnRelative = expression4.EndColumnRelative = true;
                                expression4.StartRowRelative = expression4.EndRowRelative = true;
                                expression4.UpdateText();
                            }
                        }
                        OnFormulaChangedByUI();
                    }
                }
                else if ((_formulaExpressions != null) && (_formulaExpressions.Count > 0))
                {
                    List<Excel.FormulaExpression> list = new List<Excel.FormulaExpression>();
                    int num = _cursorPositionStart;
                    int num2 = _cursorPositionEnd;
                    int num3 = 0;
                    foreach (Excel.FormulaExpression expression5 in _formulaExpressions)
                    {
                        int num4 = num3;
                        num3 += expression5.Text.Length;
                        if ((num4 <= _cursorPositionEnd) && (((num3 >= _cursorPositionStart) && (_cursorPositionStart == _cursorPositionEnd)) || ((num3 > _cursorPositionStart) && (_cursorPositionStart != _cursorPositionEnd))))
                        {
                            if (list.Count == 0)
                            {
                                num = num4;
                            }
                            num2 = num3;
                            list.Add(expression5);
                        }
                    }
                    if (list.Count > 0)
                    {
                        if (list[0].StartColumnRelative && list[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression6 in list)
                            {
                                expression6.StartColumnRelative = expression6.EndColumnRelative = false;
                                expression6.StartRowRelative = expression6.EndRowRelative = false;
                                expression6.UpdateText();
                            }
                        }
                        else if (!list[0].StartColumnRelative && !list[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression7 in list)
                            {
                                expression7.StartColumnRelative = expression7.EndColumnRelative = true;
                                expression7.StartRowRelative = expression7.EndRowRelative = false;
                                expression7.UpdateText();
                            }
                        }
                        else if (list[0].StartColumnRelative && !list[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression8 in list)
                            {
                                expression8.StartColumnRelative = expression8.EndColumnRelative = false;
                                expression8.StartRowRelative = expression8.EndRowRelative = true;
                                expression8.UpdateText();
                            }
                        }
                        else if (!list[0].StartColumnRelative && list[0].StartRowRelative)
                        {
                            foreach (Excel.FormulaExpression expression9 in list)
                            {
                                expression9.StartColumnRelative = expression9.EndColumnRelative = true;
                                expression9.StartRowRelative = expression9.EndRowRelative = true;
                                expression9.UpdateText();
                            }
                        }
                        num2 = num;
                        foreach (Excel.FormulaExpression expression10 in list)
                        {
                            num2 += expression10.Text.Length;
                        }
                        num2--;
                        _cursorPositionStart = num;
                        _cursorPositionEnd = num2;
                        OnFormulaChangedByUI();
                    }
                }
            }

            public void ClearFlickingItems()
            {
                _formulaSelectionFeature.ClearFlickingSelection();
            }

            Excel.FormulaExpression CreateFormulaExpression(CalcExpression expression, string expressionText, int baseRow, int baseColumn)
            {
                CalcRangeExpression expression2 = expression as CalcRangeExpression;
                CalcCellExpression expression3 = expression as CalcCellExpression;
                CalcExternalCellExpression expression4 = expression as CalcExternalCellExpression;
                CalcExternalRangeExpression expression5 = expression as CalcExternalRangeExpression;
                CalcNameExpression expression6 = expression as CalcNameExpression;
                CalcExternalNameExpression expression7 = expression as CalcExternalNameExpression;
                if (expression2 != null)
                {
                    CalcRangeIdentity id = expression2.GetId(baseRow, baseColumn) as CalcRangeIdentity;
                    return new Excel.FormulaExpression(this, new CellRange(id.RowIndex, id.ColumnIndex, id.RowCount, id.ColumnCount), expressionText, false, null) { StartRowRelative = expression2.StartRowRelative, StartColumnRelative = expression2.StartColumnRelative, EndRowRelative = expression2.EndRowRelative, EndColumnRelative = expression2.EndColumnRelative };
                }
                if (expression3 != null)
                {
                    CalcCellIdentity identity2 = expression3.GetId(baseRow, baseColumn) as CalcCellIdentity;
                    int rowCount = 1;
                    int columnCount = 1;
                    Worksheet sheet = _formulaSelectionFeature.Excel.ActiveSheet;
                    CellRange range2 = sheet.SpanModel.Find(identity2.RowIndex, identity2.ColumnIndex);
                    if (((range2 != null) && (range2.Row == identity2.RowIndex)) && (range2.Column == identity2.ColumnIndex))
                    {
                        rowCount = range2.RowCount;
                        columnCount = range2.ColumnCount;
                    }
                    return new Excel.FormulaExpression(this, new CellRange(identity2.RowIndex, identity2.ColumnIndex, rowCount, columnCount), expressionText, false, null) { StartRowRelative = expression3.RowRelative, StartColumnRelative = expression3.ColumnRelative, EndRowRelative = expression3.RowRelative, EndColumnRelative = expression3.ColumnRelative };
                }
                if (expression5 != null)
                {
                    CalcExternalRangeIdentity identity3 = expression5.GetId(baseRow, baseColumn) as CalcExternalRangeIdentity;
                    return new Excel.FormulaExpression(this, new CellRange(identity3.RowIndex, identity3.ColumnIndex, identity3.RowCount, identity3.ColumnCount), expressionText, false, expression5.Source as Worksheet) { StartRowRelative = expression5.StartRowRelative, StartColumnRelative = expression5.StartColumnRelative, EndRowRelative = expression5.EndRowRelative, EndColumnRelative = expression5.EndColumnRelative };
                }
                if (expression4 != null)
                {
                    CalcExternalCellIdentity identity4 = expression4.GetId(baseRow, baseColumn) as CalcExternalCellIdentity;
                    int num3 = 1;
                    int num4 = 1;
                    var source = expression4.Source as Worksheet;
                    CellRange range5 = source.SpanModel.Find(identity4.RowIndex, identity4.ColumnIndex);
                    if (((range5 != null) && (range5.Row == identity4.RowIndex)) && (range5.Column == identity4.ColumnIndex))
                    {
                        num3 = range5.RowCount;
                        num4 = range5.ColumnCount;
                    }
                    return new Excel.FormulaExpression(this, new CellRange(identity4.RowIndex, identity4.ColumnIndex, num3, num4), expressionText, false, source) { StartRowRelative = expression4.RowRelative, StartColumnRelative = expression4.ColumnRelative, EndRowRelative = expression4.RowRelative, EndColumnRelative = expression4.ColumnRelative };
                }
                if (expression6 != null)
                {
                    NameInfo customName = _formulaSelectionFeature.Excel.ActiveSheet.GetCustomName(expression6.Name);
                    if (customName == null)
                    {
                        customName = _formulaSelectionFeature.Excel.ActiveSheet.Workbook.GetCustomName(expression6.Name);
                    }
                    if (customName != null)
                    {
                        CalcReferenceExpression reference = customName.Expression as CalcReferenceExpression;
                        if (reference != null)
                        {
                            CellRange rangeFromExpression = Dt.Cells.Data.CellRangUtility.GetRangeFromExpression(reference);
                            Worksheet sheet = null;
                            if (reference is CalcExternalExpression)
                            {
                                sheet = (reference as CalcExternalExpression).Source as Worksheet;
                            }
                            return new Excel.FormulaExpression(this, rangeFromExpression, expressionText, true, sheet);
                        }
                    }
                    return new Excel.FormulaExpression(this, expressionText);
                }
                if (expression7 != null)
                {
                    var worksheet3 = expression7.Source as Worksheet;
                    if (worksheet3 != null)
                    {
                        NameInfo info2 = worksheet3.GetCustomName(expression7.Name);
                        if (info2 != null)
                        {
                            CalcReferenceExpression expression14 = info2.Expression as CalcReferenceExpression;
                            if (expression14 != null)
                            {
                                CellRange range = Dt.Cells.Data.CellRangUtility.GetRangeFromExpression(expression14);
                                Worksheet worksheet4 = null;
                                if (expression14 is CalcExternalExpression)
                                {
                                    worksheet4 = (expression14 as CalcExternalExpression).Source as Worksheet;
                                }
                                return new Excel.FormulaExpression(this, range, expressionText, true, worksheet4);
                            }
                        }
                    }
                }
                return new Excel.FormulaExpression(this, expressionText);
            }

            static CalcRangeExpression CreateRangeExpressionByCount(int row, int column, int rowCount, int columnCount, bool startRowRelative = false, bool startColumnRelative = false, bool endRowRelative = false, bool endColumnRelative = false)
            {
                if ((rowCount == -1) && (columnCount == -1))
                {
                    return new CalcRangeExpression();
                }
                if (columnCount == -1)
                {
                    return new CalcRangeExpression(row, (row + rowCount) - 1, startRowRelative, endRowRelative, true);
                }
                if (rowCount == -1)
                {
                    return new CalcRangeExpression(column, (column + columnCount) - 1, startColumnRelative, endColumnRelative, false);
                }
                return new CalcRangeExpression(row, column, (row + rowCount) - 1, (column + columnCount) - 1, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
            }

            internal string FindNameRange(CellRange range)
            {
                foreach (string str in _formulaSelectionFeature.Excel.ActiveSheet.CustomNames)
                {
                    CalcReferenceExpression reference = _formulaSelectionFeature.Excel.ActiveSheet.GetCustomName(str).Expression as CalcReferenceExpression;
                    if (reference != null)
                    {
                        CellRange rangeFromExpression = Dt.Cells.Data.CellRangUtility.GetRangeFromExpression(reference);
                        if ((rangeFromExpression != null) && rangeFromExpression.Equals(range))
                        {
                            return str;
                        }
                    }
                }
                foreach (string str2 in _formulaSelectionFeature.Excel.ActiveSheet.Workbook.CustomNames)
                {
                    CalcReferenceExpression expression = _formulaSelectionFeature.Excel.ActiveSheet.Workbook.GetCustomName(str2).Expression as CalcReferenceExpression;
                    if (expression != null)
                    {
                        CalcExternalExpression expression3 = expression as CalcExternalExpression;
                        if ((expression3 == null) || (expression3.Source == _formulaSelectionFeature.Excel.ActiveSheet))
                        {
                            CellRange range3 = Dt.Cells.Data.CellRangUtility.GetRangeFromExpression(expression);
                            if ((range3 != null) && range3.Equals(range))
                            {
                                return str2;
                            }
                        }
                    }
                }
                return null;
            }

            public IList<Excel.ColoredText> GetColoredText(bool includeSheetName = false)
            {
                List<Excel.ColoredText> list = new List<Excel.ColoredText>();
                foreach (Excel.FormulaExpression expression in GetMergedExpressionList())
                {
                    if (includeSheetName && (expression.Sheet == null))
                    {
                        if (_formulaSelectionFeature.IsInOtherSheet)
                        {
                            expression.Sheet = _formulaSelectionFeature.Excel.EditorInfo.Sheet;
                        }
                        else
                        {
                            expression.Sheet = _formulaSelectionFeature.Excel.ActiveSheet;
                        }
                    }
                    list.Add(new Excel.ColoredText(expression.Text, expression.Color));
                }
                return (IList<Excel.ColoredText>)list;
            }

            public int GetCursorPositionEnd()
            {
                if (!_splited)
                {
                    return _cursorPositionEnd;
                }
                int num = 0;
                if (_header != null)
                {
                    foreach (Excel.FormulaExpression expression in _header)
                    {
                        num += expression.Text.Length;
                    }
                }
                if (_middle != null)
                {
                    foreach (Excel.FormulaExpression expression2 in _middle)
                    {
                        num += expression2.Text.Length;
                    }
                }
                return num;
            }

            public int GetCursorPositionStart()
            {
                if (!_splited)
                {
                    return _cursorPositionStart;
                }
                int num = 0;
                if (_header != null)
                {
                    foreach (Excel.FormulaExpression expression in _header)
                    {
                        num += expression.Text.Length;
                    }
                }
                if (_middle != null)
                {
                    foreach (Excel.FormulaExpression expression2 in _middle)
                    {
                        num += expression2.Text.Length;
                    }
                }
                return num;
            }

            IList<Excel.FormulaExpression> GetMergedExpressionList()
            {
                List<Excel.FormulaExpression> list = new List<Excel.FormulaExpression>();
                if (!_splited)
                {
                    if (_formulaExpressions != null)
                    {
                        list.AddRange((IEnumerable<Excel.FormulaExpression>)_formulaExpressions);
                    }
                }
                else
                {
                    if (_header != null)
                    {
                        list.AddRange((IEnumerable<Excel.FormulaExpression>)_header);
                    }
                    if ((((_header != null) && (_header.Count > 0)) && ((_middle != null) && (_middle.Count > 0))) && ((_header[_header.Count - 1].Range != null) && (_middle[0].Range != null)))
                    {
                        char ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                        list.Add(new Excel.FormulaExpression(this, ((char)ch).ToString()));
                    }
                    if (_middle != null)
                    {
                        list.AddRange((IEnumerable<Excel.FormulaExpression>)_middle);
                    }
                    if ((((_middle != null) && (_middle.Count > 0)) && ((_footer != null) && (_footer.Count > 0))) && ((_middle[_middle.Count - 1].Range != null) && (_footer[0].Range != null)))
                    {
                        char ch2 = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                        list.Add(new Excel.FormulaExpression(this, ((char)ch2).ToString()));
                    }
                    if (_footer != null)
                    {
                        list.AddRange((IEnumerable<Excel.FormulaExpression>)_footer);
                    }
                }
                return (IList<Excel.FormulaExpression>)list;
            }

            public string GetText()
            {
                StringBuilder builder = new StringBuilder();
                foreach (Excel.ColoredText text in GetColoredText(false))
                {
                    builder.Append(text.Text);
                }
                return builder.ToString();
            }

            Windows.UI.Color NewColor()
            {
                _colorIndex = (_colorIndex + 1) % 7;
                return _colors[_colorIndex];
            }

            public void OnCursorPositionChanged(int start, int end)
            {
                _cursorPositionStart = start;
                _cursorPositionEnd = end;
                if (_splited)
                {
                    _formulaExpressions = GetMergedExpressionList();
                    _splited = false;
                    using (IEnumerator<FormulaSelectionItem> enumerator = _formulaSelectionFeature.Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.IsFlickering = false;
                        }
                    }
                }
                if (CanSelectFormulaByUI(_formulaExpressions, start, end))
                {
                    _formulaSelectionFeature.CanSelectFormula = true;
                }
                else
                {
                    _formulaSelectionFeature.CanSelectFormula = false;
                }
            }

            internal void OnFormulaChangedByUI()
            {
                EventHandler formulaChangedByUI = FormulaChangedByUI;
                if (formulaChangedByUI != null)
                {
                    formulaChangedByUI(this, EventArgs.Empty);
                }
            }

            void OnFormulaSelectionFeatureItemAdded(object sender, FormulaSelectionItemEventArgs e)
            {
                if (!_splited)
                {
                    _splited = true;
                    _header = (IList<Excel.FormulaExpression>)new List<Excel.FormulaExpression>();
                    _middle = (IList<Excel.FormulaExpression>)new List<Excel.FormulaExpression>();
                    _footer = (IList<Excel.FormulaExpression>)new List<Excel.FormulaExpression>();
                    if (_formulaExpressions != null)
                    {
                        int num = 0;
                        foreach (Excel.FormulaExpression expression in _formulaExpressions)
                        {
                            if ((num + expression.Text.Length) <= _cursorPositionStart)
                            {
                                _header.Add(expression);
                            }
                            else if (num >= _cursorPositionEnd)
                            {
                                _footer.Add(expression);
                            }
                            else if (_cursorPositionStart == _cursorPositionEnd)
                            {
                                _middle.Add(expression);
                            }
                            else
                            {
                                _formulaSelectionFeature.Items.Remove(expression.SelectionItem);
                            }
                            num += expression.Text.Length;
                        }
                    }
                }
                Excel.FormulaExpression expression2 = new Excel.FormulaExpression(this, e.Item.Range, string.Empty, false, null)
                {
                    SelectionItem = e.Item
                };
                if (_formulaSelectionFeature.Excel.ActiveSheet != _formulaSelectionFeature.Excel.EditorInfo.Sheet)
                {
                    expression2.Sheet = _formulaSelectionFeature.Excel.ActiveSheet;
                }
                if (_middle.Count > 0)
                {
                    char ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                    if (_middle[_middle.Count - 1].Text != ((char)ch).ToString())
                    {
                        char ch2 = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                        _middle.Add(new Excel.FormulaExpression(this, ((char)ch2).ToString()));
                    }
                    expression2.StartRowRelative = _middle[0].StartRowRelative;
                    expression2.StartColumnRelative = _middle[0].StartColumnRelative;
                    expression2.EndRowRelative = _middle[0].EndRowRelative;
                    expression2.EndColumnRelative = _middle[0].EndColumnRelative;
                }
                else if ((Editor != null) && Editor.IsAbsolute)
                {
                    expression2.StartRowRelative = false;
                    expression2.StartColumnRelative = false;
                    expression2.EndRowRelative = false;
                    expression2.EndColumnRelative = false;
                }
                expression2.UpdateText();
                _middle.Add(expression2);
                UpdateColors();
                OnFormulaChangedByUI();
            }

            void OnFormulaSelectionFeatureItemRemoved(object sender, FormulaSelectionItemEventArgs e)
            {
                List<Excel.FormulaExpression> list = new List<Excel.FormulaExpression>();
                for (int i = 0; i < _middle.Count; i++)
                {
                    Excel.FormulaExpression expression = _middle[i];
                    if (expression.SelectionItem == e.Item)
                    {
                        list.Add(expression);
                        if (_middle.Count > (i + 1))
                        {
                            char ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                            if (_middle[i + 1].Text == ((char)ch).ToString())
                            {
                                list.Add(_middle[i + 1]);
                            }
                        }
                        break;
                    }
                }
                foreach (Excel.FormulaExpression expression2 in list)
                {
                    _middle.Remove(expression2);
                }
                UpdateColors();
                OnFormulaChangedByUI();
            }

            public void OnFormulaTextChanged(string formulaText)
            {
                if (formulaText == null)
                {
                    formulaText = string.Empty;
                }
                _formulaExpressions = Parse(formulaText);
                _cursorPositionStart = _cursorPositionEnd = formulaText.Length;
                _splited = false;
                UpdateSelectionItemsForCurrentSheet();
                UpdateColors();
            }

            IList<Excel.FormulaExpression> Parse(string text)
            {
                List<Excel.FormulaExpression> list = new List<Excel.FormulaExpression>();
                if (!string.IsNullOrEmpty(text))
                {
                    bool flag = _formulaSelectionFeature.Excel.EditorInfo.Sheet.ReferenceStyle == ReferenceStyle.R1C1;
                    int activeRowIndex = _formulaSelectionFeature.Excel.ActiveSheet.ActiveRowIndex;
                    int activeColumnIndex = _formulaSelectionFeature.Excel.ActiveSheet.ActiveColumnIndex;
                    WorkbookParserContext context = new WorkbookParserContext(_formulaSelectionFeature.Excel.ActiveSheet.Workbook, flag, activeRowIndex, activeColumnIndex, CultureInfo.CurrentCulture);
                    CalcParser parser = new CalcParser();
                    List<ExpressionInfo> list2 = new List<ExpressionInfo>();
                    try
                    {
                        list2 = parser.ParseReferenceExpressionInfos(text, context);
                    }
                    catch
                    {
                    }
                    if (list2.Count == 0)
                    {
                        Match match = new Regex(@"^(.*?\()(.*)(\))(\s*)$").Match(text);
                        if (match.Success)
                        {
                            for (int i = 1; i < 5; i++)
                            {
                                if (!string.IsNullOrEmpty(match.Groups[i].Value))
                                {
                                    list.Add(new Excel.FormulaExpression(this, match.Groups[i].Value));
                                }
                            }
                        }
                        else
                        {
                            list.Add(new Excel.FormulaExpression(this, text));
                        }
                    }
                    else
                    {
                        int startIndex = 0;
                        foreach (ExpressionInfo info in list2)
                        {
                            if (info.StartIndex > startIndex)
                            {
                                string str = text.Substring(startIndex, info.StartIndex - startIndex);
                                foreach (string str2 in Split(str))
                                {
                                    list.Add(new Excel.FormulaExpression(this, str2));
                                }
                            }
                            startIndex = info.EndIndex + 1;
                            string expression = text.Substring(info.StartIndex, (info.EndIndex - info.StartIndex) + 1);
                            foreach (string str4 in Split(expression))
                            {
                                if (string.IsNullOrEmpty(str4))
                                {
                                    char ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                                    if (str4 != ((char)ch).ToString())
                                    {
                                        list.Add(new Excel.FormulaExpression(this, str4));
                                        continue;
                                    }
                                }
                                list.Add(CreateFormulaExpression(info.Expression, str4, activeRowIndex, activeColumnIndex));
                            }
                        }
                        if (startIndex < text.Length)
                        {
                            string str5 = text.Substring(startIndex, text.Length - startIndex);
                            foreach (string str6 in Split(str5))
                            {
                                list.Add(new Excel.FormulaExpression(this, str6));
                            }
                        }
                    }
                }
                return (IList<Excel.FormulaExpression>)list;
            }

            internal string RangeToFormula(Worksheet worksheet, CellRange range, bool startRowRelative = true, bool startColumnRelative = true, bool endRowRelative = true, bool endColumnRelative = true)
            {
                if (worksheet == null)
                {
                    worksheet = _formulaSelectionFeature.Excel.EditorInfo.Sheet;
                }
                bool flag = false;
                if ((range.RowCount == 1) && (range.ColumnCount == 1))
                {
                    flag = true;
                }
                else
                {
                    foreach (object obj2 in worksheet.SpanModel)
                    {
                        if (range.Equals(obj2))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                int baseRow = 0;
                int baseColumn = 0;
                if (startRowRelative || endRowRelative)
                {
                    baseRow = worksheet.ActiveRowIndex;
                }
                if (startColumnRelative || endColumnRelative)
                {
                    baseColumn = worksheet.ActiveColumnIndex;
                }
                if (flag)
                {
                    CalcCellExpression expression = new CalcCellExpression(range.Row - baseRow, range.Column - baseColumn, startRowRelative, startColumnRelative);
                    return ((ICalcEvaluator)_formulaSelectionFeature.Excel.EditorInfo.Sheet).Expression2Formula(expression, baseRow, baseColumn);
                }
                CalcRangeExpression expression2 = CreateRangeExpressionByCount(range.Row - baseRow, range.Column - baseColumn, range.RowCount, range.ColumnCount, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
                return ((ICalcEvaluator)_formulaSelectionFeature.Excel.EditorInfo.Sheet).Expression2Formula(expression2, baseRow, baseColumn);
            }

            void ResetColor()
            {
                _colorIndex = -1;
            }

            IList<string> Split(string expression)
            {
                if (string.IsNullOrEmpty(expression))
                {
                    return (IList<string>)new List<string>();
                }
                List<string> list = new List<string>();
                int length = -1;
                int num2 = -1;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] != ' ')
                    {
                        length = i;
                        break;
                    }
                }
                for (int j = expression.Length - 1; j >= 0; j--)
                {
                    if (expression[j] != ' ')
                    {
                        num2 = j;
                        break;
                    }
                }
                if (length == -1)
                {
                    list.Add(expression);
                    return (IList<string>)list;
                }
                if (length != 0)
                {
                    list.Add(expression.Substring(0, length));
                }
                char ch = CultureInfo.CurrentCulture.TextInfo.ListSeparator[0];
                if (expression.Substring(0, 1) == ((char)ch).ToString())
                {
                    list.Add(expression.Substring(0, 1));
                    length++;
                }
                if (num2 >= length)
                {
                    list.Add(expression.Substring(length, (num2 - length) + 1));
                }
                if (num2 != (expression.Length - 1))
                {
                    list.Add(expression.Substring(num2 + 1, (expression.Length - num2) - 1));
                }
                return (IList<string>)list;
            }

            internal void UpdateColors()
            {
                ResetColor();
                Dictionary<CellRange, Windows.UI.Color> dictionary = new Dictionary<CellRange, Windows.UI.Color>();
                foreach (Excel.FormulaExpression expression in GetMergedExpressionList())
                {
                    if (expression.Range != null)
                    {
                        Windows.UI.Color color;
                        if (!dictionary.TryGetValue(expression.Range, out color))
                        {
                            color = NewColor();
                            dictionary.Add(expression.Range, color);
                        }
                        expression.Color = color;
                    }
                }
            }

            internal void UpdateCursorPosition(Excel.FormulaExpression expression)
            {
                int num = 0;
                foreach (Excel.FormulaExpression expression2 in GetMergedExpressionList())
                {
                    num += expression2.Text.Length;
                    if (expression2 == expression)
                    {
                        _cursorPositionStart = _cursorPositionEnd = num;
                        break;
                    }
                }
            }

            internal void UpdateSelectionItemsForCurrentSheet()
            {
                _formulaSelectionFeature.Items.Clear();
                foreach (Excel.FormulaExpression expression in GetMergedExpressionList())
                {
                    if ((expression.Range != null) && (((expression.Sheet == null) && !IsInOtherSheet) || (expression.Sheet == _formulaSelectionFeature.Excel.ActiveSheet.Workbook.ActiveSheet)))
                    {
                        if (expression.SelectionItem == null)
                        {
                            expression.SelectionItem = new FormulaSelectionItem(expression.Range, false);
                        }
                        _formulaSelectionFeature.Items.Add(expression.SelectionItem);
                    }
                }
            }

            internal bool ActivateEditor
            {
                get { return _activateEditor; }
                set { _activateEditor = value; }
            }

            internal int ColumnIndex
            {
                get { return _columnIndex; }
                set { _columnIndex = value; }
            }

            internal IFormulaEditor Editor
            {
                get { return _editor; }
                set { _editor = value; }
            }

            public bool IsFormulaSelectionBegined
            {
                get { return _formulaSelectionFeature.IsSelectionBegined; }
            }

            internal bool IsInOtherSheet
            {
                get { return _formulaSelectionFeature.IsInOtherSheet; }
                set { _formulaSelectionFeature.IsInOtherSheet = value; }
            }

            public bool IsRelative { get; set; }

            internal int RowIndex
            {
                get { return _rowIndex; }
                set { _rowIndex = value; }
            }

            internal int SheetIndex
            {
                get { return _sheetIndex; }
                set { _sheetIndex = value; }
            }

            class WorkbookParserContext : CalcParserContext
            {
                Workbook _context;

                public WorkbookParserContext(Workbook context, bool useR1C1 = false, int baseRowIndex = 0, int baseColumnIndex = 0, CultureInfo culture = null)
                    : base(useR1C1, baseRowIndex, baseColumnIndex, culture)
                {
                    _context = context;
                }

                public override ICalcSource GetExternalSource(string workbookName, string worksheetName)
                {
                    if (_context != null)
                    {
                        return _context.Sheets[worksheetName];
                    }
                    return base.GetExternalSource(workbookName, worksheetName);
                }

                public override string GetExternalSourceToken(ICalcSource source)
                {
                    var worksheet = source as Worksheet;
                    if (worksheet != null)
                    {
                        return worksheet.Name;
                    }
                    return base.GetExternalSourceToken(source);
                }
            }
        }

        internal class FormulaExpression
        {
            Windows.UI.Color _color;
            bool _endColumnRelative;
            bool _endRowRelative;
            Excel.FormulaEditorConnector _formulaEditorConnector;
            FormulaSelectionItem _formulaSelectionItem;
            bool _isNameExpression;
            CellRange _range;
            Worksheet _sheet;
            bool _startColumnRelative;
            bool _startRowRelative;
            string _text;

            public FormulaExpression(Excel.FormulaEditorConnector connector, string text)
            {
                _color = Colors.Black;
                _startRowRelative = true;
                _startColumnRelative = true;
                _endRowRelative = true;
                _endColumnRelative = true;
                _formulaEditorConnector = connector;
                _text = text;
            }

            public FormulaExpression(Excel.FormulaEditorConnector connector, CellRange range, string oldText, bool isNameExpression = false, Worksheet sheet = null)
            {
                _color = Colors.Black;
                _startRowRelative = true;
                _startColumnRelative = true;
                _endRowRelative = true;
                _endColumnRelative = true;
                _formulaEditorConnector = connector;
                _range = range;
                _text = oldText;
                _isNameExpression = isNameExpression;
                _sheet = sheet;
            }

            void OnFormulaSelectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == "Range")
                {
                    _range = _formulaSelectionItem.Range;
                    _text = _formulaEditorConnector.FindNameRange(_range);
                    IsNameExpression = !string.IsNullOrEmpty(_text);
                    UpdateText();
                    _formulaEditorConnector.UpdateColors();
                    _formulaEditorConnector.OnFormulaChangedByUI();
                    _formulaEditorConnector.UpdateCursorPosition(this);
                }
            }

            public void UpdateText()
            {
                if ((_range != null) && !_isNameExpression)
                {
                    _text = _formulaEditorConnector.RangeToFormula(_sheet, _range, StartRowRelative, StartColumnRelative, EndRowRelative, EndColumnRelative);
                    if ((_sheet != null) && !string.IsNullOrEmpty(_sheet.Name))
                    {
                        bool flag = false;
                        for (int i = 0; i < _sheet.Name.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(_sheet.Name.Substring(i, 1)))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            _text = "'" + _sheet.Name + "'!" + _text;
                        }
                        else
                        {
                            _text = _sheet.Name + "!" + _text;
                        }
                    }
                }
            }

            public Windows.UI.Color Color
            {
                get { return _color; }
                set
                {
                    if (_color != value)
                    {
                        _color = value;
                        if (_formulaSelectionItem != null)
                        {
                            _formulaSelectionItem.Color = _color;
                        }
                    }
                }
            }

            public bool EndColumnRelative
            {
                get { return _endColumnRelative; }
                set { _endColumnRelative = value; }
            }

            public bool EndRowRelative
            {
                get { return _endRowRelative; }
                set { _endRowRelative = value; }
            }

            public bool IsNameExpression
            {
                get { return _isNameExpression; }
                set
                {
                    if (_isNameExpression != value)
                    {
                        _isNameExpression = value;
                        SelectionItem.CanChangeBoundsByUI = !value;
                    }
                }
            }

            public CellRange Range
            {
                get { return _range; }
            }

            public FormulaSelectionItem SelectionItem
            {
                get { return _formulaSelectionItem; }
                set
                {
                    if (_formulaSelectionItem != value)
                    {
                        if (_formulaSelectionItem != null)
                        {
                            _formulaSelectionItem.Expression = null;
                            _formulaSelectionItem.PropertyChanged -= new PropertyChangedEventHandler(OnFormulaSelectionItemPropertyChanged);
                        }
                        _formulaSelectionItem = value;
                        if (_formulaSelectionItem != null)
                        {
                            _formulaSelectionItem.CanChangeBoundsByUI = !_isNameExpression;
                            _formulaSelectionItem.Range = Range;
                            _formulaSelectionItem.Color = Color;
                            _formulaSelectionItem.Expression = this;
                            _formulaSelectionItem.PropertyChanged += new PropertyChangedEventHandler(OnFormulaSelectionItemPropertyChanged);
                        }
                    }
                }
            }

            public Worksheet Sheet
            {
                get { return _sheet; }
                set
                {
                    if (_sheet != value)
                    {
                        _sheet = value;
                        UpdateText();
                    }
                }
            }

            public bool StartColumnRelative
            {
                get { return _startColumnRelative; }
                set { _startColumnRelative = value; }
            }

            public bool StartRowRelative
            {
                get { return _startRowRelative; }
                set { _startRowRelative = value; }
            }

            public string Text
            {
                get { return _text; }
            }
        }

        internal class FormulaSelectionFeature
        {
            int _activeColumnViewportIndex;
            int _activeRowViewportIndex;
            int _anchorColumn = -1;
            int _anchorRow = -1;
            bool _canSelectFormula;
            Excel.EditorManager _editorManager;
            bool _forceSelection;
            Excel.FormulaEditorConnector _formulaEditorConnector;
            bool _isDragDropping;
            bool _isDragResizing;
            bool _isInOtherSheet;
            bool _isSelectingCells;
            bool _isSelectingColumns;
            bool _isSelectingRows;
            bool _isSelectionBegined;
            ObservableCollection<FormulaSelectionItem> _items;
            FormulaSelectionItem _lastHitItem;
            Excel.SpreadXFormulaNavigation _navigation;
            int _resizingAnchorColumn;
            int _resizingAnchorRow;
            Excel.SpreadXFormulaSelection _selection;
            Excel _excel;

            public event EventHandler<FormulaSelectionItemEventArgs> ItemAdded;

            public event EventHandler<FormulaSelectionItemEventArgs> ItemRemoved;

            public FormulaSelectionFeature(Excel excel)
            {
                _excel = excel;
                _items = new ObservableCollection<FormulaSelectionItem>();
                _items.CollectionChanged += OnItemsCollectionChanged;
                _editorManager = new Excel.EditorManager(this);
            }

            public void AddSelection(int row, int column, int rowCount, int columnCount, bool clearFlickingItems = false)
            {
                if (clearFlickingItems)
                {
                    ClearFlickingSelection();
                }
                CellRange cellRange = new CellRange(row, column, rowCount, columnCount);
                cellRange = InflateRange(cellRange);
                _anchorColumn = cellRange.Column;
                if (_anchorColumn < 0)
                {
                    _anchorColumn = 0;
                }
                _anchorRow = cellRange.Row;
                if (_anchorRow < 0)
                {
                    _anchorRow = 0;
                }
                FormulaSelectionItem item = new FormulaSelectionItem(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount, true);
                Items.Add(item);
                EventHandler<FormulaSelectionItemEventArgs> itemAdded = ItemAdded;
                if (itemAdded != null)
                {
                    itemAdded(this, new FormulaSelectionItemEventArgs(item));
                }
            }

            internal void BeginFormulaSelection(object editor)
            {
                IsSelectionBegined = true;
                IFormulaEditor editor2 = editor as IFormulaEditor;
                if (editor2 != null)
                {
                    FormulaEditorConnector.Editor = editor2;
                }
            }

            public void ChangeLastSelection(CellRange cellRange, bool changeAnchor = true)
            {
                if (Items.Count == 0)
                {
                    AddSelection(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount, false);
                }
                else
                {
                    FormulaSelectionItem item = Enumerable.LastOrDefault<FormulaSelectionItem>((IEnumerable<FormulaSelectionItem>)Items);
                    if (item != null)
                    {
                        item.Range = cellRange;
                        if (changeAnchor)
                        {
                            _anchorColumn = cellRange.Column;
                            if (_anchorColumn < 0)
                            {
                                _anchorColumn = 0;
                            }
                            _anchorRow = cellRange.Row;
                            if (_anchorRow < 0)
                            {
                                _anchorRow = 0;
                            }
                        }
                    }
                }
            }

            public void ClearFlickingSelection()
            {
                List<FormulaSelectionItem> list = new List<FormulaSelectionItem>();
                foreach (FormulaSelectionItem item in Items)
                {
                    if (item.IsFlickering)
                    {
                        list.Add(item);
                    }
                }
                foreach (FormulaSelectionItem item2 in list)
                {
                    Items.Remove(item2);
                    EventHandler<FormulaSelectionItemEventArgs> itemRemoved = ItemRemoved;
                    if (itemRemoved != null)
                    {
                        itemRemoved(this, new FormulaSelectionItemEventArgs(item2));
                    }
                }
            }

            void ContinueCellSelecting()
            {
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                ColumnLayout viewportColumnLayoutNearX = _excel.GetViewportColumnLayoutNearX(activeColumnViewportIndex, _excel.MousePosition.X);
                RowLayout viewportRowLayoutNearY = _excel.GetViewportRowLayoutNearY(activeRowViewportIndex, _excel.MousePosition.Y);
                CellLayout layout3 = _excel.GetViewportCellLayoutModel(activeRowViewportIndex, activeColumnViewportIndex).FindPoint(_excel.MousePosition.X, _excel.MousePosition.Y);
                if (layout3 != null)
                {
                    ExtendSelection(layout3.Row, layout3.Column);
                }
                else if ((viewportColumnLayoutNearX != null) && (viewportRowLayoutNearY != null))
                {
                    ExtendSelection(viewportRowLayoutNearY.Row, viewportColumnLayoutNearX.Column);
                }
                _excel.ProcessScrollTimer();
            }

            void ContinueColumnSelecting()
            {
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                ColumnLayout viewportColumnLayoutNearX = _excel.GetViewportColumnLayoutNearX(activeColumnViewportIndex, _excel.MousePosition.X);
                if (viewportColumnLayoutNearX != null)
                {
                    ExtendSelection(-1, viewportColumnLayoutNearX.Column);
                    _excel.ProcessScrollTimer();
                }
            }

            void ContinueDragDropping()
            {
                _excel.UpdateDragToViewports();
                _excel.UpdateDragToCoordicates();
                if ((_excel._dragToRow >= 0) || (_excel._dragToColumn >= 0))
                {
                    _excel.UpdateMouseCursorLocation();
                    UpdateSelection();
                    _excel.ProcessScrollTimer();
                }
            }

            internal void ContinueDragging()
            {
                if (IsSelecting)
                {
                    ContinueSelecting();
                }
                else if (_isDragDropping)
                {
                    ContinueDragDropping();
                }
                else if (_isDragResizing)
                {
                    ContinueDragResizing();
                }
            }

            void ContinueDragResizing()
            {
                _excel.UpdateDragToViewports();
                _excel.UpdateDragToCoordicates();
                if ((_excel._dragToRow >= 0) || (_excel._dragToColumn >= 0))
                {
                    _excel.UpdateMouseCursorLocation();
                    UpdateSelectionForResize();
                    _excel.ProcessScrollTimer();
                }
            }

            void ContinueRowSelecting()
            {
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                RowLayout viewportRowLayoutNearY = _excel.GetViewportRowLayoutNearY(activeRowViewportIndex, _excel.MousePosition.Y);
                if (viewportRowLayoutNearY != null)
                {
                    ExtendSelection(viewportRowLayoutNearY.Row, -1);
                    _excel.ProcessScrollTimer();
                }
            }

            void ContinueSelecting()
            {
                if ((_excel.IsWorking && IsSelecting) && (_excel.MousePosition != _excel._lastClickPoint))
                {
                    if (_isSelectingCells)
                    {
                        ContinueCellSelecting();
                    }
                    else if (_isSelectingRows)
                    {
                        ContinueRowSelecting();
                    }
                    else if (_isSelectingColumns)
                    {
                        ContinueColumnSelecting();
                    }
                }
            }

            void EndDragDropping()
            {
                _excel.HideMouseCursor();
                _isDragDropping = false;
                _excel.StopScrollTimer();
            }

            internal void EndDragging()
            {
                if (IsSelecting)
                {
                    EndSelecting();
                }
                else if (_isDragDropping)
                {
                    EndDragDropping();
                }
                else if (_isDragResizing)
                {
                    EndDragResizing();
                }
            }

            void EndDragResizing()
            {
                _excel.HideMouseCursor();
                _isDragResizing = false;
                _excel.StopScrollTimer();
                using (IEnumerator<FormulaSelectionItem> enumerator = Items.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.IsResizing = false;
                    }
                }
            }

            internal void EndFormulaSelection()
            {
                if (!IsInOtherSheet)
                {
                    _anchorRow = -1;
                    _anchorColumn = -1;
                    IsSelectionBegined = false;
                    _canSelectFormula = false;
                    _items.Clear();
                }
            }

            void EndSelecting()
            {
                _excel.IsWorking = false;
                _isSelectingCells = _isSelectingRows = _isSelectingColumns = false;
                _excel.StopScrollTimer();
            }

            void ExtendSelection(int row, int column)
            {
                FormulaSelectionItem item = Enumerable.LastOrDefault<FormulaSelectionItem>((IEnumerable<FormulaSelectionItem>)Items);
                if (item != null)
                {
                    int num = Math.Min(_anchorRow, row);
                    int num2 = Math.Min(_anchorColumn, column);
                    int num3 = Math.Max(_anchorRow, row);
                    int num4 = Math.Max(_anchorColumn, column);
                    CellRange cellRange = new CellRange(num, num2, (num3 - num) + 1, (num4 - num2) + 1);
                    cellRange = InflateRange(cellRange);
                    item.Range = cellRange;
                }
            }

            internal bool HitTest(int rowViewportIndex, int columnViewportIndex, double mouseX, double mouseY, HitTestInformation hi)
            {
                var worksheet = _excel.ActiveSheet;
                if (worksheet == null)
                {
                    return false;
                }
                if (Items.Count == 0)
                {
                    return false;
                }
                FormulaSelectionItem item = null;
                for (int i = 0; i < Items.Count; i++)
                {
                    FormulaSelectionItem item2 = Items[i];
                    if (!item2.CanChangeBoundsByUI)
                    {
                        continue;
                    }
                    int row = item2.Range.Row;
                    int column = item2.Range.Column;
                    int rowCount = item2.Range.RowCount;
                    int columnCount = item2.Range.ColumnCount;
                    if ((row == -1) && (column == -1))
                    {
                        continue;
                    }
                    if (row == -1)
                    {
                        row = 0;
                        rowCount = worksheet.RowCount;
                    }
                    if (column == -1)
                    {
                        column = 0;
                        columnCount = worksheet.ColumnCount;
                    }
                    SheetLayout sheetLayout = _excel.GetSheetLayout();
                    RowLayout layout2 = _excel.GetViewportRowLayoutModel(rowViewportIndex).Find(row);
                    RowLayout layout3 = _excel.GetViewportRowLayoutModel(rowViewportIndex).Find((row + rowCount) - 1);
                    ColumnLayout layout4 = _excel.GetViewportColumnLayoutModel(columnViewportIndex).Find(column);
                    ColumnLayout layout5 = _excel.GetViewportColumnLayoutModel(columnViewportIndex).Find((column + columnCount) - 1);
                    if ((((rowCount < worksheet.RowCount) && (layout2 == null)) && (layout3 == null)) || (((columnCount < worksheet.ColumnCount) && (layout4 == null)) && (layout5 == null)))
                    {
                        continue;
                    }
                    double num6 = Math.Ceiling((layout4 == null) ? sheetLayout.GetViewportX(columnViewportIndex) : layout4.X);
                    double num7 = Math.Ceiling((layout5 == null) ? ((double)((sheetLayout.GetViewportX(columnViewportIndex) + sheetLayout.GetViewportWidth(columnViewportIndex)) - 1.0)) : ((double)((layout5.X + layout5.Width) - 1.0)));
                    double num8 = Math.Ceiling((layout2 == null) ? sheetLayout.GetViewportY(rowViewportIndex) : layout2.Y);
                    double num9 = Math.Ceiling((layout3 == null) ? ((double)((sheetLayout.GetViewportY(rowViewportIndex) + sheetLayout.GetViewportHeight(rowViewportIndex)) - 1.0)) : ((double)((layout3.Y + layout3.Height) - 1.0)));
                    double num10 = 3.0;
                    double num11 = 3.0;
                    if ((mouseY >= (num8 - 3.0)) && (mouseY <= (num8 + 3.0)))
                    {
                        if ((mouseX >= (num6 - 3.0)) && (mouseX <= (num6 + 3.0)))
                        {
                            ViewportFormulaSelectionHitTestInformation information = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.LeftTop
                            };
                            hi.FormulaSelectionInfo = information;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                        }
                        else
                        {
                            if ((mouseX < (num7 - 3.0)) || (mouseX > (num7 + 3.0)))
                            {
                                goto Label_0391;
                            }
                            ViewportFormulaSelectionHitTestInformation information2 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.RightTop
                            };
                            hi.FormulaSelectionInfo = information2;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                        }
                        break;
                    }
                    if ((mouseY >= (num9 - 3.0)) && (mouseY <= (num9 + 3.0)))
                    {
                        if ((mouseX >= (num6 - 3.0)) && (mouseX <= (num6 + 3.0)))
                        {
                            ViewportFormulaSelectionHitTestInformation information3 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.LeftBottom
                            };
                            hi.FormulaSelectionInfo = information3;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                        if ((mouseX >= (num7 - 3.0)) && (mouseX <= (num7 + 3.0)))
                        {
                            ViewportFormulaSelectionHitTestInformation information4 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.RightBottom
                            };
                            hi.FormulaSelectionInfo = information4;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                    }
                Label_0391:
                    if ((mouseY >= (num8 - num10)) && (mouseY <= (num9 + num11)))
                    {
                        if (((layout4 != null) && (mouseX >= (num6 - num10))) && (mouseX <= (num6 + num11)))
                        {
                            ViewportFormulaSelectionHitTestInformation information5 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.Left
                            };
                            hi.FormulaSelectionInfo = information5;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                        if (((layout5 != null) && (mouseX >= (num7 - num10))) && (mouseX <= (num7 + num11)))
                        {
                            ViewportFormulaSelectionHitTestInformation information6 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.Right
                            };
                            hi.FormulaSelectionInfo = information6;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                    }
                    if ((mouseX >= (num6 - num10)) && (mouseX <= (num7 + num11)))
                    {
                        if (((layout2 != null) && (mouseY >= (num8 - num10))) && (mouseY <= (num8 + num11)))
                        {
                            ViewportFormulaSelectionHitTestInformation information7 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.Top
                            };
                            hi.FormulaSelectionInfo = information7;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                        if (((layout3 != null) && (mouseY >= (num9 - num10))) && (mouseY <= (num9 + num11)))
                        {
                            ViewportFormulaSelectionHitTestInformation information8 = new ViewportFormulaSelectionHitTestInformation
                            {
                                SelectionIndex = i,
                                Position = PositionInFormulaSelection.Bottom
                            };
                            hi.FormulaSelectionInfo = information8;
                            hi.HitTestType = HitTestType.FormulaSelection;
                            item = item2;
                            break;
                        }
                    }
                }
                if (_lastHitItem != item)
                {
                    if (_lastHitItem != null)
                    {
                        _lastHitItem.IsMouseOver = false;
                    }
                    _lastHitItem = item;
                    if ((_lastHitItem != null) && !_excel.IsWorking)
                    {
                        _lastHitItem.IsMouseOver = true;
                    }
                }
                return (_lastHitItem != null);
            }

            CellRange InflateRange(CellRange cellRange)
            {
                List<CellRange> list = new List<CellRange>();
                foreach (CellRange range in _excel.ActiveSheet.SpanModel)
                {
                    list.Add(range);
                }
                if (list.Count != 0)
                {
                    bool flag = false;
                    while (!flag)
                    {
                        flag = true;
                        for (int i = 0; i < list.Count; i++)
                        {
                            CellRange range2 = list[i];
                            if (cellRange.Intersects(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount))
                            {
                                list.RemoveAt(i--);
                                cellRange = UnionCellRange(cellRange, range2);
                                flag = false;
                                continue;
                            }
                        }
                    }
                }
                return cellRange;
            }

            void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (_excel._cellsPanels != null)
                {
                    CellsPanel[,] viewportArray = _excel._cellsPanels;
                    int upperBound = viewportArray.GetUpperBound(0);
                    int num2 = viewportArray.GetUpperBound(1);
                    for (int k = viewportArray.GetLowerBound(0); k <= upperBound; k++)
                    {
                        for (int i = viewportArray.GetLowerBound(1); i <= num2; i++)
                        {
                            CellsPanel viewport = viewportArray[k, i];
                            if (viewport != null)
                            {
                                viewport.RefreshFormulaSelection();
                            }
                        }
                    }
                    _excel.RefreshFormulaSelectionGrippers();
                }
            }

            internal void SetCursor(ViewportFormulaSelectionHitTestInformation info)
            {
                if ((info.Position == PositionInFormulaSelection.LeftTop) || (info.Position == PositionInFormulaSelection.RightBottom))
                {
                    _excel.SetBuiltInCursor(CoreCursorType.SizeNorthwestSoutheast);
                }
                else if ((info.Position == PositionInFormulaSelection.LeftBottom) || (info.Position == PositionInFormulaSelection.RightTop))
                {
                    _excel.SetBuiltInCursor(CoreCursorType.SizeNortheastSouthwest);
                }
                else
                {
                    _excel.SetMouseCursor(CursorType.DragCell_DragCursor);
                }
            }

            void StartCellSelecting()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                int row = savedHitTestInformation.ViewportInfo.Row;
                int column = savedHitTestInformation.ViewportInfo.Column;
                int rowCount = 1;
                int columnCount = 1;
                if ((savedHitTestInformation.ViewportInfo.Row > -1) && (savedHitTestInformation.ViewportInfo.Column > -1))
                {
                    bool flag;
                    bool flag2;
                    CellLayout layout = _excel.GetViewportCellLayoutModel(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.ColumnViewportIndex).FindCell(savedHitTestInformation.ViewportInfo.Row, savedHitTestInformation.ViewportInfo.Column);
                    KeyboardHelper.GetMetaKeyState(out flag2, out flag);
                    if (layout != null)
                    {
                        row = layout.Row;
                        column = layout.Column;
                        rowCount = layout.RowCount;
                        columnCount = layout.ColumnCount;
                    }
                    _excel.IsWorking = true;
                    _isSelectingCells = true;
                    _excel.SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                    _excel.SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                    if (flag)
                    {
                        AddSelection(row, column, rowCount, columnCount, false);
                    }
                    else if (flag2)
                    {
                        ExtendSelection(row, column);
                    }
                    else
                    {
                        AddSelection(row, column, rowCount, columnCount, true);
                    }
                    if (!_excel.IsWorking)
                    {
                        EndSelecting();
                    }
                    _excel.StartScrollTimer();
                }
            }

            void StartColumnSelecting()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                if ((savedHitTestInformation.HitTestType == HitTestType.Empty) || (savedHitTestInformation.HeaderInfo == null))
                {
                    savedHitTestInformation = _excel.HitTest(_excel._touchStartPoint.X, _excel._touchStartPoint.Y);
                }
                if (savedHitTestInformation.HeaderInfo != null)
                {
                    SheetLayout sheetLayout = _excel.GetSheetLayout();
                    _excel.GetViewportTopRow((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                    int column = savedHitTestInformation.HeaderInfo.Column;
                    _excel.IsWorking = true;
                    _isSelectingColumns = true;
                    _excel.SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                    _excel.SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                    if (savedHitTestInformation.HeaderInfo.Column > -1)
                    {
                        bool flag;
                        bool flag2;
                        KeyboardHelper.GetMetaKeyState(out flag2, out flag);
                        if (flag)
                        {
                            AddSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1, false);
                        }
                        else if (flag2)
                        {
                            ExtendSelection(-1, savedHitTestInformation.HeaderInfo.Column);
                        }
                        else
                        {
                            AddSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1, true);
                        }
                        if (!_excel.IsWorking)
                        {
                            EndSelecting();
                        }
                        _excel.StartScrollTimer();
                    }
                }
            }

            internal void StartDragDropping()
            {
                if (!_isDragDropping && (Items.Count != 0))
                {
                    _excel.IsWorking = true;
                    _isDragDropping = true;
                    HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                    FormulaSelectionItem item = Items[savedHitTestInformation.FormulaSelectionInfo.SelectionIndex];
                    _excel._rowOffset = Math.Max(0, Math.Min((int)(savedHitTestInformation.ViewportInfo.Row - item.Range.Row), (int)(item.Range.RowCount - 1)));
                    _excel._columnOffset = Math.Max(0, Math.Min((int)(savedHitTestInformation.ViewportInfo.Column - item.Range.Column), (int)(item.Range.ColumnCount - 1)));
                    _excel._dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
                    _excel._dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
                    _excel._dragToRowViewport = savedHitTestInformation.RowViewportIndex;
                    _excel._dragToColumnViewport = savedHitTestInformation.ColumnViewportIndex;
                    using (IEnumerator<FormulaSelectionItem> enumerator = Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.IsFlickering = false;
                        }
                    }
                    CanSelectFormula = false;
                    _excel.StartScrollTimer();
                }
            }

            internal void StartDragResizing()
            {
                if (!_isDragResizing && (Items.Count != 0))
                {
                    _excel.IsWorking = true;
                    _isDragResizing = true;
                    HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                    _excel._dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
                    _excel._dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
                    _excel._dragToRowViewport = savedHitTestInformation.RowViewportIndex;
                    _excel._dragToColumnViewport = savedHitTestInformation.ColumnViewportIndex;
                    FormulaSelectionItem item = Items[savedHitTestInformation.FormulaSelectionInfo.SelectionIndex];
                    item.IsResizing = true;
                    CellRange range = item.Range;
                    if (range.Row < 0)
                    {
                        range = new CellRange(0, range.Column, _excel.ActiveSheet.RowCount, range.ColumnCount);
                    }
                    if (range.Column < 0)
                    {
                        range = new CellRange(range.Row, 0, range.RowCount, _excel.ActiveSheet.ColumnCount);
                    }
                    switch (savedHitTestInformation.FormulaSelectionInfo.Position)
                    {
                        case PositionInFormulaSelection.LeftTop:
                            _resizingAnchorColumn = (range.Column + range.ColumnCount) - 1;
                            _resizingAnchorRow = (range.Row + range.RowCount) - 1;
                            break;

                        case PositionInFormulaSelection.RightTop:
                            _resizingAnchorColumn = range.Column;
                            _resizingAnchorRow = (range.Row + range.RowCount) - 1;
                            break;

                        case PositionInFormulaSelection.LeftBottom:
                            _resizingAnchorColumn = (range.Column + range.ColumnCount) - 1;
                            _resizingAnchorRow = range.Row;
                            break;

                        case PositionInFormulaSelection.RightBottom:
                            _resizingAnchorColumn = range.Column;
                            _resizingAnchorRow = range.Row;
                            break;
                    }
                    using (IEnumerator<FormulaSelectionItem> enumerator = Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.IsFlickering = false;
                        }
                    }
                    CanSelectFormula = false;
                    _excel.StartScrollTimer();
                }
            }

            void StartRowSelecting()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                SheetLayout sheetLayout = _excel.GetSheetLayout();
                int row = savedHitTestInformation.HeaderInfo.Row;
                _excel.GetViewportLeftColumn((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.IsWorking = true;
                _isSelectingRows = true;
                _excel.SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                if (savedHitTestInformation.HeaderInfo.Row > -1)
                {
                    bool flag;
                    bool flag2;
                    KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                    if (flag2)
                    {
                        AddSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1, false);
                    }
                    else if (flag)
                    {
                        ExtendSelection(savedHitTestInformation.HeaderInfo.Row, -1);
                    }
                    else
                    {
                        AddSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1, true);
                    }
                    if (!_excel.IsWorking)
                    {
                        EndSelecting();
                    }
                    _excel.StartScrollTimer();
                }
            }

            internal bool StartSelecting(SheetArea area)
            {
                if (CanSelectFormula)
                {
                    if (area == SheetArea.Cells)
                    {
                        StartCellSelecting();
                        return true;
                    }
                    if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
                    {
                        StartRowSelecting();
                        return true;
                    }
                    if (area == SheetArea.ColumnHeader)
                    {
                        StartColumnSelecting();
                        return true;
                    }
                    if (area == SheetArea.CornerHeader)
                    {
                        StartSheetSelecting();
                        return true;
                    }
                }
                else
                {
                    EndFormulaSelection();
                }
                return false;
            }

            void StartSheetSelecting()
            {
                bool flag;
                bool flag2;
                SheetLayout sheetLayout = _excel.GetSheetLayout();
                _excel.SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                AddSelection(0, -1, _excel.ActiveSheet.RowCount, -1, !flag2);
            }

            internal bool TouchHitTest(double mouseX, double mouseY, HitTestInformation hi)
            {
                var worksheet = _excel.ActiveSheet;
                if (worksheet != null)
                {
                    if (Items.Count == 0)
                    {
                        return false;
                    }
                    for (int i = 0; i < Items.Count; i++)
                    {
                        FormulaSelectionItem item = Items[i];
                        if (item.CanChangeBoundsByUI)
                        {
                            int row = item.Range.Row;
                            int column = item.Range.Column;
                            int rowCount = item.Range.RowCount;
                            int columnCount = item.Range.ColumnCount;
                            if ((row != -1) || (column != -1))
                            {
                                if (row == -1)
                                {
                                    row = 0;
                                    rowCount = worksheet.RowCount;
                                }
                                if (column == -1)
                                {
                                    column = 0;
                                    columnCount = worksheet.ColumnCount;
                                }
                                SheetLayout sheetLayout = _excel.GetSheetLayout();
                                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                                RowLayout layout2 = _excel.GetViewportRowLayoutModel(activeRowViewportIndex).Find(row);
                                RowLayout layout3 = _excel.GetViewportRowLayoutModel(activeRowViewportIndex).Find((row + rowCount) - 1);
                                ColumnLayout layout4 = _excel.GetViewportColumnLayoutModel(activeColumnViewportIndex).Find(column);
                                ColumnLayout layout5 = _excel.GetViewportColumnLayoutModel(activeColumnViewportIndex).Find((column + columnCount) - 1);
                                if ((((rowCount >= worksheet.RowCount) || (layout2 != null)) || (layout3 != null)) && (((columnCount >= worksheet.ColumnCount) || (layout4 != null)) || (layout5 != null)))
                                {
                                    double num8 = Math.Ceiling((layout4 == null) ? sheetLayout.GetViewportX(activeColumnViewportIndex) : layout4.X);
                                    double num9 = Math.Ceiling((layout5 == null) ? ((double)((sheetLayout.GetViewportX(activeColumnViewportIndex) + sheetLayout.GetViewportWidth(activeColumnViewportIndex)) - 1.0)) : ((double)((layout5.X + layout5.Width) - 1.0)));
                                    double num10 = Math.Ceiling((layout2 == null) ? sheetLayout.GetViewportY(activeRowViewportIndex) : layout2.Y);
                                    double num11 = Math.Ceiling((layout3 == null) ? ((double)((sheetLayout.GetViewportY(activeRowViewportIndex) + sheetLayout.GetViewportHeight(activeRowViewportIndex)) - 1.0)) : ((double)((layout3.Y + layout3.Height) - 1.0)));
                                    if (((mouseX >= (num8 - 20.0)) && (mouseX <= (num8 + 20.0))) && ((mouseY >= (num10 - 20.0)) && (mouseY <= (num10 + 20.0))))
                                    {
                                        ViewportFormulaSelectionHitTestInformation information = new ViewportFormulaSelectionHitTestInformation
                                        {
                                            SelectionIndex = i,
                                            Position = PositionInFormulaSelection.LeftTop
                                        };
                                        hi.FormulaSelectionInfo = information;
                                        hi.HitTestType = HitTestType.FormulaSelection;
                                        hi.RowViewportIndex = activeRowViewportIndex;
                                        hi.ColumnViewportIndex = activeColumnViewportIndex;
                                        return true;
                                    }
                                    if (((mouseX >= (num9 - 20.0)) && (mouseX <= (num9 + 20.0))) && ((mouseY >= (num11 - 20.0)) && (mouseY <= (num11 + 20.0))))
                                    {
                                        ViewportFormulaSelectionHitTestInformation information2 = new ViewportFormulaSelectionHitTestInformation
                                        {
                                            SelectionIndex = i,
                                            Position = PositionInFormulaSelection.RightBottom
                                        };
                                        hi.FormulaSelectionInfo = information2;
                                        hi.HitTestType = HitTestType.FormulaSelection;
                                        hi.RowViewportIndex = activeRowViewportIndex;
                                        hi.ColumnViewportIndex = activeColumnViewportIndex;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }

            internal bool TouchSelect(SheetArea area)
            {
                bool flag = false;
                if (CanSelectFormula)
                {
                    if (area == SheetArea.Cells)
                    {
                        TouchSelectCell();
                        flag = true;
                    }
                    else if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
                    {
                        TouchSelectRow();
                        flag = true;
                    }
                    else if (area == SheetArea.ColumnHeader)
                    {
                        TouchSelectColumn();
                        flag = true;
                    }
                    else if (area == SheetArea.CornerHeader)
                    {
                        TouchSelectSheet();
                        flag = true;
                    }
                }
                else
                {
                    EndFormulaSelection();
                }
                IsTouching = true;
                if (flag)
                {
                    Excel.ShowFormulaSelectionTouchGrippers();
                }
                return flag;
            }

            void TouchSelectCell()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                int row = savedHitTestInformation.ViewportInfo.Row;
                int column = savedHitTestInformation.ViewportInfo.Column;
                int rowCount = 1;
                int columnCount = 1;
                if ((savedHitTestInformation.ViewportInfo.Row > -1) && (savedHitTestInformation.ViewportInfo.Column > -1))
                {
                    bool flag;
                    bool flag2;
                    CellLayout layout = _excel.GetViewportCellLayoutModel(savedHitTestInformation.RowViewportIndex, savedHitTestInformation.ColumnViewportIndex).FindCell(savedHitTestInformation.ViewportInfo.Row, savedHitTestInformation.ViewportInfo.Column);
                    KeyboardHelper.GetMetaKeyState(out flag2, out flag);
                    if (layout != null)
                    {
                        row = layout.Row;
                        column = layout.Column;
                        rowCount = layout.RowCount;
                        columnCount = layout.ColumnCount;
                    }
                    _excel.SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                    _excel.SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                    AddSelection(row, column, rowCount, columnCount, true);
                }
            }

            void TouchSelectColumn()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                if ((savedHitTestInformation.HitTestType == HitTestType.Empty) || (savedHitTestInformation.HeaderInfo == null))
                {
                    savedHitTestInformation = _excel.HitTest(_excel._touchStartPoint.X, _excel._touchStartPoint.Y);
                }
                if (savedHitTestInformation.HeaderInfo != null)
                {
                    SheetLayout sheetLayout = _excel.GetSheetLayout();
                    _excel.GetViewportTopRow((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                    int column = savedHitTestInformation.HeaderInfo.Column;
                    _excel.SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
                    _excel.SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                    if (savedHitTestInformation.HeaderInfo.Column > -1)
                    {
                        AddSelection(-1, savedHitTestInformation.HeaderInfo.Column, -1, 1, true);
                    }
                }
            }

            void TouchSelectRow()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                SheetLayout sheetLayout = _excel.GetSheetLayout();
                int row = savedHitTestInformation.HeaderInfo.Row;
                _excel.GetViewportLeftColumn((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
                if (savedHitTestInformation.HeaderInfo.Row > -1)
                {
                    AddSelection(savedHitTestInformation.HeaderInfo.Row, -1, 1, -1, true);
                }
            }

            void TouchSelectSheet()
            {
                SheetLayout sheetLayout = _excel.GetSheetLayout();
                _excel.SetActiveColumnViewportIndex((sheetLayout.FrozenWidth > 0.0) ? -1 : 0);
                _excel.SetActiveRowViewportIndex((sheetLayout.FrozenHeight > 0.0) ? -1 : 0);
                AddSelection(0, -1, _excel.ActiveSheet.RowCount, -1, true);
            }

            static CellRange UnionCellRange(CellRange range1, CellRange range2)
            {
                int row = Math.Min(range1.Row, range2.Row);
                int column = Math.Min(range1.Column, range2.Column);
                int num3 = Math.Max((int)((range1.Row + range1.RowCount) - 1), (int)((range2.Row + range2.RowCount) - 1));
                int num4 = Math.Max((int)((range1.Column + range1.ColumnCount) - 1), (int)((range2.Column + range2.ColumnCount) - 1));
                if ((row >= 0) && (column >= 0))
                {
                    return new CellRange(row, column, (num3 - row) + 1, (num4 - column) + 1);
                }
                if (row >= 0)
                {
                    return new CellRange(row, -1, (num3 - row) + 1, -1);
                }
                if (column >= 0)
                {
                    return new CellRange(-1, column, -1, (num4 - column) + 1);
                }
                return new CellRange(-1, -1, -1, -1);
            }

            void UpdateSelection()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                FormulaSelectionItem item = Items[savedHitTestInformation.FormulaSelectionInfo.SelectionIndex];
                CellRange range = item.Range;
                int row = _excel._dragToRow - _excel._rowOffset;
                if ((range.Row == -1) && (range.RowCount == -1))
                {
                    row = -1;
                }
                else if (row < 0)
                {
                    row = 0;
                }
                else if ((row + range.RowCount) > _excel.ActiveSheet.RowCount)
                {
                    row = _excel.ActiveSheet.RowCount - range.RowCount;
                }
                int column = _excel._dragToColumn - _excel._columnOffset;
                if ((range.Column == -1) && (range.ColumnCount == -1))
                {
                    column = -1;
                }
                else if (column < 0)
                {
                    column = 0;
                }
                else if ((column + range.ColumnCount) > _excel.ActiveSheet.ColumnCount)
                {
                    column = _excel.ActiveSheet.ColumnCount - range.ColumnCount;
                }
                range = new CellRange(row, column, range.RowCount, range.ColumnCount);
                item.Range = range;
            }

            void UpdateSelectionForResize()
            {
                HitTestInformation savedHitTestInformation = _excel.GetHitInfo();
                FormulaSelectionItem item = Items[savedHitTestInformation.FormulaSelectionInfo.SelectionIndex];
                int column = Math.Min(_excel._dragToColumn, _resizingAnchorColumn);
                int row = Math.Min(_excel._dragToRow, _resizingAnchorRow);
                int num3 = Math.Max(_excel._dragToColumn, _resizingAnchorColumn);
                int num4 = Math.Max(_excel._dragToRow, _resizingAnchorRow);
                CellRange range = new CellRange(row, column, (num4 - row) + 1, (num3 - column) + 1);
                if ((range.Column == 0) && (range.ColumnCount == _excel.ActiveSheet.ColumnCount))
                {
                    range = new CellRange(range.Row, -1, range.RowCount, -1);
                }
                else if ((range.Row == 0) && (range.RowCount == _excel.ActiveSheet.RowCount))
                {
                    range = new CellRange(-1, range.Column, -1, range.ColumnCount);
                }
                item.Range = range;
            }

            public int ActiveColumnViewportIndex
            {
                get { return _activeColumnViewportIndex; }
                set
                {
                    if (_activeColumnViewportIndex != value)
                    {
                        _activeColumnViewportIndex = value;
                    }
                }
            }

            public int ActiveRowViewportIndex
            {
                get { return _activeRowViewportIndex; }
                set
                {
                    if (_activeRowViewportIndex != value)
                    {
                        _activeRowViewportIndex = value;
                    }
                }
            }

            public int AnchorColumn
            {
                get
                {
                    if (((_anchorColumn == -1) && (_excel != null)) && (_excel.ActiveSheet != null))
                    {
                        return _excel.ActiveSheet.ActiveColumnIndex;
                    }
                    return _anchorColumn;
                }
            }

            public int AnchorRow
            {
                get
                {
                    if (((_anchorRow == -1) && (_excel != null)) && (_excel.ActiveSheet != null))
                    {
                        return _excel.ActiveSheet.ActiveRowIndex;
                    }
                    return _anchorRow;
                }
            }

            internal bool CanSelectFormula
            {
                get
                {
                    if (!_canSelectFormula)
                    {
                        return ForceSelection;
                    }
                    return true;
                }
                set
                {
                    if ((!IsInOtherSheet && IsSelectionBegined) && (_canSelectFormula != value))
                    {
                        _canSelectFormula = value;
                        if (!_canSelectFormula)
                        {
                            using (IEnumerator<FormulaSelectionItem> enumerator = Items.GetEnumerator())
                            {
                                while (enumerator.MoveNext())
                                {
                                    enumerator.Current.IsFlickering = false;
                                }
                            }
                        }
                    }
                }
            }

            public bool ForceSelection
            {
                get { return _forceSelection; }
                set { _forceSelection = value; }
            }

            public Excel.FormulaEditorConnector FormulaEditorConnector
            {
                get
                {
                    if (_formulaEditorConnector == null)
                    {
                        _formulaEditorConnector = new Excel.FormulaEditorConnector(this);
                    }
                    return _formulaEditorConnector;
                }
            }

            public bool IsDragging
            {
                get
                {
                    if (!IsSelecting && !_isDragDropping)
                    {
                        return _isDragResizing;
                    }
                    return true;
                }
            }

            public bool IsFlicking
            {
                get
                {
                    using (IEnumerator<FormulaSelectionItem> enumerator = _items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.IsFlickering)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }

            public bool IsInOtherSheet
            {
                get { return _isInOtherSheet; }
                set
                {
                    if (_isInOtherSheet != value)
                    {
                        _isInOtherSheet = value;
                    }
                }
            }

            bool IsSelecting
            {
                get
                {
                    if (!_isSelectingCells && !_isSelectingRows)
                    {
                        return _isSelectingColumns;
                    }
                    return true;
                }
            }

            public bool IsSelectionBegined
            {
                get
                {
                    if (!_isSelectionBegined)
                    {
                        return ForceSelection;
                    }
                    return true;
                }
                set { _isSelectionBegined = value; }
            }

            public static bool IsTouching { get; set; }

            public IList<FormulaSelectionItem> Items
            {
                get { return (IList<FormulaSelectionItem>)_items; }
            }

            public Excel.SpreadXFormulaNavigation Navigation
            {
                get
                {
                    if (_navigation == null)
                    {
                        _navigation = new Excel.SpreadXFormulaNavigation(this);
                    }
                    return _navigation;
                }
            }

            public Excel.SpreadXFormulaSelection Selection
            {
                get
                {
                    if (_selection == null)
                    {
                        _selection = new Excel.SpreadXFormulaSelection(this);
                    }
                    return _selection;
                }
            }

            public Excel Excel
            {
                get { return _excel; }
            }
        }

        internal class GripperLocationsStruct
        {
            public Rect BottomRight { get; set; }

            public Rect TopLeft { get; set; }
        }

        internal class SpreadXFormulaNavigation
        {
            Excel.FormulaSelectionFeature _formulaSelectionFeature;
            Excel _excel;
            Excel.SpreadXFormulaTabularNavigator _tabularNavigator;

            public SpreadXFormulaNavigation(Excel.FormulaSelectionFeature formulaSelectionFeature)
            {
                _formulaSelectionFeature = formulaSelectionFeature;
                _excel = formulaSelectionFeature.Excel;
                _tabularNavigator = new Excel.SpreadXFormulaTabularNavigator(_excel);
            }

            bool MoveActiveCell(NavigationDirection direction)
            {
                if (_excel.ActiveSheet == null)
                {
                    return false;
                }
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                _excel.GetViewportTopRow(activeRowViewportIndex);
                _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                int activeRowIndex = _excel.ActiveSheet.ActiveRowIndex;
                int activeColumnIndex = _excel.ActiveSheet.ActiveColumnIndex;
                _tabularNavigator.GetNavigationStartPosition();
                TabularPosition position = MoveCurrent(direction);
                if (position.IsEmpty)
                {
                    NavigatorHelper.BringCellToVisible(_excel, _formulaSelectionFeature.AnchorRow, _formulaSelectionFeature.AnchorColumn);
                    return false;
                }
                int row = position.Row;
                int column = position.Column;
                _formulaSelectionFeature.AddSelection(row, column, 1, 1, true);
                int num5 = _excel.GetActiveRowViewportIndex();
                int num6 = _excel.GetActiveColumnViewportIndex();
                if ((activeRowViewportIndex != num5) || (activeColumnViewportIndex != num6))
                {
                    NavigatorHelper.BringCellToVisible(_excel, row, column);
                }
                return true;
            }

            TabularPosition MoveCurrent(NavigationDirection direction)
            {
                int anchorRow = _formulaSelectionFeature.AnchorRow;
                int anchorColumn = _formulaSelectionFeature.AnchorColumn;
                if ((anchorRow != -1) && (anchorColumn != -1))
                {
                    _tabularNavigator.CurrentCell = new TabularPosition(SheetArea.Cells, anchorRow, anchorColumn);
                    if (_tabularNavigator.MoveCurrent(direction))
                    {
                        TabularPosition currentCell = _tabularNavigator.CurrentCell;
                        return new TabularPosition(SheetArea.Cells, currentCell.Row, currentCell.Column);
                    }
                }
                return TabularPosition.Empty;
            }

            public void ProcessNavigation(NavigationDirection? direction)
            {
                if (((_formulaSelectionFeature.Items.Count == 0) || _formulaSelectionFeature.IsFlicking) && direction.HasValue)
                {
                    MoveActiveCell(direction.Value);
                }
            }

            bool SetActiveCell(int row, int column, bool clearSelection)
            {
                var worksheet = _excel.ActiveSheet;
                if (!_excel.RaiseLeaveCell(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, row, column))
                {
                    worksheet.SetActiveCell(row, column, clearSelection);
                    _excel.RaiseEnterCell(row, column);
                    return true;
                }
                return false;
            }
        }

        internal class SpreadXFormulaSelection
        {
            Excel.FormulaSelectionFeature _formulaSelectionFeature;
            KeyboardSelectNavigator _keyboardNavigator;
            Excel _excel;

            internal SpreadXFormulaSelection(Excel.FormulaSelectionFeature formulaSelectionFeature)
            {
                _formulaSelectionFeature = formulaSelectionFeature;
                _excel = formulaSelectionFeature.Excel;
                _keyboardNavigator = new KeyboardSelectNavigator(_excel);
            }

            static CellRange CellRangeUnion(CellRange range1, CellRange range2)
            {
                int row = Math.Min(range1.Row, range2.Row);
                int column = Math.Min(range1.Column, range2.Column);
                int num3 = Math.Max((int)((range1.Row + range1.RowCount) - 1), (int)((range2.Row + range2.RowCount) - 1));
                int num4 = Math.Max((int)((range1.Column + range1.ColumnCount) - 1), (int)((range2.Column + range2.ColumnCount) - 1));
                return new CellRange(row, column, (num3 - row) + 1, (num4 - column) + 1);
            }

            CellRange ExpandRange(List<CellRange> spans, CellRange range)
            {
                if ((spans != null) && (spans.Count > 0))
                {
                    for (int i = 0; i < spans.Count; i++)
                    {
                        CellRange range2 = spans[i];
                        if (range.Intersects(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount))
                        {
                            spans.RemoveAt(i--);
                            return ExpandRange(spans, CellRangeUnion(range, range2));
                        }
                    }
                }
                return range;
            }

            CellRange GetActiveCell()
            {
                int anchorRow = _formulaSelectionFeature.AnchorRow;
                int anchorColumn = _formulaSelectionFeature.AnchorColumn;
                CellRange range = new CellRange(anchorRow, anchorColumn, 1, 1);
                CellRange range2 = _excel.ActiveSheet.SpanModel.Find(anchorRow, anchorColumn);
                if (range2 != null)
                {
                    range = range2;
                }
                return range;
            }

            static void GetAdjustedEdge(int row, int column, int rowCount, int columnCount, NavigationDirection navigationDirection, bool shrink, out TabularPosition startPosition, out TabularPosition endPosition)
            {
                startPosition = TabularPosition.Empty;
                endPosition = TabularPosition.Empty;
                KeyboardSelectDirection none = KeyboardSelectDirection.None;
                switch (navigationDirection)
                {
                    case NavigationDirection.Left:
                    case NavigationDirection.PageLeft:
                    case NavigationDirection.Home:
                        none = KeyboardSelectDirection.Left;
                        break;

                    case NavigationDirection.Right:
                    case NavigationDirection.PageRight:
                    case NavigationDirection.End:
                        none = KeyboardSelectDirection.Right;
                        break;

                    case NavigationDirection.Up:
                    case NavigationDirection.PageUp:
                    case NavigationDirection.Top:
                    case NavigationDirection.First:
                        none = KeyboardSelectDirection.Top;
                        break;

                    case NavigationDirection.Down:
                    case NavigationDirection.PageDown:
                    case NavigationDirection.Bottom:
                    case NavigationDirection.Last:
                        none = KeyboardSelectDirection.Bottom;
                        break;
                }
                if (shrink)
                {
                    switch (navigationDirection)
                    {
                        case NavigationDirection.Left:
                            none = KeyboardSelectDirection.Right;
                            break;

                        case NavigationDirection.Right:
                            none = KeyboardSelectDirection.Left;
                            break;

                        case NavigationDirection.Up:
                            none = KeyboardSelectDirection.Bottom;
                            break;

                        case NavigationDirection.Down:
                            none = KeyboardSelectDirection.Top;
                            break;
                    }
                }
                switch (none)
                {
                    case KeyboardSelectDirection.Left:
                        startPosition = new TabularPosition(SheetArea.Cells, row, (column + columnCount) - 1);
                        endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, column);
                        return;

                    case KeyboardSelectDirection.Top:
                        startPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, column);
                        endPosition = new TabularPosition(SheetArea.Cells, row, (column + columnCount) - 1);
                        return;

                    case KeyboardSelectDirection.Right:
                        startPosition = new TabularPosition(SheetArea.Cells, row, column);
                        endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, (column + columnCount) - 1);
                        return;

                    case KeyboardSelectDirection.Bottom:
                        startPosition = new TabularPosition(SheetArea.Cells, row, column);
                        endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, (column + columnCount) - 1);
                        return;
                }
            }

            CellRange GetExpandIntersectedRange(CellRange range)
            {
                if (_excel.ActiveSheet.SpanModel.IsEmpty())
                {
                    return range;
                }
                List<CellRange> spans = new List<CellRange>();
                foreach (object obj2 in _excel.ActiveSheet.SpanModel)
                {
                    spans.Add((CellRange)obj2);
                }
                return ExpandRange(spans, range);
            }

            static KeyboardSelectKind GetKeyboardSelectionKind(NavigationDirection navigationDirection)
            {
                switch (navigationDirection)
                {
                    case NavigationDirection.Left:
                    case NavigationDirection.Right:
                    case NavigationDirection.Up:
                    case NavigationDirection.Down:
                        return KeyboardSelectKind.Line;

                    case NavigationDirection.PageUp:
                    case NavigationDirection.PageDown:
                    case NavigationDirection.PageLeft:
                    case NavigationDirection.PageRight:
                        return KeyboardSelectKind.Page;

                    case NavigationDirection.Home:
                    case NavigationDirection.End:
                    case NavigationDirection.Top:
                    case NavigationDirection.Bottom:
                    case NavigationDirection.First:
                    case NavigationDirection.Last:
                        return KeyboardSelectKind.Through;
                }
                return KeyboardSelectKind.None;
            }

            CellRange GetSelectionRange()
            {
                if (_formulaSelectionFeature.Items.Count > 0)
                {
                    return _formulaSelectionFeature.Items[_formulaSelectionFeature.Items.Count - 1].Range;
                }
                return null;
            }

            CellRange KeyboardLineSelect(CellRange currentRange, NavigationDirection navigationDirection, bool shrink)
            {
                TabularPosition position;
                TabularPosition position2;
                TabularPosition currentCell;
                CellRange expandIntersectedRange;
                int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
                int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
                int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
                int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
                GetAdjustedEdge(row, column, rowCount, columnCount, navigationDirection, shrink, out position, out position2);
                if ((position == TabularPosition.Empty) || (position2 == TabularPosition.Empty))
                {
                    return null;
                }
                _keyboardNavigator.CurrentCell = position2;
                CellRange activeCell = GetActiveCell();
                do
                {
                    if (!_keyboardNavigator.MoveCurrent(navigationDirection))
                    {
                        return null;
                    }
                    currentCell = _keyboardNavigator.CurrentCell;
                    expandIntersectedRange = GetExpandIntersectedRange(TabularPositionUnion(position, currentCell));
                    if (!expandIntersectedRange.Contains(activeCell))
                    {
                        return null;
                    }
                }
                while (expandIntersectedRange.Equals(row, column, rowCount, columnCount));
                bool flag = true;
                int viewCellRow = currentCell.Row;
                int viewCellColumn = currentCell.Column;
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                if ((navigationDirection == NavigationDirection.Up) || (navigationDirection == NavigationDirection.Down))
                {
                    if ((expandIntersectedRange.Column == 0) && (expandIntersectedRange.ColumnCount == _excel.ActiveSheet.ColumnCount))
                    {
                        if ((currentCell.Row >= viewportTopRow) && (currentCell.Row < viewportBottomRow))
                        {
                            flag = false;
                        }
                        else
                        {
                            viewCellColumn = viewportLeftColumn;
                        }
                    }
                }
                else if (((navigationDirection == NavigationDirection.Left) || (navigationDirection == NavigationDirection.Right)) && ((expandIntersectedRange.Row == 0) && (expandIntersectedRange.RowCount == _excel.ActiveSheet.RowCount)))
                {
                    if ((currentCell.Column >= viewportLeftColumn) && (currentCell.Column < viewportRightColumn))
                    {
                        flag = false;
                    }
                    else
                    {
                        viewCellRow = viewportTopRow;
                    }
                }
                if (flag)
                {
                    NavigatorHelper.BringCellToVisible(_excel, viewCellRow, viewCellColumn);
                }
                return expandIntersectedRange;
            }

            CellRange KeyboardPageSelect(CellRange currentRange, NavigationDirection direction)
            {
                int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
                int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
                int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
                int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
                int num5 = (row + rowCount) - 1;
                int num6 = (column + columnCount) - 1;
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                int num9 = _excel.ActiveSheet.RowCount;
                int num10 = _excel.ActiveSheet.ColumnCount;
                int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                _excel.GetViewportBottomRow(activeRowViewportIndex);
                int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                _excel.GetViewportRightColumn(activeColumnViewportIndex);
                int num13 = GetActiveCell().Row;
                int num14 = GetActiveCell().Column;
                CellRange range = null;
                if (direction == NavigationDirection.PageDown)
                {
                    NavigatorHelper.ScrollToNextPageOfRows(_excel);
                    int num15 = _excel.GetViewportTopRow(activeRowViewportIndex);
                    int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                    int num17 = num15 - viewportTopRow;
                    if (num17 > 0)
                    {
                        int num18 = num13;
                        int num19 = num5 + num17;
                        if (row != num13)
                        {
                            num18 = row + num17;
                            num19 = num5;
                            if (num18 >= num13)
                            {
                                num18 = num13;
                                num19 = num5 + (num17 - (num13 - row));
                            }
                        }
                        if (num19 < num15)
                        {
                            num19 = num15;
                        }
                        else if (num18 > viewportBottomRow)
                        {
                            num18 = viewportBottomRow;
                            num19 = num13;
                        }
                        else if ((num19 > viewportBottomRow) && (num13 <= viewportBottomRow))
                        {
                            num19 = viewportBottomRow;
                        }
                        return new CellRange(num18, column, (num19 - num18) + 1, columnCount);
                    }
                    int num20 = (num9 - row) - rowCount;
                    if ((num20 > 0) && (_excel.ActiveSheet.FrozenTrailingRowCount == 0))
                    {
                        int num21 = num13;
                        int num22 = num9 - 1;
                        range = new CellRange(num21, column, (num22 - num21) + 1, columnCount);
                    }
                    return range;
                }
                if (direction == NavigationDirection.PageUp)
                {
                    NavigatorHelper.ScrollToPreviousPageOfRows(_excel);
                    int num23 = _excel.GetViewportTopRow(activeRowViewportIndex);
                    int num24 = _excel.GetViewportBottomRow(activeRowViewportIndex);
                    int num25 = viewportTopRow - num23;
                    if (num25 > 0)
                    {
                        int num26 = row - num25;
                        int num27 = num5;
                        if (num5 != num13)
                        {
                            num26 = row;
                            num27 = num5 - num25;
                            if (num27 <= num13)
                            {
                                num26 = row - (num25 - (num5 - num13));
                                num27 = num13;
                            }
                        }
                        if (num27 < num23)
                        {
                            num26 = num13;
                            num27 = num23;
                        }
                        else if (num26 > num24)
                        {
                            num26 = num24;
                        }
                        else if ((num26 < num23) && (num13 >= num23))
                        {
                            num26 = num23;
                        }
                        return new CellRange(num26, column, (num27 - num26) + 1, columnCount);
                    }
                    if ((row > 0) && (_excel.ActiveSheet.FrozenRowCount == 0))
                    {
                        int num28 = 0;
                        int num29 = num13;
                        range = new CellRange(num28, column, (num29 - num28) + 1, columnCount);
                    }
                    return range;
                }
                if (direction == NavigationDirection.PageRight)
                {
                    NavigatorHelper.ScrollToNextPageOfColumns(_excel);
                    int num30 = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                    int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                    int num32 = num30 - viewportLeftColumn;
                    if (num32 > 0)
                    {
                        int num33 = num14;
                        int num34 = num6 + num32;
                        if (column != num14)
                        {
                            num33 = column + num32;
                            num34 = num6;
                            if (num33 >= num14)
                            {
                                num33 = num14;
                                num34 = num6 + (num32 - (num14 - column));
                            }
                        }
                        if (num34 < num30)
                        {
                            num34 = num30;
                        }
                        else if (num33 > viewportRightColumn)
                        {
                            num33 = viewportRightColumn;
                            num34 = num14;
                        }
                        else if ((num34 > viewportRightColumn) && (num14 <= viewportRightColumn))
                        {
                            num34 = viewportRightColumn;
                        }
                        return new CellRange(row, num33, rowCount, (num34 - num33) + 1);
                    }
                    int num35 = (num10 - column) - columnCount;
                    if ((num35 > 0) && (_excel.ActiveSheet.FrozenTrailingColumnCount == 0))
                    {
                        int num36 = num14;
                        int num37 = num10 - 1;
                        range = new CellRange(row, num36, rowCount, (num37 - num36) + 1);
                    }
                    return range;
                }
                if (direction == NavigationDirection.PageLeft)
                {
                    NavigatorHelper.ScrollToPreviousPageOfColumns(_excel);
                    int num38 = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                    int num39 = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                    int num40 = viewportLeftColumn - num38;
                    if (num40 > 0)
                    {
                        int num41 = column - num40;
                        int num42 = num6;
                        if (num6 != num14)
                        {
                            num41 = column;
                            num42 = num6 - num40;
                            if (num42 <= num14)
                            {
                                num41 = column - (num40 - (num6 - num14));
                                num42 = num14;
                            }
                        }
                        if (num42 < num38)
                        {
                            num41 = num14;
                            num42 = num38;
                        }
                        else if (num41 > num39)
                        {
                            num41 = num39;
                        }
                        else if ((num41 < num38) && (num14 >= num38))
                        {
                            num41 = num38;
                        }
                        return new CellRange(row, num41, rowCount, (num42 - num41) + 1);
                    }
                    if ((column > 0) && (_excel.ActiveSheet.FrozenColumnCount == 0))
                    {
                        int num43 = 0;
                        int num44 = num14;
                        range = new CellRange(row, num43, rowCount, (num44 - num43) + 1);
                    }
                }
                return range;
            }

            public void KeyboardSelect(NavigationDirection direction)
            {
                if ((_formulaSelectionFeature.Items.Count == 0) || _formulaSelectionFeature.IsFlicking)
                {
                    CellRange selectionRange = GetSelectionRange();
                    if (((selectionRange == null) && (_excel != null)) && (_excel.ActiveSheet != null))
                    {
                        selectionRange = GetActiveCell();
                    }
                    if (selectionRange != null)
                    {
                        KeyboardSelectKind keyboardSelectionKind = GetKeyboardSelectionKind(direction);
                        CellRange range = null;
                        switch (keyboardSelectionKind)
                        {
                            case KeyboardSelectKind.Line:
                                range = KeyboardLineSelect(selectionRange, direction, true);
                                if (range == null)
                                {
                                    range = KeyboardLineSelect(selectionRange, direction, false);
                                }
                                break;

                            case KeyboardSelectKind.Page:
                                range = KeyboardPageSelect(selectionRange, direction);
                                break;

                            case KeyboardSelectKind.Through:
                                range = KeyboardThroughSelect(selectionRange, direction);
                                break;
                        }
                        if ((range != null) && !range.Equals(selectionRange))
                        {
                            range = GetExpandIntersectedRange(range);
                            if (selectionRange.Row < 0)
                            {
                                range = new CellRange(-1, range.Column, -1, range.ColumnCount);
                            }
                            if (selectionRange.Column < 0)
                            {
                                range = new CellRange(range.Row, -1, range.RowCount, -1);
                            }
                            _formulaSelectionFeature.ChangeLastSelection(range, false);
                        }
                    }
                }
            }

            CellRange KeyboardThroughSelect(CellRange currentRange, NavigationDirection direction)
            {
                int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
                int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
                int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
                int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
                CellRange activeCell = GetActiveCell();
                CellRange range2 = null;
                if (direction == NavigationDirection.Home)
                {
                    range2 = new CellRange(row, 0, rowCount, activeCell.Column + activeCell.ColumnCount);
                }
                else if (direction == NavigationDirection.End)
                {
                    range2 = new CellRange(row, activeCell.Column, rowCount, _excel.ActiveSheet.ColumnCount - activeCell.Column);
                }
                else if (direction == NavigationDirection.Top)
                {
                    range2 = new CellRange(0, column, activeCell.Row + activeCell.RowCount, columnCount);
                }
                else if (direction == NavigationDirection.Bottom)
                {
                    range2 = new CellRange(activeCell.Row, column, _excel.ActiveSheet.RowCount - activeCell.Row, columnCount);
                }
                else if (direction == NavigationDirection.First)
                {
                    range2 = new CellRange(_excel.ActiveSheet.FrozenRowCount, _excel.ActiveSheet.FrozenColumnCount, (activeCell.Row + activeCell.RowCount) - _excel.ActiveSheet.FrozenRowCount, (activeCell.Column + activeCell.ColumnCount) - _excel.ActiveSheet.FrozenColumnCount);
                }
                else if (direction == NavigationDirection.Last)
                {
                    range2 = new CellRange(activeCell.Row, activeCell.Column, (_excel.ActiveSheet.RowCount - _excel.ActiveSheet.FrozenTrailingRowCount) - activeCell.Row, (_excel.ActiveSheet.ColumnCount - _excel.ActiveSheet.FrozenTrailingColumnCount) - activeCell.Column);
                }
                if (range2 != null)
                {
                    int viewCellRow = range2.Row;
                    int num6 = (range2.Row + range2.RowCount) - 1;
                    int viewCellColumn = range2.Column;
                    int num8 = (range2.Column + range2.ColumnCount) - 1;
                    if ((direction == NavigationDirection.Top) || (direction == NavigationDirection.First))
                    {
                        NavigatorHelper.BringCellToVisible(_excel, viewCellRow, viewCellColumn);
                        return range2;
                    }
                    if ((direction == NavigationDirection.Home) || (direction == NavigationDirection.End))
                    {
                        int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                        int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                        int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                        if (direction == NavigationDirection.Home)
                        {
                            if (num6 < viewportTopRow)
                            {
                                NavigatorHelper.BringCellToVisible(_excel, row, viewCellColumn);
                                return range2;
                            }
                            if (viewCellRow > viewportBottomRow)
                            {
                                NavigatorHelper.BringCellToVisible(_excel, num6, viewCellColumn);
                                return range2;
                            }
                            NavigatorHelper.BringCellToVisible(_excel, viewportTopRow, viewCellColumn);
                            return range2;
                        }
                        if (num6 < viewportTopRow)
                        {
                            NavigatorHelper.BringCellToVisible(_excel, row, num8);
                            return range2;
                        }
                        if (viewCellRow > viewportBottomRow)
                        {
                            NavigatorHelper.BringCellToVisible(_excel, num6, num8);
                            return range2;
                        }
                        NavigatorHelper.BringCellToVisible(_excel, viewportTopRow, num8);
                        return range2;
                    }
                    if ((direction == NavigationDirection.Bottom) || (direction == NavigationDirection.Last))
                    {
                        NavigatorHelper.BringCellToVisible(_excel, num6, num8);
                    }
                }
                return range2;
            }

            static CellRange TabularPositionUnion(TabularPosition startPosition, TabularPosition endPosition)
            {
                int row = Math.Min(startPosition.Row, endPosition.Row);
                int column = Math.Min(startPosition.Column, endPosition.Column);
                int rowCount = Math.Abs((int)(startPosition.Row - endPosition.Row)) + 1;
                return new CellRange(row, column, rowCount, Math.Abs((int)(startPosition.Column - endPosition.Column)) + 1);
            }

            enum KeyboardSelectDirection
            {
                None,
                Left,
                Top,
                Right,
                Bottom
            }

            enum KeyboardSelectKind
            {
                None,
                Line,
                Page,
                Through
            }

            class KeyboardSelectNavigator : SpreadXTabularNavigator
            {
                public KeyboardSelectNavigator(Excel excel)
                    : base(excel)
                {
                }

                public override void BringCellToVisible(TabularPosition position)
                {
                }

                public override bool CanMoveCurrentTo(TabularPosition cellPosition)
                {
                    return (((((base._excel.ActiveSheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < base._excel.ActiveSheet.RowCount) && (cellPosition.Column >= 0))) && ((cellPosition.Column < base._excel.ActiveSheet.ColumnCount) && GetRowIsVisible(cellPosition.Row))) && GetColumnIsVisible(cellPosition.Column));
                }
            }
        }

        internal class SpreadXFormulaTabularNavigator : TabularNavigator
        {
            internal Excel _excel;

            public SpreadXFormulaTabularNavigator(Excel excel)
            {
                _excel = excel;
            }

            public override void BringCellToVisible(TabularPosition position)
            {
                if ((!position.IsEmpty && (position.Area == SheetArea.Cells)) && (_excel.ActiveSheet != null))
                {
                    NavigatorHelper.BringCellToVisible(_excel, position.Row, position.Column);
                }
            }

            public override bool CanHorizontalScroll(bool isBackward)
            {
                if (_excel == null)
                {
                    return base.CanHorizontalScroll(isBackward);
                }
                if (!_excel.HorizontalScrollable)
                {
                    return false;
                }
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                if (isBackward)
                {
                    return (_excel.GetNextPageColumnCount(activeColumnViewportIndex) > 0);
                }
                return (_excel.GetPrePageColumnCount(activeColumnViewportIndex) > 0);
            }

            public override bool CanMoveCurrentTo(TabularPosition cellPosition)
            {
                return (((((_excel.ActiveSheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < _excel.ActiveSheet.RowCount) && (cellPosition.Column >= 0))) && (((cellPosition.Column < _excel.ActiveSheet.ColumnCount) && _excel.ActiveSheet.Cells[cellPosition.Row, cellPosition.Column].ActualFocusable) && GetRowIsVisible(cellPosition.Row))) && GetColumnIsVisible(cellPosition.Column));
            }

            public override bool CanVerticalScroll(bool isBackward)
            {
                if (_excel == null)
                {
                    return base.CanVerticalScroll(isBackward);
                }
                if (!_excel.VerticalScrollable)
                {
                    return false;
                }
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                if (isBackward)
                {
                    return (_excel.GetNextPageRowCount(activeRowViewportIndex) > 0);
                }
                return (_excel.GetPrePageRowCount(activeRowViewportIndex) > 0);
            }

            public override bool GetColumnIsVisible(int columnIndex)
            {
                if ((_excel == null) || (_excel.ActiveSheet == null))
                {
                    return base.GetColumnIsVisible(columnIndex);
                }
                return (_excel.ActiveSheet.GetActualColumnVisible(columnIndex, SheetArea.Cells) && (_excel.ActiveSheet.GetActualColumnWidth(columnIndex, SheetArea.Cells) > 0.0));
            }

            public override bool GetRowIsVisible(int rowIndex)
            {
                if ((_excel == null) || (_excel.ActiveSheet == null))
                {
                    return base.GetRowIsVisible(rowIndex);
                }
                return (_excel.ActiveSheet.GetActualRowVisible(rowIndex, SheetArea.Cells) && (_excel.ActiveSheet.GetActualRowHeight(rowIndex, SheetArea.Cells) > 0.0));
            }

            public override bool IsMerged(TabularPosition position, out TabularRange range)
            {
                range = new TabularRange(position, 1, 1);
                if ((_excel.ActiveSheet != null) && (_excel.ActiveSheet.SpanModel != null))
                {
                    CellRange range2 = _excel.ActiveSheet.SpanModel.Find(position.Row, position.Column);
                    if (range2 != null)
                    {
                        range = new TabularRange(position.Area, range2.Row, range2.Column, range2.RowCount, range2.ColumnCount);
                        return true;
                    }
                }
                return false;
            }

            public override void ScrollToNextPageOfColumns()
            {
                NavigatorHelper.ScrollToNextPageOfColumns(_excel);
            }

            public override void ScrollToNextPageOfRows()
            {
                NavigatorHelper.ScrollToNextPageOfRows(_excel);
            }

            public override void ScrollToPreviousPageOfColumns()
            {
                NavigatorHelper.ScrollToPreviousPageOfColumns(_excel);
            }

            public override void ScrollToPreviousPageOfRows()
            {
                NavigatorHelper.ScrollToPreviousPageOfRows(_excel);
            }

            public override TabularRange ContentBounds
            {
                get
                {
                    if ((_excel == null) || (_excel.ActiveSheet == null))
                    {
                        return base.ContentBounds;
                    }
                    var worksheet = _excel.ActiveSheet;
                    ViewportInfo viewportInfo = worksheet.GetViewportInfo();
                    int activeRowViewportIndex = worksheet.GetActiveRowViewportIndex();
                    int activeColumnViewportIndex = worksheet.GetActiveColumnViewportIndex();
                    int row = 0;
                    int column = 0;
                    int rowCount = worksheet.RowCount;
                    int columnCount = worksheet.ColumnCount;
                    if (viewportInfo.RowViewportCount > 1)
                    {
                        if (activeRowViewportIndex > 0)
                        {
                            row = worksheet.FrozenRowCount;
                            rowCount -= worksheet.FrozenRowCount;
                        }
                        if (activeRowViewportIndex < (viewportInfo.RowViewportCount - 1))
                        {
                            rowCount -= worksheet.FrozenTrailingRowCount;
                        }
                    }
                    if (viewportInfo.ColumnViewportCount > 1)
                    {
                        if (activeColumnViewportIndex > 0)
                        {
                            column = worksheet.FrozenColumnCount;
                            columnCount -= worksheet.FrozenColumnCount;
                        }
                        if (activeColumnViewportIndex < (viewportInfo.ColumnViewportCount - 1))
                        {
                            columnCount -= worksheet.FrozenTrailingColumnCount;
                        }
                    }
                    return new TabularRange(SheetArea.Cells, row, column, rowCount, columnCount);
                }
            }

            public override TabularRange CurrentViewport
            {
                get
                {
                    int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                    int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                    if (activeColumnViewportIndex == -1)
                    {
                        activeColumnViewportIndex = 0;
                    }
                    else if (activeColumnViewportIndex == _excel.ActiveSheet.GetViewportInfo().ColumnViewportCount)
                    {
                        activeColumnViewportIndex = _excel.ActiveSheet.GetViewportInfo().ColumnViewportCount - 1;
                    }
                    if (activeRowViewportIndex == -1)
                    {
                        activeRowViewportIndex = 0;
                    }
                    else if (activeRowViewportIndex == _excel.ActiveSheet.GetViewportInfo().RowViewportCount)
                    {
                        activeRowViewportIndex = _excel.ActiveSheet.GetViewportInfo().RowViewportCount - 1;
                    }
                    int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                    int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                    int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                    int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                    double viewportWidth = _excel.GetViewportWidth(activeColumnViewportIndex);
                    double viewportHeight = _excel.GetViewportHeight(activeRowViewportIndex);
                    if (NavigatorHelper.GetColumnWidth(_excel.ActiveSheet, viewportLeftColumn, viewportRightColumn) > viewportWidth)
                    {
                        viewportRightColumn--;
                    }
                    if (NavigatorHelper.GetRowHeight(_excel.ActiveSheet, viewportTopRow, viewportBottomRow) > viewportHeight)
                    {
                        viewportBottomRow--;
                    }
                    return new TabularRange(SheetArea.Cells, viewportTopRow, viewportLeftColumn, Math.Max(1, (viewportBottomRow - viewportTopRow) + 1), Math.Max(1, (viewportRightColumn - viewportLeftColumn) + 1));
                }
            }

            public override int TotalColumnCount
            {
                get { return _excel.ActiveSheet.ColumnCount; }
            }

            public override int TotalRowCount
            {
                get { return _excel.ActiveSheet.RowCount; }
            }
        }
    }
}

