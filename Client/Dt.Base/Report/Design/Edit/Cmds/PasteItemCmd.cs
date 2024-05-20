#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using System.Xml;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 粘贴
    /// </summary>
    internal class PasteItemCmd : RptCmdBase
    {
        public static string PasteItemXml = null;

        public override object Execute(object p_args)
        {
            if (string.IsNullOrEmpty(PasteItemXml))
                return null;

            RptItem item = null;
            var args = (PasteCmdArgs)p_args;
            try
            {
                using (var stream = new StringReader(PasteItemXml))
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true }))
                {
                    reader.Read();
                    switch (reader.Name)
                    {
                        case "Text":
                            item = new RptText(args.Body);
                            break;
                        case "Table":
                            item = new RptTable(args.Body);
                            break;
                        case "Matrix":
                            item = new RptMatrix(args.Body);
                            break;
                        case "Chart":
                            item = new RptChart(args.Body);
                            break;
                        case "Image":
                            item = new RptImage(args.Body);
                            break;
                        case "Sparkline":
                            item = new RptSparkline(args.Body);
                            break;
                        default:

                            break;
                    }
                    if (item == null)
                        Throw.Msg($"粘贴报表项时错误，无法识别报表项【{reader.Name}】！");

                    item.ReadXml(reader);
                    item.Row = args.CellRange.Row;
                    item.Col = args.CellRange.Column;
                    item.Part.Items.Add(item);
                    args.RptItem = item;
                }
            }
            catch (Exception ex)
            {
                if (ex is not KnownException)
                    Throw.Msg("粘贴报表项时错误：" + ex.Message);
            }

            return item;
        }

        public override void Undo(object p_args)
        {
            RptItem rptItem = ((PasteCmdArgs)p_args).RptItem;
            if (rptItem != null)
                rptItem.Part.Items.Remove(rptItem);
        }
    }

    internal class PasteCmdArgs
    {
        public PasteCmdArgs(RptPart p_body, CellRange p_range)
        {
            Body = p_body;
            CellRange = p_range;
        }

        public RptItem RptItem { get; set; }

        public RptPart Body { get; }

        /// <summary>
        /// 插入对象的区域。
        /// </summary>
        public CellRange CellRange { get; }
    }
}
