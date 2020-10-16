#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 矩阵行头列头的层次
    /// </summary>
    internal class RptMtxLevel : RptItemBase
    {
        #region 内部属性
        readonly RptMtxHeader _header;
        #endregion

        #region 构造方法
        public RptMtxLevel(RptMtxHeader p_header)
        {
            _header = p_header;
            Item = new RptText(this);
            SubTotals = new List<RptMtxSubtotal>();
            SubTitles = new List<RptMtxSubtitle>();
            // 对应字段名
            _data.AddCell<string>("field");
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public override RptRoot Root
        {
            get { return _header.Root; }
        }

        /// <summary>
        /// 获取报表项所属容器
        /// </summary>
        public override RptPart Part
        {
            get { return _header.Part; }
        }

        /// <summary>
        /// 获取报表项所属父项
        /// </summary>
        public override RptItemBase Parent
        {
            get { return _header; }
        }

        /// <summary>
        /// 获取设置字段名
        /// </summary>
        public string Field
        {
            get { return _data.Str("field"); }
            set { _data["field"] = value; }
        }

        /// <summary>
        /// 获取小计列表
        /// </summary>
        public List<RptMtxSubtotal> SubTotals { get; set; }

        /// <summary>
        /// 获取标题列表
        /// </summary>
        public List<RptMtxSubtitle> SubTitles { get; set; }

        /// <summary>
        /// 矩阵
        /// </summary>
        public RptMatrix Matrix
        {
            get { return Parent.Parent as RptMatrix; }
        }

        /// <summary>
        /// RptText
        /// </summary>
        public RptText Item { get; }

        /// <summary>
        /// 获取设置报表元素起始行索引
        /// </summary>
        public override int Row
        {
            get
            {
                RptMatrix mtx = _header.Parent as RptMatrix;
                int row = 1;
                if (_header is RptMtxRowHeader)
                {
                    int baseRow = 0;
                    if(_header.Levels.IndexOf(this) == 0)
                    {
                        baseRow = mtx.Row;
                        if(!mtx.HideColHeader)
                        {
                            baseRow += mtx.Corner.RowSpan;
                        }
                    }
                    else
                    {
                        baseRow = _header.Levels[_header.Levels.IndexOf(this) - 1].Row;
                    }
                    int count = 0;
                    IEnumerable<RptMtxSubtotal> befList = from c in SubTotals
                                                          where c.TotalLoc == TotalLocation.Before
                                                          select c;
                    foreach (RptMtxSubtotal total in befList)
                    {
                        count += total.GetCount();
                    }
                    row = baseRow + count;
                }
                else
                {
                    row = mtx.Row + _header.Levels.IndexOf(this);
                }
                return row;
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素起始列索引
        /// </summary>
        public override int Col
        {
            get
            {
                RptMatrix mtx = _header.Parent as RptMatrix;
                int col = 1;
                if (_header is RptMtxRowHeader)
                {
                    col = mtx.Col + _header.Levels.IndexOf(this);
                }
                else
                {
                    int baseCol = 0;
                    if(_header.Levels.IndexOf(this) == 0)
                    {
                        baseCol = mtx.Col;
                        if(!mtx.HideRowHeader)
                            baseCol += mtx.Corner.ColSpan;
                    }
                    else
                    {
                        baseCol = _header.Levels[_header.Levels.IndexOf(this) - 1].Col;
                    }
                    int count = 0;
                    IEnumerable<RptMtxSubtotal> befList = from c in SubTotals
                                                          where c.TotalLoc == TotalLocation.Before
                                                          select c;
                    foreach (RptMtxSubtotal total in befList)
                    {
                        count += total.GetCount();
                    }
                    col = baseCol + count;
                }
                return col;
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占行数
        /// </summary>
        public override int RowSpan
        {
            get
            {
                if (_header is RptMtxColHeader)
                    return 1;
                int span = SubTitles.Count > 0 ? 0 : 1;
                int index = _header.Levels.IndexOf(this);
                if (_header.Levels.Count > index + 1)
                {
                    ObservableCollection<RptMtxLevel> levels = new ObservableCollection<RptMtxLevel>();
                    for (int i = index + 1; i < _header.Levels.Count; i++)
                    {
                        RptMtxLevel subLevel = _header.Levels[i];
                        int totalSpan = subLevel.GetTotalSpan(subLevel.SubTotals);
                        int sublevelSpan = subLevel.RowSpan;
                        span = totalSpan + sublevelSpan;
                    }
                }
                foreach (RptMtxSubtitle title in SubTitles)
                {
                    span += title.GetCount();
                }
                return span;
            }
            set { }
        }

        /// <summary>
        /// 获取设置报表元素所占列数
        /// </summary>
        public override int ColSpan
        {
            get
            {
                if (_header is RptMtxRowHeader)
                    return 1;
                int span = SubTitles.Count > 0 ? 0 : 1;
                int index = _header.Levels.IndexOf(this);
                if (_header.Levels.Count > index + 1)
                {
                    ObservableCollection<RptMtxLevel> levels = new ObservableCollection<RptMtxLevel>();
                    for (int i = index + 1; i < _header.Levels.Count; i++)
                    {
                        RptMtxLevel subLevel = _header.Levels[i];
                        int totalSpan = subLevel.GetTotalSpan(subLevel.SubTotals);
                        int sublevelSpan = subLevel.ColSpan;
                        span = totalSpan + sublevelSpan;
                    }
                }
                foreach (RptMtxSubtitle title in SubTitles)
                {
                    span += title.GetCount();
                }
                return span;
            }
            set { }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 克隆层
        /// </summary>
        /// <param name="p_header"></param>
        /// <returns></returns>
        public RptMtxLevel Clone(RptMtxHeader p_header)
        {
            RptMtxLevel level = new RptMtxLevel(p_header);
            level.Data.Copy(this.Data);
            level.Item.Data.Copy(Item.Data);
            foreach (RptMtxSubtotal total in SubTotals)
            {
                level.SubTotals.Add(total.Clone(level));
            }
            foreach (RptMtxSubtitle title in SubTitles)
            {
                level.SubTitles.Add(title.Clone(level));
            }
            return level;
        }

        /// <summary>
        /// 递归获取小计所占行（列）数
        /// </summary>
        /// <param name="subTotal"></param>
        /// <returns></returns>
        public int GetTotalSpan(List<RptMtxSubtotal> subTotal)
        {
            int count = 0;
            foreach (RptMtxSubtotal total in subTotal)
            {
                count += total.GetCount();
            }
            return count;
        }

        /// <summary>
        /// 递归获取标题所占行（列）数
        /// </summary>
        /// <param name="p_titles"></param>
        /// <returns></returns>
        public int GetTitleSpan(List<RptMtxSubtitle> p_titles)
        {
            if (p_titles == null)
                return 0;
            int count = 0;
            foreach (RptMtxSubtitle title in p_titles)
            {
                if (title.SubTitles.Count > 0)
                {
                    count += GetTitleSpan(title.SubTitles);
                }
                else
                {
                    count += 1;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取子层所占行（列）数
        /// </summary>
        /// <param name="p_levels"></param>
        /// <returns></returns>
        int SubLevelSpan(List<RptMtxLevel> p_levels)
        {
            int span = 1;
            for (int i = p_levels.Count - 1; i >= 0; i--)
            {
                RptMtxLevel level = p_levels[i];
                int tiCount = GetTitleSpan(level.SubTitles);
                if (tiCount != 0)
                {
                    span += tiCount;
                }
                span += GetTotalSpan(level.SubTotals);  //小计与层同一级，有几行就占几行
            }
            return span;
        }
        #endregion

        #region xml
        protected override void ReadChildXml(XmlReader p_reader)
        {
            switch (p_reader.Name)
            {
                case "Text":
                    Item.ReadXml(p_reader);
                    break;
                case "Subtotal":
                    if (SubTotals == null)
                        SubTotals = new List<RptMtxSubtotal>();
                    RptMtxSubtotal sub = new RptMtxSubtotal(this);
                    sub.ReadXml(p_reader);
                    SubTotals.Add(sub);
                    break;
                case "Subtitle":
                    if (SubTitles == null)
                        SubTitles = new List<RptMtxSubtitle>();
                    RptMtxSubtitle title = new RptMtxSubtitle(this);
                    title.ReadXml(p_reader);
                    SubTitles.Add(title);
                    break;
            }
        }

        public override void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Level");
            string val = _data.Str("field");
            if (val != "")
                p_writer.WriteAttributeString("field", val);

            Item.WriteXml(p_writer);
            if (SubTotals != null && SubTotals.Count > 0)
            {
                foreach (RptMtxSubtotal subtotal in SubTotals)
                {
                    subtotal.WriteXml(p_writer);
                }
            }
            if (SubTitles != null && SubTitles.Count > 0)
            {
                foreach (RptMtxSubtitle title in SubTitles)
                {
                    title.WriteXml(p_writer);
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion
    }
}
