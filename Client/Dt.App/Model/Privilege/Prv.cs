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
#endregion

namespace Dt.App.Model
{
    #region 自动生成
    [Tbl("cm_prv")]
    public partial class Prv : Entity
    {
        #region 构造方法
        Prv() { }

        public Prv(
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

        #region 可复制
        /*
        void OnSaving()
        {
        }

        void OnDeleting()
        {
        }

        void SetID(string p_value)
        {
        }

        void SetNote(string p_value)
        {
        }
        */
        #endregion
    }
    #endregion
}
