#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Cm
{
    [Tbl("cm_role", "cm")]
    public partial class Role : Entity
    {
        public Role()
        { }

        public Role(
            long ID,
            string Name = default,
            string Note = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Note", Note);
            IsAdded = true;
        }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return (string)_cells["Name"].Val; }
            private set { _cells["Name"].Val = value; }
        }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Note
        {
            get { return (string)_cells["Note"].Val; }
            private set { _cells["Note"].Val = value; }
        }
    }
}
