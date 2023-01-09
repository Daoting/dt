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
    [Tbl("cm_user_params")]
    public partial class UserParamsObj : Entity
    {
        #region 构造方法
        UserParamsObj() { }

        public UserParamsObj(
            long UserID,
            string ParamID,
            string Value = default,
            DateTime Mtime = default)
        {
            AddCell("UserID", UserID);
            AddCell("ParamID", ParamID);
            AddCell("Value", Value);
            AddCell("Mtime", Mtime);
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
        /// 参数标识
        /// </summary>
        public string ParamID
        {
            get { return (string)this["ParamID"]; }
            set { this["ParamID"] = value; }
        }

        new public long ID { get { return -1; } }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value
        {
            get { return (string)this["Value"]; }
            set { this["Value"] = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }

        #region 静态方法
        /// <summary>
        /// 返回所有实体的列表，每个实体包含所有列值
        /// </summary>
        /// <returns></returns>
        public static Task<Table<UserParamsObj>> GetAll()
        {
            return EntityEx.GetAll<UserParamsObj>();
        }

        /// <summary>
        /// 获取新ID
        /// </summary>
        /// <returns></returns>
        public static Task<long> NewID()
        {
            return EntityEx.GetNewID<UserParamsObj>();
        }

        /// <summary>
        /// 获取新序列值
        /// </summary>
        /// <param name="p_colName">字段名称，不可为空</param>
        /// <returns>新序列值</returns>
        public static Task<int> NewSeq(string p_colName)
        {
            return EntityEx.GetNewSeq<UserParamsObj>(p_colName);
        }
        #endregion
    }
}