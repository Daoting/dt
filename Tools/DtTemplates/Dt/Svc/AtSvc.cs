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
        static string _svcUrl = "http://localhost/dt-cm";
        static List<string> _allTables;

        public static string SvcUrl
        {
            get { return _svcUrl; }
            set
            {
                if (_svcUrl != value)
                {
                    _svcUrl = value;
                    _allTables = null;
                }
            }
        }

        public static async Task<List<string>> GetAllTables()
        {
            if (_allTables == null)
            {
                _allTables = await new Rpc().Call<List<string>>(
                    SvcUrl,
                    "SysTools.所有表名"
                );

                if (_allTables == null)
                    _allTables = new List<string>();
                // 空
                _allTables.Insert(0, "");
            }
            return _allTables;
        }

        public static Task<string> CreateAgent(string p_tblName)
        {
            return new Rpc().Call<string>(
                SvcUrl,
                "SysTools.生成实体类",
                p_tblName
            );
        }

        public static Task<string> GetFvContent(string p_tblName)
        {
            return new Rpc().Call<string>(
                SvcUrl,
                "SysTools.生成Fv格内容",
                p_tblName
            );
        }

        public static Task<string> GetLvItemTemplates(string p_tblName)
        {
            return new Rpc().Call<string>(
                SvcUrl,
                "SysTools.生成Lv项模板",
                p_tblName
            );
        }

        public static Task<string> GetLvCols(string p_tblName)
        {
            return new Rpc().Call<string>(
                SvcUrl,
                "SysTools.生成Lv表格列",
                p_tblName
            );
        }

        public static Task<bool> CreateTblSql(string p_tblName)
        {
            return new Rpc().Call<bool>(
                SvcUrl,
                "SysTools.生成表的框架sql",
                p_tblName
            );
        }
    }
}
