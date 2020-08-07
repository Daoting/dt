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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a control that allow user to input formula, and convert the formula text to formula selection in spread sheet. User also
    /// can select formula in spread sheet through mouse or other input device, the control will convert the selection to text.
    /// </summary>
    public sealed partial class FormulaTextBox : TextBox, IFormulaEditor
    {
        DispatcherTimer _doubleClickTimer;
        string _footer;
        List<FormulaFunction> _functionList;
        ListBox _functionListBox = new ListBox();
        Windows.UI.Xaml.Controls.Primitives.Popup _functionPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
        TextBox _functionTextBlock = new TextBox();
        string _header;
        int _length;
        Windows.UI.Xaml.Controls.Primitives.Popup _namePopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
        bool _selectionChanged;
        Excel _spreadSheet;
        int _startIndex = -1;
        bool _textChanged;
        DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FormulaTextBox" /> class.
        /// </summary>
        public FormulaTextBox()
        {
            base.AcceptsReturn = false;
            WireEvents();
            Border border = new Border();
            border.Padding = new Thickness(2.0);
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            border.BorderThickness = new Thickness(1.0);
            border.CornerRadius = new Windows.UI.Xaml.CornerRadius(2.0);
            border.Background = new SolidColorBrush(Colors.White);
            border.Child = _functionTextBlock;
            _functionPopup.Child = border;
            _functionPopup.Height = 35.0;
            _functionListBox.MaxHeight = 130.0;
            _functionListBox.Width = 200.0;
            ResourceDictionary dictionary = null;
            Uri uri = new Uri("ms-appx:///Dt.Cells/Themes/DataLableTemplate.xaml");
            dictionary = new ResourceDictionary();
            Application.LoadComponent(dictionary, uri);
            _functionListBox.ItemTemplate = dictionary["ListBoxItemTemplate"] as DataTemplate;
            _functionListBox.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(FunctionListBoxPointerReleased), true);
            _functionListBox.KeyUp += FunctionListBoxKeyUp;
            Border border2 = new Border();
            border2.BorderBrush = new SolidColorBrush(Colors.Gray);
            border2.BorderThickness = new Thickness(1.0);
            border2.CornerRadius = new Windows.UI.Xaml.CornerRadius(2.0);
            border2.Background = new SolidColorBrush(Colors.White);
            border2.Child = _functionListBox;
            _namePopup.Child = border2;
        }

        void ChooseFunctionName()
        {
            if (_functionListBox.SelectedItem != null)
            {
                string str = _functionListBox.SelectedItem.ToString();
                string str2 = base.Text.Remove(_startIndex, _length).Insert(_startIndex, str + "(");
                base.Text = str2;
                _namePopup.IsOpen = false;
                Focus(FocusState.Programmatic);
                base.Select((_startIndex + str.Length) + 1, 0);
                _functionTextBlock.Text = (_functionListBox.SelectedItem as FormulaFunction).FullName;
                Point point = new Point(0.0, base.ActualHeight);
                point = base.TransformToVisual(null).TransformPoint(point);
                _functionPopup.HorizontalOffset = point.X;
                _functionPopup.VerticalOffset = point.Y;
                _functionPopup.IsOpen = true;
            }
        }

        void DisposeDoubleClickTimer()
        {
            if (_doubleClickTimer != null)
            {
                _doubleClickTimer.Stop();
                _doubleClickTimer = null;
            }
        }

        void FunctionListBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ChooseFunctionName();
                MoveFocusToFormulaBar();
            }
        }

        void FunctionListBoxPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_doubleClickTimer == null)
            {
                _doubleClickTimer = new DispatcherTimer();
                _doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
                DispatcherTimer timer = _doubleClickTimer;
                _doubleClickTimer.Tick += (obj, ex) =>
                {
                    DisposeDoubleClickTimer();
                };
                _doubleClickTimer.Start();
            }
            else
            {
                DisposeDoubleClickTimer();
                ChooseFunctionName();
                MoveFocusToFormulaBar();
            }
        }

        /// <summary>
        /// 找到名字开端，从而从字符串中分离出名字，名字已在字符串末端
        /// </summary>
        int GetNameStartIndex(string s, bool forNamePopup)
        {
            bool flag = false;
            int num = 0;
            if (s != null)
            {
                s = s.ToUpper();
                num = s.Length - 1;
                while (num >= 0)
                {
                    bool? nullable = IsNameChar(s[num]);
                    if (!nullable.GetValueOrDefault() && nullable.HasValue)
                    {
                        return num;
                    }
                    if (!IsNameChar(s[num]).HasValue)
                    {
                        flag = true;
                        break;
                    }
                    num--;
                }
                if (forNamePopup)
                {
                    if ((flag && (num >= 5)) && object.Equals(s.Substring(num - 5, 5), "ERROR"))
                    {
                        num -= 6;
                        while (num >= 0)
                        {
                            bool? nullable3 = IsNameChar(s[num]);
                            if (!nullable3.GetValueOrDefault() || !nullable3.HasValue)
                            {
                                return num;
                            }
                            num--;
                        }
                    }
                    return num;
                }
                if ((flag && (s.Length > 10)) && ((num == (s.Length - 5)) && object.Equals(s.Substring(s.Length - 10, 10), "ERROR.TYPE")))
                {
                    num -= 6;
                    while (num >= 0)
                    {
                        bool? nullable4 = IsNameChar(s[num]);
                        if (!nullable4.GetValueOrDefault() || !nullable4.HasValue)
                        {
                            return num;
                        }
                        num--;
                    }
                }
            }
            return num;
        }

        bool? IsNameChar(char c)
        {
            if ((c <= 'Z') && (c >= 'A'))
            {
                return true;
            }
            if ((c <= '9') && (c >= '0'))
            {
                return true;
            }
            if (c == '.')
            {
                return null;
            }
            return false;
        }

        void MoveFocusToFormulaBar()
        {
            Focus(FocusState.Programmatic);
        }

        void MoveFoucsToPopup()
        {
            _functionListBox.Focus(FocusState.Programmatic);
            if (_functionListBox.Items.Count > 0)
            {
                _functionListBox.SelectedIndex = 0;
            }
        }

        void OnEditorConnectorFormulaChangedByUI(object sender, EventArgs e)
        {
            UpdateText();
        }

        void OnFormulaTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (SpreadSheet != null)
            {
                if (FocusManager.GetFocusedElement() == this)
                {
                    SpreadSheet.View.EditorConnector.Editor = this;
                }
                _selectionChanged = true;
                StartTimer();
            }
        }

        void OnFormulaTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SpreadSheet != null)
            {
                _textChanged = true;
                _selectionChanged = true;
                ShowFunctionPopup();
                ShowNamePopup();
                StartTimer();
            }
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (_spreadSheet != null)
            {
                if (IsArrayFormula)
                {
                    Match match = new Regex(@"^\s*{\s*(=.*?)\s*}\s*$").Match(base.Text);
                    if (match.Success)
                    {
                        base.Text = match.Groups[1].Value;
                    }
                }
                _spreadSheet.View.EditorConnector.Editor = this;
                _textChanged = true;
                _selectionChanged = true;
                OnTimerTick(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (((e.Key == VirtualKey.F4) && (SpreadSheet != null)) && ((SpreadSheet.View != null) && (SpreadSheet.View.EditorConnector.Editor == this)))
            {
                SpreadSheet.View.EditorConnector.ChangeRelative();
            }
        }

        /// <summary>
        /// Called before the KeyUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);
            ShowFunctionPopup();
            if (_namePopup.IsOpen && ((e.Key == VirtualKey.Up) || (VirtualKey.Down == e.Key)))
            {
                MoveFoucsToPopup();
            }
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if ((_spreadSheet != null) && (_spreadSheet.View.EditorConnector.Editor == this))
            {
                _spreadSheet.View.EndFormulaSelection();
                _spreadSheet.View.EditorConnector.Editor = null;
            }
        }

        void OnTimerTick(object sender, object e)
        {
            if (((SpreadSheet != null) && (SpreadSheet.View != null)) && (SpreadSheet.View.EditorConnector.Editor == this))
            {
                string text = base.Text;
                Match match = new Regex(@"(^\s*=\s*)(.*?)(\s*$)").Match(text);
                if (!match.Success)
                {
                    _header = string.Empty;
                    _footer = string.Empty;
                    _textChanged = false;
                    _selectionChanged = false;
                    if (_timer != null)
                    {
                        _timer.Stop();
                    }
                    SpreadSheet.View.EditorConnector.OnFormulaTextChanged(string.Empty);
                    SpreadSheet.View.EndFormulaSelection();
                    return;
                }
                if (_textChanged)
                {
                    SpreadSheet.View.BeginFormulaSelection(null);
                    _header = match.Groups[1].Value;
                    _footer = match.Groups[3].Value;
                    SpreadSheet.View.EditorConnector.OnFormulaTextChanged(match.Groups[2].Value);
                }
                if (_selectionChanged)
                {
                    _selectionChanged = false;
                    int selectionStart = base.SelectionStart;
                    int end = selectionStart + base.SelectionLength;
                    if (_header != null)
                    {
                        selectionStart -= _header.Length;
                        end -= _header.Length;
                    }
                    SpreadSheet.View.EditorConnector.OnCursorPositionChanged(selectionStart, end);
                }
                if (_textChanged)
                {
                    _textChanged = false;
                }
            }
            if (_timer != null)
            {
                _timer.Stop();
            }
        }

        void ShowFunctionPopup()
        {
            if (base.Text == null)
            {
                _functionPopup.IsOpen = false;
            }
            else if (base.SelectionStart < 0)
            {
                _functionPopup.IsOpen = false;
            }
            else
            {
                string str = base.Text.Substring(0, base.SelectionStart);
                if (str.Length < 2)
                {
                    _functionPopup.IsOpen = false;
                }
                else if (str[0] == '=')
                {
                    int num = 0;
                    string s = str;
                    int length = 0;
                    length = str.Length - 1;
                    while (length > 0)
                    {
                        if (s[length] == '(')
                        {
                            num++;
                        }
                        if (s[length] == ')')
                        {
                            num--;
                        }
                        if (num > 0)
                        {
                            break;
                        }
                        length--;
                    }
                    if (length == 0)
                    {
                        str.IndexOf('(');
                        _functionPopup.IsOpen = false;
                    }
                    else
                    {
                        s = s.Substring(0, length);
                        int nameStartIndex = GetNameStartIndex(s, false);
                        string objA = s.Substring(nameStartIndex + 1, (length - nameStartIndex) - 1);
                        _functionTextBlock.Text = "";
                        _functionPopup.IsOpen = false;
                        objA = objA.ToUpper();
                        for (int i = 0; i < FunctionList.Count; i++)
                        {
                            if (object.Equals(objA, FunctionList[i].Name))
                            {
                                _functionTextBlock.Text = FunctionList[i].FullName.Replace(',', CultureInfo.CurrentCulture.TextInfo.ListSeparator[0]);
                                Point point = new Point(0.0, base.ActualHeight);
                                point = base.TransformToVisual(null).TransformPoint(point);
                                _functionPopup.HorizontalOffset = point.X;
                                _functionPopup.VerticalOffset = point.Y;
                                if (FocusManager.GetFocusedElement() == this)
                                {
                                    _functionPopup.IsOpen = true;
                                    return;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        void ShowNamePopup()
        {
            if (base.Text != null)
            {
                if (base.Text.Length == 0)
                {
                    _namePopup.IsOpen = false;
                }
                else if (base.Text[0] == '=')
                {
                    string s = base.Text.Substring(0, base.SelectionStart).ToUpper();
                    int nameStartIndex = GetNameStartIndex(s, true);
                    if ((s.Length - nameStartIndex) == 1)
                    {
                        _namePopup.IsOpen = false;
                    }
                    else
                    {
                        _startIndex = nameStartIndex + 1;
                        _length = (base.SelectionStart - nameStartIndex) - 1;
                        string objB = s.Substring(_startIndex);
                        _functionListBox.Items.Clear();
                        for (int i = 0; i < FunctionList.Count; i++)
                        {
                            int length = objB.Length;
                            if (FunctionList[i].Name.Length < length)
                            {
                                length = FunctionList[i].Name.Length;
                            }
                            if (object.Equals(FunctionList[i].Name.Substring(0, length), objB))
                            {
                                _functionListBox.Items.Add(FunctionList[i]);
                            }
                        }
                        if (_functionListBox.Items.Count > 0)
                        {
                            Point point = new Point(0.0, base.ActualHeight);
                            if (_functionPopup.IsOpen)
                            {
                                point.Y += 37.0;
                            }
                            point = base.TransformToVisual(null).TransformPoint(point);
                            _namePopup.HorizontalOffset = point.X;
                            _namePopup.VerticalOffset = point.Y;
                            if (FocusManager.GetFocusedElement() == this)
                            {
                                _namePopup.IsOpen = true;
                            }
                        }
                        else
                        {
                            _namePopup.IsOpen = false;
                        }
                    }
                }
            }
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
            TextChanged += OnFormulaTextBoxTextChanged;
            SelectionChanged += OnFormulaTextBoxSelectionChanged;
        }

        void UpdateText()
        {
            UnWireEvents();
            try
            {
                IList<SheetView.ColoredText> coloredText = SpreadSheet.View.EditorConnector.GetColoredText(false);
                StringBuilder builder = new StringBuilder();
                if (string.IsNullOrEmpty(_header) && (coloredText.Count > 0))
                {
                    _header = "=";
                }
                if (!string.IsNullOrEmpty(_header))
                {
                    builder.Append(_header);
                }
                foreach (SheetView.ColoredText text in coloredText)
                {
                    builder.Append(text.Text);
                }
                if (!string.IsNullOrEmpty(_footer))
                {
                    builder.Append(_footer);
                }
                base.Text = builder.ToString();
                int cursorPositionStart = SpreadSheet.View.EditorConnector.GetCursorPositionStart();
                if (_header != null)
                {
                    cursorPositionStart += _header.Length;
                }
                base.Select(cursorPositionStart, 0);
            }
            finally
            {
                WireEvents();
            }
        }

        void WireEvents()
        {
            TextChanged += OnFormulaTextBoxTextChanged;
            SelectionChanged += OnFormulaTextBoxSelectionChanged;
        }

        List<FormulaFunction> FunctionList
        {
            get
            {
                if (_functionList == null)
                {
                    _functionList = new List<FormulaFunction>((IEnumerable<FormulaFunction>)FormulaFunctionList.AllFunctions.Values);
                }
                return _functionList;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is absolute style.
        /// </summary>
        /// <value>
        /// <c>true</c> if the formula is absolute style; otherwise, <c>false</c>.
        /// </value>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the formula is array formula.
        /// </summary>
        /// <value>
        /// <c>true</c> if formula is array formula; otherwise, <c>false</c>.
        /// </value>
        public bool IsArrayFormula { get; set; }

        /// <summary>
        /// Gets or sets the spread sheet which is owner of the FormulaTextBox.
        /// </summary>
        /// <value>
        /// The spread sheet.
        /// </value>
        public Excel SpreadSheet
        {
            get { return _spreadSheet; }
            set
            {
                if (_spreadSheet != value)
                {
                    if (_spreadSheet != null)
                    {
                        _spreadSheet.View.EditorConnector.FormulaChangedByUI -= new EventHandler(OnEditorConnectorFormulaChangedByUI);
                    }
                    _spreadSheet = value;
                    if (_spreadSheet != null)
                    {
                        _spreadSheet.View.EditorConnector.FormulaChangedByUI += new EventHandler(OnEditorConnectorFormulaChangedByUI);
                    }
                }
            }
        }
    }
}

