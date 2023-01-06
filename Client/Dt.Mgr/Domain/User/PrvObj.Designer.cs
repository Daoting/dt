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
    [Tbl("cm_prv")]
    public partial class PrvObj : Entity
    {
        #region 构造方法
        PrvObj() { }

        public PrvObj(
            string ID,
            string Note = default)
        {
            AddCell("ID", ID);
            AddCell("Note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 权限名称
        /// </summary>
        new public string ID
        {
            get { return (string)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，不存在时返回null
        /// </summary>
        /// <param name="p_id">主键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<PrvObj> GetByID(object p_id)
        {
            return GetByID<PrvObj>(_svcName, p_id);
        }

        const string _svcName = "cm";
    }
}