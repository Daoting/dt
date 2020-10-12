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
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表数据源定义
    /// </summary>
    internal class RptDataSource
    {
        Table _dataSet;
        Dictionary<string, Table> _dicCols;

        public RptDataSource(RptRoot p_root)
        {
            Root = p_root;
            _dicCols = new Dictionary<string, Table>();
            _dataSet = new Table
            {
                { "id" },
                { "isscritp", typeof(bool) },
                { "srv" },
                { "sql" },
                { "cols" },
            };
            _dataSet.Changed += Root.OnCellValueChanged;
        }

        /// <summary>
        /// 获取报表模板根对象
        /// </summary>
        public RptRoot Root { get; }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table DataSet
        {
            get { return _dataSet; }
            set
            {
                _dataSet = value;
                GenerDic();
            }
        }

        /// <summary>
        /// 获取数据源字段列表
        /// </summary>
        /// <param name="p_tblID"></param>
        /// <returns></returns>
        public Table GetCols(string p_tblID)
        {
            if (_dicCols.ContainsKey(p_tblID))
            {
                return _dicCols[p_tblID].Clone();
            }
            return null;
        }

        /// <summary>
        /// 添加、更新或删除字典内容；
        /// 参数【p_dt】为null时，移除键为【p_id】的项
        /// </summary>
        /// <param name="p_id"></param>
        /// <param name="p_dt"></param>
        public void ModDicCol(string p_id, Table p_dt = null)
        {
            if (string.IsNullOrEmpty(p_id))
                return;
            if (_dicCols.ContainsKey(p_id))
            {
                _dicCols.Remove(p_id);
            }
            if (p_dt != null)
            {
                _dicCols.Add(p_id, p_dt);
            }
        }

        /// <summary>
        /// 获取某项数据源
        /// </summary>
        /// <param name="p_tblID"></param>
        /// <returns></returns>
        public RptDataSourceItem GetDataSourceItem(string p_tblID)
        {
            Row row = (from dr in _dataSet
                       where dr.Str("id") == p_tblID
                       select dr).FirstOrDefault();
            if (row != null)
                return new RptDataSourceItem(row);
            return null;
        }

        /// <summary>
        /// 生成字段 
        /// </summary>
        void GenerDic()
        {
            foreach (Row dr in _dataSet)
            {
                Table dt = new Table();
                dt.Columns.Add(new Column("id"));
                string[] cols = dr.Str("cols").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string col in cols)
                {
                    Row nCol = dt.NewRow();
                    nCol["id"] = col;
                    dt.Add(nCol);
                }
                _dicCols.Add(dr.Str("id"), dt);
            }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            _dataSet.ReadXml(p_reader);
            GenerDic();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            _dataSet.WriteXml(p_writer, "Data", "Tbl");
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
        /// 数据集名称
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
