#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using System.Text;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    public sealed partial class BlankAreaMenu : Menu
    {
        RptDesignHome _owner;

        public BlankAreaMenu(RptDesignHome p_owner)
        {
            InitializeComponent();
            _owner = p_owner;
            Opening += OnOpening;
        }

        void OnInsertText(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptText(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertText, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertTbl(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptTable(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertTable, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertMtx(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptMatrix(_owner.GetContainer());
            range = new CellRange(range.Row, range.Column, 2, 2);
            if (RptItem.ValidEmptyRange(item.Part, range))
            {
                Kit.Error("对象创建后与其他对象位置冲突，请确认。");
                return;
            }
            _owner.Excel.ActiveSheet.SetSelection(range);
            _owner.Info.ExecuteCmd(RptCmds.InsertMatrix, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertChart(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptChart(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertChart, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertImage(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptImage(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertImage, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
            item.SelectImage();
        }

        void OnInsertSparkline(Mi e)
        {
            _owner.Excel.DecorationRange = null;
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            var item = new RptSparkline(_owner.GetContainer());
            _owner.Info.ExecuteCmd(RptCmds.InsertSparkline, new InsertCmdArgs(item, range));
            _owner.UpdateSelection();
        }

        void OnInsertCopy(Mi e)
        {
            CellRange range = _owner.Excel.ActiveSheet.Selections[0];
            _owner.Info.ExecuteCmd(RptCmds.PasteItem, new PasteCmdArgs(_owner.GetContainer(), range));
            _owner.UpdateSelection();
        }

        void OnOpening(Menu menu, AsyncCancelArgs args)
        {
            _miCopy.Visibility = string.IsNullOrEmpty(PasteItemCmd.PasteItemXml) ? Visibility.Collapsed : Visibility.Visible;
        }

        internal static void CopyItem(RptItem p_item)
        {
            StringBuilder sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                p_item.WriteXml(writer);
                writer.Flush();
            }
            PasteItemCmd.PasteItemXml = sb.ToString();
        }
    }
}
