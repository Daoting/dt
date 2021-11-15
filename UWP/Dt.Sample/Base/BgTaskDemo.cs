#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 后台任务处理基类，除 AtState、Stub、UnaryRpc、Kit.Toast 外，不可使用任何UI和外部变量，保证可独立运行！！！
    /// </summary>
    public class BgTaskDemo : BgTask
    {
        //public override async Task Run()
        //{
        //    string tpName = AtState.GetCookie("LoginPhone");
        //    var cfg = await new UnaryRpc("cm", "ModelMgr.GetConfig").Call<Dict>();
        //    await Login();
        //    Kit.Toast(
        //        "样例",
        //        tpName + "\r\n" + cfg.Date("now").ToString(),
        //        new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });
        //}

        public override Task Run()
        {
            return Task.CompletedTask;
        }
    }

}
