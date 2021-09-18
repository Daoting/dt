#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample.ModuleView.OneToMany2
{
	public partial class GoodsObj
	{
		//async Task OnSaving()
		//{
		//}

		//async Task OnDeleting()
		//{
		//}
	}

    #region 自动生成
    [Tbl("oa_goods")]
    public partial class GoodsObj : Entity
    {
        #region 构造方法
        GoodsObj() { }

        public GoodsObj(
            long ID,
            long ParentID = default,
            string Name = default)
        {
            AddCell("ID", ID);
            AddCell("ParentID", ParentID);
            AddCell("Name", Name);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 所属购物
        /// </summary>
        public long ParentID
        {
            get { return (long)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }
        #endregion
    }
    #endregion
}
