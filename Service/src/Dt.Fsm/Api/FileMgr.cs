﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Fsm
{
    /// <summary>
    /// 文件服务Api
    /// </summary>
    [Api]
    public class FileMgr : DomainSvc
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public bool IsFileExists(string p_filePath)
        {
            return File.Exists(Path.Combine(Cfg.Root, p_filePath));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="p_filePath">文件ID：卷名/两级目录/xxx.ext</param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string p_filePath)
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
            await _da.Exec($"delete from fsm_file where path='{p_filePath}'");
            return true;
        }

        /// <summary>
        /// 保存文本内容的文件
        /// </summary>
        /// <param name="p_filePath">文件路径</param>
        /// <param name="p_content">文件内容</param>
        /// <returns>null 保存成功</returns>
        public async Task<string> SaveFile(string p_filePath, string p_content)
        {
            FileInfo fi = new FileInfo(Path.Combine(Cfg.Root, p_filePath));
            if (fi.Exists)
                return "文件已存在！";

            try
            {
                if (!fi.Directory.Exists)
                    fi.Directory.Create();

                // 未记录在上传文件表中，fsm_file表
                using (var stream = fi.Create())
                using (var sw = new StreamWriter(stream))
                {
                    await sw.WriteAsync(p_content);
                    await sw.FlushAsync();
                }
            }
            catch
            {
                return "文件保存出错！";
            }
            return null;
        }

        /// <summary>
        /// 获取windows应用的版本信息
        /// </summary>
        /// <returns></returns>
        public Dict GetWinAppVer()
        {
            return MsixCfg.WinAppVer;
        }
    }
}
