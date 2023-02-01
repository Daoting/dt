#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.单实体
{
    public partial class 基础X
    {
        public static async Task<基础X> New(
            string 限长4 = default,
            string 不重复 = default,
            bool 禁止选中 = default,
            int 校验后台列 = default,
            bool 禁止保存 = default,
            bool 禁止删除 = default,
            string 值变事件 = default,
            DateTime 创建时间 = default,
            DateTime 修改时间 = default)
        {
            return new 基础X(
                ID: await NewID(),
                序列: await NewSeq("序列"),
                限长4: 限长4,
                不重复: 不重复,
                禁止选中: 禁止选中,
                校验后台列: 校验后台列,
                禁止保存: 禁止保存,
                禁止删除: 禁止删除,
                值变事件: 值变事件,
                创建时间: 创建时间,
                修改时间: 修改时间);
        }

        protected override void InitHook()
        {
        }
    }
}