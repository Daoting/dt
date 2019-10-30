#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-28 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 子表描述类
    /// </summary>
    public class ChildTable
    {
        /// <summary>
        /// 子表数据
        /// </summary>
        public Table Data { get; set; }

        /// <summary>
        /// 子表的数据访问对象
        /// </summary>
        public DataAccess Da { get; set; }
    }
}