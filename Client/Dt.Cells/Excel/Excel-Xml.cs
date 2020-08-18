#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            Reset();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    ReadXmlInternal(reader);
                }
            }
        }

        void ReadXmlInternal(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "AllowUserFormula":
                    CanUserEditFormula = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AllowUndo":
                    CanUserUndo = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "FreezeLineStyle":
                    FreezeLineStyle = Serializer.DeserializeObj(typeof(Style), reader) as Style;
                    return;

                case "TrailingFreezeLineStyle":
                    TrailingFreezeLineStyle = Serializer.DeserializeObj(typeof(Style), reader) as Style;
                    return;

                case "ShowFreezeLine":
                    ShowFreezeLine = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AllowUserZoom":
                    CanUserZoom = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AutoClipboard":
                    AutoClipboard = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ClipBoardOptions":
                    ClipBoardOptions = (ClipboardPasteOptions)Serializer.DeserializeObj(typeof(ClipboardPasteOptions), reader);
                    return;

                case "AllowEditOverflow":

                    return;

                case "Protect":
                    _protect = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AllowDragDrop":
                    CanUserDragDrop = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ShowRowRangeGroup":
                    ShowRowRangeGroup = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ShowColumnRangeGroup":
                    ShowColumnRangeGroup = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AllowDragFill":
                    CanUserDragFill = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "CanTouchMultiSelect":
                    CanTouchMultiSelect = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ResizeZeroIndicator":
                    ResizeZeroIndicator = (ResizeZeroIndicator)Serializer.DeserializeObj(typeof(ResizeZeroIndicator), reader);
                    return;

                case "RangeGroupBackground":
                    RangeGroupBackground = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "RangeGroupBorderBrush":
                    RangeGroupBorderBrush = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "RangeGroupLineStroke":
                    RangeGroupLineStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "ScrollBarTrackPolicy":
                    ScrollBarTrackPolicy = (ScrollBarTrackPolicy)Serializer.DeserializeObj(typeof(ScrollBarTrackPolicy), reader);
                    return;

                case "ColumnSplitBoxAlignment":
                    ColumnSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "RowSplitBoxAlignment":
                    RowSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "HorizontalScrollBarHeight":
                    HorizontalScrollBarHeight = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "VerticalScrollBarWidth":
                    VerticalScrollBarWidth = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripVisibility":
                    TabStripVisibility = (Visibility)Serializer.DeserializeObj(typeof(Visibility), reader);
                    return;

                case "TabStripEditable":
                    TabStripEditable = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "TabStripRadio":
                    TabStripRatio = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripInsertTab":
                    {
                        bool flag = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        TabStripInsertTab = flag;
                        if (_tabStrip == null)
                        {
                            break;
                        }
                        _tabStrip.HasInsertTab = flag;
                        return;
                    }
                case "ColumnSplitBoxPolicy":
                    ColumnSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    return;

                case "RowSplitBoxPolicy":
                    RowSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    return;

                default:
                    return;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.WriteStartObj("SpreadUI", writer);
            WriteXmlInternal(writer);
            Serializer.WriteEndObj(writer);
        }

        void WriteXmlInternal(XmlWriter writer)
        {
            if (!CanUserEditFormula)
            {
                Serializer.SerializeObj(CanUserEditFormula, "AllowUserFormula", writer);
            }
            if (!CanUserUndo)
            {
                Serializer.SerializeObj(CanUserUndo, "AllowUndo", writer);
            }
            if (FreezeLineStyle != null)
            {
                Serializer.SerializeObj(FreezeLineStyle, "FreezeLineStyle", writer);
            }
            if (TrailingFreezeLineStyle != null)
            {
                Serializer.SerializeObj(TrailingFreezeLineStyle, "TrailingFreezeLineStyle", writer);
            }
            if (!ShowFreezeLine)
            {
                Serializer.SerializeObj(ShowFreezeLine, "ShowFreezeLine", writer);
            }
            if (!CanUserZoom)
            {
                Serializer.SerializeObj(CanUserZoom, "AllowUserZoom", writer);
            }
            if (!AutoClipboard)
            {
                Serializer.SerializeObj(AutoClipboard, "AutoClipboard", writer);
            }
            if (ClipBoardOptions != ClipboardPasteOptions.All)
            {
                Serializer.SerializeObj(ClipBoardOptions, "ClipBoardOptions", writer);
            }
            if (_protect)
            {
                Serializer.SerializeObj((bool)_protect, "Protect", writer);
            }
            if (!CanUserDragDrop)
            {
                Serializer.SerializeObj(CanUserDragDrop, "AllowDragDrop", writer);
            }
            if (!ShowRowRangeGroup)
            {
                Serializer.SerializeObj(ShowRowRangeGroup, "ShowRowRangeGroup", writer);
            }
            if (!ShowColumnRangeGroup)
            {
                Serializer.SerializeObj(ShowColumnRangeGroup, "ShowColumnRangeGroup", writer);
            }
            if (!CanUserDragFill)
            {
                Serializer.SerializeObj(CanUserDragFill, "AllowDragFill", writer);
            }
            if (CanTouchMultiSelect)
            {
                Serializer.SerializeObj(CanTouchMultiSelect, "CanTouchMultiSelect", writer);
            }
            if (ResizeZeroIndicator != ResizeZeroIndicator.Default)
            {
                Serializer.SerializeObj(ResizeZeroIndicator, "ResizeZeroIndicator", writer);
            }
            if (DefaultAutoFillType.HasValue)
            {
                Serializer.SerializeObj(DefaultAutoFillType, "DefaultAutoFillType", writer);
            }
            if (RangeGroupBackground != null)
            {
                Serializer.SerializeObj(RangeGroupBackground, "RangeGroupBackground", writer);
            }
            if (RangeGroupBorderBrush != null)
            {
                Serializer.SerializeObj(RangeGroupBorderBrush, "RangeGroupBorderBrush", writer);
            }
            if (RangeGroupLineStroke != null)
            {
                Serializer.SerializeObj(RangeGroupLineStroke, "RangeGroupLineStroke", writer);
            }

            if (HorizontalScrollBarHeight != 25.0)
            {
                Serializer.SerializeObj((double)HorizontalScrollBarHeight, "HorizontalScrollBarHeight", writer);
            }
            if (VerticalScrollBarWidth != 25.0)
            {
                Serializer.SerializeObj(VerticalScrollBarWidth, "VerticalScrollBarWidth", writer);
            }
            if (ScrollBarTrackPolicy != ScrollBarTrackPolicy.Both)
            {
                Serializer.SerializeObj(ScrollBarTrackPolicy, "ScrollBarTrackPolicy", writer);
            }
            if (ColumnSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(ColumnSplitBoxAlignment, "ColumnSplitBoxAlignment", writer);
            }
            if (RowSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(RowSplitBoxAlignment, "RowSplitBoxAlignment", writer);
            }
            if (TabStripVisibility != 0)
            {
                Serializer.SerializeObj(TabStripVisibility, "TabStripVisibility", writer);
            }
            if (!TabStripEditable)
            {
                Serializer.SerializeObj(TabStripEditable, "TabStripEditable", writer);
            }
            if (TabStripRatio != 0.5)
            {
                Serializer.SerializeObj(TabStripRatio, "TabStripRadio", writer);
            }
            if (!TabStripInsertTab)
            {
                Serializer.SerializeObj(TabStripInsertTab, "TabStripInsertTab", writer);
            }
            if (ColumnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(ColumnSplitBoxPolicy, "ColumnSplitBoxPolicy", writer);
            }
            if (RowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(RowSplitBoxPolicy, "RowSplitBoxPolicy", writer);
            }
        }
    }
}

