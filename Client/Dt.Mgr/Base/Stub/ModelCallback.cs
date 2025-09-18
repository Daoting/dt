#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 模型库的回调接口
    /// </summary>
    class ModelCallback : IModelCallback
    {
        /// <summary>
        /// 查询表结构信息
        /// </summary>
        /// <param name="p_tblAttr">实体类属性标签</param>
        /// <returns></returns>
        public async Task<TableSchema> GetTableSchema(TblAttribute p_tblAttr)
        {
            var tblName = p_tblAttr.Name.ToLower();
            var data = await AtModel.Query<OmTable>($"select * from OmTable where Name='{tblName}'");
            if (data.Count == 0)
                return null;

            OmTable tbl = null;
            if (data.Count > 1)
            {
                // 不同库的表重名，先取该服务的默认数据源键名
                var dbKey = await Kit.Rpc<string>(At.CurrentSvc, "Da.GetDbKey");
                foreach (var r in data)
                {
                    if (dbKey.Equals(r.DbKey, StringComparison.OrdinalIgnoreCase))
                    {
                        tbl = r;
                        break;
                    }
                }
                if (tbl == null)
                    return null;
            }
            else
            {
                tbl = data[0];
            }

            TableSchema schema = new TableSchema(tblName, tbl.Type, tbl.DbKey);
            var cols = await AtModel.Each<OmColumn>($"select * from OmColumn where TableID={tbl.ID}");
            foreach (var oc in cols)
            {
                TableCol col = new TableCol(schema);
                col.Name = oc.ColName;
                col.Type = Table.GetColType(oc.DbType);
                col.Length = oc.Length;
                col.Nullable = oc.Nullable;
                if (oc.IsPrimary)
                    schema.PrimaryKey.Add(col);
                else
                    schema.Columns.Add(col);
            }
            return schema;
        }
    }

    class OmTable : Entity
    {
        OmTable() { }

        /// <summary>
        /// 主键
        /// </summary>
        new public int ID
        {
            get { return (int)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType Type
        {
            get { return (DatabaseType)this["Type"]; }
            set { this["Type"] = value; }
        }

        /// <summary>
        /// 数据源键名
        /// </summary>
        public string DbKey
        {
            get { return (string)this["DbKey"]; }
            set { this["DbKey"] = value; }
        }
    }

    class OmColumn : Entity
    {
        OmColumn() { }

        /// <summary>
        /// 主键
        /// </summary>
        new public int ID
        {
            get { return (int)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 所属表
        /// </summary>
        public int TableID
        {
            get { return (int)this["TableID"]; }
            set { this["TableID"] = value; }
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColName
        {
            get { return (string)this["ColName"]; }
            set { this["ColName"] = value; }
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DbType
        {
            get { return (string)this["DbType"]; }
            set { this["DbType"] = value; }
        }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimary
        {
            get { return (bool)this["IsPrimary"]; }
            set { this["IsPrimary"] = value; }
        }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public int Length
        {
            get { return (int)this["Length"]; }
            set { this["Length"] = value; }
        }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable
        {
            get { return (bool)this["Nullable"]; }
            set { this["Nullable"] = value; }
        }
    }
}