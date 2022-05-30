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
        public static Task<bool> IsFileExists(string p_filePath)
        {
            return Kit.Rpc<bool>(
                "fsm",
                "FileMgr.IsFileExists",
                p_filePath
            );
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public static Task<bool> DeleteFile(string p_filePath)
        {
            return Kit.Rpc<bool>(
                "fsm",
                "FileMgr.DeleteFile",
                p_filePath
            );
        }

        /// <summary>
        /// 保存文本内容的文件
        /// </summary>
        /// <param name="p_filePath">文件路径</param>
        /// <param name="p_content">文件内容</param>
        /// <returns>null 保存成功</returns>
        public static Task<string> SaveFile(string p_filePath, string p_content)
        {
            return Kit.Rpc<string>(
                "fsm",
                "FileMgr.SaveFile",
                p_filePath,
                p_content
            );
        }
    }
}
