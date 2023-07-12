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

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 个人文件管理
    /// </summary>
    public class MyFileMgr : IFileMgr
    {
        public long FolderID { get; set; } = -1;

        public string FolderName { get; set; } = "我的文件";

        public FileMgrSetting Setting { get; set; }

        public async Task<Table> GetChildren()
        {
            return await FileMyX.GetChildren(FolderID);
        }

        public async Task<Table> GetChildFolders()
        {
            return await FileMyX.GetChildFolders(FolderID);
        }
        
        public async Task<Table> GetChildrenByType(string p_typeFilter)
        {
            return await FileMyX.GetChildrenByType(FolderID, p_typeFilter);
        }

        public async Task<Table> SearchFiles(string p_name)
        {
            return await FileMyX.SearchFiles(p_name);
        }

        public Task<bool> SaveFile(Row p_row)
        {
            var file = new FileMyX(
                ID: p_row.ID,
                ParentID: FolderID == -1 ? (long?)null : FolderID,
                Name: p_row.Str("name"),
                IsFolder: false,
                ExtName: p_row.Str("ext_name"),
                Info: p_row.Str("info"),
                Ctime: p_row.Date("ctime"),
                UserID: Kit.UserID);
            return file.Save(false);
        }

        public async Task<bool> SaveFolder(long p_id, string p_name)
        {
            FileMyX file;
            if (p_id == -1)
            {
                file = await FileMyX.New(
                    ParentID: FolderID == -1 ? (long?)null : FolderID,
                    Name: p_name,
                    IsFolder: true,
                    Ctime: Kit.Now,
                    UserID: Kit.UserID);
            }
            else
            {
                file = new FileMyX(ID: p_id);
                file.IsAdded = false;
                file["name"] = p_name;
            }
            return await file.Save();
        }

        public async Task<bool> Delete(IEnumerable<Row> p_rows)
        {
            var ls = new List<FileMyX>();
            foreach (var row in p_rows)
            {
                if (row.Bool("is_folder"))
                {
                    int cnt = await FileMyX.GetCount($"where parent_id={row.ID}");
                    if (cnt > 0)
                    {
                        Kit.Warn($"[{row.Str("name")}]含有下级文件或文件夹，无法删除！");
                        return false;
                    }
                }

                ls.Add(new FileMyX(ID: row.ID));
            }
            return await ls.Delete();
        }

        public Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID)
        {
            var ls = new List<FileMyX>();
            foreach (var row in p_files)
            {
                var pf = new FileMyX(ID: row.ID);
                pf.IsAdded = false;
                pf.ParentID = p_folderID;
                ls.Add(pf);
            }
            return ls.Save(false);
        }
    }
}
