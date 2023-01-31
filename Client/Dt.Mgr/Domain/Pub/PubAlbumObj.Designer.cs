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

namespace Dt.Mgr.Domain
{
    [Tbl("cm_pub_album")]
    public partial class PubAlbumObj : EntityX<PubAlbumObj>
    {
        #region 构造方法
        PubAlbumObj() { }

        public PubAlbumObj(CellList p_cells) : base(p_cells) { }

        public PubAlbumObj(
            long ID,
            string Name = default,
            string Creator = default,
            DateTime Ctime = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Creator", Creator);
            AddCell("Ctime", Ctime);
            IsAdded = true;
        }
        #endregion

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
    }
}