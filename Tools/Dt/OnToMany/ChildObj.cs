#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
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

namespace $rootnamespace$
{
	public partial class $childcls$Obj
	{
		//async Task OnSaving()
		//{
		//}

		//async Task OnDeleting()
		//{
		//}
	}

    #region 自动生成
    [Tbl("cm_$childcls$")]
    public partial class $childcls$Obj: Entity
    {
        $childcls$Obj() { }

        public $childcls$Obj(long ID)
        {
            AddCell("ID", ID);
            AttachHook();
        }
    }
    #endregion
}
