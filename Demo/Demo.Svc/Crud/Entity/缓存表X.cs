#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    public partial class 缓存表X
    {
        public static async Task<缓存表X> New(
            string 手机号 = default,
            string 姓名 = default)
        {
            return new 缓存表X(
                ID: await NewID(),
                手机号: 手机号,
                姓名: 姓名);
        }

        protected override void InitHook()
        {
            OnSaved(async () => await this.ClearCache(c手机号.ID));

            OnDeleted(async () => await this.ClearCache(c手机号.ID));
        }
    }
}