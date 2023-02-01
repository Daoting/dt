#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Domain
{
    public partial class 收文X
    {
        public static async Task<收文X> New(
            string 来文单位 = default,
            DateTime 来文时间 = default,
            密级 密级 = default,
            string 文件标题 = default,
            string 文件附件 = default,
            string 市场部经理意见 = default,
            string 综合部经理意见 = default,
            DateTime 收文完成时间 = default)
        {
            return new 收文X(
                ID: await NewID(),
                来文单位: 来文单位,
                来文时间: 来文时间,
                密级: 密级,
                文件标题: 文件标题,
                文件附件: 文件附件,
                市场部经理意见: 市场部经理意见,
                综合部经理意见: 综合部经理意见,
                收文完成时间: 收文完成时间);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                Throw.IfEmpty(文件标题, "文件标题不可为空");
                return Task.CompletedTask;
            });
        }
    }

    public enum 密级
    {
        普通,
        内部,
        绝密
    }
}