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
	public partial class ShoppingObj
	{
		//async Task OnSaving()
		//{
		//}

		//async Task OnDeleting()
		//{
		//}
	}

    #region 自动生成
    [Tbl("oa_shopping")]
    public partial class ShoppingObj : Entity
    {
        #region 构造方法
        ShoppingObj() { }

        public ShoppingObj(
            long ID,
            string Reason = default)
        {
            AddCell("ID", ID);
            AddCell("Reason", Reason);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 事由
        /// </summary>
        public string Reason
        {
            get { return (string)this["Reason"]; }
            set { this["Reason"] = value; }
        }
        #endregion
    }
    #endregion
}
