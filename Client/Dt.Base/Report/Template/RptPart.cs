#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 容器项，页眉、页脚、模板内容基类
    /// </summary>
    internal abstract class RptPart
    {
        public RptPart(RptRoot p_root)
        {
            Root = p_root;
            Items = new ObservableCollection<RptItem>();
            AtachEvent();
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取容器内的报表项
        /// </summary>
        public ObservableCollection<RptItem> Items { get; }

        /// <summary>
        /// 获取报表实例
        /// </summary>
        public RptRootInst Inst
        {
            get { return Root.Inst; }
        }

        /// <summary>
        /// 获取容器项所占行数
        /// </summary>
        public virtual int RowSpan
        {
            get { return 1; }
        }

        /// <summary>
        /// 获取容器项所占列数
        /// </summary>
        public int ColSpan
        {
            get
            {
                int interval = 0;
                if (Items != null && Items.Count > 0)
                {
                    interval = Items.Max(item => (item.Col + item.ColSpan));
                }
                return interval;
            }
        }

        /// <summary>
        /// 获取容器项宽度，根据所占列数
        /// </summary>
        public double Width
        {
            get
            {
                double widths = 0;
                if (Root.Cols != null && Root.Cols.Length > 0)
                {
                    foreach (double width in Root.Cols)
                    {
                        widths += width;
                    }
                }
                return widths;
            }
        }

        /// <summary>
        /// 获取容器项高度
        /// </summary>
        public abstract double Height { get; }

        /// <summary>
        /// 获取报表项容器种类
        /// </summary>
        public abstract RptPartType PartType { get; }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public virtual void ReadXml(XmlReader p_reader)
        {
            p_reader.MoveToElement();
            if (p_reader.IsEmptyElement)
            {
                p_reader.Read();
                return;
            }

            string name = p_reader.Name;
            p_reader.Read();
            while (p_reader.NodeType != XmlNodeType.None)
            {
                if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == name)
                    break;

                RptItem item = null;
                switch (p_reader.Name)
                {
                    case "Text":
                        item = new RptText(this);
                        break;
                    case "Table":
                        item = new RptTable(this);
                        break;
                    case "Matrix":
                        item = new RptMatrix(this);
                        break;
                    case "Chart":
                        item = new RptChart(this);
                        break;
                    default:
                        if (item == null)
                            Kit.Error(string.Format("反序列化报表模板时错误，无法识别报表项【{0}】！", p_reader.Name));
                        break;
                }
                item.ReadXml(p_reader);
                Items.Add(item);
            }
            p_reader.Read();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public virtual void WriteXml(XmlWriter p_writer)
        {
            // 按照从上到下从左到右的顺序序列化
            foreach (RptItem item in Items.OrderBy((obj) => obj.Col).OrderBy((obj) => obj.Row))
            {
                item.WriteXml(p_writer);
            }
        }

        /// <summary>
        /// 构造所有报表项实例
        /// </summary>
        public async Task BuildChild()
        {
            foreach (RptItem item in Items)
            {
                await item.Build();
            }
        }

        /// <summary>
        /// 删除指定区域的报表项
        /// </summary>
        /// <param name="p_range"></param>
        public void RemoveItem(CellRange p_range)
        {
            foreach (RptItem item in Items)
            {
                if (item.Row == p_range.Row
                    && item.Col == p_range.Column
                    && item.RowSpan == p_range.RowCount
                    && item.ColSpan == p_range.ColumnCount)
                {
                    Items.Remove(item);
                    break;
                }
            }
        }

        /// <summary>
        /// 获取指定位置的行高
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public abstract double GetRowHeight(int p_index);

        /// <summary>
        /// 移除事件处理
        /// </summary>
        public void DetachEvent()
        {
            Items.CollectionChanged -= OnCollectionChanged;
        }

        /// <summary>
        /// 增加事件处理
        /// </summary>
        public void AtachEvent()
        {
            Items.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// 拆分成尺寸
        /// </summary>
        /// <param name="p_size"></param>
        /// <returns></returns>
        public static double[] SplitSize(string p_size)
        {
            if (string.IsNullOrEmpty(p_size))
                return new double[0];

            string[] strs = p_size.Split(',');
            double[] sizes = new double[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                sizes[i] = Convert.ToDouble(strs[i]);
            }
            return sizes;
        }

        /// <summary>
        /// 合并尺寸成字符串
        /// </summary>
        /// <param name="p_size"></param>
        /// <returns></returns>
        public static string MergeSize(double[] p_size)
        {
            if (p_size == null || p_size.Length == 0)
                return "";

            int length = p_size.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(p_size[i]);
                if (i < length - 1)
                    sb.Append(",");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 触发报表项增删事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Root.OnItemsChanged(this, e);
        }
    }

    /// <summary>
    /// 报表项容器种类
    /// </summary>
    public enum RptPartType
    {
        /// <summary>
        /// 模板
        /// </summary>
        Body,

        /// <summary>
        /// 页眉
        /// </summary>
        Header,

        /// <summary>
        /// 页脚
        /// </summary>
        Footer
    }
}
