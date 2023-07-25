#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class WfdAtvX
    {
        public static async Task<WfdAtvX> New(
            long PrcID = default,
            string Name = default,
            WfdAtvType Type = default,
            WfdAtvExecScope ExecScope = default,
            WfdAtvExecLimit ExecLimit = default,
            long? ExecAtvID = default,
            bool AutoAccept = default,
            bool CanDelete = default,
            bool CanTerminate = default,
            bool CanJumpInto = default,
            WfdAtvTransKind TransKind = default,
            WfdAtvJoinKind JoinKind = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new WfdAtvX(
                ID: await NewID(),
                PrcID: PrcID,
                Name: Name,
                Type: Type,
                ExecScope: ExecScope,
                ExecLimit: ExecLimit,
                ExecAtvID: ExecAtvID,
                AutoAccept: AutoAccept,
                CanDelete: CanDelete,
                CanTerminate: CanTerminate,
                CanJumpInto: CanJumpInto,
                TransKind: TransKind,
                JoinKind: JoinKind,
                Ctime: Ctime,
                Mtime: Mtime);
        }

        public static Task<Table<WfdAtvX>> GetNextAtv(long p_atvid)
        {
            return Query(string.Format(Sql后续活动, p_atvid));
        }

        public static Task<WfdAtvX> GetFirstNextAtv(long p_atvid)
        {
            return First(string.Format(Sql后续活动, p_atvid));
        }

        protected override void InitHook()
        {
            
        }
    }
}