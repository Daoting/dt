#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 用户参数值
    /// </summary>
    [Tbl("CM_USER_PARAMS")]
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
            Add("USER_ID", UserID);
            Add("PARAM_ID", ParamID);
            Add("VALUE", Value);
            Add("MTIME", Mtime);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 用户标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["USER_ID"]; }
            set { this["USER_ID"] = value; }
        }

        public Cell cUserID => _cells["USER_ID"];

        /// <summary>
        /// 参数标识
        /// </summary>
        public long ParamID
        {
            get { return (long)this["PARAM_ID"]; }
            set { this["PARAM_ID"] = value; }
        }

        public Cell cParamID => _cells["PARAM_ID"];

        new public long ID { get { return -1; } }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value
        {
            get { return (string)this["VALUE"]; }
            set { this["VALUE"] = value; }
        }

        public Cell cValue => _cells["VALUE"];

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Mtime
        {
            get { return (DateTime)this["MTIME"]; }
            set { this["MTIME"] = value; }
        }

        public Cell cMtime => _cells["MTIME"];
    }
}