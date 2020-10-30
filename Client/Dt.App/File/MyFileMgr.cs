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
                return AtCm.Query("个人文件-根目录", new { userid = AtUser.ID });
            return AtCm.Query("个人文件-所有子级", new { parentid = FolderID });
        }

        public Task<Table> GetChildFolders()
        {
            return AtCm.Query("个人文件-子级文件夹", new { parentid = FolderID });
        }
        
        public Task<Table> GetChildrenByType(string p_typeFilter)
        {
            if (FolderID == -1)
                return AtCm.Query("个人文件-扩展名过滤根目录", new { userid = AtUser.ID, extname = p_typeFilter });
            return AtCm.Query("个人文件-扩展名过滤子级", new { parentid = FolderID, extname = p_typeFilter });
        }

        public Task<Table> SearchFiles(string p_name)
        {
            return AtCm.Query("个人文件-搜索文件", new { name = $"%{p_name}%", userid = AtUser.ID });
        }

        public Task<bool> SaveFile(Row p_row)
        {
            var file = new Myfile(
                ID: p_row.ID,
                ParentID: FolderID == -1 ? (long?)null : FolderID,
                Name: p_row.Str("name"),
                IsFolder: false,
                ExtName: p_row.Str("extname"),
                Info: p_row.Str("info"),
                Ctime: p_row.Date("ctime"),
                UserID: AtUser.ID);
            return new Repo<Myfile>().Save(file, false);
        }

        public async Task<bool> SaveFolder(long p_id, string p_name)
        {
            Myfile file;
            if (p_id == -1)
            {
                file = new Myfile(
                    ID: await AtCm.NewFlagID(0),
                    ParentID: FolderID == -1 ? (long?)null : FolderID,
                    Name: p_name,
                    IsFolder: true,
                    Ctime: AtSys.Now,
                    UserID: AtUser.ID);
            }
            else
            {
                file = new Myfile(ID: p_id);
                file.IsAdded = false;
                file["name"] = p_name;
            }
            return await new Repo<Myfile>().Save(file);
        }

        public async Task<bool> Delete(IEnumerable<Row> p_rows)
        {
            var ls = new List<Myfile>();
            foreach (var row in p_rows)
            {
                if (row.Bool("IsFolder"))
                {
                    int cnt = await AtCm.GetScalar<int>("个人文件-子项个数", new { parentid = row.ID });
                    if (cnt > 0)
                    {
                        AtKit.Warn($"[{row.Str("name")}]含有下级文件或文件夹，无法删除！");
                        return false;
                    }
                }

                ls.Add(new Myfile(ID: row.ID));
            }
            return await new Repo<Myfile>().BatchDelete(ls);
        }

        public Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID)
        {
            var ls = new List<Myfile>();
            foreach (var row in p_files)
            {
                var pf = new Myfile(ID: row.ID);
                pf.IsAdded = false;
                pf["ParentID"] = p_folderID;
                ls.Add(pf);
            }
            return new Repo<Myfile>().BatchSave(ls, false);
        }
    }
}
