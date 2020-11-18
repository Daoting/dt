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
    #region 自动生成
    [Tbl("cm_role")]
    public partial class Role : Entity
    {
        #region 构造方法
        Role() { }

        public Role(
            long ID,
            string Name = default,
            string Note = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Name", Name);
            AddCell<string>("Note", Note);
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

    #region 可复制
    /*
    public partial class Role
    {
        async Task OnSaving()
        {
        }
    }

        async Task OnDeleting()
        {
        }

        public static async Task<Role> New()
        {
        }

        public static async Task<Role> Get(long p_id)
        {
        }

        void SetID(long p_value)
        {
        }

        void SetName(string p_value)
        {
        }

        void SetNote(string p_value)
        {
        }
    */
    #endregion
}
