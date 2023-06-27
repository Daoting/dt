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
    class ParamsDs : DomainSvc<ParamsDs, AtCm.Info>
    {
        public static async Task<bool> SaveParams(string p_paramID, string p_value)
        {
            if (!long.TryParse(p_paramID, out var paramID))
                Throw.Msg("参数标识错误！");

            var old = new UserParamsX(
                UserID: Kit.UserID,
                ParamID: paramID);
            await Delete(old);

            var def = await ParamsX.GetByID(p_paramID);
            if (def.Value != p_value)
            {
                // 和默认值不同
                var up = new UserParamsX(
                    UserID: Kit.UserID,
                    ParamID: paramID,
                    Value: p_value,
                    Mtime: Kit.Now);
                await Save(up);
            }

            return await Commit(false);
        }

        public static async Task<T> GetParamByID<T>(long p_paramID)
        {
            var ls = await _da.FirstCol<string>("cm_参数_用户参数值ByID", new { p_userid= Kit.UserID, p_paramid = p_paramID });
            if (ls == null || ls.Count == 0)
                Throw.Msg($"用户参数[{p_paramID}]不存在！");

            if (string.IsNullOrEmpty(ls[0]))
                return default;
            return ls[0].To<T>();
        }

        public static async Task<T> GetParamByName<T>(string p_paramName)
        {
            var ls = await _da.FirstCol<string>("cm_参数_用户参数值ByName", new { p_userid = Kit.UserID, p_name = p_paramName });
            if (ls == null || ls.Count == 0)
                Throw.Msg($"用户参数[{p_paramName}]不存在！");

            if (string.IsNullOrEmpty(ls[0]))
                return default;
            return ls[0].To<T>();
        }
    }
}