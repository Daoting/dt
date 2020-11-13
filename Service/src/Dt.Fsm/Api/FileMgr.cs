#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
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
        /// 判断文件是否存在
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public bool Exists(string p_filePath)
        {
            return File.Exists(Path.Combine(Cfg.Root, p_filePath));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
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

                // 删除缩略图
                int pt = p_filePath.LastIndexOf('.');
                if (pt > 0)
                {
                    string ext = p_filePath.Substring(pt);
                    if (Cfg.IsImage(ext) || Cfg.IsVideo(ext))
                    {
                        fi = new FileInfo(Path.Combine(Cfg.Root, p_filePath.Substring(0, pt) + Cfg.ThumbPostfix));
                        if (fi.Exists)
                        {
                            try
                            {
                                fi.Delete();
                            }
                            catch { }
                        }
                    }
                }
            }
            await _dp.Exec($"delete from fsm_file where path='{p_filePath}'");
            return true;
        }
    }
}
