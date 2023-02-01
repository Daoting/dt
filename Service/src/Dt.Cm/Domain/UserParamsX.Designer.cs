#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm.Domain
{
    [Tbl("cm_user_params")]
    public partial class UserParamsX : EntityX<UserParamsX>
    {
        #region 构造方法
        UserParamsX() { }

        public UserParamsX(CellList p_cells) : base(p_cells) { }

        public UserParamsX(
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
    }
}