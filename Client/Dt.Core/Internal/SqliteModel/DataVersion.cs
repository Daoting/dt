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
    /// 用户缓存数据的版本号
    /// </summary>
    [StateTable]
    public class DataVersion
    {
        /// <summary>
        /// 数据类型，如菜单、权限等
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver { get; set; }
    }
}
