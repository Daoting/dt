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
    public partial class RoleObj
    {
        async Task OnSaving()
        {
            Throw.IfNullOrEmpty(Name, "角色名称不可为空！");

            if ((IsAdded || Cells["name"].IsChanged)
                && await AtCm.GetScalar<int>("角色-名称重复", new { name = Name }) > 0)
            {
                Throw.Msg("角色名称重复！");
            }
        }
    }

    #region 自动生成
    [Tbl("cm_role")]
    public partial class RoleObj : Entity
    {
        #region 构造方法
        RoleObj() { }

        public RoleObj(
            long ID,
            string Name = default,
            string Note = default)
        {
            AddCell("ID", ID);
            AddCell("Name", Name);
            AddCell("Note", Note);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string Note
        {
            get { return (string)this["Note"]; }
            set { this["Note"] = value; }
        }
        #endregion
    }
    #endregion
}
