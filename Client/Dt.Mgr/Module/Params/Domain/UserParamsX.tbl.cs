#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 用户参数值
    /// </summary>
    [Tbl("cm_user_params")]
    public partial class UserParamsX : EntityX<UserParamsX>
    {
        #region 构造方法
        UserParamsX() { }

        public UserParamsX(CellList p_cells) : base(p_cells) { }

        public UserParamsX(
            long UserID,
            long ParamID,
            string Value = default,
            DateTime Mtime = default)
        {
            Add("user_id", UserID);
            Add("param_id", ParamID);
            Add("value", Value);
            Add("mtime", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["user_id"]; }
            set { this["user_id"] = value; }
        }

        /// <summary>
        /// 参数标识
        /// </summary>
        public long ParamID
        {
            get { return (long)this["param_id"]; }
            set { this["param_id"] = value; }
        }

        new public long ID { get { return -1; } }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["mtime"]; }
            set { this["mtime"] = value; }
        }
    }
}