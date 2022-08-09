using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dt
{
    /// <summary>
    /// svc代理 
    /// </summary>
    public class AtSvc
    {
        public static string SvcUrl { get; set; }

        public static Task<List<string>> GetAllTables()
        {
            return new Rpc().Call<List<string>>(
                SvcUrl,
                "SysTools.所有表名"
            );
        }


        public static Task<string> CreateAgent(string p_tblName)
        {
            return new Rpc().Call<string>(
                SvcUrl,
                "SysTools.生成实体类",
                p_tblName
            );
        }
    }
}
