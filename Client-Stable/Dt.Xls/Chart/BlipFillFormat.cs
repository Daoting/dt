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
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// 
    /// </summary>
    public class BlipFillFormat : IFillFormat
    {
        private double _stretchBottomOffset;
        private double _stretchLeftOffset;
        private double _stretchRightOffset;
        private double _stretchTopOffset;
        private Dt.Xls.Chart.Tile _tile;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.BlipFillFormat" /> class.
        /// </summary>
        public BlipFillFormat()
        {
            this.Transparency = 1.0;
        }

        private byte[] ReadStreamFully(Stream input)
        {
            if (input != null)
            {
                input.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                byte[] buffer = new byte[0x4000];
                using (MemoryStream stream = new MemoryStream())
                {
                    int num;
                    while ((num = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, num);
                    }
                    return stream.ToArray();
                }
            }
            return new byte[0];
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            this.RotateWithShape = node.GetAttributeValueOrDefaultOfBooleanType("rotWithShape", false);
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "blip")
                {
                    string str = element.GetAttributeValueOrDefaultOfStringType("embed", null);
                    if (!string.IsNullOrWhiteSpace(str) && (xFile != null))
                    {
                        Dictionary<string, XFile> dictionary = new Dictionary<string, XFile>();
                        if (xFile.RelationFiles != null)
                        {
                            foreach (KeyValuePair<string, XFile> pair in xFile.RelationFiles)
                            {
                                XFile fileByRelationID = xFile.GetFileByRelationID(pair.Key);
                                if (fileByRelationID.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image")
                                {
                                    dictionary.Add(pair.Key, fileByRelationID);
                                }
                            }
                        }
                        XFile file2 = null;
                        if (dictionary.TryGetValue(str, out file2))
                        {
                            byte[] sourceArray = this.ReadStreamFully(mFolder.GetFile(file2.FileName));
                            string str2 = Path.GetExtension(file2.FileName).ToUpperInvariant();
                            ImageType bitmap = ImageType.Bitmap;
                            switch (str2)
                            {
                                case ".BMP":
                                    bitmap = ImageType.Bitmap;
                                    break;

                                case ".PNG":
                                    bitmap = ImageType.PNG;
                                    break;

                                case ".JPG":
                                case ".JPEG":
                                    bitmap = ImageType.JPG;
                                    break;

                                case ".GIF":
                                    bitmap = ImageType.Gif;
                                    break;
                            }
                            this.ExcelImage = new Dt.Xls.ExcelImage(null, bitmap, sourceArray);
                        }
                    }
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "alphaModFix")
                        {
                            this.Transparency = 1.0 - (element2.GetAttributeValueOrDefaultOfDoubleType("amt", 0.0) / 100000.0);
                        }
                    }
                    continue;
                }
                if (element.Name.LocalName == "tile")
                {
                    Dt.Xls.Chart.Tile tile = new Dt.Xls.Chart.Tile();
                    tile.ReadXml(element, mFolder, xFile);
                    this.Tile = tile;
                }
                else if (element.Name.LocalName == "stretch")
                {
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName == "fillRect")
                        {
                            this.StretchLeftOffset = element3.GetAttributeValueOrDefaultOfDoubleType("l", 0.0) / 100000.0;
                            this.StretchRightOffset = element3.GetAttributeValueOrDefaultOfDoubleType("r", 0.0) / 100000.0;
                            this.StretchTopOffset = element3.GetAttributeValueOrDefaultOfDoubleType("t", 0.0) / 100000.0;
                            this.StretchBottomOffset = element3.GetAttributeValueOrDefaultOfDoubleType("b", 0.0) / 100000.0;
                        }
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("blipFill", null, "a"))
            {
                if (this.RotateWithShape)
                {
                    writer.WriteAttributeString("rotWithShape", "1");
                }
                using (writer.WriteElement("blip", null, "a"))
                {
                    mFolder.ImageCounter++;
                    XFile excelImageIdex = mFolder.GetExcelImageIdex(this.ExcelImage);
                    if (excelImageIdex != null)
                    {
                        int num = chartFile.RelationFiles.Count + 1;
                        string str = "rId" + ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                        chartFile.RelationFiles.Add(str, excelImageIdex);
                        writer.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                        writer.WriteAttributeString("r", "embed", null, str);
                    }
                    if (!(this.Transparency - 1.0).IsZero())
                    {
                        using (writer.WriteElement("alphaModFix", null, "a"))
                        {
                            int num2 = (int)((1.0 - Transparency) * 100000.0);
                            writer.WriteAttributeString("amt", ((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
                using (writer.WriteElement("srcRect", null, "a"))
                {
                }
                if (this.Tile != null)
                {
                    this.Tile.WriteXml(writer, mFolder, chartFile);
                }
                else
                {
                    using (writer.WriteElement("stretch", null, "a"))
                    {
                        using (writer.WriteElement("fillRect", null, "a"))
                        {
                            if (!this.StretchLeftOffset.IsZero())
                            {
                                int num3 = (int)(StretchLeftOffset * 100000.0);
                                writer.WriteAttributeString("l", ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.StretchTopOffset.IsZero())
                            {
                                int num4 = (int)(StretchTopOffset * 100000.0);
                                writer.WriteAttributeString("t", ((int) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.StretchRightOffset.IsZero())
                            {
                                int num5 = (int)(StretchRightOffset * 100000.0);
                                writer.WriteAttributeString("r", ((int) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.StretchBottomOffset.IsZero())
                            {
                                int num6 = (int)(StretchBottomOffset * 100000.0);
                                writer.WriteAttributeString("b", ((int) num6).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the excel image.
        /// </summary>
        public IExcelImage ExcelImage { get; set; }

        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get { return  Dt.Xls.Chart.FillFormatType.PictureFill; }
        }

        /// <summary>
        /// Specifies whether the blip will ratate with ship
        /// </summary>
        public bool RotateWithShape { get; set; }

        /// <summary>
        /// Specifies the bottom edge of the rectangle of the stretch
        /// </summary>
        public double StretchBottomOffset
        {
            get { return  this._stretchBottomOffset; }
            set
            {
                this._stretchBottomOffset = value;
                this._tile = null;
            }
        }

        /// <summary>
        /// Specifies the left edge of the rectangle of the stretch
        /// </summary>
        public double StretchLeftOffset
        {
            get { return  this._stretchLeftOffset; }
            set
            {
                this._stretchLeftOffset = value;
                this._tile = null;
            }
        }

        /// <summary>
        /// Specifies the right edge of the rectangle of the stretch.
        /// </summary>
        public double StretchRightOffset
        {
            get { return  this._stretchRightOffset; }
            set
            {
                this._stretchRightOffset = value;
                this._tile = null;
            }
        }

        /// <summary>
        /// Specifies the top edge of the rectangle of the stretch
        /// </summary>
        public double StretchTopOffset
        {
            get { return  this._stretchTopOffset; }
            set
            {
                this._stretchTopOffset = value;
                this._tile = null;
            }
        }

        /// <summary>
        /// Represents the image tile settings.
        /// </summary>
        public Dt.Xls.Chart.Tile Tile
        {
            get { return  this._tile; }
            set
            {
                this._tile = value;
                this._stretchBottomOffset = 0.0;
                this._stretchTopOffset = 0.0;
                this._stretchLeftOffset = 0.0;
                this._stretchRightOffset = 0.0;
            }
        }

        /// <summary>
        /// Represents the blipFill transparency.
        /// </summary>
        /// <value>
        /// The transparency.
        /// </value>
        public double Transparency { get; set; }
    }
}

