#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 文件服务Api
    /// </summary>
    [Api]
    public class FileMgr : BaseApi
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string p_filePath)
        {
            if (string.IsNullOrEmpty(p_filePath))
                return false;

            FileInfo fi = new FileInfo(Path.Combine(Cfg.Root, p_filePath));
            if (fi.Exists)
            {
                try
                {
                    fi.Delete();
                }
                catch
                {
                    return false;
                }
            }
            await _c.Db.Exec($"delete from fsm_file where path='{p_filePath}'");
            return true;
        }
    }
}
