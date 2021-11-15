#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using Dt.Xls;
using Dt.Pdf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Graphics.Display;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Workbook : ICloneable, IXmlSerializable, INotifyPropertyChanged, IThemeSupport
    {
        bool _allowCellOverflow;
        bool _autoRefresh;
        ScrollBarVisibility _horizontalScrollBarPolicy;
        bool _protect;
        ShowResizeTip _resizeTip;
        ShowScrollTip _scrollTip;
        ScrollBarVisibility _verticalScrollBarPolicy;
        int _activeSheetIndex;
        bool _autoRecalculation;
        bool _isCalculating;
        CalcService _calcService;
        List<List<object>> _customNamesCache;
        StyleInfo _defaultStyle;
        TableStyle _defaultTableStyle;
        const string _DefaultThemeName = "Office";
        short _eventSuspended;
        ExcelOperator _excelOperator;
        IDictionary<string, CalcFunction> _functions;
        string _name;
        StyleInfoCollection _namedStyles;
        NameInfoCollection _names;
        ReferenceStyle _referenceStyle;
        WorksheetCollection _sheets;
        int _startSheetIndex;
        SpreadTheme _themeCached;
        string _themeName;
        SpreadThemes _themes;
        List<IUnsupportRecord> _unSupportExcelRecrods;
        bool _showDragDropTip;
        bool _showDragFillTip;

        public Workbook()
            : this(0)
        {
        }

        public Workbook(int sheetCount)
        {
            this._name = string.Empty;
            this._activeSheetIndex = -1;
            this._startSheetIndex = -1;
            this.Init(sheetCount);
        }

      

        /// <summary>
        /// Occurs when an error happens while loading or saving Excel-formatted files. 
        /// </summary>
        public event EventHandler<ExcelErrorEventArgs> ExcelError;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Worksheet ActiveSheet
        {
            get
            {
                if (((this.Sheets != null) && (-1 < this._activeSheetIndex)) && (this._activeSheetIndex < this.Sheets.Count))
                {
                    return this.Sheets[this._activeSheetIndex];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException(ResourceStrings.CouldnotSetActiveSheetToNonExistingSheet);
                }
                if (this.Sheets.Contains(value))
                {
                    int index = this.Sheets.IndexOf(value);
                    if (this._activeSheetIndex != index)
                    {
                        this._activeSheetIndex = index;
                        this.RaisePropertyChanged("ActiveSheet");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public int ActiveSheetIndex
        {
            get { return  this._activeSheetIndex; }
            set
            {
                if (this._activeSheetIndex != value)
                {
                    this._activeSheetIndex = value;
                    this.RaisePropertyChanged("ActiveSheetIndex");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool AutoRecalculation
        {
            get { return  this._autoRecalculation; }
            set
            {
                this._autoRecalculation = value;
                if (this.AutoRecalculation)
                {
                    this.FormulaService.Recalculate(0xc350, false);
                }
                this.RaisePropertyChanged("AutoRecalculation");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool AutoRefresh
        {
            get { return  this._autoRefresh; }
            set
            {
                if (this._autoRefresh != value)
                {
                    this._autoRefresh = value;
                    this.RaisePropertyChanged("AutoRefresh");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool CanCellOverflow
        {
            get { return  this._allowCellOverflow; }
            set
            {
                if (this._allowCellOverflow != value)
                {
                    this._allowCellOverflow = value;
                    this.RaisePropertyChanged("CanCellOverflow");
                }
            }
        }

        public SpreadTheme CurrentTheme
        {
            get
            {
                if (this._themeCached == null)
                {
                    foreach (SpreadTheme theme in this.Themes)
                    {
                        if ((theme != null) && string.Equals(theme.Name, this.CurrentThemeName))
                        {
                            this._themeCached = theme;
                            break;
                        }
                    }
                }
                if (this._themeCached == null)
                {
                    this._themeCached = SpreadThemes.Office;
                }
                return this._themeCached;
            }
            set
            {
                this._themeName = (value == null) ? string.Empty : value.Name;
                this._themeCached = value;
                this.RaisePropertyChanged("CurrentTheme");
            }
        }

        public string CurrentThemeName
        {
            get { return  this._themeName; }
            set
            {
                bool flag = false;
                foreach (SpreadTheme theme in this.Themes)
                {
                    if ((theme != null) && string.Equals(theme.Name, value))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    throw new NotSupportedException(string.Format(ResourceStrings.CouldnotSetCurrentThemeToNonExistingTheme, (object[])new object[] { value }));
                }
                this._themeName = value;
                this._themeCached = null;
                this.RaisePropertyChanged("CurrentThemeName");
            }
        }

        public string[] CustomFunctions
        {
            get { return  Enumerable.ToArray<string>((IEnumerable<string>)this.Functions.Keys); }
        }

        public string[] CustomNames
        {
            get { return  this.Names.GetNames(); }
        }

        internal StyleInfo DefaultStyle
        {
            get
            {
                if (this._defaultStyle == null)
                {
                    this._defaultStyle = new StyleInfo();
                    this._defaultStyle.Background = new SolidColorBrush(Colors.Transparent);
                    this._defaultStyle.Foreground = new SolidColorBrush(Colors.Black);
                    this._defaultStyle.FontFamily = DefaultStyleCollection.DefaultFontFamily;
                    this._defaultStyle.FontSize = DefaultStyleCollection.DefaultFontSize;
                    this._defaultStyle.Formatter = new GeneralFormatter();
                }
                return this._defaultStyle;
            }
            set
            {
                if (this._defaultStyle != value)
                {
                    this._defaultStyle = value;
                    StyleInfoCollection.IncreaseStyleInfoVersion();
                }
                this.RaisePropertyChanged("DefaultStyle");
            }
        }

        internal TableStyle DefaultTableStyle
        {
            get
            {
                if (this._defaultTableStyle != null)
                {
                    return this._defaultTableStyle;
                }
                return TableStyles.Medium2;
            }
            set { this._defaultTableStyle = (value == null) ? TableStyles.Medium2 : value; }
        }

        internal ExcelOperator ExcelOperator
        {
            get
            {
                if (this._excelOperator == null)
                {
                    this._excelOperator = new ExcelOperator(new ExcelReader(this), new ExcelWriter(this), null);
                }
                return this._excelOperator;
            }
            set { this._excelOperator = value; }
        }

        internal CalcService FormulaService
        {
            get
            {
                if (this._calcService == null)
                {
                    this._calcService = new CalcService();
                }
                return this._calcService;
            }
        }

        internal IDictionary<string, CalcFunction> Functions
        {
            get
            {
                if (this._functions == null)
                {
                    this._functions = (IDictionary<string, CalcFunction>)new Dictionary<string, CalcFunction>();
                }
                return this._functions;
            }
        }

        public Color GridLineColor
        {
            get
            {
                if (this.Sheets.Count > 0)
                {
                    return this.Sheets[0].GridLineColor;
                }
                return Worksheet.DefaultGridLineColor;
            }
            set
            {
                using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.GridLineColor = value;
                    }
                }
                this.RaisePropertyChanged("GridLineColor");
            }
        }

        bool IsFormulaSuspended
        {
            get
            {
                using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.IsFormulaSuspended)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public Worksheet this[int index]
        {
            get
            {
                if ((index < 0) || (index >= this.SheetCount))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return this.Sheets[index];
            }
        }

        public Worksheet this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                return this.Sheets[name];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string Name
        {
            get { return  this._name; }
            set
            {
                this._name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public StyleInfoCollection NamedStyles
        {
            get { return  this._namedStyles; }
            set
            {
                StyleInfoCollection namedStyles = this._namedStyles;
                if (value == null)
                {
                    this._namedStyles = new StyleInfoCollection();
                }
                else
                {
                    this._namedStyles = value;
                }
                foreach (Worksheet worksheet in this.Sheets)
                {
                    if (worksheet.NamedStyles == namedStyles)
                    {
                        worksheet.NamedStyles = value;
                    }
                }
                if (value != null)
                {
                    value.Owner = this;
                }
                this.RaisePropertyChanged("NamedStyles");
            }
        }

        internal NameInfoCollection Names
        {
            get
            {
                if (this._names == null)
                {
                    this._names = new NameInfoCollection();
                    this._names.Changed += new EventHandler<NameInfoCollectionChangedEventArgs>(this.OnNameInfoCollectionChanged);
                }
                return this._names;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Protect
        {
            get { return  this._protect; }
            set
            {
                if (value != this._protect)
                {
                    this._protect = value;
                    this.RaisePropertyChanged("Protect");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCalculating
        {
            get { return this._isCalculating; }
            set { this._isCalculating = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public ReferenceStyle ReferenceStyle
        {
            get { return  this._referenceStyle; }
            set
            {
                this._referenceStyle = value;
                using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ReferenceStyle = value;
                    }
                }
                this.RaisePropertyChanged("ReferenceStyle");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public int SheetCount
        {
            get
            {
                if (this.Sheets != null)
                {
                    return this.Sheets.Count;
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("SheetCount");
                }
                if (this.Sheets.Count != value)
                {
                    this.Sheets.Count = value;
                    this.RaisePropertyChanged("SheetCount");
                }
            }
        }

        public WorksheetCollection Sheets
        {
            get { return  this._sheets; }
            private set
            {
                if (this._sheets != value)
                {
                    if (this._sheets != null)
                    {
                        this._sheets.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnSheetsChanged);
                    }
                    this._sheets = value;
                    if (this._sheets != null)
                    {
                        this._sheets.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSheetsChanged);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool ShowGridLine
        {
            get
            {
                if (this.Sheets.Count > 0)
                {
                    return this.Sheets[0].ShowGridLine;
                }
                return true;
            }
            set
            {
                using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ShowGridLine = value;
                    }
                }
                this.RaisePropertyChanged("ShowGridLine");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(-1)]
        public int StartSheetIndex
        {
            get { return  this._startSheetIndex; }
            set
            {
                if ((value >= this.SheetCount) || (value < 0))
                {
                    throw new ArgumentOutOfRangeException("StartSheetIndex");
                }
                if (this._startSheetIndex != value)
                {
                    this._startSheetIndex = value;
                    this.RaisePropertyChanged("StartSheetIndex");
                }
            }
        }

        public SpreadThemes Themes
        {
            get
            {
                if (this._themes == null)
                {
                    this._themes = new SpreadThemes();
                    foreach (SpreadTheme theme in SpreadThemes.All)
                    {
                        this._themes.Add(theme.Clone() as SpreadTheme);
                    }
                }
                return this._themes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal List<IUnsupportRecord> UnSupportExcelRecrods
        {
            get
            {
                if (this._unSupportExcelRecrods == null)
                {
                    this._unSupportExcelRecrods = new List<IUnsupportRecord>();
                }
                return this._unSupportExcelRecrods;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return  this._horizontalScrollBarPolicy; }
            set
            {
                if (value != this._horizontalScrollBarPolicy)
                {
                    this._horizontalScrollBarPolicy = value;
                    this.RaisePropertyChanged("HorizontalScrollBarVisibility");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return  this._verticalScrollBarPolicy; }
            set
            {
                if (value != this._verticalScrollBarPolicy)
                {
                    this._verticalScrollBarPolicy = value;
                    this.RaisePropertyChanged("VerticalScrollBarVisibility");
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowDragDropTip
        {
            get
            {
                return this._showDragDropTip;
            }
            set
            {
                if (value != this.ShowDragDropTip)
                {
                    this._showDragDropTip = value;
                    this.RaisePropertyChanged("ShowDragDropTip");
                }
            }
        }

        [DefaultValue(true)]
        public bool ShowDragFillTip
        {
            get
            {
                return this._showDragFillTip;
            }
            set
            {
                if (value != this.ShowDragFillTip)
                {
                    this._showDragFillTip = value;
                    this.RaisePropertyChanged("ShowDragFillTip");
                }
            }
        }

        [DefaultValue(0)]
        public ShowResizeTip ShowResizeTip
        {
            get
            {
                return this._resizeTip;
            }
            set
            {
                if (value != this.ShowResizeTip)
                {
                    this._resizeTip = value;
                    this.RaisePropertyChanged("ShowResizeTip");
                }
            }
        }

        [DefaultValue(0)]
        public ShowScrollTip ShowScrollTip
        {
            get
            {
                return this._scrollTip;
            }
            set
            {
                if (value != this.ShowScrollTip)
                {
                    this._scrollTip = value;
                    this.RaisePropertyChanged("ShowScrollTip");
                }
            }
        }

        public void AddCustomFunction(CalcFunction function)
        {
            string name = function.Name.ToUpper(CultureInfo.CurrentCulture);
            if (this.Functions.ContainsKey(name))
            {
                this.Functions[name] = function;
            }
            else
            {
                this.Functions.Add(name, function);
            }
            this.OnCustomFunctionChanged(name);
        }

        public void ClearCustomFunctions()
        {
            string[] strArray = Enumerable.ToArray<string>((IEnumerable<string>)this.Functions.Keys);
            this.Functions.Clear();
            foreach (string str in strArray)
            {
                this.OnCustomFunctionChanged(str);
            }
        }

        public void ClearCustomName(string name)
        {
            this.Names.Remove(name);
            this.OnCustomNameChanged(name, true);
        }

        public void ClearCustomNames()
        {
            string[] names = this.Names.GetNames();
            this.Names.Clear();
            foreach (string str in names)
            {
                this.OnCustomNameChanged(str, true);
            }
        }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            try
            {
                XmlWriter writer = XmlWriter.Create((Stream)stream);
                if (writer != null)
                {
                    this.SaveXml(writer, false, true);
                    writer.Close();
                    stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                    XmlReader reader = XmlReader.Create((Stream)stream);
                    if (reader != null)
                    {
                        Workbook workbook = new Workbook();
                        workbook.OpenXml(reader);
                        reader.Close();
                        return workbook;
                    }
                }
            }
            finally
            {
                ((Stream)stream).Dispose();
            }
            return null;
        }

        public static void CopyTo(Worksheet source, int sourceRow, int sourceColumn, Worksheet dest, int destRow, int destColumn, int rowCount, int columnCount, CopyToOption copyOption)
        {
            Worksheet.CopyTo(source, sourceRow, sourceColumn, dest, destRow, destColumn, rowCount, columnCount, copyOption);
        }

        public SheetTable FindTable(string tableName, out Worksheet sheet)
        {
            if (this._sheets != null)
            {
                foreach (Worksheet worksheet in this._sheets)
                {
                    SheetTable table = worksheet.FindTable(tableName);
                    if (table != null)
                    {
                        sheet = worksheet;
                        return table;
                    }
                }
            }
            sheet = null;
            return null;
        }

        public CalcFunction GetCustomFunction(string name)
        {
            name = name.ToUpper(CultureInfo.CurrentCulture);
            if (this.Functions.ContainsKey(name))
            {
                return this.Functions[name];
            }
            return null;
        }

        public NameInfo GetCustomName(string name)
        {
            return this.Names.Find(name);
        }

        public Color GetThemeColor(string themeColor)
        {
            if ((this.CurrentTheme != null) && (this.CurrentTheme.Colors != null))
            {
                return this.CurrentTheme.Colors.GetThemeColor(themeColor);
            }
            return Colors.Transparent;
        }

        public FontFamily GetThemeFont(string themeFont)
        {
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(StyleInfo.FONTTHEME_BODY, themeFont, (CompareOptions)CompareOptions.IgnoreCase) == 0)
            {
                return this.CurrentTheme.BodyFontFamily;
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(StyleInfo.FONTTHEME_HEADING, themeFont, (CompareOptions)CompareOptions.IgnoreCase) == 0)
            {
                return this.CurrentTheme.HeadingFontFamily;
            }
            return null;
        }

        void Init(int sheetCount)
        {
            if ((sheetCount < 0) && (sheetCount > 0xff))
            {
                throw new ArgumentOutOfRangeException("sheetCount");
            }
            if (this._sheets != null)
            {
                using (IEnumerator<Worksheet> enumerator = this._sheets.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ResumeSpanModelEvent();
                    }
                }
            }
            this._eventSuspended = 0;
            this._name = string.Empty;
            if (this._sheets != null)
            {
                this._sheets.Clear();
            }
            else
            {
                this._sheets = new WorksheetCollection(this);
                this._sheets.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnSheetsChanged);
            }
            this._namedStyles = new StyleInfoCollection();
            this._sheets.Count = sheetCount;
            this._activeSheetIndex = (sheetCount > 0) ? 0 : -1;
            this._startSheetIndex = (sheetCount > 0) ? 0 : -1;
            if (this._names != null)
            {
                this._names.Changed -= new EventHandler<NameInfoCollectionChangedEventArgs>(this.OnNameInfoCollectionChanged);
                this._names = null;
            }
            this._functions = null;
            this._referenceStyle = ReferenceStyle.A1;
            this._autoRecalculation = true;
            this._themeCached = null;
            this._themeName = "Office";
            this._themes = null;
            this._defaultStyle = null;
            this._horizontalScrollBarPolicy = ScrollBarVisibility.Visible;
            this._verticalScrollBarPolicy = ScrollBarVisibility.Visible;
            this._protect = false;
            this._allowCellOverflow = false;
            this._defaultTableStyle = TableStyles.Medium2;
            TableStyles.RemoveAllCustomStyles();
            this._autoRefresh = true;
            this._scrollTip = ShowScrollTip.None;
            this._resizeTip = ShowResizeTip.None;
        }

        internal bool IsEventSuspend()
        {
            return (this._eventSuspended > 0);
        }

        public static void MoveTo(Worksheet source, int sourceRow, int sourceColumn, Worksheet dest, int destRow, int destColumn, int rowCount, int columnCount, CopyToOption copyOption)
        {
            Worksheet.MoveTo(source, sourceRow, sourceColumn, dest, destRow, destColumn, rowCount, columnCount, copyOption);
        }

        void OnCustomFunctionChanged(string name)
        {
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.OnCustomFunctionChanged(name);
                }
            }
        }

        void OnCustomNameChanged(string name, bool refreshNode)
        {
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.OnCustomNameChanged(name, refreshNode);
                }
            }
        }

        internal void OnExcelError(ExcelWarning warning)
        {
            this.RaiseExcelError(warning);
        }

        void OnNameInfoCollectionChanged(object sender, NameInfoCollectionChangedEventArgs e)
        {
            if (!this.IsFormulaSuspended)
            {
                this.FormulaService.Recalculate(0xc350, true);
            }
            this.RaisePropertyChanged("Names");
        }

        void OnSheetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsFormulaSuspended)
            {
                this.FormulaService.Recalculate(0xc350, true);
            }
            this.RaisePropertyChanged("Sheets");
        }

        public IAsyncAction OpenExcelAsync(Stream stream, ExcelOpenFlags openFlags = ExcelOpenFlags.NoFlagsSet)
        {
            return AsyncInfo.Run(delegate (CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    OpenExcel(stream, openFlags);
                });
            });
        }

        public void OpenExcel(Stream stream, ExcelOpenFlags openFlags)
        {
            ExtendedNumberFormatHelper.Reset();
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResetCalcEngine();
                }
            }
            Reset();
            SuspendEvent();
            ExcelReader reader = new ExcelReader(this, openFlags);
            try
            {
                this.ExcelOperator = new ExcelOperator(reader, null);
                this.ExcelOperator.Open(stream, -1, null);
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                }
                ResumeEvent();
                RaisePropertyChanged("[OpenExcel]");
            }
        }

        public void OpenXml(Stream xmlStream)
        {
            this.OpenXmlOnBackground(xmlStream);
        }

        public void OpenXml(XmlReader reader)
        {
            this.OpenXmlOnBackground(reader);
        }

        public IAsyncAction OpenXmlAsync(Stream xmlStream)
        {
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    this.OpenXmlOnBackground(xmlStream);
                });
            });
        }

        public IAsyncAction OpenXmlAsync(XmlReader reader)
        {
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    this.OpenXmlOnBackground(reader);
                });
            });
        }

        internal void OpenXmlOnBackground(Stream xmlStream)
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(xmlStream))
                {
                    this.OpenXmlOnBackground(reader);
                }
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
        }

        internal void OpenXmlOnBackground(XmlReader reader)
        {
            this.SuspendEvent();
            this.SuspendCalcService();
            try
            {
                Serializer.InitReader(reader);
                this.ReadXmlPrivate(reader);
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
            finally
            {
                ResumeCalcService();
                this.ResumeEvent();
                this.RaisePropertyChanged("[OpenXml]");
            }
        }

        void RaiseExcelError(ExcelWarning warning)
        {
            if (this.ExcelError != null)
            {
                this.ExcelError(this, new ExcelErrorEventArgs(warning));
            }
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (!this.IsEventSuspend() && (this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Recalculate()
        {
            this.FormulaService.Recalculate(0xc350, false);
        }

        void ReadXmlPrivate(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.SuspendEvent();
            this.Sheets.SuspendEvent();
            this.Init(0);

            List<Worksheet> list = new List<Worksheet>();
            List<SpreadTheme> list2 = null;
            this._name = Serializer.ReadAttribute("name", reader);
            string str = Serializer.ReadAttribute("theme", reader);
            if (this._themeName != null)
            {
                this._themeName = str;
            }

            while (reader.Read())
            {
                XmlReader reader2;
                if (reader.NodeType == XmlNodeType.Element && !string.IsNullOrEmpty(reader.Name))
                {
                    switch (reader.Name)
                    {
                        case "ActiveSheetIndex":
                            this._activeSheetIndex = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "StartSheetIndex":
                            this._startSheetIndex = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                            break;

                        case "Sheets":
                            Serializer.ReadAttributeInt("count", 0, reader);
                            reader2 = Serializer.ExtractNode(reader);
                            Serializer.InitReader(reader2);
                            while (reader2.Read())
                            {
                                if (reader2.NodeType == XmlNodeType.Element && "Sheet" == reader2.Name)
                                {
                                    XmlReader stReader = Serializer.ExtractNode(reader2);
                                    Worksheet worksheet = new Worksheet();
                                    list.Add(worksheet);
                                    worksheet.ReadXmlInternal(reader, false);
                                    stReader.Close();
                                }
                            }
                            reader2.Close();
                            break;

                        case "Names":
                            if (this._customNamesCache == null)
                            {
                                this._customNamesCache = new List<List<object>>();
                            }
                            Serializer.DeserializeNameInfos(reader, this._customNamesCache);
                            break;

                        case "NamedStyles":
                            this._namedStyles = Serializer.DeserializeObj(typeof(StyleInfoCollection), reader) as StyleInfoCollection;
                            break;

                        case "DefaultStyle":
                            this._defaultStyle = Serializer.DeserializeObj(typeof(StyleInfo), reader) as StyleInfo;
                            break;

                        case "Themes":
                            list2 = Serializer.DeserializeObj(typeof(List<SpreadTheme>), reader) as List<SpreadTheme>;
                            break;

                        case "HorizontalScrollBarPolicy":
                            this._horizontalScrollBarPolicy = (ScrollBarVisibility)Serializer.DeserializeObj(typeof(ScrollBarVisibility), reader);
                            break;

                        case "VerticalScrollBarPolicy":
                            this._verticalScrollBarPolicy = (ScrollBarVisibility)Serializer.DeserializeObj(typeof(ScrollBarVisibility), reader);
                            break;

                        case "Protect":
                            this._protect = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "AllowCellOverflow":
                            this._allowCellOverflow = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "DefaultTableStyle":
                            this._defaultTableStyle = Serializer.DeserializeObj(typeof(TableStyle), reader) as TableStyle;
                            break;

                        case "ReferenceStyle":
                            this._referenceStyle = (ReferenceStyle)Serializer.DeserializeObj(typeof(ReferenceStyle), reader);
                            break;

                        case "AutoCalculation":
                            this._autoRecalculation = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                            break;

                        case "ShowResizeTip":
                            this._resizeTip = (ShowResizeTip)Serializer.DeserializeObj(typeof(ShowResizeTip), reader);
                            break;

                        case "ShowScrollTip":
                            this._scrollTip = (ShowScrollTip)Serializer.DeserializeObj(typeof(ShowScrollTip), reader);
                            break;
                    }
                }
            }

            if (list2 != null && list2.Count > 0)
            {
                this._themes = new SpreadThemes();
                this._themes.AddRange((IList<SpreadTheme>)list2);
                foreach (SpreadTheme theme in SpreadThemes.All)
                {
                    if (!this._themes.Contains(theme.Name))
                    {
                        this._themes.Add(theme);
                    }
                }
            }

            if (this._namedStyles == null)
            {
                this._namedStyles = new StyleInfoCollection();
            }

            foreach (Worksheet worksheet2 in list)
            {
                this.Sheets.Add(worksheet2);
                NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, worksheet2, this.Sheets.Count - 1);
                this.Sheets.RaiseCollectionChanged(worksheet2, args);
            }

            if (this._names == null)
            {
                this._names = new NameInfoCollection();
                this._names.Changed += new EventHandler<NameInfoCollectionChangedEventArgs>(this.OnNameInfoCollectionChanged);
            }

            Serializer.UpdateCutomNameString(this._names, (this.SheetCount > 0) ? ((ICalcEvaluator)this.Sheets[0]) : new Worksheet(), this._customNamesCache);
            this._customNamesCache = null;
            using (IEnumerator<Worksheet> enumerator2 = this.Sheets.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.InvalidateBeforeRecalculate();
                }
            }
            this.FormulaService.Recalculate(0xc350, false);
            using (IEnumerator<Worksheet> enumerator3 = this.Sheets.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.InvalidateAfterRecalculate();
                }
            }
            this.Sheets.ResumeEvent();
            this.ResumeEvent();
        }

        public void RemoveCustomFunctions(string name)
        {
            name = name.ToUpper(CultureInfo.CurrentCulture);
            if (this.Functions.ContainsKey(name))
            {
                this.Functions.Remove(name);
                this.OnCustomFunctionChanged(name);
            }
        }

        public void Reset()
        {
            this.Init(0);
        }


        public void ResetThemes()
        {
            this._themes = null;
            this._themeCached = null;
        }

        public void ResumeCalcService()
        {
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResumeCalcService(false);
                }
            }
            if (!this.IsFormulaSuspended)
            {
                this.FormulaService.Recalculate(0xc350, true);
            }
        }

        public void ResumeEvent()
        {
            this._eventSuspended--;
            if (this._eventSuspended < 0)
            {
                this._eventSuspended = 0;
            }
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResumeSpanModelEvent();
                }
            }
        }

        public IAsyncAction SaveExcelAsync(Stream stream, ExcelFileFormat workbookType, ExcelSaveFlags saveFlags)
        {
            if ((this.Sheets == null) || (this.Sheets.Count == 0))
            {
                throw new InvalidOperationException(ResourceStrings.SaveEmptyWorkbookToExcelError);
            }
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    this.SaveExcel(stream, workbookType, saveFlags);
                });
            });
        }

        public void SaveExcel(Stream stream, ExcelFileFormat workbookType, ExcelSaveFlags saveFlags)
        {
            ExcelWriter writer = new ExcelWriter(this, saveFlags);
            this.ExcelOperator = new ExcelOperator(null, writer, null);
            this.ExcelOperator.Save(stream, (ExcelFileType)workbookType, null);
        }

        public IAsyncAction SavePdfAsync(Stream stream, int[] sheetIndexs, PdfExportSettings settings)
        {
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    SavePdf(stream, sheetIndexs, settings);
                });
            });
        }

        public void SavePdf(Stream stream, int[] sheetIndexs, PdfExportSettings settings)
        {
            if (stream == null || !stream.CanWrite)
                throw new ArgumentException();

            if (this.Sheets.Count > 0)
            {
                List<int> list = new List<int>();
                if ((sheetIndexs == null) || (sheetIndexs.Length <= 0))
                {
                    for (int k = 0; k < this.Sheets.Count; k++)
                    {
                        list.Add(k);
                    }
                }
                else
                {
                    for (int m = 0; m < sheetIndexs.Length; m++)
                    {
                        int num3 = sheetIndexs[m];
                        if ((num3 < 0) || (num3 >= this.Sheets.Count))
                        {
                            throw new ArgumentOutOfRangeException("sheetIndex");
                        }
                        list.Add(num3);
                    }
                }
                // hdt
                OpenTypeFontUtility measure = new OpenTypeFontUtility(GcReportContext.defaultFont, (int)UnitManager.Dpi);
                Dictionary<Worksheet, GcReportContext> dictionary = new Dictionary<Worksheet, GcReportContext>();
                List<GcReportContext> list2 = new List<GcReportContext>();
                int pageCount = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    Worksheet worksheet = this.Sheets[list[i]];
                    GcReportContext context = null;
                    bool flag = false;
                    if (dictionary.ContainsKey(worksheet))
                    {
                        context = dictionary[worksheet];
                    }
                    else
                    {
                        flag = true;
                        context = new GcReportContext(worksheet.GetPrintableReport(), (int)UnitManager.Dpi, measure)
                        {
                            UnitsPerInch = 0x48
                        };
                        context.GeneratePageBlocks();
                    }
                    pageCount += context.GetPageCount();
                    list2.Add(context);
                    if (flag)
                    {
                        dictionary.Add(worksheet, context);
                    }
                }
                PdfDocument doc = PdfExporter.CreatePdfDocument();
                int num6 = 1;
                for (int j = 0; j < list2.Count; j++)
                {
                    GcReportContext frc = list2[j];
                    GcReport report = frc.Report;
                    if (j == 0)
                    {
                        report.Bookmark = string.IsNullOrEmpty(this.Name) ? "Workbook" : this.Name;
                    }
                    else
                    {
                        report.FirstPageNumber = num6;
                    }
                    num6 += frc.GetPageCount();
                    new PdfExporter(report, settings) { Dpi = frc.Dpi }.ExportInner(stream, doc, false, j == (list2.Count - 1), j == (list2.Count - 1), measure, frc, pageCount);
                }
                PdfExporter.SavePdfDocment(doc, stream);
            }
            else
            {
                new PdfExporter(new GcReport(), settings).Export(stream);
            }
        }

        public void SaveXml(Stream xmlStream)
        {
            this.SaveXml(xmlStream, false);
        }

        public void SaveXml(XmlWriter writer)
        {
            this.SaveXml(writer, false);
        }

        public void SaveXml(Stream xmlStream, bool dataOnly)
        {
            this.SaveXml(xmlStream, dataOnly, false);
        }

        public void SaveXml(XmlWriter writer, bool dataOnly)
        {
            this.SaveXml(writer, dataOnly, false);
        }

        public void SaveXml(Stream xmlStream, bool dataOnly, bool saveDataSource)
        {
            this.SaveXmlBackGround(xmlStream, dataOnly, saveDataSource);
        }

        public void SaveXml(XmlWriter writer, bool dataOnly, bool saveDataSource)
        {
            this.SaveXmlBackGround(writer, dataOnly, saveDataSource);
        }

        public IAsyncAction SaveXmlAsync(Stream xmlStream)
        {
            return this.SaveXmlAsync(xmlStream, false);
        }

        public IAsyncAction SaveXmlAsync(XmlWriter writer)
        {
            return this.SaveXmlAsync(writer, false);
        }

        public IAsyncAction SaveXmlAsync(Stream xmlStream, bool dataOnly)
        {
            return this.SaveXmlAsync(xmlStream, dataOnly, false);
        }

        public IAsyncAction SaveXmlAsync(XmlWriter writer, bool dataOnly)
        {
            return this.SaveXmlAsync(writer, dataOnly, false);
        }

        public IAsyncAction SaveXmlAsync(Stream xmlStream, bool dataOnly, bool saveDataSource)
        {
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    this.SaveXmlBackGround(xmlStream, dataOnly, saveDataSource);
                });
            });
        }

        public IAsyncAction SaveXmlAsync(XmlWriter writer, bool dataOnly, bool saveDataSource)
        {
            return AsyncInfo.Run(delegate(CancellationToken token)
            {
                return Task.Factory.StartNew(delegate
                {
                    this.SaveXmlBackGround(writer, dataOnly, saveDataSource);
                });
            });
        }

        void SaveXmlBackGround(Stream xmlStream, bool dataOnly, bool saveDataSource)
        {
            try
            {
                XmlWriter writer = XmlWriter.Create(xmlStream);
                if (writer != null)
                {
                    this.SaveXmlBackGround(writer, dataOnly, saveDataSource);
                    writer.Close();
                }
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
        }

        void SaveXmlBackGround(XmlWriter writer, bool dataOnly, bool saveDataSource)
        {
            try
            {
                writer.WriteStartElement("Workbook");
                this.WriteXmlPrivate(writer, dataOnly);
                writer.WriteEndElement();
            }
            catch (Exception exception)
            {
                while ((exception is TargetInvocationException) && (exception.InnerException != null))
                {
                    exception = exception.InnerException;
                }
                throw exception;
            }
        }

        public void Search(int sheetIndex, string searchString, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            this.Sheets[sheetIndex].Search(searchString, out foundRowIndex, out foundColumnIndex);
        }

        public void Search(int sheetIndex, string searchString, SearchFlags searchFlags, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            this.Sheets[sheetIndex].Search(searchString, searchFlags, out foundRowIndex, out foundColumnIndex);
        }


        public void Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            this.Sheets[sheetIndex].Search(searchString, searchFlags, searchOrder, out foundRowIndex, out foundColumnIndex);
        }

        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet worksheet = this.Sheets[sheetIndex];
            return worksheet.Search(searchString, searchFlags, searchOrder, searchTarget, out foundRowIndex, out foundColumnIndex);
        }

        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet worksheet = this.Sheets[sheetIndex];
            return worksheet.Search(searchString, searchFlags, searchOrder, searchTarget, sheetArea, out foundRowIndex, out foundColumnIndex);
        }

        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet worksheet = this.Sheets[sheetIndex];
            return worksheet.Search(searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, out foundRowIndex, out foundColumnIndex);
        }

        public SearchFoundFlags Search(int sheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundRowIndex, out int foundColumnIndex)
        {
            if ((0 > sheetIndex) || (sheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException("sheetIndex");
            }
            Worksheet worksheet = this.Sheets[sheetIndex];
            return worksheet.Search(searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundRowIndex, out foundColumnIndex);
        }

        public SearchFoundFlags Search(int startSheetIndex, int endSheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundSheetIndex, out int foundRowIndex, out int foundColumnIndex)
        {
            foundSheetIndex = -1;
            foundRowIndex = -1;
            foundColumnIndex = -1;
            if ((((endSheetIndex < startSheetIndex) || (0 > startSheetIndex)) || ((startSheetIndex >= this.SheetCount) || (0 > endSheetIndex))) || (endSheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException(ResourceStrings.SearchArgumentOutOfRange);
            }
            for (int i = startSheetIndex; i <= endSheetIndex; i++)
            {
                SearchFoundFlags flags = this.Sheets[i].Search(searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundRowIndex, out foundColumnIndex);
                if (flags != SearchFoundFlags.None)
                {
                    foundSheetIndex = i;
                    return flags;
                }
            }
            return SearchFoundFlags.None;
        }

        public SearchFoundFlags Search(int startSheetIndex, int endSheetIndex, string searchString, SearchFlags searchFlags, SearchOrder searchOrder, SearchFoundFlags searchTarget, SheetArea sheetArea, int rowStart, int columnStart, int rowEnd, int columnEnd, out int foundSheetIndex, out int foundRowIndex, out int foundColumnIndex, out string foundString)
        {
            foundSheetIndex = -1;
            foundString = string.Empty;
            foundRowIndex = -1;
            foundColumnIndex = -1;
            if ((((endSheetIndex < startSheetIndex) || (0 > startSheetIndex)) || ((startSheetIndex >= this.SheetCount) || (0 > endSheetIndex))) || (endSheetIndex >= this.SheetCount))
            {
                throw new ArgumentOutOfRangeException(ResourceStrings.SearchArgumentOutOfRange);
            }
            for (int i = startSheetIndex; i <= endSheetIndex; i++)
            {
                SearchFoundFlags flags = this.Sheets[i].Search(searchString, searchFlags, searchOrder, searchTarget, sheetArea, rowStart, columnStart, rowEnd, columnEnd, out foundRowIndex, out foundColumnIndex, out foundString);
                if (flags != SearchFoundFlags.None)
                {
                    foundSheetIndex = i;
                    return flags;
                }
            }
            return SearchFoundFlags.None;
        }

        public void SetCustomName(string name, int baseRow, int baseColumn, CalcExpression expression)
        {
            this.SetCustomNameInternal(name, baseRow, baseColumn, expression, true);
        }

        public void SetCustomName(string name, Worksheet worksheet, int baseRow, int baseColumn, string formula)
        {
            CalcExpression expression = ((ICalcEvaluator)worksheet).Formula2Expression(formula, baseRow, baseColumn) as CalcExpression;
            this.Names.Add(new NameInfo(name, baseRow, baseColumn, expression));
            this.OnCustomNameChanged(name, true);
        }

        public void SetCustomName(string name, Worksheet worksheet, int row, int column, int rowCount, int columnCount)
        {
            CalcExpression expression = CalcExpressionHelper.CreateExternalRangeExpressionByCount(worksheet, row, column, rowCount, columnCount, false, false, false, false);
            this.Names.Add(new NameInfo(name, 0, 0, expression));
            this.OnCustomNameChanged(name, true);
        }

        internal void SetCustomNameInternal(string name, int baseRow, int baseColumn, CalcExpression expression, bool refreshNode)
        {
            this.Names.Add(new NameInfo(name, baseRow, baseColumn, expression));
            this.OnCustomNameChanged(name, refreshNode);
        }

        public void SuspendCalcService()
        {
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.SuspendCalcService();
                }
            }
        }

        public void SuspendEvent()
        {
            this._eventSuspended++;
            using (IEnumerator<Worksheet> enumerator = this.Sheets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.SuspendSpanModelEvent();
                }
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }


        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.ReadXmlPrivate(reader);
        }


        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlPrivate(writer, false);
        }

        void WriteXmlPrivate(XmlWriter writer, bool dataOnly)
        {
            Serializer.InitWriter(writer);
            if (!string.IsNullOrEmpty(this._name))
            {
                Serializer.WriteAttr("name", this._name, writer);
            }
            if (!string.IsNullOrEmpty(this._themeName))
            {
                Serializer.WriteAttr("theme", this._themeName, writer);
            }
            List<SpreadTheme> list = new List<SpreadTheme>();
            HashSet<string> set = new HashSet<string>();
            foreach (SpreadTheme theme in SpreadThemes.All)
            {
                set.Add(theme.Name);
            }
            foreach (SpreadTheme theme2 in this.Themes)
            {
                if (theme2.IsDirty || !set.Contains(theme2.Name))
                {
                    list.Add(theme2);
                }
            }
            if (list.Count > 0)
            {
                Serializer.SerializeObj(list, "Themes", writer);
            }
            if (!dataOnly && (this._activeSheetIndex != -1))
            {
                Serializer.SerializeObj((int)this._activeSheetIndex, "ActiveSheetIndex", writer);
            }
            if (!dataOnly && (this._startSheetIndex != -1))
            {
                Serializer.SerializeObj((int)this._startSheetIndex, "StartSheetIndex", writer);
            }
            if ((this.Sheets != null) && (this.Sheets.Count > 0))
            {
                Serializer.WriteStartObj("Sheets", writer);
                Serializer.WriteAttr("count", (int)this._sheets.Count, writer);
                for (int i = 0; i < this.Sheets.Count; i++)
                {
                    Serializer.WriteStartObj("Sheet", writer);
                    Serializer.WriteAttr("index", (int)i, writer);
                    this.Sheets[i].WriteXmlInternal(writer, dataOnly,ExcludePrintContent.None);
                    Serializer.WriteEndObj(writer);
                }
                Serializer.WriteEndObj(writer);
            }
            if (!dataOnly)
            {
                if ((this._names != null) && (this._names.Count > 0))
                {
                    Serializer.SerializeNameInfos(this._names, "Names", (this.SheetCount > 0) ? ((ICalcEvaluator)this._sheets[0]) : new Worksheet(), writer);
                }
                if (this._referenceStyle != ReferenceStyle.A1)
                {
                    Serializer.SerializeObj(this._referenceStyle, "ReferenceStyle", writer);
                }
                if (!this._autoRecalculation)
                {
                    Serializer.SerializeObj((bool)this._autoRecalculation, "AutoCalculation", writer);
                }
                if ((this._namedStyles != null) && (this._namedStyles.Count > 0))
                {
                    Serializer.WriteStartObj("NamedStyles", writer);
                    Serializer.SerializeObj(this._namedStyles, null, writer);
                    Serializer.WriteEndObj(writer);
                }
                if (this._horizontalScrollBarPolicy != ScrollBarVisibility.Visible)
                {
                    Serializer.SerializeObj(this._horizontalScrollBarPolicy, "HorizontalScrollBarPolicy", writer);
                }
                if (this._verticalScrollBarPolicy != ScrollBarVisibility.Visible)
                {
                    Serializer.SerializeObj(this._verticalScrollBarPolicy, "VerticalScrollBarPolicy", writer);
                }
                if (this._protect)
                {
                    Serializer.SerializeObj((bool)this._protect, "Protect", writer);
                }
                if (this._allowCellOverflow)
                {
                    Serializer.SerializeObj((bool)this._allowCellOverflow, "AllowCellOverflow", writer);
                }
                if (this._scrollTip != ShowScrollTip.None)
                {
                    Serializer.SerializeObj(this._scrollTip, "ShowScrollTip", writer);
                }
                if (this._resizeTip != ShowResizeTip.None)
                {
                    Serializer.SerializeObj(this._resizeTip, "ShowResizeTip", writer);
                }
            }
            if ((this._defaultTableStyle != TableStyles.Medium2) && (this._defaultTableStyle != null))
            {
                Serializer.SerializeObj(this._defaultTableStyle, "DefaultTableStyle", writer);
            }
        }

    }
}