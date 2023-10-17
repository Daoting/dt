#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo
{
    public partial class 扩展1X
    {
        public static async Task<扩展1X> New(
            string 扩展1名称 = default,
            bool 禁止选中 = default,
            bool 禁止保存 = default)
        {
            return new 扩展1X(
                ID: await NewID(),
                扩展1名称: 扩展1名称,
                禁止选中: 禁止选中,
                禁止保存: 禁止保存);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (禁止保存)
                {
                    Throw.Msg("已选中[禁止保存]，保存前校验不通过！");
                }
                return Task.CompletedTask;
            });

            //OnDeleting(() =>
            //{

            //    return Task.CompletedTask;
            //});

            OnChanging(c禁止选中, e =>
            {
                Throw.If(e.Bool, "[禁止选中]列无法选中");
            });
        }
    }
}