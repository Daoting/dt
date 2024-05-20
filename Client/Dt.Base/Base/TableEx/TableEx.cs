#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Table扩展方法
    /// </summary>
    public static class TableEx
    {
        /// <summary>
        /// 显示报表预览
        /// </summary>
        /// <param name="p_tbl">Table数据</param>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_inNewWin">是否在新窗口中显示，false在对话框中显示</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <param name="p_isPdf">报表是否采用Pdf格式</param>
        public static async void ShowReport(
            this Table p_tbl,
            TblRptInfo p_info = null,
            bool p_inNewWin = false,
            bool p_showSettingDlg = false,
            bool p_isPdf = false)
        {
            if (p_tbl == null)
                return;

            var info = p_info == null ? new TblRptInfo() : p_info;
            if (p_showSettingDlg)
                await new TblRptSettingDlg().ShowDlg(info);
            new TblExporter(p_tbl, info).ShowReport(p_inNewWin, p_isPdf);
        }

        /// <summary>
        /// 导出Excel报表
        /// </summary>
        /// <param name="p_tbl">Table数据</param>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_stream">输出目标，null时提示选择要保存的文件</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <returns></returns>
        public static async Task ExportExcel(
            this Table p_tbl,
            TblRptInfo p_info = null,
            Stream p_stream = null,
            bool p_showSettingDlg = false)
        {
            if (p_tbl == null)
                return;

            Stream stream = p_stream;
            if (stream == null)
            {
                var fp = Kit.GetFileSavePicker();
                fp.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
                fp.SuggestedFileName = "新文件";
                var sf = await fp.PickSaveFileAsync();
                if (sf == null)
                    return;

                stream = await sf.OpenStreamForWriteAsync();
            }
            
            var info = p_info == null ? new TblRptInfo { AutoPaperSize = true } : p_info;
            if (p_showSettingDlg)
                await new TblRptSettingDlg().ShowDlg(info);
            
            await new TblExporter(p_tbl, info).SaveExcel(stream);
            if (stream != p_stream)
            {
                stream.Close();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 导出Pdf报表
        /// </summary>
        /// <param name="p_tbl">Table数据</param>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_stream">输出目标，null时提示选择要保存的文件</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <returns></returns>
        public static async Task ExportPdf(
            this Table p_tbl,
            TblRptInfo p_info = null,
            Stream p_stream = null,
            bool p_showSettingDlg = false)
        {
            if (p_tbl == null)
                return;

            Stream stream = p_stream;
            if (stream == null)
            {
                var fp = Kit.GetFileSavePicker();
                fp.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
                fp.SuggestedFileName = "新文件";
                var sf = await fp.PickSaveFileAsync();
                if (sf == null)
                    return;

                stream = await sf.OpenStreamForWriteAsync();
            }

            var info = p_info == null ? new TblRptInfo() : p_info;
            if (p_showSettingDlg)
                await new TblRptSettingDlg().ShowDlg(info);

            await new TblExporter(p_tbl, info).SavePdf(stream);
            if (stream != p_stream)
            {
                stream.Close();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="p_tbl">Table数据</param>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        public static async void Print(
            this Table p_tbl,
            TblRptInfo p_info = null,
            bool p_showSettingDlg = false)
        {
            if (p_tbl == null)
                return;

            var info = p_info == null ? new TblRptInfo() : p_info;
            if (p_showSettingDlg)
                await new TblRptSettingDlg().ShowDlg(info);
            new TblExporter(p_tbl, info).Print();
        }
    }
}