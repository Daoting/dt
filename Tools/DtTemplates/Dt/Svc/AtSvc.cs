using Dt.Core;
using System;
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
        static string _svcUrl = "https://localhost:1234";
        //static string _svcUrl = "http://localhost/dt-cm";
        static List<string> _allSvcs = null;
        static string _svcName = "";

        public static void BindSvcUrl(TextBox p_tb)
        {
            p_tb.Text = _svcUrl;
            p_tb.TextChanged += (s, e) => _svcUrl = p_tb.Text.TrimEnd('/');
        }

        public static async void BindSvcName(ComboBox p_cb)
        {
            if (_allSvcs == null)
                _allSvcs = await GetAllSvcNames();

            p_cb.DataSource = _allSvcs;
            p_cb.SelectedItem = _svcName;
            p_cb.SelectedIndexChanged += (s, e) => _svcName = p_cb.SelectedItem.ToString();
        }

        /// <summary>
        /// 获取所有服务名称
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetAllSvcNames()
        {
            return new Rpc().Call<List<string>>(
                    _svcUrl,
                    _svcName,
                    "SysTools.GetAllSvcNames"
                );
        }

        /// <summary>
        /// 获取最新的所有表名
        /// </summary>
        /// <returns></returns>
        public static Task<List<string>> GetAllTables()
        {
            return new Rpc().Call<List<string>>(
                    _svcUrl,
                    _svcName,
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
                _svcName,
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
                _svcName,
                "SysTools.GetEntityClassEx",
                p_tblName,
                p_clsName
            );
        }

        public static Task<string> GetFvCells(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                _svcName,
                "SysTools.GetFvCells",
                p_tblNames
            );
        }

        public static Task<string> GetLvItemTemplate(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                _svcName,
                "SysTools.GetLvItemTemplate",
                p_tblNames
            );
        }

        public static Task<string> GetLvTableCols(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                _svcName,
                "SysTools.GetLvTableCols",
                p_tblNames
            );
        }

        public static Task<string> GetQueryFvCells(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                _svcName,
                "SysTools.GetQueryFvCells",
                p_tblNames
            );
        }

        public static Task<string> GetQueryFvData(List<string> p_tblNames)
        {
            return new Rpc().Call<string>(
                _svcUrl,
                _svcName,
                "SysTools.GetQueryFvData",
                p_tblNames
            );
        }
    }
}
