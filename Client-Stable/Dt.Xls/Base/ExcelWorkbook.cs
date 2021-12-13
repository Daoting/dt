#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents a Workbook
    /// </summary>
    public class ExcelWorkbook : IExcelWorkbook
    {
        private int _activeSheetIndex;
        private List<IBuiltInName> _builtInNameList;
        private ExcelCalculationMode _calculationMode = ExcelCalculationMode.Automatic;
        private List<IFunction> _customOrFunctionNameList;
        private IExtendedFormat _defaultCellFormat;
        private Dictionary<int, IExtendedFormat> _excelCellFormats;
        private List<IExternalWorkbookInfo> _externWorkbooks;
        private bool _isIterataCalculate;
        private double _maximumChange = 0.001;
        private int _maximumIterations = 100;
        private Dictionary<string, IName> _namedCellRanges;
        private bool _needInit;
        private ExcelOperator _operator;
        private bool _precisionAsDisplay = true;
        private bool _recalculateBeforeSave = true;
        private ExcelReferenceStyle _referenceStyle;
        private ExcelWorksheetCollection _sheets;
        private List<IExcelStyle> _styleList;
        private List<IExcelTableStyle> _tableStyles;
        private double _tabStripRatio = 0.6;

        internal List<IExcelStyle> GetBuiltInStyles()
        {
            BuiltInExcelStyles styles = new BuiltInExcelStyles();
            return styles.GetBuiltInStyls();
        }

        private ExcelPaletteColor GetCloestColorIndex(GcColor color)
        {
            if (this.ColorPalette == null)
            {
                return ColorExtension.GetCloestColorIndex(color);
            }
            int num = -1;
            double maxValue = double.MaxValue;
            for (int i = Math.Min(this.ColorPalette.Count - 1, 0x40); i >= 0; i--)
            {
                uint num4 = this.ColorPalette[i].ToArgb();
                double num5 = (Math.Abs((double) ((((num4 & 0xff0000) >> 0x10) - color.R) * 0.3)) + Math.Abs((double) ((((num4 & 0xff00) >> 8) - color.G) * 0.59))) + Math.Abs((double) (((num4 & 0xff) - color.B) * 0.11));
                if (num5 < maxValue)
                {
                    maxValue = num5;
                    num = i;
                }
            }
            return (ExcelPaletteColor) num;
        }

        /// <summary>
        /// Gets the closest palette color of specified color.
        /// </summary>
        /// <param name="color">An <see cref="T:Dt.Xls.IExcelColor" /> instance used to locate the palette color.</param>
        /// <returns>The closest palette color</returns>
        public ExcelPaletteColor GetPaletteColor(IExcelColor color)
        {
            if (color.ColorType == ExcelColorType.Indexed)
            {
                return (ExcelPaletteColor) color.Value;
            }
            if (color.ColorType != ExcelColorType.Theme)
            {
                return this.GetCloestColorIndex(ColorExtension.FromArgb(color.Value));
            }
            GcColor systemColor = new GcColor();
            if (this.Theme == null)
            {
                int num = (int) color.Value;
                if ((num < 0) || (num > 11))
                {
                    return ExcelPaletteColor.SystemWindowBackgroundColor;
                }
                switch (color.Value)
                {
                    case 0:
                        systemColor = GcSystemColors.GetSystemColor(GcSystemColorIndex.Window);
                        break;

                    case 1:
                        systemColor = GcSystemColors.GetSystemColor(GcSystemColorIndex.WindowText);
                        break;

                    case 2:
                        systemColor = ColorExtension.FromArgb(0xeeece1);
                        break;

                    case 3:
                        systemColor = ColorExtension.FromArgb(0x1f497d);
                        break;

                    case 4:
                        systemColor = ColorExtension.FromArgb(0x4f81bd);
                        break;

                    case 5:
                        systemColor = ColorExtension.FromArgb(0xc0504d);
                        break;

                    case 6:
                        systemColor = ColorExtension.FromArgb(0x9bbb59);
                        break;

                    case 7:
                        systemColor = ColorExtension.FromArgb(0x8064a2);
                        break;

                    case 8:
                        systemColor = ColorExtension.FromArgb(0x4bacc6);
                        break;

                    case 9:
                        systemColor = ColorExtension.FromArgb(0xf79646);
                        break;

                    case 10:
                        systemColor = ColorExtension.FromArgb(0xff);
                        break;

                    case 11:
                        systemColor = ColorExtension.FromArgb(0x800080);
                        break;
                }
            }
            else
            {
                int num2 = (int) color.Value;
                if ((num2 < 0) || (num2 > 11))
                {
                    return ExcelPaletteColor.SystemWindowBackgroundColor;
                }
                systemColor = this.GetThemeColor((ColorSchemeIndex) num2);
            }
            return this.GetCloestColorIndex(systemColor);
        }

        /// <summary>
        /// Gets the color of the theme.
        /// </summary>
        /// <param name="colorSchemeIndex">Index of the color scheme.</param>
        /// <returns></returns>
        public GcColor GetThemeColor(ColorSchemeIndex colorSchemeIndex)
        {
            int num = (int) colorSchemeIndex;
            if ((this.Theme == null) || (num > 11))
            {
                return new GcColor();
            }
            while (true)
            {
                IExcelColor color = this.Theme.ColorScheme.SchemeColors[num];
                if (color.ColorType == ExcelColorType.RGB)
                {
                    return ColorExtension.FromArgb(color.Value);
                }
                if (color.ColorType == ExcelColorType.Indexed)
                {
                    if (this.ColorPalette != null)
                    {
                        return this.ColorPalette[(int) color.Value];
                    }
                    return ColorExtension.GetPaletteColor((int) color.Value);
                }
                if (color.ColorType == ExcelColorType.Theme)
                {
                    num = (int) this.Theme.ColorScheme.SchemeColors[num].Value;
                }
            }
        }

        private void Init()
        {
            this.Is1904Date = false;
            this._precisionAsDisplay = true;
            this._recalculateBeforeSave = true;
            this.SaveExternalLinks = false;
            this._calculationMode = ExcelCalculationMode.Automatic;
            this._maximumIterations = 100;
            this._isIterataCalculate = false;
            this._maximumChange = 0.001;
            this._externWorkbooks = null;
            this._customOrFunctionNameList = null;
            this._builtInNameList = null;
            this._namedCellRanges = null;
            this._excelCellFormats = null;
            this._defaultCellFormat = null;
            this._excelCellFormats = null;
            this._defaultCellFormat = null;
            this._styleList = null;
            this._tableStyles = null;
            this._operator = null;
            this._sheets = null;
            this.FirstDisplayedTabIndex = 0;
            this.SelectedTabCount = 0;
            this._activeSheetIndex = 0;
            this.ActivePaneIndex = 0;
            this.TabStripPolicy = ExcelTabStripPolicy.Always;
            this._tabStripRatio = 0.6;
            this.VerticalScrollBarPolicy = ExcelScrollBarPolicy.AsNeeded;
            this.HorizontalScrollBarPolicy = ExcelScrollBarPolicy.AsNeeded;
            this._referenceStyle = ExcelReferenceStyle.A1;
            this.Locked = false;
            this.ExcelRect = null;
            this.IsWindowHidden = false;
            this.IsWindowDisplayAsIcon = false;
            if (this.ColorPalette != null)
            {
                this.ColorPalette = null;
            }
            if (this.DifferentialFormattings != null)
            {
                this.DifferentialFormattings = null;
            }
            this.Theme = null;
        }

        private void InitIfNeeded(int sheetIndex)
        {
            if ((sheetIndex == -1) || this._needInit)
            {
                this.Init();
            }
        }

        /// <summary>
        /// Open the specified stream.
        /// </summary>
        /// <param name="inStream">The stream to be opened</param>
        /// <param name="sheetIndex">the index of the Worksheet which will be opened. if the value is -1, it will open all
        /// sheets</param>
        /// <param name="password">The password used to decrypt the stream</param>
        public void Open(Stream inStream, int sheetIndex = -1, string password = null)
        {
            this.InitIfNeeded(sheetIndex);
            this.Operator.Open(inStream, sheetIndex, password);
            this._needInit = true;
        }

        /// <summary>
        /// Save the workbook to stream, it will use the specified file type as the file format, the file will be encrypt if the password is not null.
        /// </summary>
        /// <param name="outStream">The stream the workbook will be saved in</param>
        /// <param name="workbookType">the <see cref="T:Dt.Xls.ExcelFileType" /> specifics the file type. there are two major file type in Excel, if you want to 
        /// save an Excel 97-2003 compatible file, use Dt.Xls.ExcelFileType.Biff, otherwise, use ExcelFileType.OpenXML to save a Excel 2007 or later file. </param>
        /// <param name="password">The password which will be used to encrypt the file.</param>
        public void Save(Stream outStream, ExcelFileType workbookType, string password = null)
        {
            this.Operator.Save(outStream, workbookType, password);
        }

        /// <summary>
        /// Gets or sets the index of the active pane.
        /// </summary>
        /// <value>The index of the active pane.</value>
        public int ActivePaneIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the active sheet.
        /// </summary>
        /// <value>The index of the active sheet.</value>
        public int ActiveSheetIndex
        {
            get
            {
                if (this.Worksheets.Count == 0)
                {
                    return 0;
                }
                if ((this._activeSheetIndex >= 0) && (this._activeSheetIndex < this.Worksheets.Count))
                {
                    if (!this.Worksheets[this._activeSheetIndex].IsVisible)
                    {
                        while ((this._activeSheetIndex < this.Worksheets.Count) && !this.Worksheets[this._activeSheetIndex].IsVisible)
                        {
                            this._activeSheetIndex = (this._activeSheetIndex + 1) % this.Worksheets.Count;
                        }
                        return this._activeSheetIndex;
                    }
                    return this._activeSheetIndex;
                }
                for (int i = 0; i < this.Worksheets.Count; i++)
                {
                    if (this.Worksheets[i].IsVisible)
                    {
                        this._activeSheetIndex = i;
                        break;
                    }
                }
                return this._activeSheetIndex;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("ActiveSheetIndex");
                }
                this._activeSheetIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the built-in names used in the workbook
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IBuiltInName" /> represents the built-in names information of the workbook
        /// </value>
        public List<IBuiltInName> BuiltInNameList
        {
            get
            {
                if (this._builtInNameList == null)
                {
                    this._builtInNameList = new List<IBuiltInName>();
                }
                return this._builtInNameList;
            }
            set { this._builtInNameList = value; }
        }

        /// <summary>
        /// Gets or sets the calculation mode of the workbook
        /// </summary>
        /// <value>The calculation mode of the workbook</value>
        public ExcelCalculationMode CalculationMode
        {
            get { return  this._calculationMode; }
            set { this._calculationMode = value; }
        }

        /// <summary>
        /// Represents the custom color palette used in workbook.
        /// </summary>
        /// <remarks>
        /// If returns null, it means use default palette
        /// </remarks>
        public Dictionary<int, GcColor> ColorPalette { get; set; }

        /// <summary>
        /// Gets or sets the custom or function names used in the workbook.
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IFunction" /> represents the custom or function names
        /// </value>
        public List<IFunction> CustomOrFunctionNameList
        {
            get
            {
                if (this._customOrFunctionNameList == null)
                {
                    this._customOrFunctionNameList = new List<IFunction>();
                }
                return this._customOrFunctionNameList;
            }
            set { this._customOrFunctionNameList = value; }
        }

        /// <summary>
        /// Gets or sets the default cell format of the workbook..
        /// </summary>
        /// <value>The default cell format.</value>
        public IExtendedFormat DefaultCellFormat
        {
            get
            {
                if (this._defaultCellFormat == null)
                {
                    ExtendedFormat format = new ExtendedFormat {
                        Border = new ExcelBorder(),
                        Font = new ExcelFont().Default,
                        NumberFormatIndex = 0,
                        IsLocked = true,
                        VerticalAlign = ExcelVerticalAlignment.Bottom,
                        HorizontalAlign = ExcelHorizontalAlignment.General
                    };
                    this._defaultCellFormat = format;
                }
                return this._defaultCellFormat;
            }
            set { this._defaultCellFormat = value; }
        }

        /// <summary>
        /// Get or set the name of the default table style to apply to new PivotTables.
        /// </summary>
        public string DefaultPivotTableStyleName { get; set; }

        /// <summary>
        /// Get or set the name of the default table style to apply to new tables.
        /// </summary>
        public string DefaultTableStyleName { get; set; }

        /// <summary>
        /// Gets or sets the differential formatting settings of the workbook.
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IDifferentialFormatting" /> represents the differential formatting settings.
        /// </value>
        public List<IDifferentialFormatting> DifferentialFormattings { get; set; }

        /// <summary>
        /// Gets or sets the excel cell formats used in the workbook.
        /// </summary>
        /// <value>The excel cell formats.</value>
        public Dictionary<int, IExtendedFormat> ExcelCellFormats
        {
            get
            {
                if (this._excelCellFormats == null)
                {
                    this._excelCellFormats = new Dictionary<int, IExtendedFormat>();
                }
                return this._excelCellFormats;
            }
            set { this._excelCellFormats = value; }
        }

        /// <summary>
        /// Gets or sets the excel rect used to represents the workbook window.
        /// </summary>
        /// <value>The excel rect represents the workbook window</value>
        public IExcelRect ExcelRect { get; set; }

        /// <summary>
        /// Gets the excel styles used in the workbook.
        /// </summary>
        /// <value>The excel styles.</value>
        public List<IExcelStyle> ExcelStyles
        {
            get
            {
                if (this._styleList == null)
                {
                    this._styleList = this.GetBuiltInStyles();
                }
                return this._styleList;
            }
        }

        /// <summary>
        /// Gets or sets the extern workbook information used in the workbook
        /// </summary>
        /// <value>
        /// A collection of <see cref="T:Dt.Xls.IExternalWorkbookInfo" /> represents the extern workbook information.
        /// </value>
        public List<IExternalWorkbookInfo> ExternWorkbooks
        {
            get
            {
                if (this._externWorkbooks == null)
                {
                    this._externWorkbooks = new List<IExternalWorkbookInfo>();
                }
                return this._externWorkbooks;
            }
            set { this._externWorkbooks = value; }
        }

        /// <summary>
        /// Gets or sets the first index of the displayed tab.
        /// </summary>
        /// <value>The first index of the displayed tab.</value>
        public int FirstDisplayedTabIndex { get; set; }

        /// <summary>
        /// Gets or sets the horizontal scroll bar policy.
        /// </summary>
        /// <value>The horizontal scroll bar policy.</value>
        public ExcelScrollBarPolicy HorizontalScrollBarPolicy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workbook use 1904 date system.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it use 1904 date system; otherwise, <see langword="false" />.
        /// </value>
        public bool Is1904Date { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is iterate calculate.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is iterate calculate; otherwise, <see langword="false" />.
        /// </value>
        public bool IsIterataCalculate
        {
            get { return  this._isIterataCalculate; }
            set { this._isIterataCalculate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is window display as icon.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is window display as icon; otherwise, <see langword="false" />.
        /// </value>
        public bool IsWindowDisplayAsIcon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is window hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is window hidden; otherwise, <see langword="false" />.
        /// </value>
        public bool IsWindowHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="T:Dt.Xls.IExcelWorkbook" /> instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if locked; otherwise, <see langword="false" />.
        /// </value>
        public bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the maximum change of the workbook.
        /// </summary>
        /// <value>The value of maximum change.</value>
        public double MaximumChange
        {
            get { return  this._maximumChange; }
            set { this._maximumChange = value; }
        }

        /// <summary>
        /// Gets or sets the maximum iterations of the workbook
        /// </summary>
        /// <value>The maximum iterations of the workbook</value>
        public int MaximumIterations
        {
            get { return  this._maximumIterations; }
            set { this._maximumIterations = value; }
        }

        /// <summary>
        /// Gets or sets the global named cell ranges.
        /// </summary>
        /// <value>
        /// A dictionary represents the global named cell ranges of the workbook
        /// </value>
        public Dictionary<string, IName> NamedCellRanges
        {
            get
            {
                if (this._namedCellRanges == null)
                {
                    this._namedCellRanges = new Dictionary<string, IName>();
                }
                return this._namedCellRanges;
            }
            set { this._namedCellRanges = value; }
        }

        /// <summary>
        /// Gets the excel operator which used to read or write information from (to) excel.
        /// </summary>
        /// <value>The excel operator.</value>
        public ExcelOperator Operator
        {
            get
            {
                if (this._operator == null)
                {
                    this._operator = new ExcelOperator(new ExcelReader(this), new ExcelWriter(this), null);
                }
                return this._operator;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the value is full precision.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if is full precision; otherwise, <see langword="false" />.
        /// </value>
        public bool PrecisionAsDisplay
        {
            get { return  this._precisionAsDisplay; }
            set { this._precisionAsDisplay = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether recalculate formulas before save.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if recalculate all formulas before save; otherwise, <see langword="false" />.
        /// </value>
        public bool RecalculateBeforeSave
        {
            get { return  this._recalculateBeforeSave; }
            set { this._recalculateBeforeSave = value; }
        }

        /// <summary>
        /// Gets or sets the workbook reference style.
        /// </summary>
        /// <value>The reference style of the workbook.</value>
        public ExcelReferenceStyle ReferenceStyle
        {
            get { return  this._referenceStyle; }
            set { this._referenceStyle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether save external links during save.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if save external links; otherwise, <see langword="false" />.
        /// </value>
        public bool SaveExternalLinks { get; set; }

        /// <summary>
        /// Gets or sets the selected tab count.
        /// </summary>
        /// <value>The selected tab count.</value>
        public int SelectedTabCount { get; set; }

        /// <summary>
        /// Get the table styles used in the current workbook
        /// </summary>
        /// <value></value>
        public List<IExcelTableStyle> TableStyles
        {
            get
            {
                if (this._tableStyles == null)
                {
                    this._tableStyles = new List<IExcelTableStyle>();
                }
                return this._tableStyles;
            }
        }

        /// <summary>
        /// Gets or sets the tab strip policy.
        /// </summary>
        /// <value>The tab strip policy.</value>
        public ExcelTabStripPolicy TabStripPolicy { get; set; }

        /// <summary>
        /// Gets or sets the tab strip ratio.
        /// </summary>
        /// <value>The tab strip ratio.</value>
        public double TabStripRatio
        {
            get { return  this._tabStripRatio; }
            set { this._tabStripRatio = value; }
        }

        /// <summary>
        /// Represents the Theme used in the workbook
        /// </summary>
        public IExcelTheme Theme { get; set; }

        /// <summary>
        /// Gets or sets the vertical scroll bar policy.
        /// </summary>
        /// <value>The vertical scroll bar policy.</value>
        public ExcelScrollBarPolicy VerticalScrollBarPolicy { get; set; }

        /// <summary>
        /// Get an <see cref="T:Dt.Xls.ExcelWorksheet" /> collection that represents all the worksheets in the workbook.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Xls.ExcelWorksheet" /> collection that represents all the worksheets in the workbook.
        /// </value>
        public ExcelWorksheetCollection Worksheets
        {
            get
            {
                if (this._sheets == null)
                {
                    this._sheets = new ExcelWorksheetCollection(this);
                }
                return this._sheets;
            }
        }
    }
}

