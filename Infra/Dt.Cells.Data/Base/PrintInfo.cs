#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the information to use when printing a <see cref="T:Worksheet" />.
    /// </summary>
    public class PrintInfo : IXmlSerializable
    {
        #region 成员变量
        bool _bestFitColumns;
        bool _bestFitRows;
        bool _blackAndWhite;
        Centering _centering;
        int _columnEnd;
        int _columnStart;
        int _firstPageNumber;
        int _fitPagesTall;
        int _fitPagesWide;
        string _footerCenter;
        byte[] _footerCenterImage;
        string _footerLeft;
        byte[] _footerLeftImage;
        string _footerRight;
        byte[] _footerRightImage;
        string _headerCenter;
        byte[] _headerCenterImage;
        string _headerLeft;
        byte[] _headerLeftImage;
        string _headerRight;
        byte[] _headerRightImage;
        Margins _margin;
        string _noteFontFamilyName;
        PrintPageOrientation _orientation;
        PrintPageOrder _pageOrder;
        string _pageRange;
        PaperSize _paperSize;
        PrintNotes _printNotes;
        bool _printShapes;
        int _repeatColumnEnd;
        int _repeatColumnStart;
        int _repeatRowEnd;
        int _repeatRowStart;
        int _rowEnd;
        int _rowStart;
        bool _showBorder;
        VisibilityType _showColumnFooter;
        VisibilityType _showColumnHeader;
        bool _showGridLine;
        VisibilityType _showRowFooter;
        VisibilityType _showRowHeader;
        bool _useMax;
        Watermark _watermark;
        double _zoomFactor;
        #endregion

        /// <summary>
        /// the property change event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new set of print settings.
        /// </summary>
        public PrintInfo()
        {
            Init();
        }

        /// <summary>
        /// Gets or sets the paper size for printing. 
        /// </summary>
        /// <value>A <see cref="P:PrintInfo.PaperSize" /> object that represents the paper size for printing.</value>
        public PaperSize PaperSize
        {
            get
            {
                if (_paperSize == null)
                {
                    _paperSize = new PaperSize();
                }
                return _paperSize;
            }
            set
            {
                _paperSize = value;
                RaiseCellChanged("PaperSize");
            }
        }

        /// <summary>
        /// Gets or sets the margins for printing, in hundredths of an inch.  
        /// </summary>
        /// <value>The margins for printing, in hundredths of an inch.</value>
        public Margins Margin
        {
            get { return _margin; }
            set
            {
                _margin = value;
                RaiseCellChanged("Margin");
            }
        }

        /// <summary>
        /// Gets or sets the page orientation used for printing. 
        /// </summary>
        /// <value>
        /// A value that specifies the orientation for the printed page.
        /// The default value is <see cref="T:PrintPageOrientation">Portrait</see>.
        /// </value>
        [DefaultValue(1)]
        public PrintPageOrientation Orientation
        {
            get { return _orientation; }
            set
            {
                _orientation = value;
                RaiseCellChanged("Orientation");
            }
        }

        /// <summary>
        /// Gets or sets how the printed page is centered.  
        /// </summary>
        /// <value>A value that specifies how to center the printed page. The default value is <see cref="P:PrintInfo.Centering">None</see>.</value>
        [DefaultValue(0)]
        public Centering Centering
        {
            get { return _centering; }
            set
            {
                _centering = value;
                RaiseCellChanged("Centering");
            }
        }

        /// <summary>
        /// Gets or sets whether to print an outline border around the entire control.
        /// </summary>
        /// <value>
        /// <c>true</c> if an outline border is printed around the control; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowBorder
        {
            get { return _showBorder; }
            set
            {
                _showBorder = value;
                RaiseCellChanged("ShowBorder");
            }
        }

        /// <summary>
        /// Gets or sets whether to print the row header.
        /// </summary>
        /// <value>A value that determines whether to print the row header. The default value is <see cref="T:VisibilityType">Inherit</see>.</value>
        [DefaultValue(0)]
        public VisibilityType ShowRowHeader
        {
            get { return _showRowHeader; }
            set
            {
                _showRowHeader = value;
                RaiseCellChanged("ShowRowHeader");
            }
        }

        /// <summary>
        /// Gets or sets whether to print the column header.
        /// </summary>
        /// <value>A value that determines whether to print the column header. The default value is <see cref="T:VisibilityType">Inherit</see>.</value>
        [DefaultValue(0)]
        public VisibilityType ShowColumnHeader
        {
            get { return _showColumnHeader; }
            set
            {
                _showColumnHeader = value;
                RaiseCellChanged("ShowColumnHeader");
            }
        }

        /// <summary>
        /// Gets or sets whether to print the grid lines.
        /// </summary>
        /// <value>
        /// <c>true</c> to print the grid lines; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowGridLine
        {
            get { return _showGridLine; }
            set
            {
                _showGridLine = value;
                RaiseCellChanged("ShowGridLine");
            }
        }

        /// <summary>
        /// Gets or sets the page range for printing.
        /// </summary>
        /// <remarks>
        /// Type page numbers or page ranges separated by commas
        /// counting from the beginning of the document.
        /// For example, type "1,3,5-12".
        /// </remarks>
        /// <value>A string that provides page numbers or page ranges. The default value is an empty string.</value>
        [DefaultValue("")]
        public string PageRange
        {
            get { return _pageRange; }
            set
            {
                _pageRange = value;
                RaiseCellChanged("PageRange");
            }
        }

        /// <summary>
        /// Gets or sets whether column widths are adjusted to fit the longest text width for printing. 
        /// </summary>
        /// <value>
        /// <c>true</c> if the column widths are adjusted to fit the longest text for printing; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool BestFitColumns
        {
            get { return  _bestFitColumns; }
            set
            {
                _bestFitColumns = value;
                RaiseCellChanged("BestFitColumns");
            }
        }

        /// <summary>
        /// Gets or sets whether row heights are adjusted to fit the tallest text height for printing.
        /// </summary>
        /// <value>
        /// <c>true</c> if the row heights are adjusted to fit the tallest text for printing; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool BestFitRows
        {
            get { return  _bestFitRows; }
            set
            {
                _bestFitRows = value;
                RaiseCellChanged("BestFitRows");
            }
        }

        /// <summary>
        /// Gets or sets whether to print in black and white.  
        /// </summary>
        /// <value>
        /// <c>true</c> to print in black and white; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool BlackAndWhite
        {
            get { return  _blackAndWhite; }
            set
            {
                _blackAndWhite = value;
                RaiseCellChanged("BlackAndWhite");
            }
        }

        /// <summary>
        /// Gets or sets the last column to print when printing a cell range.
        /// </summary>
        /// <value>The column index of the last column to print in a cell range.</value>
        [DefaultValue(-1)]
        public int ColumnEnd
        {
            get { return  _columnEnd; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("ColumnEnd", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _columnEnd = value;
                RaiseCellChanged("ColumnEnd");
            }
        }

        /// <summary>
        /// Gets or sets the first column to print when printing a cell range.
        /// </summary>
        /// <value>The column index of the first column to print in a cell range.</value>
        [DefaultValue(-1)]
        public int ColumnStart
        {
            get { return  _columnStart; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("ColumnStart", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _columnStart = value;
                RaiseCellChanged("ColumnStart");
            }
        }

        /// <summary>
        /// Gets or sets the page number to print on the first page. 
        /// </summary>
        /// <value>The page number to print on the first page. The default value is 1.</value>
        [DefaultValue(1)]
        public int FirstPageNumber
        {
            get { return  _firstPageNumber; }
            set
            {
                _firstPageNumber = value;
                RaiseCellChanged("FirstPageNumber");
            }
        }

        /// <summary>
        /// Gets or sets the number of vertical pages to check when optimizing printing.  
        /// </summary>
        /// <value>The number of vertical pages to check.</value>
        [DefaultValue(-1)]
        public int FitPagesTall
        {
            get { return  _fitPagesTall; }
            set
            {
                if ((value <= 0) && (value != -1))
                {
                    throw new ArgumentException();
                }
                _fitPagesTall = value;
                RaiseCellChanged("FitPagesTall");
            }
        }

        /// <summary>
        /// Gets or sets the number of horizontal pages to check when optimizing the printing.  
        /// </summary>
        /// <value>The number of horizontal pages to check.</value>
        [DefaultValue(-1)]
        public int FitPagesWide
        {
            get { return  _fitPagesWide; }
            set
            {
                if ((value <= 0) && (value != -1))
                {
                    throw new ArgumentException();
                }
                _fitPagesWide = value;
                RaiseCellChanged("FitPagesWide");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the center footer on printed pages.  
        /// </summary>
        /// <value>
        /// The text and format of the center footer for the printed pages of the report. 
        /// The default value is an empty string, which means that no footers are printed.
        /// </value>
        public string FooterCenter
        {
            get { return  _footerCenter; }
            set
            {
                _footerCenter = value;
                RaiseCellChanged("FooterCenter");
            }
        }

        /// <summary>
        /// Gets or sets the image for the center section of the footer. 
        /// </summary>
        /// <value>
        /// The image for the center portion of the printed footer. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] FooterCenterImage
        {
            get { return  _footerCenterImage; }
            set
            {
                _footerCenterImage = value;
                RaiseCellChanged("FooterCenterImage");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the left footer on printed pages.  
        /// </summary>
        /// <value>
        /// The text and format of the left footer for the printed pages of the report. 
        /// The default value is an empty string, which means that no footers are printed.
        /// </value>
        public string FooterLeft
        {
            get { return  _footerLeft; }
            set
            {
                _footerLeft = value;
                RaiseCellChanged("FooterLeft");
            }
        }

        /// <summary>
        /// Gets or sets the image for the left section of the footer. 
        /// </summary>
        /// <value>
        /// The image for the left portion of the printed footer. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] FooterLeftImage
        {
            get { return  _footerLeftImage; }
            set
            {
                _footerLeftImage = value;
                RaiseCellChanged("FooterLeftImage");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the right footer on printed pages.  
        /// </summary>
        /// <value>
        /// The text and format of the right footer for the printed pages of the report. 
        /// The default value is an empty string, which means that no footers are printed.
        /// </value>
        public string FooterRight
        {
            get { return  _footerRight; }
            set
            {
                _footerRight = value;
                RaiseCellChanged("FooterRight");
            }
        }

        /// <summary>
        /// Gets or sets the image for the right section of the footer. 
        /// </summary>
        /// <value>
        /// The image for the right portion of the printed footer. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] FooterRightImage
        {
            get { return  _footerRightImage; }
            set
            {
                _footerRightImage = value;
                RaiseCellChanged("FooterRightImage");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the center header on printed pages. 
        /// </summary>
        /// <value>
        /// The text and format of the center header for the printed pages of the report. 
        /// The default value is an empty string, which means that no headers are printed.
        /// </value>
        [DefaultValue("")]
        public string HeaderCenter
        {
            get { return  _headerCenter; }
            set
            {
                _headerCenter = value;
                RaiseCellChanged("HeaderCenter");
            }
        }

        /// <summary>
        /// Gets or sets the image for the center section of the header. 
        /// </summary>
        /// <value>
        /// The image for the center portion of the printed header. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] HeaderCenterImage
        {
            get { return  _headerCenterImage; }
            set
            {
                _headerCenterImage = value;
                RaiseCellChanged("HeaderCenterImage");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the left header on printed pages. 
        /// </summary>
        /// <value>
        /// The text and format of the left header for the printed pages of the report. 
        /// The default value is an empty string, which means that no headers are printed.
        /// </value>
        [DefaultValue("")]
        public string HeaderLeft
        {
            get { return  _headerLeft; }
            set
            {
                _headerLeft = value;
                RaiseCellChanged("HeaderLeft");
            }
        }

        /// <summary>
        /// Gets or sets the image for the left section of the header. 
        /// </summary>
        /// <value>
        /// The image for the left portion of the printed header. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] HeaderLeftImage
        {
            get { return  _headerLeftImage; }
            set
            {
                _headerLeftImage = value;
                RaiseCellChanged("HeaderLeftImage");
            }
        }

        /// <summary>
        /// Gets or sets the text and format of the right header on printed pages. 
        /// </summary>
        /// <value>
        /// The text and format of the right header for the printed pages of the report. 
        /// The default value is an empty string, which means that no headers are printed.
        /// </value>
        [DefaultValue("")]
        public string HeaderRight
        {
            get { return  _headerRight; }
            set
            {
                _headerRight = value;
                RaiseCellChanged("HeaderRight");
            }
        }

        /// <summary>
        /// Gets or sets the image for the right section of the header. 
        /// </summary>
        /// <value>
        /// The image for the right portion of the printed header. 
        /// The default value is null, which means that no image is specified.
        /// </value>
        [DefaultValue((string) null)]
        public byte[] HeaderRightImage
        {
            get { return  _headerRightImage; }
            set
            {
                _headerRightImage = value;
                RaiseCellChanged("HeaderRightImage");
            }
        }

        /// <summary>
        /// Gets or sets the order in which pages print.  
        /// </summary>
        /// <value>A value that specifies the order in which pages print. The default value is <see cref="T:PrintPageOrder">Auto</see>.</value>
        [DefaultValue(0)]
        public PrintPageOrder PageOrder
        {
            get { return  _pageOrder; }
            set
            {
                _pageOrder = value;
                RaiseCellChanged("PageOrder");
            }
        }

        /// <summary>
        /// Gets or sets the last column of a range of columns to print on the left of each page.
        /// </summary>
        /// <value>The column index of the last column of the range of columns to print on the left of every page.</value>
        [DefaultValue(-1)]
        public int RepeatColumnEnd
        {
            get { return  _repeatColumnEnd; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RepeatColumnEnd", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _repeatColumnEnd = value;
                RaiseCellChanged("RepeatColumnEnd");
            }
        }

        /// <summary>
        /// Gets or sets the first column of a range of columns to print on the left of each page.
        /// </summary>
        /// <value>The column index of the first column of the range of columns to print on the left of every page.</value>
        [DefaultValue(-1)]
        public int RepeatColumnStart
        {
            get { return  _repeatColumnStart; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RepeatColumnStart", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _repeatColumnStart = value;
                RaiseCellChanged("RepeatColumnStart");
            }
        }

        /// <summary>
        /// Gets or sets the last row of a range of rows to print at the top of each page.
        /// </summary>
        /// <value>The row index for the last row of the range of rows to print at the top of every page.</value>
        [DefaultValue(-1)]
        public int RepeatRowEnd
        {
            get { return  _repeatRowEnd; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RepeatRowEnd", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _repeatRowEnd = value;
                RaiseCellChanged("RepeatRowEnd");
            }
        }

        /// <summary>
        /// Gets or sets the first row of a range of rows to print at the top of each page.
        /// </summary>
        /// <value>The row index for the first row of the range of rows to print at the top of every page.</value>
        [DefaultValue(-1)]
        public int RepeatRowStart
        {
            get { return  _repeatRowStart; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RepeatRowStart", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _repeatRowStart = value;
                RaiseCellChanged("RepeatRowStart");
            }
        }

        /// <summary>
        /// Gets or sets the last row to print when printing a cell range.
        /// </summary>
        /// <value>The row index of the last row to print in a cell range.</value>
        [DefaultValue(-1)]
        public int RowEnd
        {
            get { return  _rowEnd; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RowEnd", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _rowEnd = value;
                RaiseCellChanged("RowEnd");
            }
        }

        /// <summary>
        /// Gets or sets the first row to print when printing a cell range.
        /// </summary>
        /// <value>The row index of the first row to print in a cell range.</value>
        [DefaultValue(-1)]
        public int RowStart
        {
            get { return  _rowStart; }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("RowStart", ResourceStrings.ReportingPrintInfoRepeatColumnError);
                }
                _rowStart = value;
                RaiseCellChanged("RowStart");
            }
        }

        /// <summary>
        /// Gets or sets whether to print only rows,columns that contain data.
        /// </summary>
        /// <value>
        /// <c>true</c> if only rows and columns that contain data are to be printed; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool UseMax
        {
            get { return  _useMax; }
            set
            {
                _useMax = value;
                RaiseCellChanged("UseMax");
            }
        }

        /// <summary>
        /// Gets or sets the zoom factor used for printing.  
        /// </summary>
        /// <value>A value that specifies the amount to enlarge or reduce the printed worksheet. The default value is 1.</value>
        /// <remarks>Specify the value as a number between 0.1 (10% zoom) and 4 (400% zoom).</remarks>
        [DefaultValue(1)]
        public double ZoomFactor
        {
            get { return  _zoomFactor; }
            set
            {
                if ((value < 0.1) || (value > 4.0))
                {
                    throw new ArgumentOutOfRangeException("ZoomFactor", ResourceStrings.ZoomFactorOutOfRange);
                }
                _zoomFactor = value;
                RaiseCellChanged("ZoomFactor");
            }
        }

        /// <summary>
        /// Gets or sets the name of the note font family.
        /// </summary>
        /// <value>The name of the note font family.</value>
        internal string DefaultNoteFontName
        {
            get
            {
                if (string.IsNullOrEmpty(_noteFontFamilyName))
                {
                    return DefaultStyleCollection.DefaultFontName;
                }
                return _noteFontFamilyName;
            }
            set
            {
                if (value != DefaultStyleCollection.DefaultFontName)
                {
                    _noteFontFamilyName = value;
                    RaiseCellChanged("DefaultNoteFontName");
                }
            }
        }

        /// <summary>
        /// Gets or sets how cell notes are printed after the sheets.
        /// </summary>
        /// <value>A value indicating how to print the cell notes. The default value is <see cref="P:PrintInfo.PrintNotes">None</see>.</value>
        [DefaultValue(0)]
        internal PrintNotes PrintNotes
        {
            get { return _printNotes; }
            set
            {
                _printNotes = value;
                RaiseCellChanged("PrintNotes");
            }
        }

        /// <summary>
        /// Gets or sets whether to print shapes.
        /// </summary>
        /// <value>
        /// <c>true</c> if shapes are to be printed; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        internal bool PrintShapes
        {
            get { return _printShapes; }
            set
            {
                _printShapes = value;
                RaiseCellChanged("PrintShapes");
            }
        }

        /// <summary>
        /// Gets or sets whether to print the column footer.
        /// </summary>
        /// <value>A value that determines whether to print the column footer. The default value is <see cref="T:VisibilityType">Inherit</see>.</value>
        [DefaultValue(0)]
        internal VisibilityType ShowColumnFooter
        {
            get { return _showColumnFooter; }
            set
            {
                _showColumnFooter = value;
                RaiseCellChanged("VisibilityType");
            }
        }

        /// <summary>
        /// Gets or sets whether to print the row footer.
        /// </summary>
        /// <value>A value that determines whether to print the row footer. The default value is <see cref="T:VisibilityType">Inherit</see>.</value>
        [DefaultValue(0)]
        internal VisibilityType ShowRowFooter
        {
            get { return _showRowFooter; }
            set
            {
                _showRowFooter = value;
                RaiseCellChanged("ShowRowFooter");
            }
        }

        /// <summary>
        /// Gets the report's watermark.
        /// </summary>
        /// <value>A <see cref="P:PrintInfo.Watermark" /> object specifying the report's watermark.</value>
        internal Watermark Watermark
        {
            get { return _watermark; }
        }

        #region 内部方法
        /// <summary>
        /// Applies to report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="workbookName">Name of the workbook.</param>
        /// <param name="worksheetName">Name of the worksheet.</param>
        internal void ApplyToGcReport(GcReport report, string workbookName, string worksheetName)
        {
            report.Centering = _centering;
            report.FirstPageNumber = _firstPageNumber;
            Windows.Foundation.Size pageSize = Utilities.GetPageSize(_paperSize, _orientation);
            if ((!string.IsNullOrEmpty(_headerLeft) || !string.IsNullOrEmpty(_headerCenter)) || !string.IsNullOrEmpty(_headerRight))
            {
                Margins margins = _margin ?? new Margins();
                int width = (int)Math.Floor((double)((pageSize.Width - margins.Left) - margins.Right));
                int height = margins.Top - margins.Header;
                if ((width > 0) && (height > 0))
                {
                    GcRichLabel label = new GcRichLabel(0, 0, width, height)
                    {
                        LeftSource = _headerLeft,
                        CenterSource = _headerCenter,
                        RightSource = _headerRight
                    };
                    try
                    {
                        if ((_headerLeftImage != null) && (_headerLeftImage.Length > 0))
                        {
                            label.LeftImage = Image.GetInstance(_headerLeftImage);
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if ((_headerCenterImage != null) && (_headerCenterImage.Length > 0))
                        {
                            label.CenterImage = Image.GetInstance(_headerCenterImage);
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if ((_headerRightImage != null) && (_headerRightImage.Length > 0))
                        {
                            label.RightImage = Image.GetInstance(_headerRightImage);
                        }
                    }
                    catch
                    {
                    }
                    label.WorkbookName = workbookName;
                    label.WorksheetName = worksheetName;
                    report.TopMargin.Controls.Add(label);
                }
            }
            if ((!string.IsNullOrEmpty(_footerLeft) || !string.IsNullOrEmpty(_footerCenter)) || !string.IsNullOrEmpty(_footerRight))
            {
                Margins margins2 = _margin ?? new Margins();
                int num3 = (int)Math.Floor((double)((pageSize.Width - margins2.Left) - margins2.Right));
                int num4 = margins2.Bottom - margins2.Footer;
                if ((num3 > 0) && (num4 > 0))
                {
                    GcRichLabel label2 = new GcRichLabel(0, 0, num3, num4)
                    {
                        LeftSource = _footerLeft,
                        CenterSource = _footerCenter,
                        RightSource = _footerRight
                    };
                    try
                    {
                        if ((_footerLeftImage != null) && (_footerLeftImage.Length > 0))
                        {
                            label2.LeftImage = Image.GetInstance(_footerLeftImage);
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if ((_footerCenterImage != null) && (_footerCenterImage.Length > 0))
                        {
                            label2.CenterImage = Image.GetInstance(_footerCenterImage);
                        }
                    }
                    catch
                    {
                    }
                    try
                    {
                        if ((_footerRightImage != null) && (_footerRightImage.Length > 0))
                        {
                            label2.RightImage = Image.GetInstance(_footerRightImage);
                        }
                    }
                    catch
                    {
                    }
                    label2.WorkbookName = workbookName;
                    label2.WorksheetName = worksheetName;
                    label2.VerticalAlignment = TextVerticalAlignment.Bottom;
                    report.BottomMargin.Controls.Add(label2);
                }
            }
            if (_margin != null)
            {
                report.Margin = _margin;
            }
            report.Orientation = _orientation;
            report.PageRange = _pageRange;
            report.PageOrder = _pageOrder;
            report.BlackAndWhite = _blackAndWhite;
            report.ZoomFactor = _zoomFactor;
            report.FitPagesTall = _fitPagesTall;
            report.FitPagesWide = _fitPagesWide;
            if (_watermark != null)
            {
                report.Watermark = _watermark;
            }
            if (_paperSize != null)
            {
                report.PaperSize = _paperSize;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            _bestFitRows = false;
            _bestFitColumns = false;
            _columnStart = -1;
            _columnEnd = -1;
            _rowStart = -1;
            _rowEnd = -1;
            _printNotes = PrintNotes.None;
            _printShapes = true;
            _repeatColumnStart = -1;
            _repeatColumnEnd = -1;
            _repeatRowStart = -1;
            _repeatRowEnd = -1;
            _showBorder = true;
            _showGridLine = true;
            _showColumnHeader = VisibilityType.Inherit;
            _showRowHeader = VisibilityType.Inherit;
            _showColumnFooter = VisibilityType.Inherit;
            _showRowFooter = VisibilityType.Inherit;
            _useMax = true;
            _noteFontFamilyName = string.Empty;
            _centering = Centering.None;
            _firstPageNumber = 1;
            _headerLeft = string.Empty;
            _headerCenter = string.Empty;
            _headerRight = string.Empty;
            _footerLeft = string.Empty;
            _footerCenter = string.Empty;
            _footerRight = string.Empty;
            _headerLeftImage = null;
            _headerCenterImage = null;
            _headerRightImage = null;
            _footerLeftImage = null;
            _footerCenterImage = null;
            _footerRightImage = null;
            _margin = new Margins();
            _orientation = PrintPageOrientation.Portrait;
            _pageRange = string.Empty;
            _pageOrder = PrintPageOrder.Auto;
            _blackAndWhite = false;
            _zoomFactor = 1.0;
            _fitPagesTall = -1;
            _fitPagesWide = -1;
            _watermark = new Watermark();
            _paperSize = new PaperSize();
        }

        void RaiseCellChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    ReadXmlBase(reader);
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            WriteXmlBase(writer);
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlBase(XmlReader reader)
        {
            Serializer.InitReader(reader);
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "BestFitRows":
                        _bestFitRows = Serializer.ReadAttributeBoolean("value", false, reader);
                        return;

                    case "BestFitCols":
                        _bestFitColumns = Serializer.ReadAttributeBoolean("value", false, reader);
                        return;

                    case "ColumnStart":
                        _columnStart = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "ColumnEnd":
                        _columnEnd = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RowStart":
                        _rowStart = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RowEnd":
                        _rowEnd = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RepeatColumnStart":
                        _repeatColumnStart = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RepeatColumnEnd":
                        _repeatColumnEnd = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RepeatRowStart":
                        _repeatRowStart = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "RepeatRowEnd":
                        _repeatRowEnd = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "ShowBorder":
                        _showBorder = Serializer.ReadAttributeBoolean("value", true, reader);
                        return;

                    case "ShowGridLine":
                        _showGridLine = Serializer.ReadAttributeBoolean("value", true, reader);
                        return;

                    case "ShowColumnHeader":
                        _showColumnHeader = Serializer.ReadAttributeEnum<VisibilityType>("value", VisibilityType.Inherit, reader);
                        return;

                    case "ShowRowHeader":
                        _showRowHeader = Serializer.ReadAttributeEnum<VisibilityType>("value", VisibilityType.Inherit, reader);
                        return;

                    case "ShowColumnFooter":
                        _showColumnFooter = Serializer.ReadAttributeEnum<VisibilityType>("value", VisibilityType.Inherit, reader);
                        return;

                    case "UseMax":
                        _useMax = Serializer.ReadAttributeBoolean("value", true, reader);
                        return;

                    case "NoteFontFamilyName":
                        _noteFontFamilyName = Serializer.ReadAttribute("value", reader);
                        return;

                    case "Centering":
                        _centering = Serializer.ReadAttributeEnum<Centering>("value", Centering.None, reader);
                        return;

                    case "FirstPageNumber":
                        _firstPageNumber = Serializer.ReadAttributeInt("value", 1, reader);
                        return;

                    case "HeaderLeft":
                        _headerLeft = Serializer.ReadAttribute("value", reader);
                        return;

                    case "HeaderCenter":
                        _headerCenter = Serializer.ReadAttribute("value", reader);
                        return;

                    case "HeaderRight":
                        _headerRight = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FooterLeft":
                        _footerLeft = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FooterCenter":
                        _footerCenter = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FooterRight":
                        _footerRight = Serializer.ReadAttribute("value", reader);
                        return;

                    case "HeaderLeftImage":
                        {
                            List<byte> list = new List<byte>();
                            Serializer.DeserializeList((IList)list, reader);
                            _headerLeftImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list);
                            return;
                        }
                    case "HeaderCenterImage":
                        {
                            List<byte> list2 = new List<byte>();
                            Serializer.DeserializeList((IList)list2, reader);
                            _headerCenterImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list2);
                            return;
                        }
                    case "HeaderRightImage":
                        {
                            List<byte> list3 = new List<byte>();
                            Serializer.DeserializeList((IList)list3, reader);
                            _headerRightImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list3);
                            return;
                        }
                    case "FooterLeftImage":
                        {
                            List<byte> list4 = new List<byte>();
                            Serializer.DeserializeList((IList)list4, reader);
                            _footerLeftImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list4);
                            return;
                        }
                    case "FooterCenterImage":
                        {
                            List<byte> list5 = new List<byte>();
                            Serializer.DeserializeList((IList)list5, reader);
                            _footerCenterImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list5);
                            return;
                        }
                    case "FooterRightImage":
                        {
                            List<byte> list6 = new List<byte>();
                            Serializer.DeserializeList((IList)list6, reader);
                            _footerRightImage = Enumerable.ToArray<byte>((IEnumerable<byte>)list6);
                            return;
                        }
                    case "Margin":
                        _margin = Serializer.DeserializeObj(typeof(Margins), reader) as Margins;
                        return;

                    case "Orientation":
                        _orientation = Serializer.ReadAttributeEnum<PrintPageOrientation>("value", PrintPageOrientation.Portrait, reader);
                        return;

                    case "PageRange":
                        _pageRange = Serializer.ReadAttribute("value", reader);
                        return;

                    case "PageOrder":
                        _pageOrder = Serializer.ReadAttributeEnum<PrintPageOrder>("value", PrintPageOrder.Auto, reader);
                        return;

                    case "BlackAndWhite":
                        _blackAndWhite = Serializer.ReadAttributeBoolean("value", true, reader);
                        return;

                    case "ZoomFactor":
                        _zoomFactor = Serializer.ReadAttributeDouble("value", 1.0, reader);
                        return;

                    case "FitPagesTall":
                        _fitPagesTall = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "FitPagesWide":
                        _fitPagesWide = Serializer.ReadAttributeInt("value", -1, reader);
                        return;

                    case "Watermark":
                        _watermark = Serializer.DeserializeObj(typeof(Watermark), reader) as Watermark;
                        return;

                    case "PaperSize":
                        _paperSize = Serializer.DeserializeObj(typeof(PaperSize), reader) as PaperSize;
                        break;

                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteXmlBase(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (_bestFitRows)
            {
                Serializer.SerializeObj((bool)_bestFitRows, "BestFitRows", false, writer);
            }
            if (_bestFitColumns)
            {
                Serializer.SerializeObj((bool)_bestFitColumns, "BestFitCols", false, writer);
            }
            if (_columnStart != -1)
            {
                Serializer.SerializeObj((int)_columnStart, "ColumnStart", false, writer);
            }
            if (_columnEnd != -1)
            {
                Serializer.SerializeObj((int)_columnEnd, "ColumnEnd", false, writer);
            }
            if (_rowStart != -1)
            {
                Serializer.SerializeObj((int)_rowStart, "RowStart", false, writer);
            }
            if (_rowEnd != -1)
            {
                Serializer.SerializeObj((int)_rowEnd, "RowEnd", false, writer);
            }
            if (_repeatColumnStart != -1)
            {
                Serializer.SerializeObj((int)_repeatColumnStart, "RepeatColumnStart", false, writer);
            }
            if (_repeatColumnEnd != -1)
            {
                Serializer.SerializeObj((int)_repeatColumnEnd, "RepeatColumnEnd", false, writer);
            }
            if (_repeatRowStart != -1)
            {
                Serializer.SerializeObj((int)_repeatRowStart, "RepeatRowStart", false, writer);
            }
            if (_repeatRowEnd != -1)
            {
                Serializer.SerializeObj((int)_repeatRowEnd, "RepeatRowEnd", false, writer);
            }
            if (!_showBorder)
            {
                Serializer.SerializeObj((bool)_showBorder, "ShowBorder", false, writer);
            }
            if (!_showGridLine)
            {
                Serializer.SerializeObj((bool)_showGridLine, "ShowGridLine", false, writer);
            }
            if (_showColumnHeader != VisibilityType.Inherit)
            {
                Serializer.SerializeObj(_showColumnHeader, "ShowColumnHeader", false, writer);
            }
            if (_showColumnFooter != VisibilityType.Inherit)
            {
                Serializer.SerializeObj(_showColumnFooter, "ShowColumnFooter", false, writer);
            }
            if (_showRowHeader != VisibilityType.Inherit)
            {
                Serializer.SerializeObj(_showRowHeader, "ShowRowHeader", false, writer);
            }
            if (!_useMax)
            {
                Serializer.SerializeObj((bool)_useMax, "UseMax", false, writer);
            }
            if (!string.IsNullOrEmpty(_noteFontFamilyName))
            {
                Serializer.SerializeObj(_noteFontFamilyName, "NoteFontFamilyName", false, writer);
            }
            if (_centering != Centering.None)
            {
                Serializer.SerializeObj(_centering, "Centering", false, writer);
            }
            if (_firstPageNumber != 1)
            {
                Serializer.SerializeObj((int)_firstPageNumber, "FirstPageNumber", false, writer);
            }
            if (!string.IsNullOrEmpty(_headerLeft))
            {
                Serializer.SerializeObj(_headerLeft, "HeaderLeft", false, writer);
            }
            if (!string.IsNullOrEmpty(_headerCenter))
            {
                Serializer.SerializeObj(_headerCenter, "HeaderCenter", false, writer);
            }
            if (!string.IsNullOrEmpty(_headerRight))
            {
                Serializer.SerializeObj(_headerRight, "HeaderRight", false, writer);
            }
            if (!string.IsNullOrEmpty(_footerLeft))
            {
                Serializer.SerializeObj(_footerLeft, "FooterLeft", false, writer);
            }
            if (!string.IsNullOrEmpty(_footerCenter))
            {
                Serializer.SerializeObj(_footerCenter, "FooterCenter", false, writer);
            }
            if (!string.IsNullOrEmpty(_footerRight))
            {
                Serializer.SerializeObj(_footerRight, "FooterRight", false, writer);
            }
            if (_headerLeftImage != null)
            {
                Serializer.SerializeObj(_headerLeftImage, "HeaderLeftImage", false, writer);
            }
            if (_headerCenterImage != null)
            {
                Serializer.SerializeObj(_headerCenterImage, "HeaderCenterImage", false, writer);
            }
            if (_headerRightImage != null)
            {
                Serializer.SerializeObj(_headerRightImage, "HeaderRightImage", false, writer);
            }
            if (_footerLeftImage != null)
            {
                Serializer.SerializeObj(_footerLeftImage, "FooterLeftImage", false, writer);
            }
            if (_footerCenterImage != null)
            {
                Serializer.SerializeObj(_footerCenterImage, "FooterCenterImage", false, writer);
            }
            if (_footerRightImage != null)
            {
                Serializer.SerializeObj(_footerRightImage, "FooterRightImage", false, writer);
            }
            Serializer.SerializeObj(_margin, "Margin", true, writer);
            if (_orientation != PrintPageOrientation.Portrait)
            {
                Serializer.SerializeObj(_orientation, "Orientation", false, writer);
            }
            if (!string.IsNullOrEmpty(_pageRange))
            {
                Serializer.SerializeObj(_pageRange, "PageRange", false, writer);
            }
            if (_pageOrder != PrintPageOrder.Auto)
            {
                Serializer.SerializeObj(_pageOrder, "PageOrder", false, writer);
            }
            if (_blackAndWhite)
            {
                Serializer.SerializeObj((bool)_blackAndWhite, "BlackAndWhite", false, writer);
            }
            if (_zoomFactor != 1.0)
            {
                Serializer.SerializeObj((double)_zoomFactor, "ZoomFactor", false, writer);
            }
            if (_fitPagesTall != -1)
            {
                Serializer.SerializeObj((int)_fitPagesTall, "FitPagesTall", false, writer);
            }
            if (_fitPagesWide != -1)
            {
                Serializer.SerializeObj((int)_fitPagesWide, "FitPagesWide", false, writer);
            }
            Serializer.SerializeObj(_paperSize, "PaperSize", true, writer);
        }
        #endregion
    }
}

