#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_user_role")]
    public partial class UserRoleObj : Entity
    {
        #region 构造方法
        UserRoleObj() { }

        public UserRoleObj(
            long UserID,
            long RoleID)
        {
            AddCell("UserID", UserID);
            AddCell("RoleID", RoleID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        new public long ID { get { return -1; } }

        #region 静态方法
        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<UserRoleObj>> GetAll()
        {
            return EntityEx.GetAll<UserRoleObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<UserRoleObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<UserRoleObj>(p_colName);
        }
        #endregion
    }
}