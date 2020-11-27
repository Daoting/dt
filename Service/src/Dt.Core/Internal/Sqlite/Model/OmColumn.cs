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

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// 系统列定义
    /// </summary>
    [Table]
    public class OmColumn
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>
        /// 所属表名
        /// </summary>
        [Indexed, MaxLength(30)]
        public string TabName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        [MaxLength(30)]
        public string ColName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        [MaxLength(128)]
        public string DbType { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments { get; set; }
    }
}
