#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents that the current fill format in NoFill
    /// </summary>
    public class NoFillFormat : IFillFormat
    {
        internal void ReadXml(XElement node)
        {
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("noFill", null, "a"))
            {
            }
        }

        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get { return  Dt.Xls.Chart.FillFormatType.NoFill; }
        }
    }
}

