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

namespace Dt.MgrDemo.单实体
{
    public partial class 主表X
    {
        public static async Task<主表X> New(
            string 主表名称 = default,
            string 限长4 = default,
            string 不重复 = default)
        {
            return new 主表X(
                ID: await NewID(),
                主表名称: 主表名称,
                限长4: 限长4,
                不重复: 不重复);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                if (c不重复.IsChanged)
                {
                    int cnt = await AtSvc.GetScalar<int>($"select count(1) from demo_主表 where 不重复='{不重复}' and ID!={ID}");
                    if (cnt > 0)
                    {
                        Throw.Msg("[不重复]列存在重复值！");
                    }
                }
            });

            //OnDeleting(() =>
            //{

            //    return Task.CompletedTask;
            //});

            OnChanging(c限长4, e =>
            {
                Throw.If(e.Utf8Length > 4, "超出最大长度4");
            });
        }
    }
}