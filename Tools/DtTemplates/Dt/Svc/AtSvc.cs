using Dt.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt
{
    /// <summary>
    /// svc代理 
    /// </summary>
    public class AtSvc
    {
        //static string _svcUrl = "https://localhost:1234";
        static string _svcUrl = "http://localhost/dt-cm";

        public static void BindSvcUrl(TextBox p_tb)
        {
            p_tb.Text = _svcUrl;
            p_tb.TextChanged += (s, e) => _svcUrl = p_tb.Text;
        }

        /// <summary>
        /// 获取最新的所有表名
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetAllTables()
        {
            return new Rpc().Call<List<string>>(
                    _svcUrl,
                    "SysTools.GetAllTables"
                );
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_clsName">类名，null时按规则生成：移除前后缀，首字母大写</param>
        /// <returns></returns>
        public static Task<string> GetEntityClass(string p_tblName, string p_clsName)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetEntityClass",
                p_tblName,
                p_clsName
            );
        }

        /// <summary>
        /// 生成实体类的扩展部分，如 InitHook New
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_clsName">类名，null时按规则生成：移除前后缀，首字母大写</param>
        /// <returns></returns>
        public static Task<string> GetEntityClassEx(string p_tblName, string p_clsName = null)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetEntityClassEx",
                p_tblName,
                p_clsName
            );
        }

        public static Task<string> GetFvCells(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetFvCells",
                p_tblNames
            );
        }

        public static Task<string> GetLvItemTemplate(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetLvItemTemplate",
                p_tblNames
            );
        }

        public static Task<string> GetLvTableCols(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetLvTableCols",
                p_tblNames
            );
        }

        public static Task<string> GetSingleTblSql(string p_tblName, string p_title, bool p_blurQuery)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetSingleTblSql",
                p_tblName,
                p_title,
                p_blurQuery
            );
        }

        public static Task<string> GetOneToManySql(string p_parentTbl, string p_parentTitle, List<string> p_childTbls, List<string> p_childTitles, bool p_blurQuery)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetOneToManySql",
                p_parentTbl,
                p_parentTitle,
                p_childTbls,
                p_childTitles,
                p_blurQuery
            );
        }

        public static Task<bool> ExistParentID(string p_tblName)
        {
            return new Rpc().Call<bool>(
                _svcUrl,
                "SysTools.ExistParentID",
                p_tblName
            );
        }

        public static Task<string> GetManyToManySql(string p_mainTbl, string p_mainTitle, bool p_blurQuery)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                "SysTools.GetManyToManySql",
                p_mainTbl,
                p_mainTitle,
                p_blurQuery
            );
        }
    }
}
