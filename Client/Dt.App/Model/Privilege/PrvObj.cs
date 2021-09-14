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
    public partial class PrvObj
    {
        async Task OnSaving()
        {
            Throw.IfNullOrEmpty(ID, "权限名称不可为空！");

            if ((IsAdded || Cells["id"].IsChanged)
                && await AtCm.GetScalar<int>("权限-名称重复", new { id = ID }) > 0)
            {
                Throw.Msg("权限名称重复！");
            }
        }
    }

    #region 自动生成
    [Tbl("cm_prv")]
    public partial class PrvObj : Entity
    {
        #region 构造方法
        PrvObj() { }

        public PrvObj(
            string ID,
            string Note = default)
        {
            AddCell<string>("ID", ID);
            AddCell<string>("Note", Note);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
        #endregion
    }
    #endregion
}
