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
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表数据源定义
    /// </summary>
    internal class RptDataSource
    {
        public RptDataSource(RptRoot p_root)
        {
            Root = p_root;
            DataSet = new Table
            {
                { "id" },
                { "isscritp", typeof(bool) },
                { "srv" },
                { "sql" },
                { "cols" },
            };
            DataSet.Changed += Root.OnCellValueChanged;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table DataSet { get; }

        /// <summary>
        /// 获取某项数据源
        /// </summary>
        /// <param name="p_tblID"></param>
        /// <returns></returns>
        public RptDataSourceItem GetDataSourceItem(string p_tblID)
        {
            Row row = (from dr in DataSet
                       where dr.Str("id") == p_tblID
                       select dr).FirstOrDefault();
            if (row != null)
                return new RptDataSourceItem(row);
            return null;
        }

        /// <summary>
        /// 获取指定数据源的列Table
        /// </summary>
        /// <param name="p_tblID"></param>
        /// <returns></returns>
        public Table GetColsData(string p_tblID)
        {
            Table tbl = new Table { { "id" } };
            Row row = (from dr in DataSet
                       where dr.Str("id") == p_tblID
                       select dr).FirstOrDefault();
            if (row != null)
            {
                string[] cols = row.Str("cols").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string col in cols)
                {
                    tbl.AddRow(new { id = col });
                }
            }
            return tbl;
        }

        /// <summary>
        /// 更新指定数据源的列
        /// </summary>
        /// <param name="p_tblID"></param>
        /// <param name="p_cols"></param>
        public void UpdateCols(string p_tblID, Table p_cols)
        {
            Row row = (from dr in DataSet
                       where dr.Str("id") == p_tblID
                       select dr).FirstOrDefault();
            if (row != null)
            {
                // 合并列
                StringBuilder sb = new StringBuilder();
                foreach (var r in p_cols)
                {
                    if (sb.Length > 0)
                        sb.Append(",");
                    sb.Append(r.Str(0));
                }
                row["cols"] = sb.ToString();
            }
        }

        public bool IsValid()
        {
            bool fail = (from row in DataSet
                         where row.Str("id") == string.Empty
                         select row).Any();
            if (fail)
            {
                AtKit.Warn("数据名称不可为空！");
                return false;
            }

            fail = DataSet.GroupBy(r => r.Str("id")).Where(g => g.Count() > 1).Any();
            if (fail)
            {
                AtKit.Warn("数据名称不可重复！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            DataSet.ReadXml(p_reader, "sql");
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Data");
            foreach (Row row in DataSet)
            {
                p_writer.WriteStartElement("Tbl");

                p_writer.WriteAttributeString("id", row.Str("id"));

                if (row.Bool("isscritp"))
                    p_writer.WriteAttributeString("isscritp", "True");

                string val = row.Str("srv");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("srv", val);

                val = row.Str("cols");
                if (val != string.Empty)
                    p_writer.WriteAttributeString("cols", val);

                val = row.Str("sql");
                if (val != string.Empty)
                    p_writer.WriteCData(val);
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();
        }
    }

    internal class RptDataSourceItem
    {
        Row _row;

        public RptDataSourceItem(Row p_row)
        {
            _row = p_row;
        }

        /// <summary>
        /// 数据名称
        /// </summary>
        public string ID
        {
            get { return _row.Str("id"); }
        }

        /// <summary>
        /// 是否通过脚本获取数据源
        /// </summary>
        public bool IsScritp
        {
            get { return _row.Bool("isscritp"); }
        }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Srv
        {
            get { return _row.Str("srv"); }
        }

        /// <summary>
        /// Sql语句
        /// </summary>
        public string Sql
        {
            get { return _row.Str("sql"); }
        }

        /// <summary>
        /// 数据源列
        /// </summary>
        public string Cols
        {
            get { return _row.Str("cols"); }
        }
    }
}
