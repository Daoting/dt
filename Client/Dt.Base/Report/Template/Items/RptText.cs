#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Media;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using Windows.UI;
using Windows.UI.Text;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 文本项
    /// </summary>
    public class RptText : RptItem
    {
        #region 成员变量
        public const string DefaultFontName = "SimSun";
        public const double DefaultFontSize = 15.0;
        public const string ScriptValue = "#script#";
        const string _defaultForeground = "#ff000000";
        const string _defaultBackground = "#00ffffff";
        const string _defaultHorAlign = "General";
        const string _defaultVerAlign = "Center";
        const string _defaultBorder = "Thin";
        RptItemBase _parent;
        #endregion

        #region 构造方法
        public RptText(RptPart p_owner)
            : base(p_owner)
        {
            _data.Add<string>("val");
            _data.Add<bool>("hidetopdup");
            _data.Add<bool>("hideleftdup");
            _data.Add<bool>("autoheight");
            _data.Add<bool>("handleclick");
            _data.Add<bool>("wordwrap");
            _data.Add("fontfamily", DefaultFontName);
            _data.Add("fontsize", DefaultFontSize);
            _data.Add<bool>("bold");
            _data.Add<bool>("italic");
            _data.Add<bool>("underline");
            _data.Add<bool>("strikeout");
            _data.Add("foreground", _defaultForeground);
            _data.Add("background", _defaultBackground);
            _data.Add("horalign", _defaultHorAlign);
            _data.Add("veralign", _defaultVerAlign);
            _data.Add("margin", 0);
            _data.Add("lbc", _defaultForeground);
            _data.Add("tbc", _defaultForeground);
            _data.Add("rbc", _defaultForeground);
            _data.Add("bbc", _defaultForeground);
            _data.Add("lbs", _defaultBorder);
            _data.Add("tbs", _defaultBorder);
            _data.Add("rbs", _defaultBorder);
            _data.Add("bbs", _defaultBorder);
        }

        public RptText(RptItemBase p_parent)
            : this(p_parent.Part)
        {
            _parent = p_parent;
        }
        #endregion

        #region 属性
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
        /// 获取或设置字体，默认 SimSun
        /// </summary>
        public string FontFamily
        {
            get { return _data.Str("fontfamily"); }
            set { _data["fontfamily"] = value; }
        }

        /// <summary>
        /// 字体大小属性，默认 15
        /// </summary>
        public double FontSize
        {
            get { return _data.Double("fontsize"); }
            set { _data["fontsize"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用加粗样式，默认false
        /// </summary>
        public bool Bold
        {
            get { return _data.Bool("bold"); }
            set { _data["bold"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用斜体样式，默认false
        /// </summary>
        public bool Italic
        {
            get { return _data.Bool("italic"); }
            set { _data["italic"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用下划线样式，默认false
        /// </summary>
        public bool UnderLine
        {
            get { return _data.Bool("underline"); }
            set { _data["underline"] = value; }
        }

        /// <summary>
        /// 获取或设置是否应用删除线样式，默认false
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
        /// 获取或设置水平对齐方式，默认General：合并单元格时居中，数字居右，其它居左
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
        /// 获取是否包含总页数、页号、外部调用等占位符
        /// </summary>
        public PlaceholderType Placeholder { get; set; }

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
        #endregion

        #region 外部方法
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
            if (p_cell.WordWrap != WordWrap)
                p_cell.WordWrap = WordWrap;

            if (FontFamily != DefaultFontName
                || (p_cell.FontFamily != null && FontFamily != p_cell.FontFamily.Source))
                p_cell.FontFamily = new FontFamily(FontFamily);

            if (FontSize != DefaultFontSize
                || (p_cell.FontSize != -1 && FontSize != p_cell.FontSize))
                p_cell.FontSize = FontSize;

            if ((Bold && p_cell.FontWeight != FontWeights.Bold)
                || (!Bold && p_cell.FontWeight == FontWeights.Bold))
                p_cell.FontWeight = Bold ? FontWeights.Bold : FontWeights.Normal;

            if ((Italic && p_cell.FontStyle != FontStyle.Italic)
                || (!Italic && p_cell.FontStyle == FontStyle.Italic))
                p_cell.FontStyle = Italic ? FontStyle.Italic : FontStyle.Normal;

            if (UnderLine != p_cell.Underline)
                p_cell.Underline = UnderLine;

            if (StrikeOut != p_cell.Strikethrough)
                p_cell.Strikethrough = StrikeOut;

            if (HandleClick)
                p_cell.Foreground = Dt.Base.Res.主蓝;
            else if (_data.Str("foreground") != _defaultForeground)
                p_cell.Foreground = new SolidColorBrush(Foreground);
            else if (p_cell.Foreground != null)
                p_cell.Foreground = null;

            if (_data.Str("background") != _defaultBackground)
                p_cell.Background = new SolidColorBrush(Background);
            else if (p_cell.Background != null)
                p_cell.Background = null;

            if (Horalign != p_cell.HorizontalAlignment)
                p_cell.HorizontalAlignment = Horalign;

            if (Veralign != p_cell.VerticalAlignment)
                p_cell.VerticalAlignment = Veralign;

            if (Margin != p_cell.TextIndent)
                p_cell.TextIndent = Margin;

            Worksheet sheet = p_cell.Worksheet;

            // 超出异常
            if (p_cell.RowSpan > 1
                && p_cell.Row.Index + p_cell.RowSpan > sheet.Rows.Count)
                p_cell.RowSpan = sheet.Rows.Count - p_cell.Row.Index;
            if (p_cell.ColumnSpan > 1
                && p_cell.Column.Index + p_cell.ColumnSpan > sheet.Columns.Count)
                p_cell.ColumnSpan = sheet.Columns.Count - p_cell.Column.Index;

            // 直接设置cell.BorderRight在合并单元格时不显示！
            var range = new CellRange(p_cell.Row.Index, p_cell.Column.Index, p_cell.RowSpan, p_cell.ColumnSpan);
            sheet.SetBorder(range, new BorderLine(LeftColor, LeftStyle), SetBorderOptions.Left);
            sheet.SetBorder(range, new BorderLine(RightColor, RightStyle), SetBorderOptions.Right);
            sheet.SetBorder(range, new BorderLine(TopColor, TopStyle), SetBorderOptions.Top);
            sheet.SetBorder(range, new BorderLine(BottomColor, BottomStyle), SetBorderOptions.Bottom);
        }

        /// <summary>
        /// 解析值：固定文本、脚本获取、表达式
        /// </summary>
        public void ParseVal()
        {
            Expressions = null;
            IsScriptRender = false;

            string val = _data.Str("val").Trim();
            if (string.IsNullOrEmpty(val))
                return;

            // 连接符：||
            string[] subs = val.Split("||");
            char start = val[0];
            if (subs.Length > 1 || start == ':' || start == '@')
            {
                // 解析表达式
                Expressions = new List<RptExpression>();
                foreach (string item in subs)
                {
                    string str = item.Trim();
                    if (str == "")
                        continue;

                    var prefix = str[0];
                    if (prefix == ':')
                    {
                        int end = str.IndexOf(')');
                        if (end < 0)
                            Throw.Msg("值表达式不正确：" + str);
                        Expressions.Add(ParseExpression(str.Substring(1, end)));
                    }
                    else if (prefix == '@')
                    {
                        RptExpression exp = new RptExpression();
                        exp.Func = RptExpFunc.Call;
                        // 保持原始大小写，否则类型字典无法匹配
                        exp.VarName = str.Substring(1);
                        Placeholder |= PlaceholderType.Call;
                        Expressions.Add(exp);
                    }
                    else
                    {
                        RptExpression exp = new RptExpression();
                        exp.Func = RptExpFunc.Unknown;
                        exp.VarName = str;
                        Expressions.Add(exp);
                    }
                }
            }
            else
            {
                IsScriptRender = (val.ToLower() == ScriptValue);
            }
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
        /// 设置边框
        /// </summary>
        /// <param name="p_noborder">true 无边框，false 默认边框</param>
        public void SetBorder(bool p_noborder)
        {
            if (p_noborder)
            {
                _data["lbs"] = "None";
                _data["lbs"] = "None";
                _data["tbs"] = "None";
                _data["rbs"] = "None";
                _data["bbs"] = "None";
            }
            else
            {
                _data["lbs"] = _defaultBorder;
                _data["tbs"] = _defaultBorder;
                _data["rbs"] = _defaultBorder;
                _data["bbs"] = _defaultBorder;
            }

            _data["lbc"] = _defaultForeground;
            _data["tbc"] = _defaultForeground;
            _data["rbc"] = _defaultForeground;
            _data["bbc"] = _defaultForeground;
        }
        #endregion

        #region xml
        /// <summary>
        /// 加载xml时解析表达式
        /// </summary>
        /// <param name="p_reader"></param>
        public override void ReadXml(XmlReader p_reader)
        {
            _data.ReadXml(p_reader);
            ParseVal();
            p_reader.Read();
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
            if (val != _defaultHorAlign)
                p_writer.WriteAttributeString("horalign", val);
            val = _data.Str("veralign");
            if (val != _defaultVerAlign)
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

        #region 解析
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
                    case "group":
                        exp.Func = RptExpFunc.Group;
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
                    case "var":
                        exp.Func = RptExpFunc.Var;
                        string valName = match.Groups[2].Value;
                        exp.VarName = valName;
                        if (valName == "总页数")
                            Placeholder |= PlaceholderType.PageCount;
                        else if (valName == "页号")
                            Placeholder |= PlaceholderType.PageNum;
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
        #endregion
    }
}
