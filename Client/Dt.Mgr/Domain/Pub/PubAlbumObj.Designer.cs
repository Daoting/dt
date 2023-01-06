#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
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
    public partial class PubAlbumObj : Entity
    {
        #region 构造方法
        PubAlbumObj() { }

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

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PubAlbumObj> GetByID(object p_id)
        {
            return GetByID<PubAlbumObj>(_svcName, p_id);
        }

        const string _svcName = "cm";
    }
}