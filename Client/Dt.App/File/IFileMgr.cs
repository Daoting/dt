#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 收藏菜单项
    /// </summary>
    public interface IFileMgr
    {
        long FolderID { get; set; }

        string FolderName { get; set; }

        bool AllowEdit { get; }
        Task<Table> GetFiles();

        Task<bool> SaveFolder(long p_id, string p_name);

        Task<bool> SaveFile(Row p_row);

        Task<bool> Delete(Row p_row);
    }
}
