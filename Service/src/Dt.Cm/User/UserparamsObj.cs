#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.Cm
{
    #region 自动生成
    [Tbl("cm_userparams")]
    public partial class UserparamsObj : Entity
    {
        #region 构造方法
        UserparamsObj() { }

        public UserparamsObj(
            long UserID,
            string ParamID,
            string Value = default,
            DateTime Mtime = default)
        {
            AddCell<long>("UserID", UserID);
            AddCell<string>("ParamID", ParamID);
            AddCell<string>("Value", Value);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
        #endregion
    }
    #endregion
}
