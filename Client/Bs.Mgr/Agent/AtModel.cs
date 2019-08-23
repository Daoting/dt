#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 模型相关Api代理类
    /// </summary>
    public static class AtModel
    {
        /// <summary>
        /// 刷新模型
        /// 1. 刷新表结构缓存
        /// 2. 重新生成sqlite模型文件
        /// </summary>
        public static async void UpdateModel()
        {
            if (await AtKit.Confirm("确认要刷新模型吗？"))
            {
                Dict dt = await new UnaryRpc("Ac", "SrvMgr.刷新模型").Call<Dict>();
                if ((bool)dt["suc"])
                    AtKit.Msg("刷新模型成功！");
                else
                    AtKit.Warn((string)dt["msg"]);
            }
        }
    }
}
