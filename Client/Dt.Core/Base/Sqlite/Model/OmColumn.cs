#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
#endregion

namespace Dt.Core.Model
{
    /// <summary>
    /// 系统列定义，在模型库中
    /// </summary>
    public class OmColumn : Entity
    {
        #region 构造方法
        OmColumn() { }

        public OmColumn(
            int ID,
            string TabName = default,
            string ColName = default,
            string DbType = default,
            bool IsPrimary = default,
            int Length = default,
            bool Nullable = default,
            string Comments = default)
        {
            AddCell("ID", ID);
            AddCell("TabName", TabName);
            AddCell("ColName", ColName);
            AddCell("DbType", DbType);
            AddCell("IsPrimary", IsPrimary);
            AddCell("Length", Length);
            AddCell("Nullable", Nullable);
            AddCell("Comments", Comments);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        /// <summary>
        /// 主键
        /// </summary>
        new public int ID
        {
            get { return (int)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 所属表名
        /// </summary>
        public string TabName
        {
            get { return (string)this["TabName"]; }
            set { this["TabName"] = value; }
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

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments
        {
            get { return (string)this["Comments"]; }
            set { this["Comments"] = value; }
        }
    }
}
