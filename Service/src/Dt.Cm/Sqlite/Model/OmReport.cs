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
    /// 报表模板
    /// </summary>
    [Table]
    public class OmReport
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, MaxLength(32)]
        public string ID { get; set; }

        /// <summary>
        /// 报表名称
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 报表定义
        /// </summary>
        public string Define { get; set; }
    }
}
