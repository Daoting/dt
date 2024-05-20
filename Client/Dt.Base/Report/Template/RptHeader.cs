#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using System;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 页眉
    /// </summary>
    public class RptHeader : RptPart
    {
        public const double DefaultHeight = 25;
        double _height = DefaultHeight;
        bool _defaultHeader;
        
        public RptHeader(RptRoot p_root)
            : base(p_root)
        {
        }

        /// <summary>
        /// 采用默认页眉变化事件
        /// </summary>
        public event Action DefaultHeaderChanged;
        
        /// <summary>
        /// 是否采用默认页眉：报表名称居中显示、带下划线
        /// </summary>
        public bool DefaultHeader
        {
            get { return _defaultHeader; }
            set
            {
                if (_defaultHeader != value)
                {
                    _defaultHeader = value;
                    Items.Clear();
                    DefaultHeaderChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// 获取高度，默认25
        /// </summary>
        public override double Height
        {
            get { return _height; }
        }

        /// <summary>
        /// 与报表内容的间距，默认25
        /// </summary>
        public double BodySpacing { get; set; } = DefaultHeight;

        /// <summary>
        /// 实际高度，头无子元素时认为高度为0.
        /// </summary>
        public double ActualHeight
        {
            get { return Items.Count > 0 ? _height + BodySpacing : 0; }
        }

        /// <summary>
        /// 获取报表项容器种类
        /// </summary>
        public override RptPartType PartType
        {
            get { return RptPartType.Header; }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public override void ReadXml(XmlReader p_reader)
        {
            DetachEvent();
            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                string id = p_reader.Name;
                if (id == "height")
                {
                    _height = Convert.ToDouble(p_reader.Value);
                }
                else if (id == "bodyspacing")
                {
                    BodySpacing = Convert.ToDouble(p_reader.Value);
                }
                else if (id == "defaultheader")
                {
                    _defaultHeader = "True".Equals(p_reader.Value, StringComparison.OrdinalIgnoreCase);
                }
            }
            base.ReadXml(p_reader);
            AtachEvent();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Header");
            if (_defaultHeader)
            {
                p_writer.WriteAttributeString("defaultheader", "True");
            }
            else
            {
                if (_height != DefaultHeight)
                    p_writer.WriteAttributeString("height", _height.ToString());
                if (BodySpacing != DefaultHeight)
                    p_writer.WriteAttributeString("bodyspacing", BodySpacing.ToString());
                base.WriteXml(p_writer);
            }
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 构造页眉实例
        /// </summary>
        public Task Build()
        {
            if (_defaultHeader)
            {
                Inst.Header = new RptHeaderInst(this);

                // 采用默认页眉：报表名称居中显示、带下划线
                var rt = new RptText(this);
                rt.Val = ":Var(报表名称)";
                rt.Horalign = Cells.Data.CellHorizontalAlignment.Center;
                rt.LeftStyle = Cells.Data.BorderLineStyle.None;
                rt.TopStyle = Cells.Data.BorderLineStyle.None;
                rt.RightStyle = Cells.Data.BorderLineStyle.None;
                rt.ParseVal();

                Items.Clear();
                Items.Add(rt);
                rt.Build();
            }
            else if (Items.Count > 0)
            {
                Inst.Header = new RptHeaderInst(this);
                return BuildChild();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 设置高度
        /// </summary>
        /// <param name="p_height"></param>
        public void SetHeight(double p_height)
        {
            _height = p_height;
        }

        /// <summary>
        /// 获取指定位置的行高
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public override double GetRowHeight(int p_index)
        {
            if (p_index != 0)
                Throw.Msg("报表头只包含一行！");
            return _height;
        }
    }
}
