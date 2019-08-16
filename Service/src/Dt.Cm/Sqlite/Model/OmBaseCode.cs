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
    /// 基础代码
    /// </summary>
    [Table]
    public class OmBaseCode
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, MaxLength(64)]
        public string ID { get; set; }

        /// <summary>
        /// 所属分组
        /// </summary>
        [Indexed, MaxLength(64)]
        public string Grp { get; set; }
    }
}
