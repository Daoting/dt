#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Cells.Data;
using Dt.Cells.UndoRedo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView
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
                    _trailingFreezeLineStyle = Serializer.DeserializeObj(typeof(Style), reader) as Style;
                    return;

                case "ShowFreezeLine":
                    _showFreezeLine = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
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
                    _showRowRangeGroup = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ShowColumnRangeGroup":
                    _showColumnRangeGroup = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AllowDragFill":
                    CanUserDragFill = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "CanTouchMultiSelect":
                    CanTouchMultiSelect = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ResizeZeroIndicator":
                    _resizeZeroIndicator = (ResizeZeroIndicator)Serializer.DeserializeObj(typeof(ResizeZeroIndicator), reader);
                    return;

                case "RangeGroupBackground":
                    _rangeGroupBackground = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "RangeGroupBorderBrush":
                    _rangeGroupBorderBrush = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "RangeGroupLineStroke":
                    _rangeGroupLineStroke = (Brush)Serializer.DeserializeObj(typeof(Brush), reader);
                    return;

                case "ScrollBarTrackPolicy":
                    _scrollBarTrackPolicy = (ScrollBarTrackPolicy)Serializer.DeserializeObj(typeof(ScrollBarTrackPolicy), reader);
                    return;

                case "ColumnSplitBoxAlignment":
                    _columnSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "RowSplitBoxAlignment":
                    _rowSplitBoxAlignment = (SplitBoxAlignment)Serializer.DeserializeObj(typeof(SplitBoxAlignment), reader);
                    return;

                case "HorizontalScrollBarHeight":
                    _horizontalScrollBarHeight = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "VerticalScrollBarWidth":
                    _verticalScrollBarWidth = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripVisibility":
                    _tabStripVisibility = (Visibility)Serializer.DeserializeObj(typeof(Visibility), reader);
                    return;

                case "TabStripEditable":
                    _tabStripEditable = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "TabStripRadio":
                    _tabStripRatio = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "TabStripInsertTab":
                    {
                        bool flag = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        _tabStripInsertTab = flag;
                        if (_tabStrip == null)
                        {
                            break;
                        }
                        _tabStrip.HasInsertTab = flag;
                        return;
                    }
                case "ColumnSplitBoxPolicy":
                    _columnSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
                    return;

                case "RowSplitBoxPolicy":
                    _rowSplitBoxPolicy = (SplitBoxPolicy)Serializer.DeserializeObj(typeof(SplitBoxPolicy), reader);
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
            if (_trailingFreezeLineStyle != null)
            {
                Serializer.SerializeObj(_trailingFreezeLineStyle, "TrailingFreezeLineStyle", writer);
            }
            if (!_showFreezeLine)
            {
                Serializer.SerializeObj((bool)_showFreezeLine, "ShowFreezeLine", writer);
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
            if (!_showRowRangeGroup)
            {
                Serializer.SerializeObj((bool)_showRowRangeGroup, "ShowRowRangeGroup", writer);
            }
            if (!_showColumnRangeGroup)
            {
                Serializer.SerializeObj((bool)_showColumnRangeGroup, "ShowColumnRangeGroup", writer);
            }
            if (!CanUserDragFill)
            {
                Serializer.SerializeObj(CanUserDragFill, "AllowDragFill", writer);
            }
            if (CanTouchMultiSelect)
            {
                Serializer.SerializeObj(CanTouchMultiSelect, "CanTouchMultiSelect", writer);
            }
            if (_resizeZeroIndicator != ResizeZeroIndicator.Default)
            {
                Serializer.SerializeObj(_resizeZeroIndicator, "ResizeZeroIndicator", writer);
            }
            if (DefaultAutoFillType.HasValue)
            {
                Serializer.SerializeObj(DefaultAutoFillType, "DefaultAutoFillType", writer);
            }
            if (_rangeGroupBackground != null)
            {
                Serializer.SerializeObj(_rangeGroupBackground, "RangeGroupBackground", writer);
            }
            if (_rangeGroupBorderBrush != null)
            {
                Serializer.SerializeObj(_rangeGroupBorderBrush, "RangeGroupBorderBrush", writer);
            }
            if (_rangeGroupLineStroke != null)
            {
                Serializer.SerializeObj(_rangeGroupLineStroke, "RangeGroupLineStroke", writer);
            }

            if (_horizontalScrollBarHeight != 25.0)
            {
                Serializer.SerializeObj((double)_horizontalScrollBarHeight, "HorizontalScrollBarHeight", writer);
            }
            if (_verticalScrollBarWidth != 25.0)
            {
                Serializer.SerializeObj((double)_verticalScrollBarWidth, "VerticalScrollBarWidth", writer);
            }
            if (_scrollBarTrackPolicy != ScrollBarTrackPolicy.Both)
            {
                Serializer.SerializeObj(_scrollBarTrackPolicy, "ScrollBarTrackPolicy", writer);
            }
            if (_columnSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(_columnSplitBoxAlignment, "ColumnSplitBoxAlignment", writer);
            }
            if (_rowSplitBoxAlignment != SplitBoxAlignment.Leading)
            {
                Serializer.SerializeObj(_rowSplitBoxAlignment, "RowSplitBoxAlignment", writer);
            }
            if (_tabStripVisibility != 0)
            {
                Serializer.SerializeObj(_tabStripVisibility, "TabStripVisibility", writer);
            }
            if (!_tabStripEditable)
            {
                Serializer.SerializeObj((bool)_tabStripEditable, "TabStripEditable", writer);
            }
            if (_tabStripRatio != 0.5)
            {
                Serializer.SerializeObj((double)_tabStripRatio, "TabStripRadio", writer);
            }
            if (!_tabStripInsertTab)
            {
                Serializer.SerializeObj((bool)_tabStripInsertTab, "TabStripInsertTab", writer);
            }
            if (_columnSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(_columnSplitBoxPolicy, "ColumnSplitBoxPolicy", writer);
            }
            if (_rowSplitBoxPolicy != SplitBoxPolicy.Always)
            {
                Serializer.SerializeObj(_rowSplitBoxPolicy, "RowSplitBoxPolicy", writer);
            }
        }
    }
}

