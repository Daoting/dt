namespace Dt.Mgr
{
    /// <summary>
    /// 系统内核Api
    /// </summary>
    public partial class AtCm
    {
        /// <summary>
        /// 获取参数配置，包括服务器时间、所有服务地址、sqlite文件版本号
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetConfig()
        {
            return Kit.Rpc<Dict>(
                "cm",
                "SysKernel.GetConfig"
            );
        }

        /// <summary>
        /// 更新服务端所有sqlite文件，包括sqlite.json中定义的所有sqlite文件，异步处理
        /// </summary>
        /// <returns></returns>
        public static Task<string> UpdateAllSqliteFile()
        {
            return Kit.Rpc<string>(
                "cm",
                "SysKernel.UpdateAllSqliteFile"
            );
        }

        /// <summary>
        /// 更新服务端单个sqlite文件，确保文件名已在sqlite.json中定义
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <returns></returns>
        public static Task<string> UpdateSqliteFile(string p_fileName)
        {
            return Kit.Rpc<string>(
                "cm",
                "SysKernel.UpdateSqliteFile",
                p_fileName
            );
        }

        /// <summary>
        /// 获取所有sqlite文件名
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetAllSqliteFile()
        {
            return Kit.Rpc<List<string>>(
                "cm",
                "SysKernel.GetAllSqliteFile"
            );
        }
    }
}
