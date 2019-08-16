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
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 基础框架Api代理类（自动生成）
    /// </summary>
    public static partial class AtFw
    {
        #region Fw
        /// <summary>
        /// 获取参数配置，包括模型文件版本号
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetConfig()
        {
            return new UnaryRpc(
                "cm",
                "Fw.GetConfig"
            ).Call<Dict>();
        }

        public static Task<Dict> GetUserInfo()
        {
            return new UnaryRpc(
                "cm",
                "Fw.GetUserInfo"
            ).Call<Dict>();
        }
        #endregion
    }
}
