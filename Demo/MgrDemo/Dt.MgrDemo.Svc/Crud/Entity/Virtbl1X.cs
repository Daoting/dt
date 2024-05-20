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

namespace Demo.Crud
{
    public partial class Virtbl1X
    {
        public static async Task<Virtbl1X> New(
            string Name1 = default)
        {
            return new Virtbl1X(
                ID: await NewID(),
                Name1: Name1);
        }

        protected override void InitHook()
        {
        }
    }
}