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
    /// 公共文件管理
    /// </summary>
    public class PubFileMgr : IFileMgr
    {
        long _folderID = -1;
        string _folderName;
        protected long _rootFolderID = 1;
        protected string _rootFolderName = "公共文件";

        public long FolderID
        {
            get
            {
                if (_folderID == -1)
                    return _rootFolderID;
                return _folderID;
            }
            set { _folderID = value; }
        }

        public string FolderName
        {
            get
            {
                if (string.IsNullOrEmpty(_folderName))
                    return _rootFolderName;
                return _folderName;
            }
            set { _folderName = value; }
        }

        public bool AllowEdit
        {
            get { return AtUser.HasPrv("公共文件管理"); }
        }

        public Task<Table> GetChildren()
        {
            return AtCm.Query("文件-所有子级", new { parentid = FolderID });
        }

        public Task<Table> GetChildFolders()
        {
            return AtCm.Query("文件-子级文件夹", new { parentid = FolderID });
        }

        public Task<Table> SearchFiles(string p_name)
        {
            return AtCm.Query("文件-搜索文件", new { name = $"%{p_name}%" });
        }

        public Task<bool> SaveFile(Row p_row)
        {
            Pubfile pf = new Pubfile(
                    ID: p_row.ID,
                    ParentID: FolderID,
                    Name: p_row.Str("name"),
                    IsFolder: false,
                    Info: p_row.Str("info"),
                    Ctime: p_row.Date("ctime"));
            return new Repo<Pubfile>().Save(pf, false);
        }

        public async Task<bool> SaveFolder(long p_id, string p_name)
        {
            Pubfile pf;
            if (p_id == -1)
            {
                pf = new Pubfile(
                    ID: await AtCm.NewFlagID(0),
                    ParentID: FolderID,
                    Name: p_name,
                    IsFolder: true,
                    Ctime: AtSys.Now);
            }
            else
            {
                pf = new Pubfile(ID: p_id);
                pf.IsAdded = false;
                pf["name"] = p_name;
            }
            return await new Repo<Pubfile>().Save(pf);
        }

        public async Task<bool> Delete(IEnumerable<Row> p_rows)
        {
            var ls = new List<Pubfile>();
            foreach (var row in p_rows)
            {
                if (row.Bool("IsFolder"))
                {
                    int cnt = await AtCm.GetScalar<int>("文件-子项个数", new { parentid = row.ID });
                    if (cnt > 0)
                    {
                        AtKit.Warn($"[{row.Str("name")}]含有下级文件或文件夹，无法删除！");
                        return false;
                    }
                }

                ls.Add(new Pubfile(ID: row.ID));
            }
            return await new Repo<Pubfile>().BatchDelete(ls);
        }

        public Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID)
        {
            var ls = new List<Pubfile>();
            foreach (var row in p_files)
            {
                var pf = new Pubfile(ID: row.ID);
                pf.IsAdded = false;
                pf["ParentID"] = p_folderID;
                ls.Add(pf);
            }
            return new Repo<Pubfile>().BatchSave(ls, false);
        }
    }

    public class ResFileMgr : PubFileMgr
    {
        public ResFileMgr()
        {
            _rootFolderID = 2;
            _rootFolderName = "素材库";
        }
    }
}
