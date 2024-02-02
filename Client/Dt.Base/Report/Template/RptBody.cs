#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 模板内容
    /// </summary>
    internal class RptBody : RptPart
    {
        public RptBody(RptRoot p_root)
            : base(p_root)
        {
        }

        /// <summary>
        /// 获取内容区域所有行高
        /// </summary>
        public double[] Rows { get; set; }

        /// <summary>
        /// 获取内容区域所占行数
        /// </summary>
        public override int RowSpan
        {
            get
            {
                int interval = 0;
                if (Items != null && Items.Count > 0) 
                {
                    interval = Items.Max(item => (item.Row + item.RowSpan));
                }
                return interval;
            }
        }

        /// <summary>
        /// 获取容器项高度，根据所占行数
        /// </summary>
        public override double Height
        {
            get
            {
                double heights = 0;
                if (Rows != null && Rows.Length > 0) 
                {
                    foreach (double height in Rows) 
                    {
                        heights += height;
                    }
                }
                return heights;
            }
        }

        /// <summary>
        /// 获取报表项容器种类
        /// </summary>
        public override RptPartType PartType
        {
            get { return RptPartType.Body; }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public override void ReadXml(XmlReader p_reader)
        {
            DetachEvent();
            // 行高列宽
            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                string id = p_reader.Name;
                if (id == "rows")
                {
                    Rows = SplitSize(p_reader.Value);
                    break;
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
            p_writer.WriteStartElement("Body");
            p_writer.WriteAttributeString("rows", MergeSize(Rows));
            base.WriteXml(p_writer);
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 构造报表内容实例
        /// </summary>
        public Task Build()
        {
            if (Items.Count > 0)
            {
                Inst.Body = new RptBodyInst(this);
                return BuildChild();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取指定位置的行高
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public override double GetRowHeight(int p_index)
        {
            if (p_index < 0 || p_index >= Rows.Length)
                throw new Exception("获取行高的位置超出范围！");
            return Rows[p_index];
        }
    }
}
