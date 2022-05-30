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
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// specifies the 3-D view of the chart.
    /// </summary>
    public class ViewIn3D
    {
        private double depthPercent = 100.0;
        private double heightPercent = 100.0;
        private double perspective = 15.0;
        private bool rightAngleAxes = true;
        private double xRotation = 20.0;
        private double yRotation = 15.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ViewIn3D" /> class.
        /// </summary>
        public ViewIn3D()
        {
            this.perspective = 15.0;
        }

        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "rotY")
                {
                    this.RotationX = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "rotX")
                {
                    this.RotationY = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "rAngAx")
                {
                    this.RightAngleAxes = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "perspective")
                {
                    this.RightAngleAxes = false;
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("val", 30);
                    if (num == 0)
                    {
                        this.perspective = 0.1;
                    }
                    else
                    {
                        this.perspective = num / 2;
                    }
                }
                else if (element.Name.LocalName == "hPercent")
                {
                    this.HeightPercent = element.GetAttributeValueOrDefaultOfInt32Type("val", 100);
                }
                else if (element.Name.LocalName == "depthPercent")
                {
                    this.DepthPercent = element.GetAttributeValueOrDefaultOfInt32Type("val", 100);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("view3D", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("rotX", null, "c", "val", ((double) this.RotationY).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteLeafElementWithAttribute("rotY", null, "c", "val", ((double) this.RotationX).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteLeafElementWithAttribute("rAngAx", null, "c", "val", this.RightAngleAxes ? "1" : "0");
                if (this.perspective != 15.0)
                {
                    int num = (int)(perspective * 2.0);
                    writer.WriteLeafElementWithAttribute("perspective", null, "c", "val", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.HeightPercent != 100.0)
                {
                    writer.WriteLeafElementWithAttribute("hPercent", null, "c", "val", ((double) this.HeightPercent).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.DepthPercent != 100.0)
                {
                    writer.WriteLeafElementWithAttribute("depthPercent", null, "c", "val", ((double) this.DepthPercent).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// specifies the depth of a 3-D chart as a percentage of the chart width.
        /// </summary>
        public double DepthPercent
        {
            get { return  this.depthPercent; }
            set
            {
                if ((value < 20.0) || (value > 2000.0))
                {
                    throw new ArgumentOutOfRangeException("DepthPercent");
                }
                this.depthPercent = value;
            }
        }

        /// <summary>
        /// specifies the height of a 3-D chart as a percentage of the chart width.
        /// </summary>
        public double HeightPercent
        {
            get { return  this.heightPercent; }
            set
            {
                if ((value < 5.0) || (value > 500.0))
                {
                    throw new ArgumentOutOfRangeException("HeightPercent");
                }
                this.heightPercent = value;
            }
        }

        /// <summary>
        /// Specifies the field of view angle for the 3-D chart. the element is ignored if right angle axes is true.
        /// </summary>
        public double Perspective
        {
            get { return  this.perspective; }
            set
            {
                if ((value < 0.0) || (value > 100.0))
                {
                    throw new ArgumentOutOfRangeException("Perspective");
                }
                this.perspective = value;
            }
        }

        /// <summary>
        /// specifies that the chart axes are at the right angle, rather than drawn in persperction. applies only to 3-D charts.
        /// </summary>
        public bool RightAngleAxes
        {
            get { return  this.rightAngleAxes; }
            set { this.rightAngleAxes = value; }
        }

        /// <summary>
        /// specifies the amount a 3-D chart shall be rorated in the X direciton
        /// </summary>
        public double RotationX
        {
            get { return  this.xRotation; }
            set
            {
                if ((value > 360.0) || (value < 0.0))
                {
                    throw new ArgumentOutOfRangeException("XRotation");
                }
                this.xRotation = value;
            }
        }

        /// <summary>
        /// specifies the amount a 3-D chart shall be rorated in the Y direciton
        /// </summary>
        public double RotationY
        {
            get { return  this.yRotation; }
            set
            {
                if ((value > 90.0) || (value < -90.0))
                {
                    throw new ArgumentOutOfRangeException("YRotation");
                }
                this.yRotation = value;
            }
        }
    }
}

