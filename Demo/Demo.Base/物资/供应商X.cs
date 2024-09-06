#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 供应商X
    {
        public static async Task<供应商X> New(
            string 名称 = default,
            string 执照号 = default,
            DateTime? 执照效期 = default,
            string 税务登记号 = default,
            string 地址 = default,
            string 电话 = default,
            string 开户银行 = default,
            string 帐号 = default,
            string 联系人 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default,
            string 备注 = default)
        {
            return new 供应商X(
                ID: await NewID(),
                名称: 名称,
                执照号: 执照号,
                执照效期: 执照效期,
                税务登记号: 税务登记号,
                地址: 地址,
                电话: 电话,
                开户银行: 开户银行,
                帐号: 帐号,
                联系人: 联系人,
                建档时间: 建档时间,
                撤档时间: 撤档时间,
                备注: 备注);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (名称 == "")
                    Throw.Msg("名称不可为空！", c名称);

                if (IsAdded)
                {
                    建档时间 = Kit.Now;
                }
                return Task.CompletedTask;
            });

            //OnSaved(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnDeleting(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnDeleted(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnChanging(cName, e =>
            //{

            //});
        }

        #region Sql

        #endregion
    }
}