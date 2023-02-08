#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.CrudSqlite
{
    public partial class Virtbl3X
    {
        public static async Task<Virtbl3X> New(
            string Name3 = default)
        {
            return new Virtbl3X(
                ID: await NewID(),
                Name3: Name3);
        }

        protected override void InitHook()
        {
        }
    }
}