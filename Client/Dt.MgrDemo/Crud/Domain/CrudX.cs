#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    public partial class CrudX
    {
        public static async Task<CrudX> New(
            string Name = default,
            int Dispidx = default,
            DateTime Mtime = default)
        {
            return new CrudX(
                ID: await NewID(),
                Name: Name,
                Dispidx: Dispidx,
                Mtime: Mtime);
        }

        protected override void InitHook()
        {
        }
    }
}