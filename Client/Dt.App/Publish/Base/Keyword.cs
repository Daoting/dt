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
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    public partial class Keyword
    {
        void OnSaving()
        {
            Throw.IfNullOrEmpty(ID, "关键字不可为空！");
        }
    }

    #region 自动生成
    [Tbl("pub_keyword")]
    public partial class Keyword : Entity
    {
        #region 构造方法
        Keyword() { }

        public Keyword(
            string ID,
            string Creator = default,
            DateTime Ctime = default)
        {
            AddCell<string>("ID", ID);
            AddCell<string>("Creator", Creator);
            AddCell<DateTime>("Ctime", Ctime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 关键字
        /// </summary>
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return (string)this["Creator"]; }
            set { this["Creator"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }
        #endregion
    }
    #endregion

    #region 可复制
    /*
    public partial class Keyword
    {
        async Task OnSaving()
        {
        }
    }

        async Task OnDeleting()
        {
        }

        public static async Task<Keyword> New()
        {
        }

        public static async Task<Keyword> Get(long p_id)
        {
        }

        void SetID(string p_value)
        {
        }

        void SetCreator(string p_value)
        {
        }

        void SetCtime(DateTime p_value)
        {
        }
    */
    #endregion
}
