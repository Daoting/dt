#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_role_prv")]
    public partial class RolePrvObj : Entity
    {
        #region 构造方法
        RolePrvObj() { }

        public RolePrvObj(
            long RoleID,
            string PrvID)
        {
            AddCell("RoleID", RoleID);
            AddCell("PrvID", PrvID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string PrvID
        {
            get { return (string)this["PrvID"]; }
            set { this["PrvID"] = value; }
        }

        new public long ID { get { return -1; } }

        #region 静态方法
        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<RolePrvObj>> Query(string p_filter = null)
        {
            return EntityEx.Query<RolePrvObj>();
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<RolePrvObj>> Page(int p_starRow, int p_pageSize, string p_filter = null)
        {
            return EntityEx.Page<RolePrvObj>(p_starRow, p_pageSize, p_filter);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<RolePrvObj> First(string p_filter)
        {
            return EntityEx.First<RolePrvObj>(p_filter);
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<RolePrvObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<RolePrvObj>(p_colName);
        }
        #endregion
    }
}