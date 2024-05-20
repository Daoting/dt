#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Demo.Crud
{
    public partial class CacheTbl1X
    {
        public static async Task<CacheTbl1X> New(
            string Phone = default,
            string Name = default)
        {
            return new CacheTbl1X(
                ID: await NewID(),
                Phone: Phone,
                Name: Name);
        }

        protected override void InitHook()
        {
            OnSaved(async () => await this.ClearCache(nameof(Phone)));

            OnDeleted(async () => await this.ClearCache(nameof(Phone)));
        }
    }
}