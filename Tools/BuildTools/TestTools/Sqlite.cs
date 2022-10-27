namespace Dt.Core
{
    /// <summary>
    /// 客户端Cookie字典
    /// </summary>
    [Sqlite("state")]
    public class ClientCookie
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Val { get; set; }
    }

    [Sqlite("state")]
    public class CellLastVal
    {
        /// <summary>
        /// 单元格唯一标识：BaseUri + Fv.Name + FvCell.ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 单元格最后编辑的值
        /// </summary>
        public string Val { get; set; }
    }

    [Sqlite("local")]
    public class LocalDict
    {
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Val { get; set; }
    }

    /// <summary>
    /// 类型和sqlite表的映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SqliteAttribute : Attribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_dbName">sqlite库文件名，不包括扩展名</param>
        public SqliteAttribute(string p_dbName)
        {
            DbName = p_dbName.ToLower();
        }

        /// <summary>
        /// sqlite库文件名，小写不包括扩展名
        /// </summary>
        public string DbName { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    { }

    /// <summary>
    /// sqlite库的描述信息
    /// </summary>
    public class SqliteTblsInfo
    {
        /// <summary>
        /// 库结构的版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 表结构的映射类型
        /// </summary>
        public IList<Type> Tables { get; set; }
    }
}