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
    public partial class ParamsObj
    {
        async Task OnSaving()
        {
            Throw.IfNullOrEmpty(ID, "参数名不可为空！");

            if ((IsAdded || Cells["ID"].IsChanged)
                && await AtCm.GetScalar<int>("参数-重复名称", new { id = ID }) > 0)
            {
                Throw.Msg("参数名重复！");
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
    [Tbl("cm_params")]
    public partial class ParamsObj : Entity
    {
        #region 构造方法
        ParamsObj() { }

        public ParamsObj(
            string ID,
            string Value = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            AddCell<string>("ID", ID);
            AddCell<string>("Value", Value);
            AddCell<string>("Note", Note);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<DateTime>("Mtime", Mtime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 用户参数标识
        /// </summary>
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 参数缺省值
        /// </summary>
        public string Value
        {
            get { return (string)this["Value"]; }
            set { this["Value"] = value; }
        }

        /// <summary>
        /// 参数描述
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
