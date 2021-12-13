#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents a pattern fill format.
    /// </summary>
    public class PatternFill : IFillFormat
    {
        private static Dictionary<string, ChartFormatPatternFillStyle> _hatchStyles = new Dictionary<string, ChartFormatPatternFillStyle>();

        static PatternFill()
        {
            _hatchStyles.Add("pct5", ChartFormatPatternFillStyle.Percent05);
            _hatchStyles.Add("pct5", ChartFormatPatternFillStyle.Percent05);
            _hatchStyles.Add("pct10", ChartFormatPatternFillStyle.Percent10);
            _hatchStyles.Add("pct20", ChartFormatPatternFillStyle.Percent20);
            _hatchStyles.Add("pct25", ChartFormatPatternFillStyle.Percent25);
            _hatchStyles.Add("pct30", ChartFormatPatternFillStyle.Percent30);
            _hatchStyles.Add("pct40", ChartFormatPatternFillStyle.Percent40);
            _hatchStyles.Add("pct50", ChartFormatPatternFillStyle.Percent50);
            _hatchStyles.Add("pct60", ChartFormatPatternFillStyle.Percent60);
            _hatchStyles.Add("pct70", ChartFormatPatternFillStyle.Percent70);
            _hatchStyles.Add("pct75", ChartFormatPatternFillStyle.Percent75);
            _hatchStyles.Add("pct80", ChartFormatPatternFillStyle.Percent80);
            _hatchStyles.Add("pct90", ChartFormatPatternFillStyle.Percent90);
            _hatchStyles.Add("horz", ChartFormatPatternFillStyle.Horizontal);
            _hatchStyles.Add("vert", ChartFormatPatternFillStyle.Vertical);
            _hatchStyles.Add("ltHorz", ChartFormatPatternFillStyle.LightHorizontal);
            _hatchStyles.Add("ltVert", ChartFormatPatternFillStyle.LightVertical);
            _hatchStyles.Add("dkHorz", ChartFormatPatternFillStyle.DarkHorizontal);
            _hatchStyles.Add("dkVert", ChartFormatPatternFillStyle.DarkVertical);
            _hatchStyles.Add("narHorz", ChartFormatPatternFillStyle.NarrowHorizontal);
            _hatchStyles.Add("narVert", ChartFormatPatternFillStyle.NarrowVertical);
            _hatchStyles.Add("dashHorz", ChartFormatPatternFillStyle.DashedHorizontal);
            _hatchStyles.Add("dashVert", ChartFormatPatternFillStyle.DashedVertical);
            _hatchStyles.Add("cross", ChartFormatPatternFillStyle.Cross);
            _hatchStyles.Add("dnDiag", ChartFormatPatternFillStyle.ForwardDiagonal);
            _hatchStyles.Add("upDiag", ChartFormatPatternFillStyle.BackwardDiagonal);
            _hatchStyles.Add("ltDnDiag", ChartFormatPatternFillStyle.LightDownwardDiagonal);
            _hatchStyles.Add("ltUpDiag", ChartFormatPatternFillStyle.LightUpwardDiagonal);
            _hatchStyles.Add("dkDnDiag", ChartFormatPatternFillStyle.DarkDownwardDiagonal);
            _hatchStyles.Add("dkUpDiag", ChartFormatPatternFillStyle.DarkUpwardDiagonal);
            _hatchStyles.Add("wdDnDiag", ChartFormatPatternFillStyle.WideDownwardDiagonal);
            _hatchStyles.Add("wdUpDiag", ChartFormatPatternFillStyle.WideUpwardDiagonal);
            _hatchStyles.Add("dashDnDiag", ChartFormatPatternFillStyle.DashedDownwardDiagonal);
            _hatchStyles.Add("dashUpDiag", ChartFormatPatternFillStyle.DashedUpwardDiagonal);
            _hatchStyles.Add("diagCross", ChartFormatPatternFillStyle.DiagonalCross);
            _hatchStyles.Add("smCheck", ChartFormatPatternFillStyle.SmallCheckerBoard);
            _hatchStyles.Add("lgCheck", ChartFormatPatternFillStyle.LargeCheckerBoard);
            _hatchStyles.Add("smGrid", ChartFormatPatternFillStyle.SmallGrid);
            _hatchStyles.Add("lgGrid", ChartFormatPatternFillStyle.LargeGrid);
            _hatchStyles.Add("dotGrid", ChartFormatPatternFillStyle.DottedGrid);
            _hatchStyles.Add("smConfetti", ChartFormatPatternFillStyle.SmallConfetti);
            _hatchStyles.Add("lgConfetti", ChartFormatPatternFillStyle.LargeConfetti);
            _hatchStyles.Add("horzBrick", ChartFormatPatternFillStyle.HorizontalBrick);
            _hatchStyles.Add("diagBrick", ChartFormatPatternFillStyle.DiagonalBrick);
            _hatchStyles.Add("solidDmnd", ChartFormatPatternFillStyle.SolidDiamond);
            _hatchStyles.Add("openDmnd", ChartFormatPatternFillStyle.OutlinedDiamond);
            _hatchStyles.Add("dotDmnd", ChartFormatPatternFillStyle.DottedDiamond);
            _hatchStyles.Add("plaid", ChartFormatPatternFillStyle.Plaid);
            _hatchStyles.Add("sphere", ChartFormatPatternFillStyle.Sphere);
            _hatchStyles.Add("weave", ChartFormatPatternFillStyle.Weave);
            _hatchStyles.Add("divot", ChartFormatPatternFillStyle.Divot);
            _hatchStyles.Add("shingle", ChartFormatPatternFillStyle.Shingle);
            _hatchStyles.Add("wave", ChartFormatPatternFillStyle.Wave);
            _hatchStyles.Add("trellis", ChartFormatPatternFillStyle.Trellis);
            _hatchStyles.Add("zigZag", ChartFormatPatternFillStyle.ZigZag);
        }

        internal void ReadXml(XElement node)
        {
            string str = node.GetAttributeValueOrDefaultOfStringType("prst", "pct5");
            ChartFormatPatternFillStyle style = ChartFormatPatternFillStyle.Percent05;
            _hatchStyles.TryGetValue(str, out style);
            this.FillPattern = style;
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "bgClr")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        this.BackgroudDrawingColorSettings = new ExcelDrawingColorSettings();
                        this.BackgroundColor = ChartColorHelper.ReadColor(element2, this.BackgroudDrawingColorSettings);
                    }
                }
                else if (element.Name.LocalName == "fgClr")
                {
                    foreach (XElement element3 in element.Elements())
                    {
                        this.ForegroudDrawingColorSettings = new ExcelDrawingColorSettings();
                        this.ForegroundColor = ChartColorHelper.ReadColor(element3, this.ForegroudDrawingColorSettings);
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("pattFill", null, "a"))
            {
                string str = "pct5";
                foreach (KeyValuePair<string, ChartFormatPatternFillStyle> pair in _hatchStyles)
                {
                    if (((ChartFormatPatternFillStyle) pair.Value) == this.FillPattern)
                    {
                        str = pair.Key;
                        break;
                    }
                }
                writer.WriteAttributeString("prst", null, str);
                if (this.ForegroundColor != null)
                {
                    using (writer.WriteElement("fgClr", null, "a"))
                    {
                        ChartColorHelper.WriteColor(writer, this.ForegroundColor, this.ForegroudDrawingColorSettings);
                    }
                }
                if (this.BackgroundColor != null)
                {
                    using (writer.WriteElement("bgClr", null, "a"))
                    {
                        ChartColorHelper.WriteColor(writer, this.BackgroundColor, this.BackgroudDrawingColorSettings);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the backgroud drawing color settings.
        /// </summary>
        public ExcelDrawingColorSettings BackgroudDrawingColorSettings { get; set; }

        /// <summary>
        /// Specifies the fill background color.
        /// </summary>
        public IExcelColor BackgroundColor { get; set; }

        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get { return  Dt.Xls.Chart.FillFormatType.PatternFill; }
        }

        /// <summary>
        /// Specifies the fill pattern.
        /// </summary>
        public ChartFormatPatternFillStyle FillPattern { get; set; }

        /// <summary>
        /// Gets or sets the foregroud drawing color settings.
        /// </summary>
        public ExcelDrawingColorSettings ForegroudDrawingColorSettings { get; set; }

        /// <summary>
        /// Specifies the fill fore ground color.
        /// </summary>
        public IExcelColor ForegroundColor { get; set; }
    }
}

