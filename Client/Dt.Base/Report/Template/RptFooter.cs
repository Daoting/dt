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
    internal class RptFooter : RptPart
    {
        double _height = 40.0;

        public RptFooter(RptRoot p_root)
            : base(p_root)
        {
        }

        /// <summary>
        /// 获取高度
        /// </summary>
        public override double Height
        {
            get { return _height; }
        }

        /// <summary>
        /// 实际高度，头无子元素时认为高度为0.
        /// </summary>
        public double ActualHeight
        {
            get { return Items.Count > 0 ? _height : 0; }
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
            p_writer.WriteStartElement("Footer");
            if (_height != 40)
                p_writer.WriteAttributeString("height", _height.ToString());
            base.WriteXml(p_writer);
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 构造页脚实例
        /// </summary>
        public Task Build()
        {
            if (Items.Count > 0)
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
                throw new Exception("报表尾只包含一行！");
            return _height;
        }
    }
}
