#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public partial class RptObj
    {
        async Task OnSaving()
        {
            Throw.IfNullOrEmpty(Name, "报表名称不可为空！");

            if ((IsAdded || Cells["name"].IsChanged)
                && await AtCm.GetScalar<int>("报表-重复名称", new { name = Name }) > 0)
            {
                Throw.Msg("报表名称重复！");
            }

            if (IsAdded)
            {
                Ctime = Mtime = Kit.Now;
            }
            else
            {
                Mtime = Kit.Now;
            }
        }
    }

    #region 自动生成
    [Tbl("cm_rpt")]
    public partial class RptObj : Entity
    {
        #region 构造方法
        RptObj() { }

        public RptObj(
            long ID,
            string Name = default,
            string Define = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Define", Define);
            AddCell<string>("Note", Note);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 报表名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 报表模板定义
        /// </summary>
        public string Define
        {
            get { return (string)this["Define"]; }
            set { this["Define"] = value; }
        }

        /// <summary>
        /// 报表描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
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
