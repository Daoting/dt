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
    public partial class AlbumObj
    {
        Task OnSaving()
        {
            Throw.IfNullOrEmpty(Name, "名称不可为空！");
            return Task.CompletedTask;
        }
    }

    #region 自动生成
    [Tbl("pub_album")]
    public partial class AlbumObj : Entity
    {
        #region 构造方法
        AlbumObj() { }

        public AlbumObj(
            long ID,
            string Name = default,
            string Creator = default,
            DateTime Ctime = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Creator", Creator);
            AddCell<DateTime>("Ctime", Ctime);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
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
}
