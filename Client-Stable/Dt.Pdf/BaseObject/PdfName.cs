#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Exceptions;
using System;
using System.Text;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Name.
    /// </summary>
    public class PdfName : PdfObjectBase
    {
        /// <summary>
        /// PdfName A
        /// </summary>
        public static readonly PdfName A = new PdfName("A", true);
        /// <summary>
        /// PdfName AA
        /// </summary>
        public static readonly PdfName AA = new PdfName("AA", true);
        /// <summary>
        /// PdfName AbsoluteColorimetric
        /// </summary>
        public static readonly PdfName AbsoluteColorimetric = new PdfName("AbsoluteColorimetric", true);
        /// <summary>
        /// PdfName AcroForm
        /// </summary>
        public static readonly PdfName AcroForm = new PdfName("AcroForm", true);
        /// <summary>
        /// PdfName Action
        /// </summary>
        public static readonly PdfName Action = new PdfName("Action", true);
        /// <summary>
        /// PdfName Annot
        /// </summary>
        public static readonly PdfName Annot = new PdfName("Annot", true);
        /// <summary>
        /// PdfName Annots
        /// </summary>
        public static readonly PdfName Annots = new PdfName("Annots", true);
        /// <summary>
        /// PdfName AntiAlias
        /// </summary>
        public static readonly PdfName AntiAlias = new PdfName("AntiAlias", true);
        /// <summary>
        /// PdfName AP
        /// </summary>
        public static readonly PdfName AP = new PdfName("AP", true);
        /// <summary>
        /// PdfName Ascent
        /// </summary>
        public static readonly PdfName Ascent = new PdfName("Ascent", true);
        /// <summary>
        /// PdfName ASCII85Decode
        /// </summary>
        public static readonly PdfName ASCII85Decode = new PdfName("ASCII85Decode", true);
        /// <summary>
        /// PdfName ASCIIHexDecode
        /// </summary>
        public static readonly PdfName ASCIIHexDecode = new PdfName("ASCIIHexDecode", true);
        /// <summary>
        /// PdfName Author
        /// </summary>
        public static readonly PdfName Author = new PdfName("Author", true);
        /// <summary>
        /// PdfName BaseFont
        /// </summary>
        public static readonly PdfName BaseFont = new PdfName("BaseFont", true);
        /// <summary>
        /// PdfName BBox
        /// </summary>
        public static readonly PdfName BBox = new PdfName("BBox", true);
        /// <summary>
        /// PdfName BitsPerComponent
        /// </summary>
        public static readonly PdfName BitsPerComponent = new PdfName("BitsPerComponent", true);
        /// <summary>
        /// PdfName BitsPerSample
        /// </summary>
        public static readonly PdfName BitsPerSample = new PdfName("BitsPerSample", true);
        /// <summary>
        /// PdfName BlackIs1
        /// </summary>
        public static readonly PdfName BlackIs1 = new PdfName("BlackIs1", true);
        /// <summary>
        /// PdfName Blinds
        /// </summary>
        public static readonly PdfName Blinds = new PdfName("Blinds", true);
        /// <summary>
        /// PdfName Border
        /// </summary>
        public static readonly PdfName Border = new PdfName("Border", true);
        /// <summary>
        /// PdfName Bounds
        /// </summary>
        public static readonly PdfName Bounds = new PdfName("Bounds", true);
        /// <summary>
        /// PdfName Box
        /// </summary>
        public static readonly PdfName Box = new PdfName("Box", true);
        /// <summary>
        /// PdfName C0
        /// </summary>
        public static readonly PdfName C0 = new PdfName("C0", true);
        /// <summary>
        /// PdfName C1
        /// </summary>
        public static readonly PdfName C1 = new PdfName("C1", true);
        /// <summary>
        /// PdfName ca
        /// </summary>
        public static readonly PdfName ca = new PdfName("ca", true);
        /// <summary>
        /// PdfName CA
        /// </summary>
        public static readonly PdfName CA = new PdfName("CA", true);
        /// <summary>
        /// PdfName CalGray
        /// </summary>
        public static readonly PdfName CalGray = new PdfName("CalGray", true);
        /// <summary>
        /// PdfName CalRGB
        /// </summary>
        public static readonly PdfName CalRGB = new PdfName("CalRGB", true);
        /// <summary>
        /// PdfName CapHeight
        /// </summary>
        public static readonly PdfName CapHeight = new PdfName("CapHeight", true);
        /// <summary>
        /// PdfName Catalog
        /// </summary>
        public static readonly PdfName Catalog = new PdfName("Catalog", true);
        /// <summary>
        /// PdfName CCITTFaxDecode
        /// </summary>
        public static readonly PdfName CCITTFaxDecode = new PdfName("CCITTFaxDecode", true);
        /// <summary>
        /// PdfName CenterWindow
        /// </summary>
        public static readonly PdfName CenterWindow = new PdfName("CenterWindow", true);
        /// <summary>
        /// PdfName CIDFontType0
        /// </summary>
        public static readonly PdfName CIDFontType0 = new PdfName("CIDFontType0", true);
        /// <summary>
        /// PdfName CIDFontType0C
        /// </summary>
        public static readonly PdfName CIDFontType0C = new PdfName("CIDFontType0C", true);
        /// <summary>
        /// PdfName CIDFontType2
        /// </summary>
        public static readonly PdfName CIDFontType2 = new PdfName("CIDFontType2", true);
        /// <summary>
        /// PdfName CIDSystemInfo
        /// </summary>
        public static readonly PdfName CIDSystemInfo = new PdfName("CIDSystemInfo", true);
        /// <summary>
        /// PdfName Colltion
        /// </summary>
        public static readonly PdfName Colltion = new PdfName("Colltion", true);
        /// <summary>
        /// PdfName Colors
        /// </summary>
        public static readonly PdfName Colors = new PdfName("Colors", true);
        /// <summary>
        /// PdfName ColorSpace
        /// </summary>
        public static readonly PdfName ColorSpace = new PdfName("ColorSpace", true);
        /// <summary>
        /// PdfName Columns
        /// </summary>
        public static readonly PdfName Columns = new PdfName("Columns", true);
        /// <summary>
        /// PdfName Contents
        /// </summary>
        public static readonly PdfName Contents = new PdfName("Contents", true);
        /// <summary>
        /// PdfName Coords
        /// </summary>
        public static readonly PdfName Coords = new PdfName("Coords", true);
        /// <summary>
        /// PdfName Count
        /// </summary>
        public static readonly PdfName Count = new PdfName("Count", true);
        /// <summary>
        /// PdfName Courier
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Courier = new PdfName("Courier", true);
        /// <summary>
        /// PdfName Courier−Bold
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Courier_Bold = new PdfName("Courier-Bold", true);
        /// <summary>
        /// PdfName Courier−BoldOblique
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Courier_BoldOblique = new PdfName("Courier-BoldOblique", true);
        /// <summary>
        /// PdfName Courier−Oblique
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Courier_Oblique = new PdfName("Courier-Oblique", true);
        /// <summary>
        /// PdfName Cover
        /// </summary>
        public static readonly PdfName Cover = new PdfName("Cover", true);
        /// <summary>
        /// PdfName CreationDate
        /// </summary>
        public static readonly PdfName CreationDate = new PdfName("CreationDate", true);
        /// <summary>
        /// PdfName Creator
        /// </summary>
        public static readonly PdfName Creator = new PdfName("Creator", true);
        /// <summary>
        /// PdfName DCTDecode
        /// </summary>
        public static readonly PdfName DCTDecode = new PdfName("DCTDecode", true);
        /// <summary>
        /// PdfName Decode
        /// </summary>
        public static readonly PdfName Decode = new PdfName("Decode", true);
        /// <summary>
        /// PdfName DecodeParms
        /// </summary>
        public static readonly PdfName DecodeParms = new PdfName("DecodeParms", true);
        /// <summary>
        /// PdfName DescendantFonts
        /// </summary>
        public static readonly PdfName DescendantFonts = new PdfName("DescendantFonts", true);
        /// <summary>
        /// PdfName Descent
        /// </summary>
        public static readonly PdfName Descent = new PdfName("Descent", true);
        /// <summary>
        /// PdfName Dest
        /// </summary>
        public static readonly PdfName Dest = new PdfName("Dest", true);
        /// <summary>
        /// PdfName Dests
        /// </summary>
        public static readonly PdfName Dests = new PdfName("Dests", true);
        /// <summary>
        /// PdfName DeviceCMYK
        /// </summary>
        public static readonly PdfName DeviceCMYK = new PdfName("DeviceCMYK", true);
        /// <summary>
        /// PdfName DeviceGray
        /// </summary>
        public static readonly PdfName DeviceGray = new PdfName("DeviceGray", true);
        /// <summary>
        /// PdfName DeviceRGB
        /// </summary>
        public static readonly PdfName DeviceRGB = new PdfName("DeviceRGB", true);
        /// <summary>
        /// PdfName Direction
        /// </summary>
        public static readonly PdfName Direction = new PdfName("Direction", true);
        /// <summary>
        /// PdfName DisplayDocTitle
        /// </summary>
        public static readonly PdfName DisplayDocTitle = new PdfName("DisplayDocTitle", true);
        /// <summary>
        /// PdfName Dissolve
        /// </summary>
        public static readonly PdfName Dissolve = new PdfName("Dissolve", true);
        /// <summary>
        /// PdfName Domain
        /// </summary>
        public static readonly PdfName Domain = new PdfName("Domain", true);
        /// <summary>
        /// PdfName Duplex
        /// </summary>
        public static readonly PdfName Duplex = new PdfName("Duplex", true);
        /// <summary>
        /// PdfName DuplexFlipLongEdge
        /// </summary>
        public static readonly PdfName DuplexFlipLongEdge = new PdfName("DuplexFlipLongEdge", true);
        /// <summary>
        /// PdfName DuplexFlipShortEdge
        /// </summary>
        public static readonly PdfName DuplexFlipShortEdge = new PdfName("DuplexFlipShortEdge", true);
        /// <summary>
        /// PdfName Dur
        /// </summary>
        public static readonly PdfName Dur = new PdfName("Dur", true);
        /// <summary>
        /// PdfName DW
        /// </summary>
        public static readonly PdfName DW = new PdfName("DW", true);
        /// <summary>
        /// PdfName EF
        /// </summary>
        public static readonly PdfName EF = new PdfName("EF", true);
        /// <summary>
        /// PdfName EmbeddedFile
        /// </summary>
        public static readonly PdfName EmbeddedFile = new PdfName("EmbeddedFile", true);
        /// <summary>
        /// PdfName Encode
        /// </summary>
        public static readonly PdfName Encode = new PdfName("Encode", true);
        /// <summary>
        /// PdfName EncodedByteAlign
        /// </summary>
        public static readonly PdfName EncodedByteAlign = new PdfName("EncodedByteAlign", true);
        /// <summary>
        /// PdfName Encoding
        /// </summary>
        public static readonly PdfName Encoding = new PdfName("Encoding", true);
        /// <summary>
        /// PdfName EndOfBlock
        /// </summary>
        public static readonly PdfName EndOfBlock = new PdfName("EndOfBlock", true);
        /// <summary>
        /// PdfName EndOfLine
        /// </summary>
        public static readonly PdfName EndOfLine = new PdfName("EndOfLine", true);
        /// <summary>
        /// PdfName Extend
        /// </summary>
        public static readonly PdfName Extend = new PdfName("Extend", true);
        /// <summary>
        /// PdfName ExtGState
        /// </summary>
        public static readonly PdfName ExtGState = new PdfName("ExtGState", true);
        /// <summary>
        /// PdfName F
        /// </summary>
        public static readonly PdfName F = new PdfName("F", true);
        /// <summary>
        /// PdfName Fade
        /// </summary>
        public static readonly PdfName Fade = new PdfName("Fade", true);
        /// <summary>
        /// PdfName FileAttachment
        /// </summary>
        public static readonly PdfName FileAttachment = new PdfName("FileAttachment", true);
        /// <summary>
        /// PdfName Filespec
        /// </summary>
        public static readonly PdfName Filespec = new PdfName("Filespec", true);
        /// <summary>
        /// PdfName Filter
        /// </summary>
        public static readonly PdfName Filter = new PdfName("Filter", true);
        /// <summary>
        /// PdfName First
        /// </summary>
        public static readonly PdfName First = new PdfName("First", true);
        /// <summary>
        /// PdfName FirstChar
        /// </summary>
        public static readonly PdfName FirstChar = new PdfName("FirstChar", true);
        /// <summary>
        /// PdfName Fit
        /// </summary>
        public static readonly PdfName Fit = new PdfName("Fit", true);
        /// <summary>
        /// PdfName FitB
        /// </summary>
        public static readonly PdfName FitB = new PdfName("FitB", true);
        /// <summary>
        /// PdfName FitBH
        /// </summary>
        public static readonly PdfName FitBH = new PdfName("FitBH", true);
        /// <summary>
        /// PdfName FitBV
        /// </summary>
        public static readonly PdfName FitBV = new PdfName("FitBV", true);
        /// <summary>
        /// PdfName FitH
        /// </summary>
        public static readonly PdfName FitH = new PdfName("FitH", true);
        /// <summary>
        /// PdfName FitR
        /// </summary>
        public static readonly PdfName FitR = new PdfName("FitR", true);
        /// <summary>
        /// PdfName FitV
        /// </summary>
        public static readonly PdfName FitV = new PdfName("FitV", true);
        /// <summary>
        /// PdfName FitWindow
        /// </summary>
        public static readonly PdfName FitWindow = new PdfName("FitWindow", true);
        /// <summary>
        /// PdfName FixedPrint
        /// </summary>
        public static readonly PdfName FixedPrint = new PdfName("FixedPrint", true);
        /// <summary>
        /// PdfName Flags
        /// </summary>
        public static readonly PdfName Flags = new PdfName("Flags", true);
        /// <summary>
        /// PdfName FlateDecode
        /// </summary>
        public static readonly PdfName FlateDecode = new PdfName("FlateDecode", true);
        /// <summary>
        /// PdfName Fly
        /// </summary>
        public static readonly PdfName Fly = new PdfName("Fly", true);
        /// <summary>
        /// PdfName Font
        /// </summary>
        public static readonly PdfName Font = new PdfName("Font", true);
        /// <summary>
        /// PdfName FontBBox
        /// </summary>
        public static readonly PdfName FontBBox = new PdfName("FontBBox", true);
        /// <summary>
        /// PdfName FontDescriptor
        /// </summary>
        public static readonly PdfName FontDescriptor = new PdfName("FontDescriptor", true);
        /// <summary>
        /// PdfName FontFile
        /// </summary>
        public static readonly PdfName FontFile = new PdfName("FontFile", true);
        /// <summary>
        /// PdfName FontFile2
        /// </summary>
        public static readonly PdfName FontFile2 = new PdfName("FontFile2", true);
        /// <summary>
        /// PdfName FontFile3
        /// </summary>
        public static readonly PdfName FontFile3 = new PdfName("FontFile3", true);
        /// <summary>
        /// PdfName FontName
        /// </summary>
        public static readonly PdfName FontName = new PdfName("FontName", true);
        /// <summary>
        /// PdfName Form
        /// </summary>
        public static readonly PdfName Form = new PdfName("Form", true);
        /// <summary>
        /// PdfName FS
        /// </summary>
        public static readonly PdfName FS = new PdfName("FS", true);
        /// <summary>
        /// PdfName FullScreen
        /// </summary>
        public static readonly PdfName FullScreen = new PdfName("FullScreen", true);
        /// <summary>
        /// PdfName Function
        /// </summary>
        public static readonly PdfName Function = new PdfName("Function", true);
        /// <summary>
        /// PdfName Functions
        /// </summary>
        public static readonly PdfName Functions = new PdfName("Functions", true);
        /// <summary>
        /// PdfName FunctionType
        /// </summary>
        public static readonly PdfName FunctionType = new PdfName("FunctionType", true);
        /// <summary>
        /// PdfName Gamma
        /// </summary>
        public static readonly PdfName Gamma = new PdfName("Gamma", true);
        /// <summary>
        /// PdfName Glitter
        /// </summary>
        public static readonly PdfName Glitter = new PdfName("Glitter", true);
        /// <summary>
        /// PdfName H
        /// </summary>
        public static readonly PdfName H = new PdfName("H", true);
        /// <summary>
        /// PdfName Height
        /// </summary>
        public static readonly PdfName Height = new PdfName("Height", true);
        /// <summary>
        /// PdfName Helvetica
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Helvetica = new PdfName("Helvetica", true);
        /// <summary>
        /// PdfName Helvetica−Bold
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Helvetica_Bold = new PdfName("Helvetica-Bold", true);
        /// <summary>
        /// PdfName Helvetica−BoldOblique
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Helvetica_BoldOblique = new PdfName("Helvetica-BoldOblique", true);
        /// <summary>
        /// PdfName Helvetica−Oblique
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Helvetica_Oblique = new PdfName("Helvetica-Oblique", true);
        /// <summary>
        /// PdfName HideMenubar
        /// </summary>
        public static readonly PdfName HideMenubar = new PdfName("HideMenubar", true);
        /// <summary>
        /// PdfName HideToolbar
        /// </summary>
        public static readonly PdfName HideToolbar = new PdfName("HideToolbar", true);
        /// <summary>
        /// PdfName HideWindowUI
        /// </summary>
        public static readonly PdfName HideWindowUI = new PdfName("HideWindowUI", true);
        /// <summary>
        /// PdfName ID
        /// </summary>
        public static readonly PdfName ID = new PdfName("ID", true);
        /// <summary>
        /// PdfName Image
        /// </summary>
        public static readonly PdfName Image = new PdfName("Image", true);
        /// <summary>
        /// PdfName ImageB
        /// </summary>
        public static readonly PdfName ImageB = new PdfName("ImageB", true);
        /// <summary>
        /// PdfName ImageC
        /// </summary>
        public static readonly PdfName ImageC = new PdfName("ImageC", true);
        /// <summary>
        /// PdfName ImageI
        /// </summary>
        public static readonly PdfName ImageI = new PdfName("ImageI", true);
        /// <summary>
        /// PdfName ImageMask
        /// </summary>
        public static readonly PdfName ImageMask = new PdfName("ImageMask", true);
        /// <summary>
        /// PdfName Indexed
        /// </summary>
        public static readonly PdfName Indexed = new PdfName("Indexed", true);
        /// <summary>
        /// PdfName Info
        /// </summary>
        public static readonly PdfName Info = new PdfName("Info", true);
        /// <summary>
        /// PdfName Intent
        /// </summary>
        public static readonly PdfName Intent = new PdfName("Intent", true);
        /// <summary>
        /// PdfName IsMap
        /// </summary>
        public static readonly PdfName IsMap = new PdfName("IsMap", true);
        /// <summary>
        /// PdfName ItalicAngle
        /// </summary>
        public static readonly PdfName ItalicAngle = new PdfName("ItalicAngle", true);
        /// <summary>
        /// PdfName JPXDecode
        /// </summary>
        public static readonly PdfName JPXDecode = new PdfName("JPXDecode", true);
        /// <summary>
        /// PdfName K
        /// </summary>
        public static readonly PdfName K = new PdfName("K", true);
        /// <summary>
        /// PdfName Keywords
        /// </summary>
        public static readonly PdfName Keywords = new PdfName("Keywords", true);
        /// <summary>
        /// PdfName Kids
        /// </summary>
        public static readonly PdfName Kids = new PdfName("Kids", true);
        /// <summary>
        /// PdfName Lang
        /// </summary>
        public static readonly PdfName Lang = new PdfName("Lang", true);
        /// <summary>
        /// PdfName Last
        /// </summary>
        public static readonly PdfName Last = new PdfName("Last", true);
        /// <summary>
        /// PdfName LastChar
        /// </summary>
        public static readonly PdfName LastChar = new PdfName("LastChar", true);
        /// <summary>
        /// PdfName Legal
        /// </summary>
        public static readonly PdfName Legal = new PdfName("Legal", true);
        /// <summary>
        /// PdfName Length
        /// </summary>
        public static readonly PdfName Length = new PdfName("Length", true);
        /// <summary>
        /// PdfName Length1
        /// </summary>
        public static readonly PdfName Length1 = new PdfName("Length1", true);
        /// <summary>
        /// PdfName Link
        /// </summary>
        public static readonly PdfName Link = new PdfName("Link", true);
        /// <summary>
        /// PdfName LZWDecode
        /// </summary>
        public static readonly PdfName LZWDecode = new PdfName("LZWDecode", true);
        /// <summary>
        /// PdfName M
        /// </summary>
        public static readonly PdfName M = new PdfName("M", true);
        /// <summary>
        /// PdfName MarkInfo
        /// </summary>
        public static readonly PdfName MarkInfo = new PdfName("MarkInfo", true);
        /// <summary>
        /// PdfName Mask
        /// </summary>
        public static readonly PdfName Mask = new PdfName("Mask", true);
        /// <summary>
        /// PdfName Matrix
        /// </summary>
        public static readonly PdfName Matrix = new PdfName("Matrix", true);
        /// <summary>
        /// PdfName MediaBox
        /// </summary>
        public static readonly PdfName MediaBox = new PdfName("MediaBox", true);
        /// <summary>
        /// PdfName Metadata
        /// </summary>
        public static readonly PdfName Metadata = new PdfName("Metadata", true);
        /// <summary>
        /// PdfName MMType1
        /// </summary>
        public static readonly PdfName MMType1 = new PdfName("MMType1", true);
        /// <summary>
        /// PdfName ModDate
        /// </summary>
        public static readonly PdfName ModDate = new PdfName("ModDate", true);
        /// <summary>
        /// PdfName N
        /// </summary>
        public static readonly PdfName N = new PdfName("N", true);
        private string name;
        /// <summary>
        /// PdfName Name
        /// </summary>
        public static readonly PdfName Name_ = new PdfName("Name", true);
        /// <summary>
        /// PdfName Names
        /// </summary>
        public static readonly PdfName Names = new PdfName("Names", true);
        /// <summary>
        /// PdfName NeedsRendering
        /// </summary>
        public static readonly PdfName NeedsRendering = new PdfName("NeedsRendering", true);
        /// <summary>
        /// PdfName Next
        /// </summary>
        public static readonly PdfName Next = new PdfName("Next", true);
        /// <summary>
        /// PdfName None
        /// </summary>
        public static readonly PdfName None = new PdfName("None", true);
        /// <summary>
        /// PdfName FullScreenPageMode
        /// </summary>
        public static readonly PdfName NonFullScreenPageMode = new PdfName("FullScreenPageMode", true);
        /// <summary>
        /// PdfName NumCopies
        /// </summary>
        public static readonly PdfName NumCopies = new PdfName("NumCopies", true);
        /// <summary>
        /// PdfName OCProperties
        /// </summary>
        public static readonly PdfName OCProperties = new PdfName("OCProperties", true);
        /// <summary>
        /// PdfName OneColumn
        /// </summary>
        public static readonly PdfName OneColumn = new PdfName("OneColumn", true);
        /// <summary>
        /// PdfName Open
        /// </summary>
        public static readonly PdfName Open = new PdfName("Open", true);
        /// <summary>
        /// PdfName OpenAction
        /// </summary>
        public static readonly PdfName OpenAction = new PdfName("OpenAction", true);
        /// <summary>
        /// PdfName OpenType
        /// </summary>
        public static readonly PdfName OpenType = new PdfName("OpenType", true);
        /// <summary>
        /// PdfName Order
        /// </summary>
        public static readonly PdfName Order = new PdfName("Order", true);
        /// <summary>
        /// PdfName Ordering
        /// </summary>
        public static readonly PdfName Ordering = new PdfName("Ordering", true);
        /// <summary>
        /// PdfName Outlines
        /// </summary>
        public static readonly PdfName Outlines = new PdfName("Outlines", true);
        /// <summary>
        /// PdfName OutputIntents
        /// </summary>
        public static readonly PdfName OutputIntents = new PdfName("OutputIntents", true);
        /// <summary>
        /// PdfName Page
        /// </summary>
        public static readonly PdfName Page = new PdfName("Page", true);
        /// <summary>
        /// PdfName PageLabels
        /// </summary>
        public static readonly PdfName PageLabels = new PdfName("PageLabels", true);
        /// <summary>
        /// PdfName PageLayout
        /// </summary>
        public static readonly PdfName PageLayout = new PdfName("PageLayout", true);
        /// <summary>
        /// PdfName PageMode
        /// </summary>
        public static readonly PdfName PageMode = new PdfName("PageMode", true);
        /// <summary>
        /// PdfName Pages
        /// </summary>
        public static readonly PdfName Pages = new PdfName("Pages", true);
        /// <summary>
        /// PdfName PaintType
        /// </summary>
        public static readonly PdfName PaintType = new PdfName("PaintType", true);
        /// <summary>
        /// PdfName Parent
        /// </summary>
        public static readonly PdfName Parent = new PdfName("Parent", true);
        /// <summary>
        /// PdfName Pattern
        /// </summary>
        public static readonly PdfName Pattern = new PdfName("Pattern", true);
        /// <summary>
        /// PdfName PatternType
        /// </summary>
        public static readonly PdfName PatternType = new PdfName("PatternType", true);
        /// <summary>
        /// PdfName PDF
        /// </summary>
        public static readonly PdfName PDF = new PdfName("PDF", true);
        /// <summary>
        /// PdfName Perceptual
        /// </summary>
        public static readonly PdfName Perceptual = new PdfName("Perceptual", true);
        /// <summary>
        /// PdfName Perms
        /// </summary>
        public static readonly PdfName Perms = new PdfName("Perms", true);
        /// <summary>
        /// PdfName PickTrayByPDFSize
        /// </summary>
        public static readonly PdfName PickTrayByPDFSize = new PdfName("PickTrayByPDFSize", true);
        /// <summary>
        /// PdfName PieceInfo
        /// </summary>
        public static readonly PdfName PieceInfo = new PdfName("PieceInfo", true);
        /// <summary>
        /// PdfName Predictor
        /// </summary>
        public static readonly PdfName Predictor = new PdfName("Predictor", true);
        private const byte prefix = 0x2f;
        /// <summary>
        /// PdfName Prev
        /// </summary>
        public static readonly PdfName Prev = new PdfName("Prev", true);
        /// <summary>
        /// PdfName PrintPageRange
        /// </summary>
        public static readonly PdfName PrintPageRange = new PdfName("PrintPageRange", true);
        /// <summary>
        /// PdfName PrintScaling
        /// </summary>
        public static readonly PdfName PrintScaling = new PdfName("PrintScaling", true);
        /// <summary>
        /// PdfName ProcSet
        /// </summary>
        public static readonly PdfName ProcSet = new PdfName("ProcSet", true);
        /// <summary>
        /// PdfName Producer
        /// </summary>
        public static readonly PdfName Producer = new PdfName("Producer", true);
        /// <summary>
        /// PdfName Properties
        /// </summary>
        public static readonly PdfName Properties = new PdfName("Properties", true);
        /// <summary>
        /// PdfName Push
        /// </summary>
        public static readonly PdfName Push = new PdfName("Push", true);
        /// <summary>
        /// PdfName R
        /// </summary>
        public static readonly PdfName R = new PdfName("R", true);
        /// <summary>
        /// PdfName R2L
        /// </summary>
        public static readonly PdfName R2L = new PdfName("R2L", true);
        /// <summary>
        /// PdfName Range
        /// </summary>
        public static readonly PdfName Range = new PdfName("Range", true);
        /// <summary>
        /// PdfName Rect
        /// </summary>
        public static readonly PdfName Rect = new PdfName("Rect", true);
        /// <summary>
        /// PdfName Registry
        /// </summary>
        public static readonly PdfName Registry = new PdfName("Registry", true);
        /// <summary>
        /// PdfName RelativeColorimetric
        /// </summary>
        public static readonly PdfName RelativeColorimetric = new PdfName("RelativeColorimetric", true);
        /// <summary>
        /// PdfName Requirements
        /// </summary>
        public static readonly PdfName Requirements = new PdfName("Requirements", true);
        /// <summary>
        /// PdfName Resources
        /// </summary>
        public static readonly PdfName Resources = new PdfName("Resources", true);
        /// <summary>
        /// PdfName Root
        /// </summary>
        public static readonly PdfName Root = new PdfName("Root", true);
        /// <summary>
        /// PdfName Rows
        /// </summary>
        public static readonly PdfName Rows = new PdfName("Rows", true);
        /// <summary>
        /// PdfName S
        /// </summary>
        public static readonly PdfName S = new PdfName("S", true);
        private readonly bool safeName;
        /// <summary>
        /// PdfName Saturation
        /// </summary>
        public static readonly PdfName Saturation = new PdfName("Saturation", true);
        /// <summary>
        /// PdfName Shading
        /// </summary>
        public static readonly PdfName Shading = new PdfName("Shading", true);
        /// <summary>
        /// PdfName ShadingType
        /// </summary>
        public static readonly PdfName ShadingType = new PdfName("ShadingType", true);
        /// <summary>
        /// PdfName Simplex
        /// </summary>
        public static readonly PdfName Simplex = new PdfName("Simplex", true);
        /// <summary>
        /// PdfName SinglePage
        /// </summary>
        public static readonly PdfName SinglePage = new PdfName("SinglePage", true);
        /// <summary>
        /// PdfName Size
        /// </summary>
        public static readonly PdfName Size = new PdfName("Size", true);
        /// <summary>
        /// PdfName SMask
        /// </summary>
        public static readonly PdfName SMask = new PdfName("SMask", true);
        /// <summary>
        /// PdfName SpiderInfo
        /// </summary>
        public static readonly PdfName SpiderInfo = new PdfName("SpiderInfo", true);
        /// <summary>
        /// PdfName Split
        /// </summary>
        public static readonly PdfName Split = new PdfName("Split", true);
        /// <summary>
        /// PdfName StemV
        /// </summary>
        public static readonly PdfName StemV = new PdfName("StemV", true);
        /// <summary>
        /// PdfName StructTreeRoot
        /// </summary>
        public static readonly PdfName StructTreeRoot = new PdfName("StructTreeRoot", true);
        /// <summary>
        /// PdfName Subject
        /// </summary>
        public static readonly PdfName Subject = new PdfName("Subject", true);
        /// <summary>
        /// PdfName Subtype
        /// </summary>
        public static readonly PdfName Subtype = new PdfName("Subtype", true);
        /// <summary>
        /// PdfName Supplement
        /// </summary>
        public static readonly PdfName Supplement = new PdfName("Supplement", true);
        /// <summary>
        /// PdfName Symbol
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Symbol = new PdfName("Symbol", true);
        /// <summary>
        /// PdfName Text
        /// </summary>
        public static readonly PdfName Text = new PdfName("Text", true);
        /// <summary>
        /// PdfName Threads
        /// </summary>
        public static readonly PdfName Threads = new PdfName("Threads", true);
        /// <summary>
        /// PdfName TilingType
        /// </summary>
        public static readonly PdfName TilingType = new PdfName("TilingType", true);
        /// <summary>
        /// PdfName Times−Bold
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Times_Bold = new PdfName("Times-Bold", true);
        /// <summary>
        /// PdfName Times−BoldItalic
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Times_BoldItalic = new PdfName("Times-BoldItalic", true);
        /// <summary>
        /// PdfName Times−Italic
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Times_Italic = new PdfName("Times-Italic", true);
        /// <summary>
        /// PdfName Times−Roman
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName Times_Roman = new PdfName("Times-Roman", true);
        /// <summary>
        /// PdfName Title
        /// </summary>
        public static readonly PdfName Title = new PdfName("Title", true);
        /// <summary>
        /// PdfName ToUnicode
        /// </summary>
        public static readonly PdfName ToUnicode = new PdfName("ToUnicode", true);
        /// <summary>
        /// PdfName Trans
        /// </summary>
        public static readonly PdfName Trans = new PdfName("Trans", true);
        /// <summary>
        /// PdfName TrueType
        /// </summary>
        public static readonly PdfName TrueType = new PdfName("TrueType", true);
        /// <summary>
        /// PdfName TwoColumnLeft
        /// </summary>
        public static readonly PdfName TwoColumnLeft = new PdfName("TwoColumnLeft", true);
        /// <summary>
        /// PdfName TwoColumnRight
        /// </summary>
        public static readonly PdfName TwoColumnRight = new PdfName("TwoColumnRight", true);
        /// <summary>
        /// PdfName TwoPageLeft
        /// </summary>
        public static readonly PdfName TwoPageLeft = new PdfName("TwoPageLeft", true);
        /// <summary>
        /// PdfName TwoPageRight
        /// </summary>
        public static readonly PdfName TwoPageRight = new PdfName("TwoPageRight", true);
        /// <summary>
        /// PdfName Type
        /// </summary>
        public static readonly PdfName Type = new PdfName("Type", true);
        /// <summary>
        /// PdfName Type0
        /// </summary>
        public static readonly PdfName Type0 = new PdfName("Type0", true);
        /// <summary>
        /// PdfName Type1
        /// </summary>
        public static readonly PdfName Type1 = new PdfName("Type1", true);
        /// <summary>
        /// PdfName Type1C
        /// </summary>
        public static readonly PdfName Type1C = new PdfName("Type1C", true);
        /// <summary>
        /// PdfName Type3
        /// </summary>
        public static readonly PdfName Type3 = new PdfName("Type3", true);
        /// <summary>
        /// PdfName UF
        /// </summary>
        public static readonly PdfName UF = new PdfName("UF", true);
        /// <summary>
        /// PdfName Uncover
        /// </summary>
        public static readonly PdfName Uncover = new PdfName("Uncover", true);
        /// <summary>
        /// PdfName URI
        /// </summary>
        public static readonly PdfName URI = new PdfName("URI", true);
        /// <summary>
        /// PdfName UseAttachments
        /// </summary>
        public static readonly PdfName UseAttachments = new PdfName("UseAttachments", true);
        /// <summary>
        /// PdfName UseNone
        /// </summary>
        public static readonly PdfName UseNone = new PdfName("UseNone", true);
        /// <summary>
        /// PdfName UseOC
        /// </summary>
        public static readonly PdfName UseOC = new PdfName("UseOC", true);
        /// <summary>
        /// PdfName UseOutlines
        /// </summary>
        public static readonly PdfName UseOutlines = new PdfName("UseOutlines", true);
        /// <summary>
        /// PdfName UseThumbs
        /// </summary>
        public static readonly PdfName UseThumbs = new PdfName("UseThumbs", true);
        /// <summary>
        /// PdfName V
        /// </summary>
        public static readonly PdfName V = new PdfName("V", true);
        /// <summary>
        /// PdfName Version
        /// </summary>
        public static readonly PdfName Version = new PdfName("Name", true);
        /// <summary>
        /// PdfName ViewerPreferences
        /// </summary>
        public static readonly PdfName ViewerPreferences = new PdfName("ViewerPreferences", true);
        /// <summary>
        /// PdfName W
        /// </summary>
        public static readonly PdfName W = new PdfName("W", true);
        /// <summary>
        /// PdfName Watermark
        /// </summary>
        public static readonly PdfName Watermark = new PdfName("Watermark", true);
        /// <summary>
        /// PdfName WhitePoint
        /// </summary>
        public static readonly PdfName WhitePoint = new PdfName("WhitePoint", true);
        /// <summary>
        /// PdfName Width
        /// </summary>
        public static readonly PdfName Width = new PdfName("Width", true);
        /// <summary>
        /// PdfName Widths
        /// </summary>
        public static readonly PdfName Widths = new PdfName("Widths", true);
        /// <summary>
        /// PdfName Wipe
        /// </summary>
        public static readonly PdfName Wipe = new PdfName("Wipe", true);
        /// <summary>
        /// PdfName XObject
        /// </summary>
        public static readonly PdfName XObject = new PdfName("XObject", true);
        /// <summary>
        /// PdfName XStep
        /// </summary>
        public static readonly PdfName XStep = new PdfName("XStep", true);
        /// <summary>
        /// PdfName XYZ
        /// </summary>
        public static readonly PdfName XYZ = new PdfName("XYZ", true);
        /// <summary>
        /// PdfName YStep
        /// </summary>
        public static readonly PdfName YStep = new PdfName("YStep", true);
        /// <summary>
        /// PdfName ZapfDingbats
        /// A name of a standard 14 type 1 font
        /// </summary>
        public static readonly PdfName ZapfDingbats = new PdfName("ZapfDingbats", true);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfName" /> class.
        /// </summary>
        public PdfName()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfName" /> class.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public PdfName(PdfName obj)
        {
            if (obj == null)
            {
                throw new PdfArgumentNullException("obj");
            }
            this.name = obj.name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfName" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public PdfName(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfName" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="safeName">if set to <c>true</c> [safe name].</param>
        internal PdfName(string name, bool safeName) : this(name)
        {
            this.safeName = safeName;
        }

        /// <summary>
        /// Equalses the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public bool Equals(PdfName obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            return (object.ReferenceEquals(this, obj) || (object.Equals(obj.name, this.name) && obj.safeName.Equals(this.safeName)));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj" /> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof(PdfName))
            {
                return false;
            }
            return this.Equals((PdfName) obj);
        }

        /// <summary>
        /// Fixes the name. Escape special chars.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        private static string FixName(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new PdfArgumentNullException(str);
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in str)
            {
                switch (ch)
                {
                    case '#':
                    case '%':
                    case '(':
                    case ')':
                    case '/':
                    case ' ':
                    case '<':
                    case '>':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                    {
                        builder.Append('#');
                        int num2 = ch;
                        builder.Append(((int) num2).ToString("X2"));
                        continue;
                    }
                }
                if ((ch < ' ') || (ch > '~'))
                {
                    builder.Append('#');
                    int num3 = ch;
                    builder.Append(((int) num3).ToString("X2"));
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return ((((this.name != null) ? this.name.GetHashCode() : 0) * 0x18d) ^ this.safeName.GetHashCode());
        }

        /// <summary>
        /// Read data from Pdf reader
        /// </summary>
        /// <param name="reader">Pdf Reader</param>
        public override void ToObject(IPdfReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            this.WriteTo(writer.Psw);
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="psw">The PSW.</param>
        internal void WriteTo(PdfStreamWriter psw)
        {
            string name = this.name;
            if (!this.safeName)
            {
                name = FixName(name);
            }
            psw.WriteByte(0x2f).WriteString(name);
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return  (((char) '/') + this.name); }
            set { this.name = value; }
        }
    }
}

