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
    public partial class CrudObj
    {
        public static async Task<CrudObj> New(
            string Name = default,
            int Dispidx = default,
            DateTime Mtime = default)
        {
            return new CrudObj(
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