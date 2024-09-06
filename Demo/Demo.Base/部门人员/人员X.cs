#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [View1("v_人员")]
    public partial class 人员X
    {
        public static async Task<人员X> New(
            string 姓名 = default,
            DateTime? 出生日期 = default,
            Gender? 性别 = default,
            DateTime? 工作日期 = default,
            string 办公室电话 = default,
            string 电子邮件 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default,
            string 撤档原因 = default,
            long? UserID = default)
        {
            var x = new 人员X(
                ID: await NewID(),
                姓名: 姓名,
                出生日期: 出生日期,
                性别: 性别,
                工作日期: 工作日期,
                办公室电话: 办公室电话,
                电子邮件: 电子邮件,
                建档时间: 建档时间,
                撤档时间: 撤档时间,
                撤档原因: 撤档原因,
                UserID: UserID);

            x.Add("账号", "");
            return x;
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (姓名 == "")
                    Throw.Msg("姓名不可为空！", c姓名);

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