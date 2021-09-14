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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.File
{
    /// <summary>
    /// 个人文件管理
    /// </summary>
    public class MyFileMgr : IFileMgr
    {
        public long FolderID { get; set; } = -1;

        public string FolderName { get; set; } = "我的文件";

        public FileMgrSetting Setting { get; set; }

        public Task<Table> GetChildren()
        {
            if (FolderID == -1)
                return AtCm.Query("个人文件-根目录", new { userid = Kit.UserID });
            return AtCm.Query("个人文件-所有子级", new { parentid = FolderID });
        }

        public Task<Table> GetChildFolders()
        {
            return AtCm.Query("个人文件-子级文件夹", new { parentid = FolderID });
        }
        
        public Task<Table> GetChildrenByType(string p_typeFilter)
        {
            if (FolderID == -1)
                return AtCm.Query("个人文件-扩展名过滤根目录", new { userid = Kit.UserID, extname = p_typeFilter });
            return AtCm.Query("个人文件-扩展名过滤子级", new { parentid = FolderID, extname = p_typeFilter });
        }

        public Task<Table> SearchFiles(string p_name)
        {
            return AtCm.Query("个人文件-搜索文件", new { name = $"%{p_name}%", userid = Kit.UserID });
        }

        public Task<bool> SaveFile(Row p_row)
        {
            var file = new MyfileObj(
                ID: p_row.ID,
                ParentID: FolderID == -1 ? (long?)null : FolderID,
                Name: p_row.Str("name"),
                IsFolder: false,
                ExtName: p_row.Str("extname"),
                Info: p_row.Str("info"),
                Ctime: p_row.Date("ctime"),
                UserID: Kit.UserID);
            return AtCm.Save(file, false);
        }

        public async Task<bool> SaveFolder(long p_id, string p_name)
        {
            MyfileObj file;
            if (p_id == -1)
            {
                file = new MyfileObj(
                    ID: await AtCm.NewID(),
                    ParentID: FolderID == -1 ? (long?)null : FolderID,
                    Name: p_name,
                    IsFolder: true,
                    Ctime: Kit.Now,
                    UserID: Kit.UserID);
            }
            else
            {
                file = new MyfileObj(ID: p_id);
                file.IsAdded = false;
                file["name"] = p_name;
            }
            return await AtCm.Save(file);
        }

        public async Task<bool> Delete(IEnumerable<Row> p_rows)
        {
            var ls = new List<MyfileObj>();
            foreach (var row in p_rows)
            {
                if (row.Bool("IsFolder"))
                {
                    int cnt = await AtCm.GetScalar<int>("个人文件-子项个数", new { parentid = row.ID });
                    if (cnt > 0)
                    {
                        Kit.Warn($"[{row.Str("name")}]含有下级文件或文件夹，无法删除！");
                        return false;
                    }
                }

                ls.Add(new MyfileObj(ID: row.ID));
            }
            return await AtCm.BatchDelete(ls);
        }

        public Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID)
        {
            var ls = new List<MyfileObj>();
            foreach (var row in p_files)
            {
                var pf = new MyfileObj(ID: row.ID);
                pf.IsAdded = false;
                pf["ParentID"] = p_folderID;
                ls.Add(pf);
            }
            return AtCm.BatchSave(ls, false);
        }
    }
}
