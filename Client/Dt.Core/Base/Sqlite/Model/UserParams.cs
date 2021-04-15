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
    /// 用户参数
    /// </summary>
    [StateTable]
    public class UserParams
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Val { get; set; }
    }
}
