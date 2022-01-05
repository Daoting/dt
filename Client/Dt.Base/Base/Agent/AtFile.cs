#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 文件服务Api代理类（自动生成）
    /// </summary>
    public static class AtFile
    {
        #region FileMgr
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
        #endregion
    }
}