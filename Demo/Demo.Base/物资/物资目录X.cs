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
    [View1("v_物资目录")]
    public partial class 物资目录X
    {
        public static async Task<物资目录X> New(
            long? 分类id = default,
            string 名称 = default,
            string 规格 = default,
            string 产地 = default,
            float? 成本价 = default,
            物资核算方式? 核算方式 = default,
            int? 摊销月数 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default)
        {
            string name = "";
            if (分类id > 0)
            {
                name = await 物资分类X.GetScalar<string>("名称", "where id=" + 分类id);
            }
            else
            {
                var fl = await 物资分类X.First("where 1=1 order by id");
                if (fl != null)
                {
                    分类id = fl.ID;
                    name = fl.名称;
                }
            }
            
            var x = new 物资目录X(
                ID: await NewID(),
                分类id: 分类id,
                名称: 名称,
                规格: 规格,
                产地: 产地,
                成本价: 成本价,
                核算方式: 核算方式,
                摊销月数: 摊销月数,
                建档时间: 建档时间,
                撤档时间: 撤档时间);
            x.Add("物资分类", name);
            return x;
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (分类id == null)
                    Throw.Msg("物资分类不可为空！", _cells.TryGet("物资分类"));

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