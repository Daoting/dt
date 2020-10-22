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
    /// 公共文件管理
    /// </summary>
    public class PubFileMgr : IFileMgr
    {
        public long FolderID { get; set; }
        public string FolderName { get; set; }

        public bool AllowEdit
        {
            get { return AtUser.HasPrv("公共文件管理"); }
        }

        public async Task<bool> Delete(Row p_row)
        {
            if (p_row.Bool("IsFolder"))
            {
                int cnt = await AtCm.GetScalar<int>("文件-子项个数", new { parentid = p_row.Long("id") });
                if (cnt > 0)
                {
                    AtKit.Warn("含有下级文件或文件夹，无法删除！");
                    return false;
                }
            }
            return await new Repo<Pubfile>().DelByID(p_row.Long("id"));
        }

        public Task<Table> GetFiles()
        {
            //Table tbl = Table.Create("cm_pubfile");
            //tbl.AddRow(new { id = 2, parentid = 1, name = "文件.jpg", info = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]" });
            //tbl.AddRow(new { id = 3, parentid = 1, name = "文件夹", IsFolder = true });
            //return Task.FromResult(tbl);
            return AtCm.Query("文件-查询目录", new { parentid = FolderID });
        }

        public Task<bool> SaveFile(Row p_row)
        {
            Pubfile pf = new Pubfile(
                    ID: p_row.Long("id"),
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
    }
}
