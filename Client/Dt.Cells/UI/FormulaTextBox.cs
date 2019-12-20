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
        private DispatcherTimer _doubleClickTimer;
        private string _footer;
        private List<FormulaFunction> _functionList;
        private ListBox _functionListBox = new ListBox();
        private Windows.UI.Xaml.Controls.Primitives.Popup _functionPopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
        private TextBox _functionTextBlock = new TextBox();
        private string _header;
        private int _length;
        private Windows.UI.Xaml.Controls.Primitives.Popup _namePopup = new Windows.UI.Xaml.Controls.Primitives.Popup();
        private bool _selectionChanged;
        private Excel _spreadSheet;
        private int _startIndex = -1;
        private bool _textChanged;
        private DispatcherTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UI.FormulaTextBox" /> class.
        /// </summary>
        public FormulaTextBox()
        {
            base.AcceptsReturn = false;
            this.WireEvents();
            Border border = new Border();
            border.Padding = new Windows.UI.Xaml.Thickness(2.0);
            border.BorderBrush = new SolidColorBrush(Colors.Gray);
            border.BorderThickness = new Windows.UI.Xaml.Thickness(1.0);
            border.CornerRadius = new Windows.UI.Xaml.CornerRadius(2.0);
            border.Background = new SolidColorBrush(Colors.White);
            border.Child = this._functionTextBlock;
            this._functionPopup.Child = border;
            this._functionPopup.Height = 35.0;
            this._functionListBox.MaxHeight = 130.0;
            this._functionListBox.Width = 200.0;
            ResourceDictionary dictionary = null;
            Uri uri = new Uri("ms-appx:///Dt.Cells/Themes/DataLableTemplate.xaml");
            dictionary = new ResourceDictionary();
            Application.LoadComponent(dictionary, uri);
            this._functionListBox.ItemTemplate = dictionary["ListBoxItemTemplate"] as DataTemplate;
            this._functionListBox.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(this.FunctionListBoxPointerReleased), true);
            _functionListBox.KeyUp += FunctionListBoxKeyUp;
            Border border2 = new Border();
            border2.BorderBrush = new SolidColorBrush(Colors.Gray);
            border2.BorderThickness = new Windows.UI.Xaml.Thickness(1.0);
            border2.CornerRadius = new Windows.UI.Xaml.CornerRadius(2.0);
            border2.Background = new SolidColorBrush(Colors.White);
            border2.Child = this._functionListBox;
            this._namePopup.Child = border2;
        }

        private void ChooseFunctionName()
        {
            if (this._functionListBox.SelectedItem != null)
            {
                string str = this._functionListBox.SelectedItem.ToString();
                string str2 = base.Text.Remove(this._startIndex, this._length).Insert(this._startIndex, str + "(");
                base.Text = str2;
                this._namePopup.IsOpen = false;
                this.Focus(FocusState.Programmatic);
                base.Select((this._startIndex + str.Length) + 1, 0);
                this._functionTextBlock.Text = (this._functionListBox.SelectedItem as FormulaFunction).FullName;
                Windows.Foundation.Point point = new Windows.Foundation.Point(0.0, base.ActualHeight);
                point = base.TransformToVisual(null).TransformPoint(point);
                this._functionPopup.HorizontalOffset = point.X;
                this._functionPopup.VerticalOffset = point.Y;
                this._functionPopup.IsOpen = true;
            }
        }

        private void DisposeDoubleClickTimer()
        {
            if (this._doubleClickTimer != null)
            {
                this._doubleClickTimer.Stop();
                this._doubleClickTimer = null;
            }
        }

        private void FunctionListBoxKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.ChooseFunctionName();
                this.MoveFocusToFormulaBar();
            }
        }

        private void FunctionListBoxPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (this._doubleClickTimer == null)
            {
                this._doubleClickTimer = new DispatcherTimer();
                this._doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
                DispatcherTimer timer = this._doubleClickTimer;
                _doubleClickTimer.Tick += (obj, ex) =>
                {
                    DisposeDoubleClickTimer();
                };
                this._doubleClickTimer.Start();
            }
            else
            {
                this.DisposeDoubleClickTimer();
                this.ChooseFunctionName();
                this.MoveFocusToFormulaBar();
            }
        }

        /// <summary>
        /// 找到名字开端，从而从字符串中分离出名字，名字已在字符串末端
        /// </summary>
        private int GetNameStartIndex(string s, bool forNamePopup)
        {
            bool flag = false;
            int num = 0;
            if (s != null)
            {
                s = s.ToUpper();
                num = s.Length - 1;
                while (num >= 0)
                {
                    bool? nullable = this.IsNameChar(s[num]);
                    if (!nullable.GetValueOrDefault() && nullable.HasValue)
                    {
                        return num;
                    }
                    if (!this.IsNameChar(s[num]).HasValue)
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
                            bool? nullable3 = this.IsNameChar(s[num]);
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
                        bool? nullable4 = this.IsNameChar(s[num]);
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

        private bool? IsNameChar(char c)
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

        private void MoveFocusToFormulaBar()
        {
            this.Focus(FocusState.Programmatic);
        }

        private void MoveFoucsToPopup()
        {
            this._functionListBox.Focus(FocusState.Programmatic);
            if (this._functionListBox.Items.Count > 0)
            {
                this._functionListBox.SelectedIndex = 0;
            }
        }

        private void OnEditorConnectorFormulaChangedByUI(object sender, EventArgs e)
        {
            this.UpdateText();
        }

        private void OnFormulaTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (this.SpreadSheet != null)
            {
                if (FocusManager.GetFocusedElement() == this)
                {
                    this.SpreadSheet.View.EditorConnector.Editor = this;
                }
                this._selectionChanged = true;
                this.StartTimer();
            }
        }

        private void OnFormulaTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.SpreadSheet != null)
            {
                this._textChanged = true;
                this._selectionChanged = true;
                this.ShowFunctionPopup();
                this.ShowNamePopup();
                this.StartTimer();
            }
        }

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            if (this._spreadSheet != null)
            {
                if (this.IsArrayFormula)
                {
                    Match match = new Regex(@"^\s*{\s*(=.*?)\s*}\s*$").Match(base.Text);
                    if (match.Success)
                    {
                        base.Text = match.Groups[1].Value;
                    }
                }
                this._spreadSheet.View.EditorConnector.Editor = this;
                this._textChanged = true;
                this._selectionChanged = true;
                this.OnTimerTick(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (((e.Key == VirtualKey.F4) && (this.SpreadSheet != null)) && ((this.SpreadSheet.View != null) && (this.SpreadSheet.View.EditorConnector.Editor == this)))
            {
                this.SpreadSheet.View.EditorConnector.ChangeRelative();
            }
        }

        /// <summary>
        /// Called before the KeyUp event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);
            this.ShowFunctionPopup();
            if (this._namePopup.IsOpen && ((e.Key == VirtualKey.Up) || (VirtualKey.Down == e.Key)))
            {
                this.MoveFoucsToPopup();
            }
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if ((this._spreadSheet != null) && (this._spreadSheet.View.EditorConnector.Editor == this))
            {
                this._spreadSheet.View.EndFormulaSelection();
                this._spreadSheet.View.EditorConnector.Editor = null;
            }
        }

        private void OnTimerTick(object sender, object e)
        {
            if (((this.SpreadSheet != null) && (this.SpreadSheet.View != null)) && (this.SpreadSheet.View.EditorConnector.Editor == this))
            {
                string text = base.Text;
                Match match = new Regex(@"(^\s*=\s*)(.*?)(\s*$)").Match(text);
                if (!match.Success)
                {
                    this._header = string.Empty;
                    this._footer = string.Empty;
                    this._textChanged = false;
                    this._selectionChanged = false;
                    if (this._timer != null)
                    {
                        this._timer.Stop();
                    }
                    this.SpreadSheet.View.EditorConnector.OnFormulaTextChanged(string.Empty);
                    this.SpreadSheet.View.EndFormulaSelection();
                    return;
                }
                if (this._textChanged)
                {
                    this.SpreadSheet.View.BeginFormulaSelection(null);
                    this._header = match.Groups[1].Value;
                    this._footer = match.Groups[3].Value;
                    this.SpreadSheet.View.EditorConnector.OnFormulaTextChanged(match.Groups[2].Value);
                }
                if (this._selectionChanged)
                {
                    this._selectionChanged = false;
                    int selectionStart = base.SelectionStart;
                    int end = selectionStart + base.SelectionLength;
                    if (this._header != null)
                    {
                        selectionStart -= this._header.Length;
                        end -= this._header.Length;
                    }
                    this.SpreadSheet.View.EditorConnector.OnCursorPositionChanged(selectionStart, end);
                }
                if (this._textChanged)
                {
                    this._textChanged = false;
                }
            }
            if (this._timer != null)
            {
                this._timer.Stop();
            }
        }

        private void ShowFunctionPopup()
        {
            if (base.Text == null)
            {
                this._functionPopup.IsOpen = false;
            }
            else if (base.SelectionStart < 0)
            {
                this._functionPopup.IsOpen = false;
            }
            else
            {
                string str = base.Text.Substring(0, base.SelectionStart);
                if (str.Length < 2)
                {
                    this._functionPopup.IsOpen = false;
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
                        this._functionPopup.IsOpen = false;
                    }
                    else
                    {
                        s = s.Substring(0, length);
                        int nameStartIndex = this.GetNameStartIndex(s, false);
                        string objA = s.Substring(nameStartIndex + 1, (length - nameStartIndex) - 1);
                        this._functionTextBlock.Text = "";
                        this._functionPopup.IsOpen = false;
                        objA = objA.ToUpper();
                        for (int i = 0; i < this.FunctionList.Count; i++)
                        {
                            if (object.Equals(objA, this.FunctionList[i].Name))
                            {
                                this._functionTextBlock.Text = this.FunctionList[i].FullName.Replace(',', CultureInfo.CurrentCulture.TextInfo.ListSeparator[0]);
                                Windows.Foundation.Point point = new Windows.Foundation.Point(0.0, base.ActualHeight);
                                point = base.TransformToVisual(null).TransformPoint(point);
                                this._functionPopup.HorizontalOffset = point.X;
                                this._functionPopup.VerticalOffset = point.Y;
                                if (FocusManager.GetFocusedElement() == this)
                                {
                                    this._functionPopup.IsOpen = true;
                                    return;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ShowNamePopup()
        {
            if (base.Text != null)
            {
                if (base.Text.Length == 0)
                {
                    this._namePopup.IsOpen = false;
                }
                else if (base.Text[0] == '=')
                {
                    string s = base.Text.Substring(0, base.SelectionStart).ToUpper();
                    int nameStartIndex = this.GetNameStartIndex(s, true);
                    if ((s.Length - nameStartIndex) == 1)
                    {
                        this._namePopup.IsOpen = false;
                    }
                    else
                    {
                        this._startIndex = nameStartIndex + 1;
                        this._length = (base.SelectionStart - nameStartIndex) - 1;
                        string objB = s.Substring(this._startIndex);
                        this._functionListBox.Items.Clear();
                        for (int i = 0; i < this.FunctionList.Count; i++)
                        {
                            int length = objB.Length;
                            if (this.FunctionList[i].Name.Length < length)
                            {
                                length = this.FunctionList[i].Name.Length;
                            }
                            if (object.Equals(this.FunctionList[i].Name.Substring(0, length), objB))
                            {
                                this._functionListBox.Items.Add(this.FunctionList[i]);
                            }
                        }
                        if (this._functionListBox.Items.Count > 0)
                        {
                            Windows.Foundation.Point point = new Windows.Foundation.Point(0.0, base.ActualHeight);
                            if (this._functionPopup.IsOpen)
                            {
                                point.Y += 37.0;
                            }
                            point = base.TransformToVisual(null).TransformPoint(point);
                            this._namePopup.HorizontalOffset = point.X;
                            this._namePopup.VerticalOffset = point.Y;
                            if (FocusManager.GetFocusedElement() == this)
                            {
                                this._namePopup.IsOpen = true;
                            }
                        }
                        else
                        {
                            this._namePopup.IsOpen = false;
                        }
                    }
                }
            }
        }

        private void StartTimer()
        {
            if (this._timer == null)
            {
                this._timer = new DispatcherTimer();
                this._timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                _timer.Tick += OnTimerTick;
            }
            this._timer.Stop();
            this._timer.Start();
        }

        private void UnWireEvents()
        {
            this.TextChanged += OnFormulaTextBoxTextChanged;
            this.SelectionChanged += OnFormulaTextBoxSelectionChanged;
        }

        private void UpdateText()
        {
            this.UnWireEvents();
            try
            {
                IList<SheetView.ColoredText> coloredText = this.SpreadSheet.View.EditorConnector.GetColoredText(false);
                StringBuilder builder = new StringBuilder();
                if (string.IsNullOrEmpty(this._header) && (coloredText.Count > 0))
                {
                    this._header = "=";
                }
                if (!string.IsNullOrEmpty(this._header))
                {
                    builder.Append(this._header);
                }
                foreach (SheetView.ColoredText text in coloredText)
                {
                    builder.Append(text.Text);
                }
                if (!string.IsNullOrEmpty(this._footer))
                {
                    builder.Append(this._footer);
                }
                base.Text = builder.ToString();
                int cursorPositionStart = this.SpreadSheet.View.EditorConnector.GetCursorPositionStart();
                if (this._header != null)
                {
                    cursorPositionStart += this._header.Length;
                }
                base.Select(cursorPositionStart, 0);
            }
            finally
            {
                this.WireEvents();
            }
        }

        private void WireEvents()
        {
            TextChanged += OnFormulaTextBoxTextChanged;
            SelectionChanged += OnFormulaTextBoxSelectionChanged;
        }

        private List<FormulaFunction> FunctionList
        {
            get
            {
                if (this._functionList == null)
                {
                    this._functionList = new List<FormulaFunction>((IEnumerable<FormulaFunction>)FormulaFunctionList.AllFunctions.Values);
                }
                return this._functionList;
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
            get { return this._spreadSheet; }
            set
            {
                if (this._spreadSheet != value)
                {
                    if (this._spreadSheet != null)
                    {
                        this._spreadSheet.View.EditorConnector.FormulaChangedByUI -= new EventHandler(this.OnEditorConnectorFormulaChangedByUI);
                    }
                    this._spreadSheet = value;
                    if (this._spreadSheet != null)
                    {
                        this._spreadSheet.View.EditorConnector.FormulaChangedByUI += new EventHandler(this.OnEditorConnectorFormulaChangedByUI);
                    }
                }
            }
        }
    }
}

