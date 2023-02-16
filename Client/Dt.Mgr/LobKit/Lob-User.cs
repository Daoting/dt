#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 当前登录用户相关信息
    /// </summary>
    public partial class LobKit
    {
        #region 权限
        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_id">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static async Task<bool> HasPrv(string p_id)
        {
            int cnt = await AtLob.GetScalar<int>("select count(*) from DataVer where id='privilege'");
            if (cnt == 0)
            {
                // 查询服务端
                Dict dt = await Kit.Rpc<Dict>(
                    "cm",
                    "UserRelated.GetPrivileges",
                    Kit.UserID
                );

                // 记录版本号
                var ver = new DataVerX(ID: "privilege", Ver: dt.Str("ver"));
                await ver.Save(false);

                // 清空旧数据
                await AtLob.Exec("delete from UserPrivilege");
                // 插入新数据
                var ls = (List<string>)dt["result"];
                if (ls != null && ls.Count > 0)
                {
                    List<Dict> dts = new List<Dict>();
                    foreach (var prv in ls)
                    {
                        dts.Add(new Dict { { "prv", prv } });
                    }
                    var d = new Dict();
                    d["text"] = "insert into UserPrivilege (prv) values (@prv)";
                    d["params"] = dts;
                    await AtLob.BatchExec(new List<Dict> { d });
                }
            }

            return await AtLob.GetScalar<int>($"select count(*) from UserPrivilege where Prv='{p_id}'") > 0;
        }
        #endregion

        #region 用户参数
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static async Task<T> GetParam<T>(string p_paramID)
        {
            await InitParams();

            var row = await AtLob.First($"select val from UserParams where id='{p_paramID}'");
            Throw.IfNull(row, $"无参数【{p_paramID}】");

            string val = row.Str(0);
            if (string.IsNullOrEmpty(val))
                return default;

            Type type = typeof(T);
            if (type == typeof(string))
                return (T)(object)val;

            object result;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception(string.Format("参数【{0}】的值转换异常：无法将【{1}】转换到【{2}】类型！", p_paramID, val, type));
            }
            return (T)result;
        }

        static async Task InitParams()
        {
            int cnt = await AtLob.GetScalar<int>("select count(*) from DataVer where id='params'");
            if (cnt > 0)
                return;

            // 查询服务端
            Dict dt = await Kit.Rpc<Dict>(
                    "cm",
                    "UserRelated.GetParams",
                    Kit.UserID
                );

            // 记录版本号
            var ver = new DataVerX(ID: "params", Ver: dt.Str("ver"));
            await ver.Save(false);

            // 清空旧数据
            await AtLob.Exec("delete from UserParams");

            // 插入新数据
            var tbl = (Table)dt["result"];
            if (tbl != null && tbl.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var row in tbl)
                {
                    dts.Add(new Dict { { "id", row.Str(0) }, { "val", row.Str(1) } });
                }
                var d = new Dict();
                d["text"] = "insert into UserParams (id,val) values (@id, @val)";
                d["params"] = dts;
                await AtLob.BatchExec(new List<Dict> { d });
            }
        }
        #endregion
    }
}
