#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.UIDemo
{
    [Sqlite("rptdemo")]
    public partial class OmReportX : EntityX<OmReportX>
    {
        #region 构造方法
        OmReportX() { }

        public OmReportX(CellList p_cells) : base(p_cells) { }

        public OmReportX(
            long ID,
            string Name = default,
            string Define = default)
        {
            Add("ID", ID);
            Add("Name", Name);
            Add("Define", Define);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 主键标识
        /// </summary>
        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        public string Define
        {
            get { return (string)this["Define"]; }
            set { this["Define"] = value; }
        }
    }
}