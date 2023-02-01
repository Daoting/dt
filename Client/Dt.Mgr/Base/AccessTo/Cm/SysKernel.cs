﻿namespace Dt.Mgr
{
    /// <summary>
    /// 系统内核Api
    /// </summary>
    public partial class AtCm
    {
        /// <summary>
        /// 获取参数配置，包括服务器时间、所有服务地址、模型文件版本号
        /// </summary>
        /// <returns></returns>
        public static Task<List<object>> GetConfig()
        {
            return Kit.Rpc<List<object>>(
                "cm",
                "SysKernel.GetConfig"
            );
        }

        /// <summary>
        /// 更新服务端表结构缓存和sqlite模型库文件
        /// </summary>
        /// <returns></returns>
        public static Task<bool> UpdateModel()
        {
            return Kit.Rpc<bool>(
                "cm",
                "SysKernel.UpdateModel"
            );
        }
    }
}