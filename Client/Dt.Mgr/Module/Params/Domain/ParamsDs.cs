#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class ParamsDs : DomainSvc<ParamsDs>
    {
        public static async Task<bool> SaveParams(string p_paramID, string p_value)
        {
            if (!long.TryParse(p_paramID, out var paramID))
                Throw.Msg("参数标识错误！");

            var w = At.NewWriter();
            var old = new UserParamsX(
                UserID: Kit.UserID,
                ParamID: paramID);
            await w.Delete(old);

            var def = await ParamsX.GetByID(p_paramID);
            if (def.Value != p_value)
            {
                // 和默认值不同
                var up = new UserParamsX(
                    UserID: Kit.UserID,
                    ParamID: paramID,
                    Value: p_value,
                    Mtime: Kit.Now);
                await w.Save(up);
            }

            return await w.Commit(false);
        }

        public static async Task<T> GetParamByID<T>(long p_paramID)
        {
            var ls = await At.FirstCol<string>(string.Format(Sql用户参数值byid, Kit.UserID, p_paramID));
            if (ls == null || ls.Count == 0)
                Throw.Msg($"用户参数[{p_paramID}]不存在！");

            if (string.IsNullOrEmpty(ls[0]))
                return default;
            return ls[0].To<T>();
        }

        public static async Task<T> GetParamByName<T>(string p_paramName)
        {
            var ls = await At.FirstCol<string>(string.Format(Sql用户参数值byname, Kit.UserID, p_paramName));
            if (ls == null || ls.Count == 0)
                Throw.Msg($"用户参数[{p_paramName}]不存在！");

            if (string.IsNullOrEmpty(ls[0]))
                return default;
            return ls[0].To<T>();
        }

        public static Task<Table> GetUserParams(long p_userID)
        {
            return At.Query(string.Format(Sql用户参数列表, p_userID));
        }
    }
}