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
    [Tbl("cm_prv", "cm")]
    public partial class Prv : Entity
    {
        public Prv()
        { }

        public Prv(
            string ID,
            string Note = default)
        {
            AddCell<string>("ID", ID);
            AddCell<string>("Note", Note);
            IsAdded = true;
        }

        /// <summary>
        /// 权限名称
        /// </summary>
        new public string ID
        {
            get { return (string)_cells["ID"].Val; }
            private set { _cells["ID"].Val = value; }
        }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Note
        {
            get { return (string)_cells["Note"].Val; }
            private set { _cells["Note"].Val = value; }
        }
    }
}
