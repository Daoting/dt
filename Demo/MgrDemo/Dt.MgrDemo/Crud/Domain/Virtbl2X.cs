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

namespace Dt.MgrDemo.Crud
{
    public partial class Virtbl2X
    {
        public static async Task<Virtbl2X> New(
            string Name2 = default)
        {
            return new Virtbl2X(
                ID: await NewID(),
                Name2: Name2);
        }

        protected override void InitHook()
        {
        }
    }
}