namespace Dt.Agent
{
    /// <summary>
    /// 文件服务Api
    /// </summary>
    public partial class AtFsm
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public static Task<bool> Exists(string p_filePath)
        {
            return Kit.Rpc<bool>(
                "fsm",
                "FileMgr.Exists",
                p_filePath
            );
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public static Task<bool> Delete(string p_filePath)
        {
            return Kit.Rpc<bool>(
                "fsm",
                "FileMgr.Delete",
                p_filePath
            );
        }
    }
}
