#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the paper size.
    /// </summary>
    public class PaperSize : IXmlSerializable
    {
        static readonly Dictionary<PrintMediaSize, Size> _dict;
        double _height;
        double _width;
        PrintMediaSize _mediaSize;
        
        static PaperSize()
        {
            Dictionary<PrintMediaSize, Size> dict = new Dictionary<PrintMediaSize, Size>();
            dict.Add(PrintMediaSize.PrinterCustom, Size.Empty);
            dict.Add(PrintMediaSize.BusinessCard, new Size(10, 20));
            dict.Add(PrintMediaSize.IsoA2, new Size(1587.35998535156, 2244.9599609375));
            dict.Add(PrintMediaSize.IsoA3, new Size(1122.48193359375, 1587.35998535156));
            dict.Add(PrintMediaSize.IsoA3Extra, new Size(1216.95874023438, 1681.84069824219));
            dict.Add(PrintMediaSize.IsoA3Rotated, new Size(1587.35998535156, 1122.48193359375));
            dict.Add(PrintMediaSize.IsoA4, new Size(793.681884765625, 1122.48193359375));
            dict.Add(PrintMediaSize.IsoA4Extra, new Size(889.681884765625, 1218.08129882813));
            dict.Add(PrintMediaSize.IsoA4Rotated, new Size(1122.48193359375, 793.681884765625));
            dict.Add(PrintMediaSize.IsoA5, new Size(559.358764648438, 793.681884765625));
            dict.Add(PrintMediaSize.IsoA5Extra, new Size(657.600036621094, 888.158752441406));
            dict.Add(PrintMediaSize.IsoA5Rotated, new Size(793.681884765625, 559.358764648438));
            dict.Add(PrintMediaSize.IsoA6, new Size(396.80126953125, 559.358764648438));
            dict.Add(PrintMediaSize.IsoA6Rotated, new Size(559.358764648438, 396.80126953125));
            dict.Add(PrintMediaSize.IsoB4, new Size(944.881896972656, 1334.16186523438));
            dict.Add(PrintMediaSize.IsoB4Envelope, new Size(944.881896972656, 1334.16186523438));
            dict.Add(PrintMediaSize.IsoB5Envelope, new Size(665.121276855469, 944.881896972656));
            dict.Add(PrintMediaSize.IsoB5Extra, new Size(759.681274414063, 1043.11938476563));
            dict.Add(PrintMediaSize.IsoC3Envelope, new Size(1224.55944824219, 1730.95935058594));
            dict.Add(PrintMediaSize.IsoC4Envelope, new Size(865.440002441406, 1224.55944824219));
            dict.Add(PrintMediaSize.IsoC5Envelope, new Size(612.241882324219, 865.440002441406));
            dict.Add(PrintMediaSize.IsoC6C5Envelope, new Size(430.801910400391, 865.440002441406));
            dict.Add(PrintMediaSize.IsoC6Envelope, new Size(430.801910400391, 612.241882324219));
            dict.Add(PrintMediaSize.IsoDLEnvelope, new Size(415.679992675781, 831.439392089844));
            dict.Add(PrintMediaSize.JapanChou3Envelope, new Size(453.520629882813, 888.158752441406));
            dict.Add(PrintMediaSize.JapanChou3EnvelopeRotated, new Size(888.158752441406, 453.520629882813));
            dict.Add(PrintMediaSize.JapanChou4Envelope, new Size(340.081909179688, 774.799377441406));
            dict.Add(PrintMediaSize.JapanChou4EnvelopeRotated, new Size(774.799377441406, 340.081909179688));
            dict.Add(PrintMediaSize.JapanDoubleHagakiPostcard, new Size(755.841247558594, 559.358764648438));
            dict.Add(PrintMediaSize.JapanDoubleHagakiPostcardRotated, new Size(559.358764648438, 755.841247558594));
            dict.Add(PrintMediaSize.JapanHagakiPostcard, new Size(377.918731689453, 559.358764648438));
            dict.Add(PrintMediaSize.JapanHagakiPostcardRotated, new Size(559.358764648438, 377.918731689453));
            dict.Add(PrintMediaSize.JapanKaku2Envelope, new Size(907.041259765625, 1254.79943847656));
            dict.Add(PrintMediaSize.JapanKaku2EnvelopeRotated, new Size(1254.79943847656, 907.041259765625));
            dict.Add(PrintMediaSize.JapanKaku3Envelope, new Size(816.3212890625, 1046.88000488281));
            dict.Add(PrintMediaSize.JapanKaku3EnvelopeRotated, new Size(1046.88000488281, 816.3212890625));
            dict.Add(PrintMediaSize.JapanYou4Envelope, new Size(396.80126953125, 888.158752441406));
            dict.Add(PrintMediaSize.JapanYou4EnvelopeRotated, new Size(888.158752441406, 396.80126953125));
            dict.Add(PrintMediaSize.JisB4, new Size(971.281921386719, 1375.68005371094));
            dict.Add(PrintMediaSize.JisB4Rotated, new Size(1375.68005371094, 971.281921386719));
            dict.Add(PrintMediaSize.JisB5, new Size(687.840026855469, 971.281921386719));
            dict.Add(PrintMediaSize.JisB5Rotated, new Size(971.281921386719, 687.840026855469));
            dict.Add(PrintMediaSize.JisB6, new Size(483.760650634766, 687.840026855469));
            dict.Add(PrintMediaSize.JisB6Rotated, new Size(687.840026855469, 483.760650634766));
            dict.Add(PrintMediaSize.NorthAmerica10x11, new Size(960, 1056));
            dict.Add(PrintMediaSize.NorthAmerica10x14, new Size(960, 1344));
            dict.Add(PrintMediaSize.NorthAmerica11x17, new Size(1056, 1632));
            dict.Add(PrintMediaSize.NorthAmerica9x11, new Size(864, 1056));
            dict.Add(PrintMediaSize.NorthAmericaCSheet, new Size(1632, 2112));
            dict.Add(PrintMediaSize.NorthAmericaDSheet, new Size(2112, 3264));
            dict.Add(PrintMediaSize.NorthAmericaESheet, new Size(3264, 4224));
            dict.Add(PrintMediaSize.NorthAmericaExecutive, new Size(695.761901855469, 1008));
            dict.Add(PrintMediaSize.NorthAmericaGermanLegalFanfold, new Size(816, 1248));
            dict.Add(PrintMediaSize.NorthAmericaGermanStandardFanfold, new Size(816, 1152));
            dict.Add(PrintMediaSize.NorthAmericaLegal, new Size(816, 1344));
            dict.Add(PrintMediaSize.NorthAmericaLegalExtra, new Size(912, 1440));
            dict.Add(PrintMediaSize.NorthAmericaLetter, new Size(816, 1056));
            dict.Add(PrintMediaSize.NorthAmericaLetterExtra, new Size(912, 1152));
            dict.Add(PrintMediaSize.NorthAmericaLetterPlus, new Size(816, 1218.08129882813));
            dict.Add(PrintMediaSize.NorthAmericaLetterRotated, new Size(1056, 816));
            dict.Add(PrintMediaSize.NorthAmericaMonarchEnvelope, new Size(371.841278076172, 720));
            dict.Add(PrintMediaSize.NorthAmericaNote, new Size(816, 1056));
            dict.Add(PrintMediaSize.NorthAmericaNumber10Envelope, new Size(395.678741455078, 912));
            dict.Add(PrintMediaSize.NorthAmericaNumber11Envelope, new Size(432, 995.841247558594));
            dict.Add(PrintMediaSize.NorthAmericaNumber12Envelope, new Size(455.761901855469, 1056));
            dict.Add(PrintMediaSize.NorthAmericaNumber14Envelope, new Size(480, 1104));
            dict.Add(PrintMediaSize.NorthAmericaNumber9Envelope, new Size(371.841278076172, 851.841247558594));
            dict.Add(PrintMediaSize.NorthAmericaPersonalEnvelope, new Size(347.678741455078, 624));
            dict.Add(PrintMediaSize.NorthAmericaQuarto, new Size(812.560668945313, 1039.35876464844));
            dict.Add(PrintMediaSize.NorthAmericaStatement, new Size(528, 816));
            dict.Add(PrintMediaSize.NorthAmericaSuperA, new Size(857.918762207031, 1345.44006347656));
            dict.Add(PrintMediaSize.NorthAmericaSuperB, new Size(1152.72192382813, 1840.56188964844));
            dict.Add(PrintMediaSize.NorthAmericaTabloid, new Size(1056, 1632));
            dict.Add(PrintMediaSize.OtherMetricA4Plus, new Size(793.681884765625, 1247.19873046875));
            dict.Add(PrintMediaSize.OtherMetricFolio, new Size(816, 1248));
            dict.Add(PrintMediaSize.OtherMetricInviteEnvelope, new Size(831.439392089844, 831.439392089844));
            dict.Add(PrintMediaSize.OtherMetricItalianEnvelope, new Size(415.679992675781, 869.280029296875));
            dict.Add(PrintMediaSize.Prc10Envelope, new Size(1224.55944824219, 1730.95935058594));
            dict.Add(PrintMediaSize.Prc1Envelope, new Size(385.440002441406, 623.599365234375));
            dict.Add(PrintMediaSize.Prc1EnvelopeRotated, new Size(623.599365234375, 385.440002441406));
            dict.Add(PrintMediaSize.Prc3Envelope, new Size(472.399383544922, 665.121276855469));
            dict.Add(PrintMediaSize.Prc3EnvelopeRotated, new Size(665.121276855469, 472.399383544922));
            dict.Add(PrintMediaSize.Prc4Envelope, new Size(415.679992675781, 786.081298828125));
            dict.Add(PrintMediaSize.Prc4EnvelopeRotated, new Size(786.081298828125, 415.679992675781));
            dict.Add(PrintMediaSize.Prc5Envelope, new Size(415.679992675781, 831.439392089844));
            dict.Add(PrintMediaSize.Prc5EnvelopeRotated, new Size(831.439392089844, 415.679992675781));
            dict.Add(PrintMediaSize.Prc6Envelope, new Size(453.520629882813, 869.280029296875));
            dict.Add(PrintMediaSize.Prc6EnvelopeRotated, new Size(869.280029296875, 453.520629882813));
            dict.Add(PrintMediaSize.Prc7Envelope, new Size(604.720642089844, 869.280029296875));
            dict.Add(PrintMediaSize.Prc7EnvelopeRotated, new Size(869.280029296875, 604.720642089844));
            dict.Add(PrintMediaSize.Prc8Envelope, new Size(453.520629882813, 1167.83996582031));
            dict.Add(PrintMediaSize.Prc8EnvelopeRotated, new Size(1167.83996582031, 453.520629882813));
            dict.Add(PrintMediaSize.Prc9Envelope, new Size(865.440002441406, 1224.55944824219));
            dict.Add(PrintMediaSize.Prc9EnvelopeRotated, new Size(1224.55944824219, 865.440002441406));
            _dict = dict;
        }

        /// <summary>
        /// Creates a new paper size setting.
        /// </summary>
        public PaperSize()
        {
            Init();
        }

        public PaperSize(PrintMediaSize mediaSize) : this()
        {
            MediaSize = mediaSize;
        }

        public static Dictionary<PrintMediaSize, Size> Dict
        {
            get { return _dict; }
        }

        /// <summary>
        /// Creates a new paper size setting with a custom paper size.
        /// </summary>
        /// <param name="width">The custom width.</param>
        /// <param name="height">The custom height.</param>
        public PaperSize(double width, double height) : this()
        {
            if (width <= 0.0)
            {
                throw new ArgumentOutOfRangeException("width", ResourceStrings.ReportingPaperSizeWidthError);
            }
            if (height <= 0.0)
            {
                throw new ArgumentOutOfRangeException("height", ResourceStrings.ReportingPaperSizeHightError);
            }
            _mediaSize = PrintMediaSize.PrinterCustom;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Init()
        {
            _mediaSize = PrintMediaSize.NorthAmericaLetter;
            Size pageSize = _dict[_mediaSize];
            _width = pageSize.Width;
            _height = pageSize.Height;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlBase(XmlReader reader)
        {
            string str;
            Serializer.InitReader(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "MediaSize")
                {
                    if (str != "PaperWidth")
                    {
                        if (str == "PaperHeight")
                        {
                            _height = Serializer.ReadAttributeDouble("value", 0.0, reader);
                        }
                        return;
                    }
                }
                else
                {
                    _mediaSize = Serializer.ReadAttributeEnum("value", PrintMediaSize.NorthAmericaLetter, reader);
                    if (_mediaSize != PrintMediaSize.PrinterCustom)
                    {
                        try
                        {
                            Size pageSize = _dict[_mediaSize];
                            _width = (int)pageSize.Width;
                            _height = (int)pageSize.Height;
                        }
                        catch
                        {

                        }
                    }
                    return;
                }
                _width = Serializer.ReadAttributeDouble("value", 0.0, reader);
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
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            Init();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
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
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteXmlBase(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (_mediaSize != PrintMediaSize.NorthAmericaLetter)
            {
                Serializer.SerializeObj(_mediaSize, "MediaSize", false, writer);
            }
            if (_mediaSize == PrintMediaSize.NotAvailable)
            {
                Serializer.SerializeObj((double) _width, "PaperWidth", false, writer);
                Serializer.SerializeObj((double) _height, "PaperHeight", false, writer);
            }
        }

        /// <summary>
        /// Gets or sets the height of the paper, in hundredths of an inch. 
        /// </summary>
        /// <value>The height of the paper. The default value is 1100, which is 11 inches.</value>
        [DefaultValue(0x44c)]
        public double Height
        {
            get { return  _height; }
            set
            {
                if (_height != value)
                {
                    _mediaSize = PrintMediaSize.NotAvailable;
                }
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("height", ResourceStrings.ReportingPaperSizeHightError);
                }
                _height = value;
            }
        }

        /// <summary>
        /// 获取设置纸张类型
        /// </summary>
        public PrintMediaSize MediaSize
        {
            get { return _mediaSize; }
            set
            {
                if (_mediaSize != value)
                {
                    _mediaSize = value;
                    try
                    {
                        Size pageSize = _dict[_mediaSize];
                        _width = pageSize.Width;
                        _height = pageSize.Height;
                    }
                    catch
                    {
                       
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the paper, in hundredths of an inch. 
        /// </summary>
        /// <value>The width of the paper. The default value is 850, which is 8.5 inches.</value>
        [DefaultValue(850)]
        public double Width
        {
            get { return  _width; }
            set
            {
                if (_width != value)
                {
                    _mediaSize = PrintMediaSize.PrinterCustom;
                }
                if (value <= 0.0)
                {
                    throw new ArgumentOutOfRangeException("width", ResourceStrings.ReportingPaperSizeWidthError);
                }
                _width = value;
            }
        }
    }
}

