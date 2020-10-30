#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 文件管理接口
    /// </summary>
    public interface IFileMgr
    {
        /// <summary>
        /// 当前文件夹标识
        /// </summary>
        long FolderID { get; set; }

        /// <summary>
        /// 当前文件夹
        /// </summary>
        string FolderName { get; set; }

        /// <summary>
        /// 参数设置
        /// </summary>
        FileMgrSetting Setting { get; set; }

        /// <summary>
        /// 获取所有子项
        /// </summary>
        /// <returns></returns>
        Task<Table> GetChildren();

        /// <summary>
        /// 获取子文件夹
        /// </summary>
        /// <returns></returns>
        Task<Table> GetChildFolders();

        /// <summary>
        /// 获取所有子文件夹和指定扩展名的文件
        /// </summary>
        /// <param name="p_typeFilter">文件扩展名，逗号隔开</param>
        /// <returns></returns>
        Task<Table> GetChildrenByType(string p_typeFilter);

        /// <summary>
        /// 查询文件
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        Task<Table> SearchFiles(string p_name);

        /// <summary>
        /// 保存文件夹
        /// </summary>
        /// <param name="p_id"></param>
        /// <param name="p_name"></param>
        /// <returns></returns>
        Task<bool> SaveFolder(long p_id, string p_name);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="p_row"></param>
        /// <returns></returns>
        Task<bool> SaveFile(Row p_row);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        Task<bool> Delete(IEnumerable<Row> p_rows);

        /// <summary>
        /// 移到
        /// </summary>
        /// <param name="p_files"></param>
        /// <param name="p_folderID">目标文件夹</param>
        /// <returns></returns>
        Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID);
    }
}
