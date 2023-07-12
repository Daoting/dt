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

        public FileMgrSetting Setting { get; set; }

        public async Task<Table> GetChildren()
        {
            return await FilePubX.Query($"where parent_id={FolderID}");
        }

        public async Task<Table> GetChildFolders()
        {
            return await FilePubX.Query($"where is_folder=1 and parent_id={FolderID}");
        }

        public async Task<Table> GetChildrenByType(string p_typeFilter)
        {
            return await FilePubX.Query($"where parent_id={FolderID} and ( is_folder = 1 or locate( ext_name, '{p_typeFilter}' ) )");
        }

        public async Task<Table> SearchFiles(string p_name)
        {
            return await FilePubX.Query($"where is_folder=0 and name like '%{p_name}%' limit 20");
        }

        public Task<bool> SaveFile(Row p_row)
        {
            FilePubX pf = new FilePubX(
                    ID: p_row.ID,
                    ParentID: FolderID,
                    Name: p_row.Str("name"),
                    IsFolder: false,
                    ExtName: p_row.Str("ext_name"),
                    Info: p_row.Str("info"),
                    Ctime: p_row.Date("ctime"));
            return pf.Save(false);
        }

        public async Task<bool> SaveFolder(long p_id, string p_name)
        {
            FilePubX pf;
            if (p_id == -1)
            {
                pf = await FilePubX.New(
                    ParentID: FolderID,
                    Name: p_name,
                    IsFolder: true,
                    Ctime: Kit.Now);
            }
            else
            {
                pf = new FilePubX(ID: p_id);
                pf.IsAdded = false;
                pf["name"] = p_name;
            }
            return await pf.Save();
        }

        public async Task<bool> Delete(IEnumerable<Row> p_rows)
        {
            var ls = new List<FilePubX>();
            foreach (var row in p_rows)
            {
                if (row.Bool("is_folder"))
                {
                    int cnt = await FilePubX.GetCount($"where parent_id={row.ID}");
                    if (cnt > 0)
                    {
                        Kit.Warn($"[{row.Str("name")}]含有下级文件或文件夹，无法删除！");
                        return false;
                    }
                }

                ls.Add(new FilePubX(ID: row.ID));
            }
            return await ls.Delete();
        }

        public Task<bool> MoveFiles(IEnumerable<Row> p_files, long p_folderID)
        {
            var ls = new List<FilePubX>();
            foreach (var row in p_files)
            {
                var pf = new FilePubX(ID: row.ID);
                pf.IsAdded = false;
                pf.ParentID = p_folderID;
                ls.Add(pf);
            }
            return ls.Save(false);
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
