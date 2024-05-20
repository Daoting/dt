#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表预览导出打印相关
    /// </summary>
    public partial class Lv
    {
        public static readonly DependencyProperty ShowReportBtnProperty = DependencyProperty.Register(
            "ShowReportBtn",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false, OnReload));
        
        /// <summary>
        /// 在表格视图的左上脚是否显示报表按钮，点击按钮显示导出、预览、打印等菜单项，默认false
        /// </summary>
        public bool ShowReportBtn
        {
            get { return (bool)GetValue(ShowReportBtnProperty); }
            set { SetValue(ShowReportBtnProperty, value); }
        }

        /// <summary>
        /// 显示报表预览
        /// </summary>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_inNewWin">是否在新窗口中显示，false在对话框中显示</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <param name="p_isPdf">报表是否采用Pdf格式</param>
        public async void ShowReport(
            LvRptInfo p_info = null,
            bool p_inNewWin = false,
            bool p_showSettingDlg = false,
            bool p_isPdf = false)
        {
            var info = p_info == null ? new LvRptInfo() : p_info;
            if (p_showSettingDlg)
                await new LvRptSettingDlg().ShowDlg(info);
            new LvExporter(this, info).ShowReport(p_inNewWin, p_isPdf);
        }

        /// <summary>
        /// 导出Excel报表
        /// </summary>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_stream">输出目标，null时提示选择要保存的文件</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <returns></returns>
        public async Task ExportExcel(LvRptInfo p_info = null, Stream p_stream = null, bool p_showSettingDlg = false)
        {
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
            
            var info = p_info == null ? new LvRptInfo { AutoPaperSize = true } : p_info;
            if (p_showSettingDlg)
                await new LvRptSettingDlg().ShowDlg(info);
            
            await new LvExporter(this, info).SaveExcel(stream);
            if (stream != p_stream)
            {
                stream.Close();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 导出Pdf报表
        /// </summary>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_stream">输出目标，null时提示选择要保存的文件</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        /// <returns></returns>
        public async Task ExportPdf(LvRptInfo p_info = null, Stream p_stream = null, bool p_showSettingDlg = false)
        {
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

            var info = p_info == null ? new LvRptInfo() : p_info;
            if (p_showSettingDlg)
                await new LvRptSettingDlg().ShowDlg(info);

            await new LvExporter(this, info).SavePdf(stream);
            if (stream != p_stream)
            {
                stream.Close();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 打印报表
        /// </summary>
        /// <param name="p_info">报表描述</param>
        /// <param name="p_showSettingDlg">是否显示报表选项和页面设置对话框</param>
        public async void Print(LvRptInfo p_info = null, bool p_showSettingDlg = false)
        {
            var info = p_info == null ? new LvRptInfo() : p_info;
            if (p_showSettingDlg)
                await new LvRptSettingDlg().ShowDlg(info);
            new LvExporter(this, info).Print();
        }
    }
}