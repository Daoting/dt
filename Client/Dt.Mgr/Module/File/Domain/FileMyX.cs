#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-10 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class FileMyX
    {
        public static async Task<FileMyX> New(
            long? ParentID = default,
            string Name = default,
            bool IsFolder = default,
            string ExtName = default,
            string Info = default,
            DateTime Ctime = default,
            long UserID = default)
        {
            return new FileMyX(
                ID: await NewID(),
                ParentID: ParentID,
                Name: Name,
                IsFolder: IsFolder,
                ExtName: ExtName,
                Info: Info,
                Ctime: Ctime,
                UserID: UserID);
        }

        public static Task<Table<FileMyX>> GetChildren(long p_parentID)
        {
            if (p_parentID == -1)
                return Query($"where parent_id is null and user_id={Kit.UserID}");
            return Query($"where parent_id={p_parentID}");
        }

        public static Task<Table<FileMyX>> GetChildFolders(long p_parentID)
        {
            if (p_parentID == -1)
                return Query($"where is_folder='1' and parent_id is null and user_id={p_parentID}");
            return Query($"where is_folder='1' and parent_id={p_parentID}");
        }

        public static Task<Table<FileMyX>> SearchFiles(string p_name)
        {
            return Query($"where is_folder='0' and user_id={Kit.UserID} and name like '%{p_name}%'");
        }

        public static Task<Table<FileMyX>> GetChildrenByType(long p_parentID, string p_typeFilter)
        {
            if (p_parentID == -1)
                return Query($"where parent_id is null and user_id = {Kit.UserID} and ( is_folder = '1' or locate( ext_name, '{p_typeFilter}' ) )");
            return Query($"where parent_id={p_parentID} and ( is_folder = '1' or locate( ext_name, '{p_typeFilter}' ) )");
        }
    }
}