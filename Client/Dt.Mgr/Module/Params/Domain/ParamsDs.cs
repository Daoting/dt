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

            var w = await UserParamsX.NewWriter();
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

        #region Sql
        const string Sql用户参数值byid = @"
select value from cm_user_params
where
	user_id = {0}
	and param_id = {1}
union
select value from cm_params
where
	id = {1}
";

        const string Sql用户参数值byname = @"
select a.value
from
	cm_user_params a,
	cm_params b 
where
	a.param_id = b.id 
	and a.user_id = {0} 
	and b.name = '{1}'
union
select value from cm_params
where
	name = '{1}'
";

        const string Sql用户参数列表 = @"
select param_id,value from cm_user_params
where user_id={0}
union
select id,value from cm_params a
where
    not exists (
    select param_id from cm_user_params b
    where
	    a.id = b.param_id 
	    and user_id = {0} )
";
        #endregion
    }
}