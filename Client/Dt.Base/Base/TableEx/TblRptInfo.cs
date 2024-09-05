#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Report;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Table数据导出打印时的报表设置
    /// </summary>
    public class TblRptInfo : RptInfo
    {
        readonly RptTable _tbl;
        
        public TblRptInfo()
        {
            Name = "Table数据";
            Root = new RptRoot();
            
            _tbl = new RptTable(Root.Body);
            _tbl.Tbl = "tbl";
            Root.Body.Items.Add(_tbl);

            Root.Data.DataSet.AddRow(new { name = "tbl", isscritp = true });
            CacheTemplate = false;
            ScriptObj = new TblRptScript();
        }

        /// <summary>
        /// 输出行序号，默认true
        /// </summary>
        public bool RowNO { get; set; } = true;

        /// <summary>
        /// 输出列头，默认true
        /// </summary>
        public bool ShowColHeader { get; set; } = true;
        
        /// <summary>
        /// 垂直分页时在每页重复输出列头，默认true
        /// </summary>
        public bool RepeatColHeader
        {
            get { return _tbl.RepeatColFooter; }
            set { _tbl.RepeatColFooter = value; }
        }

        /// <summary>
        /// 水平分页时在每页重复输出行头的列数，默认0
        /// </summary>
        public int RepeatRowHeaderCols
        {
            get { return _tbl.RepeatRowHeaderCols; }
            set { _tbl.RepeatRowHeaderCols = value; }
        }

        /// <summary>
        /// 启用单页并排多列表格，默认false，true时填满页面后自动换列
        /// </summary>
        public bool EnableMultiColTbl
        {
            get { return _tbl.RowBreakCount == -1; }
            set { _tbl.RowBreakCount = value ? -1 : 0; }
        }

        /// <summary>
        /// 是否自动调整纸张大小，确保只一页，默认false
        /// </summary>
        public bool AutoPaperSize
        {
            get { return Root.PageSetting.AutoPaperSize; }
            set { Root.PageSetting.AutoPaperSize = value; }
        }

        /// <summary>
        /// 纸张名称
        /// </summary>
        public string PaperName
        {
            get { return Root.PageSetting.PaperName; }
            set { Root.PageSetting.PaperName = value; }
        }

        /// <summary>
        /// 纸张高度，单位：0.01英寸
        /// </summary>
        public double PageHeight
        {
            get { return Root.PageSetting.Height; }
            set { Root.PageSetting.Height = value; }
        }

        /// <summary>
        /// 纸张宽度，单位：0.01英寸
        /// </summary>
        public double PageWidth
        {
            get { return Root.PageSetting.Width; }
            set { Root.PageSetting.Width = value; }
        }

        /// <summary>
        /// 页面左边距，单位：0.01英寸
        /// </summary>
        public int PageLeftMargin
        {
            get { return Root.PageSetting.LeftMargin; }
            set { Root.PageSetting.LeftMargin = value; }
        }

        /// <summary>
        /// 页面上边距，单位：0.01英寸
        /// </summary>
        public int PageTopMargin
        {
            get { return Root.PageSetting.TopMargin; }
            set { Root.PageSetting.TopMargin = value; }
        }

        /// <summary>
        /// 页面右边距，单位：0.01英寸
        /// </summary>
        public int PageRightMargin
        {
            get { return Root.PageSetting.RightMargin; }
            set { Root.PageSetting.RightMargin = value; }
        }

        /// <summary>
        /// 页面下边距，单位：0.01英寸
        /// </summary>
        public int PageBottomMargin
        {
            get { return Root.PageSetting.BottomMargin; }
            set { Root.PageSetting.BottomMargin = value; }
        }

        /// <summary>
        /// 页面是否横向
        /// </summary>
        public bool PageLandscape
        {
            get { return Root.PageSetting.Landscape; }
            set { Root.PageSetting.Landscape = value; }
        }

        /// <summary>
        /// 是否采用默认页眉：报表名称居中显示、带下划线
        /// </summary>
        public bool DefaultPageHeader
        {
            get { return Root.Header.DefaultHeader; }
            set { Root.Header.DefaultHeader = value; }
        }

        /// <summary>
        /// 是否采用默认页脚：居中显示的页码
        /// </summary>
        public bool DefaultPageFooter
        {
            get { return Root.Footer.DefaultFooter; }
            set { Root.Footer.DefaultFooter = value; }
        }

        public RptTable Table => _tbl;
    }
}
