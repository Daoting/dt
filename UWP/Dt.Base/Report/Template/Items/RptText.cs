#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 文本项
    /// </summary>
    internal class RptText : RptItem
    {
        #region 成员变量
        public const string DefaultFontName = "Segoe UI";
        public const double DefaultFontSize = 16.0;
        public const string ScriptValue = "#script#";
        const string _defaultForeground = "#ff000000";
        const string _defaultBackground = "#00ffffff";
        const string _defaultAlign = "Center";
        const string _defaultBorder = "Thin";
        RptItemBase _parent;
        #endregion

        public RptText(RptPart p_owner)
            : base(p_owner)
        {
            _data.AddCell<string>("val");
            _data.AddCell<bool>("hidetopdup");
            _data.AddCell<bool>("hideleftdup");
            _data.AddCell<bool>("autoheight");
            _data.AddCell<bool>("handleclick");
            _data.AddCell<bool>("wordwrap");
            _data.AddCell("fontfamily", DefaultFontName);
            _data.AddCell("fontsize", DefaultFontSize);
            _data.AddCell<bool>("bold");
            _data.AddCell<bool>("italic");
            _data.AddCell<bool>("underline");
            _data.AddCell<bool>("strikeout");
            _data.AddCell("foreground", _defaultForeground);
            _data.AddCell("background", _defaultBackground);
            _data.AddCell("horalign", _defaultAlign);
            _data.AddCell("veralign", _defaultAlign);
            _data.AddCell("margin", 0);
            _data.AddCell("lbc", _defaultForeground);
            _data.AddCell("tbc", _defaultForeground);
            _data.AddCell("rbc", _defaultForeground);
            _data.AddCell("bbc", _defaultForeground);
            _data.AddCell("lbs", _defaultBorder);
            _data.AddCell("tbs", _defaultBorder);
            _data.AddCell("rbs", _defaultBorder);
            _data.AddCell("bbs", _defaultBorder);
        }

        public RptText(RptItemBase p_parent)
            : this(p_parent.Part)
        {
            _parent = p_parent;
        }

        /// <summary>
        /// 获取父对象
        /// </summary>
        public override RptItemBase Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// 获取或设置文本的内容值
        /// </summary>
        public string Val
        {
            get { return _data.Str("val"); }
            set { _data["val"] = value; }
        }

        /// <summary>
        /// 获取或设置是否合并纵向内容
        /// </summary>
        public bool HideTopDup
        {
            get { return _data.Bool("hidetopdup"); }
            set { _data["hidetopdup"] = value; }
        }

        /// <summary>
        /// 获取或设置是否合并横向内容
        /// </summary>
        public bool HideLeftDup
        {
            get { return _data.Bool("hideleftdup"); }
            set { _data["hideleftdup"] = value; }
        }

        /// <summary>
        /// 获取或设置是否换行
        /// </summary>
        public bool WordWrap
        {
            get { return _data.Bool("wordwrap"); }
            set { _data["wordwrap"] = value; }
        }

        /// <summary>
        /// 获取或设置是否自动改变行高
        /// </summary>
        public bool AutoHeight
        {
            get { return _data.Bool("autoheight"); }
            set { _data["autoheight"] = value; }
        }

        /// <summary>
        /// 获取设置点击时是否执行脚本
        /// </summary>
        public bool HandleClick
        {
            get { return _data.Bool("handleclick"); }
            set { _data["handleclick"] = value; }
        }

        /// <summary>
        /// 获取表达式列表
        /// </summary>
        public List<RptExpression> Expressions { get; set; }

        /// <summary>
        /// 获取或设置字体
        /// </summary>
        public string FontFamily
        {
            get { return _data.Str("fontfamily"); }
            set { _data["fontfamily"] = value; }
        }

        /// <summary>
        /// 字体大小属性
        /// </summary>
        public double FontSize
        {
            get { return _data.Double("fontsize"); }
            set { _data["fontsize"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用加粗样式
        /// </summary>
        public bool Bold
        {
            get { return _data.Bool("bold"); }
            set { _data["bold"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用斜体样式
        /// </summary>
        public bool Italic
        {
            get { return _data.Bool("italic"); }
            set { _data["italic"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用下划线样式
        /// </summary>
        public bool UnderLine
        {
            get { return _data.Bool("underline"); }
            set { _data["underline"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用删除线样式
        /// </summary>
        public bool StrikeOut
        {
            get { return _data.Bool("strikeout"); }
            set { _data["strikeout"] = value; }
        }

        /// <summary>
        /// 获取设置前景颜色
        /// </summary>
        public Color Foreground
        {
            get { return HexStringToColor(_data.Str("foreground")); }
            set { _data["foreground"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置背景颜色
        /// </summary>
        public Color Background
        {
            get { return HexStringToColor(_data.Str("background")); }
            set { _data["background"] = value.ToString(); }
        }

        /// <summary>
        /// 获取或设置水平对齐方式
        /// </summary>
        public CellHorizontalAlignment Horalign
        {
            get { return (CellHorizontalAlignment)Enum.Parse(typeof(CellHorizontalAlignment), _data.Str("horalign")); }
            set { _data["horalign"] = value.ToString(); }
        }

        /// <summary>
        /// 获取或设置垂直对齐方式
        /// </summary>
        public CellVerticalAlignment Veralign
        {
            get { return (CellVerticalAlignment)Enum.Parse(typeof(CellVerticalAlignment), _data.Str("veralign")); }
            set { _data["veralign"] = value.ToString(); }
        }

        /// <summary>
        /// 缩进 由逗号分隔的数字
        /// </summary>
        public int Margin
        {
            get { return _data.Int("margin"); }
            set { _data["margin"] = value; }
        }

        /// <summary>
        /// 获取设置左边框颜色
        /// </summary>
        public Color LeftColor
        {
            get { return HexStringToColor(_data.Str("lbc")); }
            set { _data["lbc"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置上边框颜色
        /// </summary>
        public Color TopColor
        {
            get { return HexStringToColor(_data.Str("tbc")); }
            set { _data["tbc"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置右边框颜色
        /// </summary>
        public Color RightColor
        {
            get { return HexStringToColor(_data.Str("rbc")); }
            set { _data["rbc"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置下边框颜色
        /// </summary>
        public Color BottomColor
        {
            get { return HexStringToColor(_data.Str("bbc")); }
            set { _data["bbc"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置左边框样式
        /// </summary>
        public BorderLineStyle LeftStyle
        {
            get { return (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), _data.Str("lbs"), true); }
            set { _data["lbs"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置上边框样式
        /// </summary>
        public BorderLineStyle TopStyle
        {
            get { return (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), _data.Str("tbs"), true); }
            set { _data["tbs"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置右边框样式
        /// </summary>
        public BorderLineStyle RightStyle
        {
            get { return (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), _data.Str("rbs"), true); }
            set { _data["rbs"] = value.ToString(); }
        }

        /// <summary>
        /// 获取设置下边框样式
        /// </summary>
        public BorderLineStyle BottomStyle
        {
            get { return (BorderLineStyle)Enum.Parse(typeof(BorderLineStyle), _data.Str("bbs"), true); }
            set { _data["bbs"] = value.ToString(); }
        }

        /// <summary>
        /// 获取是否包含总页数或页号占位符
        /// </summary>
        public bool ExistPlaceholder { get; set; }

        /// <summary>
        /// 是否通过脚本绘制单元格内容和样式
        /// </summary>
        public bool IsScriptRender { get; private set; }

        /// <summary>
        ///  克隆方法
        /// </summary>
        /// <returns></returns>
        public override RptItem Clone()
        {
            RptText newOne = new RptText(_part);
            newOne.Data.Copy(_data);
            return newOne;
        }

        /// <summary>
        /// 为克隆表格增加改写对象父对象的方法。
        /// </summary>
        /// <param name="p_parent"></param>
        public void SetParentItem(RptItemBase p_parent)
        {
            _parent = p_parent;
        }

        /// <summary>
        /// 构造报表项实例
        /// </summary>
        public override Task Build()
        {
            RptRootInst inst = _part.Inst;
            RptTextInst txt = new RptTextInst(this);
            if (_parent != null)
                inst.CurrentParent.AddChild(txt);
            else if (_part.PartType == RptPartType.Header)
                inst.Header.AddChild(txt);
            else if (_part.PartType == RptPartType.Footer)
                inst.Footer.AddChild(txt);
            else
                inst.Body.AddChild(txt);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 给 cell 实例对象设定RptText的样式
        /// </summary>
        /// <param name="p_cell"></param>
        public void ApplyStyle(Cells.Data.Cell p_cell)
        {
            p_cell.WordWrap = WordWrap;
            p_cell.FontFamily = new FontFamily(FontFamily);
            p_cell.FontSize = FontSize;
            p_cell.FontWeight = Bold ? FontWeights.Bold : FontWeights.Normal;
            p_cell.FontStyle = Italic ? FontStyle.Italic : FontStyle.Normal;
            p_cell.Underline = UnderLine;
            p_cell.Strikethrough = StrikeOut;
            p_cell.Foreground = HandleClick ? Dt.Base.Res.主蓝 : new SolidColorBrush(Foreground);
            p_cell.Background = Background.A == 0 ? null : new SolidColorBrush(Background);
            p_cell.HorizontalAlignment = Horalign;
            p_cell.VerticalAlignment = Veralign;
            p_cell.TextIndent = Margin;

            if (p_cell.RowSpan > 1 || p_cell.ColumnSpan > 1)
            {
                Worksheet sheet = p_cell.Worksheet;
                for (int i = p_cell.Column.Index; i < p_cell.Column.Index + p_cell.ColumnSpan; i++)
                {
                    sheet[p_cell.Row.Index, i].BorderTop = TopStyle == BorderLineStyle.None ? null : new BorderLine(TopColor, TopStyle);
                    sheet[p_cell.Row.Index + p_cell.RowSpan - 1, i].BorderBottom = BottomStyle == BorderLineStyle.None ? null : new BorderLine(BottomColor, BottomStyle);
                }
                for (int i = p_cell.Row.Index; i < p_cell.Row.Index + p_cell.RowSpan; i++)
                {
                    sheet[i, p_cell.Column.Index].BorderLeft = LeftStyle == BorderLineStyle.None ? null : new BorderLine(LeftColor, LeftStyle);
                    sheet[i, p_cell.Column.Index + p_cell.ColumnSpan - 1].BorderRight = RightStyle == BorderLineStyle.None ? null : new BorderLine(RightColor, RightStyle);
                }
            }
            else
            {
                p_cell.BorderLeft = LeftStyle == BorderLineStyle.None ? null : new BorderLine(LeftColor, LeftStyle);
                p_cell.BorderTop = TopStyle == BorderLineStyle.None ? null : new BorderLine(TopColor, TopStyle);
                p_cell.BorderRight = RightStyle == BorderLineStyle.None ? null : new BorderLine(RightColor, RightStyle);
                p_cell.BorderBottom = BottomStyle == BorderLineStyle.None ? null : new BorderLine(BottomColor, BottomStyle);
            }
        }

        #region xml
        /// <summary>
        /// 加载xml时解析表达式
        /// </summary>
        /// <param name="p_reader"></param>
        public override void ReadXml(XmlReader p_reader)
        {
            base.ReadXml(p_reader);

            string val = _data.Str("val").Trim();
            if (string.IsNullOrEmpty(val))
                return;

            if (!val.StartsWith(":"))
            {
                IsScriptRender = (val.ToLower() == ScriptValue);
                return;
            }

            // 解析表达式
            if (Expressions == null)
                Expressions = new List<RptExpression>();
            string[] subs = val.Substring(1).Split('+');
            foreach (string item in subs)
            {
                string str = item.Trim();
                if (str != "")
                    Expressions.Add(ParseExpression(str));
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Text");
            WritePosition(p_writer);

            if (!_data.IsEmpty("val"))
                p_writer.WriteAttributeString("val", _data.Str("val"));
            if (HideTopDup)
                p_writer.WriteAttributeString("hidetopdup", "True");
            if (HideLeftDup)
                p_writer.WriteAttributeString("hideleftdup", "True");
            if (AutoHeight)
                p_writer.WriteAttributeString("autoheight", "True");
            if (HandleClick)
                p_writer.WriteAttributeString("handleclick", "True");
            if (WordWrap)
                p_writer.WriteAttributeString("wordwrap", "True");
            if (FontFamily != DefaultFontName)
                p_writer.WriteAttributeString("fontfamily", FontFamily);
            if (FontSize != DefaultFontSize)
                p_writer.WriteAttributeString("fontsize", _data.Str("fontsize"));
            if (Bold)
                p_writer.WriteAttributeString("bold", "True");
            if (Italic)
                p_writer.WriteAttributeString("italic", "True");
            if (UnderLine)
                p_writer.WriteAttributeString("underline", "True");
            if (StrikeOut)
                p_writer.WriteAttributeString("strikeout", "True");
            string val = _data.Str("foreground");
            if (val != _defaultForeground)
                p_writer.WriteAttributeString("foreground", val);
            val = _data.Str("background");
            if (val != _defaultBackground)
                p_writer.WriteAttributeString("background", val);
            val = _data.Str("horalign");
            if (val != _defaultAlign)
                p_writer.WriteAttributeString("horalign", val);
            val = _data.Str("veralign");
            if (val != _defaultAlign)
                p_writer.WriteAttributeString("veralign", val);
            if (Margin != 0)
                p_writer.WriteAttributeString("margin", _data.Str("margin"));

            val = _data.Str("lbc");
            if (val != _defaultForeground)
                p_writer.WriteAttributeString("lbc", val);
            val = _data.Str("tbc");
            if (val != _defaultForeground)
                p_writer.WriteAttributeString("tbc", val);
            val = _data.Str("rbc");
            if (val != _defaultForeground)
                p_writer.WriteAttributeString("rbc", val);
            val = _data.Str("bbc");
            if (val != _defaultForeground)
                p_writer.WriteAttributeString("bbc", val);

            val = _data.Str("lbs");
            if (val != _defaultBorder)
                p_writer.WriteAttributeString("lbs", val);
            val = _data.Str("tbs");
            if (val != _defaultBorder)
                p_writer.WriteAttributeString("tbs", val);
            val = _data.Str("rbs");
            if (val != _defaultBorder)
                p_writer.WriteAttributeString("rbs", val);
            val = _data.Str("bbs");
            if (val != _defaultBorder)
                p_writer.WriteAttributeString("bbs", val);

            p_writer.WriteEndElement();
        }
        #endregion

        /// <summary>
        /// 解析表达式
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        RptExpression ParseExpression(string p_str)
        {
            RptExpression exp = new RptExpression();
            string str = p_str.ToLower();

            // 两个参数情况
            Match match = Regex.Match(str, @"^(\S*)\s*\(\s*(\w*)\s*,\s*(\w*)\s*\)$");
            if (match.Success && match.Groups.Count == 4)
            {
                switch (match.Groups[1].Value)
                {
                    case "val":
                        exp.Func = RptExpFunc.Val;
                        break;
                    case "sum":
                        exp.Func = RptExpFunc.Sum;
                        break;
                    case "avg":
                        exp.Func = RptExpFunc.Avg;
                        break;
                    case "max":
                        exp.Func = RptExpFunc.Max;
                        break;
                    case "min":
                        exp.Func = RptExpFunc.Min;
                        break;
                    default:
                        exp.Func = RptExpFunc.Unknown;
                        break;
                }

                if (exp.Func != RptExpFunc.Unknown)
                {
                    exp.DataName = match.Groups[2].Value;
                    exp.VarName = match.Groups[3].Value;
                }
                else
                {
                    exp.VarName = p_str;
                }
                return exp;
            }

            // 一个参数情况
            match = Regex.Match(str, @"(\S*)\s*\(\s*(\w*)\s*\)$");
            if (match.Success && match.Groups.Count == 3)
            {
                switch (match.Groups[1].Value)
                {
                    case "count":
                        exp.Func = RptExpFunc.Count;
                        exp.DataName = match.Groups[2].Value;
                        break;
                    case "index":
                        exp.Func = RptExpFunc.Index;
                        exp.DataName = match.Groups[2].Value;
                        break;
                    case "param":
                        exp.Func = RptExpFunc.Param;
                        exp.VarName = match.Groups[2].Value;
                        break;
                    case "global":
                        exp.Func = RptExpFunc.Global;
                        string valName = match.Groups[2].Value;
                        exp.VarName = valName;
                        if (valName == "总页数" || valName == "页号")
                            ExistPlaceholder = true;
                        break;
                    default:
                        exp.Func = RptExpFunc.Unknown;
                        exp.VarName = p_str;
                        break;
                }
                return exp;
            }

            exp.Func = RptExpFunc.Unknown;
            exp.VarName = p_str;
            return exp;
        }

        static Color HexStringToColor(string p_hexColor)
        {
            Color color = new Color();
            if (string.IsNullOrEmpty(p_hexColor))
                return color;
            p_hexColor = p_hexColor.Trim('#');
            if (p_hexColor.Length != 8)
                return color;
            try
            {
                color = Color.FromArgb(byte.Parse(p_hexColor.Substring(0, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(2, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(4, 2), NumberStyles.HexNumber),
                    byte.Parse(p_hexColor.Substring(6, 2), NumberStyles.HexNumber));
            }
            catch
            {
                return color;
            }
            return color;
        }
    }
}
