#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
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
    /// 页脚
    /// </summary>
    public class RptFooter : RptPart
    {
        double _height = RptHeader.DefaultHeight;
        bool _defaultFooter;
        
        public RptFooter(RptRoot p_root)
            : base(p_root)
        {
        }

        /// <summary>
        /// 采用默认页脚变化事件
        /// </summary>
        public event Action DefaultFooterChanged;
        
        /// <summary>
        /// 是否采用默认页脚：居中显示的页码
        /// </summary>
        public bool DefaultFooter
        {
            get { return _defaultFooter; }
            set
            {
                if (_defaultFooter != value)
                {
                    _defaultFooter = value;
                    Items.Clear();
                    DefaultFooterChanged?.Invoke();
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
        public double BodySpacing { get; set; } = RptHeader.DefaultHeight;

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
            get { return RptPartType.Footer; }
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
                else if (id == "defaultfooter")
                {
                    _defaultFooter = "True".Equals(p_reader.Value, StringComparison.OrdinalIgnoreCase);
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
            p_writer.WriteStartElement("Footer");
            if (_defaultFooter)
            {
                p_writer.WriteAttributeString("defaultfooter", "True");
            }
            else
            {
                if (_height != RptHeader.DefaultHeight)
                    p_writer.WriteAttributeString("height", _height.ToString());
                if (BodySpacing != RptHeader.DefaultHeight)
                    p_writer.WriteAttributeString("bodyspacing", BodySpacing.ToString());
                base.WriteXml(p_writer);
            }
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 构造页脚实例
        /// </summary>
        public Task Build()
        {
            if (_defaultFooter)
            {
                Inst.Footer = new RptFooterInst(this);

                // 采用默认页脚：居中显示的页码
                var rt = new RptText(this);
                rt.Val = ":Var(页号)";
                rt.Horalign = Cells.Data.CellHorizontalAlignment.Center;
                rt.LeftStyle = Cells.Data.BorderLineStyle.None;
                rt.TopStyle = Cells.Data.BorderLineStyle.None;
                rt.RightStyle = Cells.Data.BorderLineStyle.None;
                rt.BottomStyle = Cells.Data.BorderLineStyle.None;
                rt.ParseVal();
                
                Items.Clear();
                Items.Add(rt);
                rt.Build();
            }
            else if(Items.Count > 0)
            {
                Inst.Footer = new RptFooterInst(this);
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
                Throw.Msg("报列尾只包含一行！");
            return _height;
        }
    }
}
