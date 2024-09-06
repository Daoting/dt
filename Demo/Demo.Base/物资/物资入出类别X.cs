#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public partial class 物资入出类别X
    {
        public static async Task<物资入出类别X> New(
            string 名称 = default,
            short 系数 = default,
            string 单号前缀 = default)
        {
            return new 物资入出类别X(
                ID: await NewSeq("id"),
                名称: 名称,
                系数: 系数,
                单号前缀: 单号前缀);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (名称 == "")
                    Throw.Msg("名称不可为空！", c名称);

                if (单号前缀 == "")
                    Throw.Msg("单号前缀不可为空！", c单号前缀);
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