#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    public static partial class SvcHost
    {
        /// <summary>
        /// 启动服务，任一环节失败即启动失败
        /// + 确定基础路径
        /// + 创建日志对象
        /// + 读取配置
        /// + 整理服务存根
        /// + 启动http服务器
        /// 此方法不可异步，否则启动有问题！！！
        /// </summary>
        /// <param name="p_args">命令行参数，基础路径，空时dll所在路径为基础路径</param>
        /// <param name="p_stubs">可用的服务存根对象，不包括系统内置服务(cm da fsm msg)</param>
        public static void Run(string[] p_args, Dictionary<string, Stub> p_stubs = null)
        {
            if (p_stubs == null)
                p_stubs = new Dictionary<string, Stub>();

            // 系统内置服务
            p_stubs["cm"] = new Dt.Cm.SvcStub();
            p_stubs["da"] = new Dt.Da.SvcStub();
            p_stubs["fsm"] = new Dt.Fsm.SvcStub();
            p_stubs["msg"] = new Dt.Msg.SvcStub();
            Launcher.Run(p_args, p_stubs);
        }
    }
}