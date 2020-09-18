#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System;
using System.Collections.Specialized;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表模板根元素
    /// </summary>
    internal class RptRoot
    {
        #region 构造方法
        public RptRoot()
        {
            Params = new RptParams();
            Data = new RptDataSource();
            Setting = new RptSetting();
            Header = new RptHeader(this);
            Body = new RptBody(this);
            Footer = new RptFooter(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 序列化开始前
        /// </summary>
        public event EventHandler Serializing;

        /// <summary>
        /// 序列化结束后
        /// </summary>
        public event EventHandler Serialized;

        /// <summary>
        /// 报表项增删事件
        /// </summary>
        public event EventHandler<NotifyCollectionChangedEventArgs> ItemsChanged;

        /// <summary>
        /// 文本项属性值变化事件
        /// </summary>
        public event EventHandler<Cell> TextChanged;

        /// <summary>
        /// 报表项更新事件
        /// </summary>
        public event EventHandler<bool> Updated;
        #endregion

        #region 属性
        /// <summary>
        /// 获取报表名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取报表参数
        /// </summary>
        public RptParams Params { get; }

        /// <summary>
        /// 获取报表数据源
        /// </summary>
        public RptDataSource Data { get; }

        /// <summary>
        /// 获取报表页面设置
        /// </summary>
        public RptSetting Setting { get; }

        /// <summary>
        /// 获取报表页眉
        /// </summary>
        public RptHeader Header { get; }

        /// <summary>
        /// 获取报表模板
        /// </summary>
        public RptBody Body { get; }

        /// <summary>
        /// 获取报表页脚
        /// </summary>
        public RptFooter Footer { get; }

        /// <summary>
        /// 获取报表实例
        /// </summary>
        public RptRootInst Inst { get; set; }

        /// <summary>
        /// 获取设置所有列宽
        /// </summary>
        public double[] Cols { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            if (p_reader == null || p_reader.IsEmptyElement || p_reader.Name != "Rpt")
                throw new Exception("加载报表模板根节点时出错！");

            // 报表名称
            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                string id = p_reader.Name;
                if (id == "name")
                    Name = p_reader.Value;
                else if (id == "cols")
                    Cols = RptPart.SplitSize(p_reader.Value);
            }

            // 模板内容
            p_reader.Read();
            while (p_reader.NodeType != XmlNodeType.None)
            {
                if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == "Rpt")
                    break;

                switch (p_reader.Name)
                {
                    case "Params":
                        Params.ReadXml(p_reader);
                        break;
                    case "Data":
                        Data.ReadXml(p_reader);
                        break;
                    case "Setting":
                        Setting.ReadXml(p_reader);
                        break;
                    case "Header":
                        Header.ReadXml(p_reader);
                        break;
                    case "Body":
                        Body.ReadXml(p_reader);
                        break;
                    case "Footer":
                        Footer.ReadXml(p_reader);
                        break;
                }
            }
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Rpt");
            p_writer.WriteAttributeString("name", Name);
            p_writer.WriteAttributeString("cols", RptPart.MergeSize(Cols));

            Params.WriteXml(p_writer);
            Data.WriteXml(p_writer);
            Setting.WriteXml(p_writer);
            Header.WriteXml(p_writer);
            Body.WriteXml(p_writer);
            Footer.WriteXml(p_writer);

            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 构造报表实例
        /// </summary>
        /// <param name="p_inst"></param>
        public void Build(RptRootInst p_inst)
        {
            Inst = p_inst;
            Header.Build();
            Body.Build();
            Footer.Build();
        }

        /// <summary>
        /// 触发序列化开始前事件
        /// </summary>
        public void OnBeforeSerialize()
        {
            Body.DetachEvent();
            Serializing?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发序列化结束后事件
        /// </summary>
        public void OnAfterSerialize()
        {
            Body.AtachEvent();
            Serialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发报表项增删事件
        /// </summary>
        /// <param name="p_container"></param>
        /// <param name="e"></param>
        public void OnItemsChanged(RptPart p_container, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(p_container, e);
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        (e.NewItems[i] as RptItem).Data.AcceptChanges();
                    }
                }
            }
        }

        /// <summary>
        /// 触发报表项属性值变化事件
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_cell"></param>
        public void OnValueChanged(RptItemBase p_item, Cell p_cell)
        {
            if (p_item is RptText pt && TextChanged != null)
                TextChanged(pt, p_cell);
            p_cell.AcceptChanges();
        }

        /// <summary>
        /// 触发报表项更新事件
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="e"></param>
        public void OnUpdated(RptItem p_item, bool e)
        {
            if (Updated != null)
            {
                Updated(p_item, e);
                p_item.Data.AcceptChanges();
            }
        }
        #endregion
    }
}
