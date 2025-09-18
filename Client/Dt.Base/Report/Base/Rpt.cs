#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base.Report;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源包装类
    /// </summary>
    public static class Rpt
    {
        const string _assertMsg = "报表描述信息不完整！";

        /// <summary>
        /// 打开新窗口显示报表
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        /// <param name="p_isPdf">报表是否采用Pdf格式</param>
        /// <param name="p_winTitle"></param>
        /// <param name="p_icon">图标</param>
        public static async void Show(RptInfo p_info, bool p_isPdf = false, string p_winTitle = null, Icons p_icon = Icons.折线图)
        {
            Throw.IfNull(p_info, _assertMsg);

            if (await p_info.Init())
            {
                Kit.OpenWin(
                    typeof(RptWin),
                    string.IsNullOrEmpty(p_winTitle) ? p_info.Name : p_winTitle,
                    p_icon,
                    new RptWinParams { Info = p_info, IsPdf = p_isPdf });
            }
            else
            {
                Kit.Warn($"初始化报表模板[{p_info.Name}]出错！");
            }
        }

        /// <summary>
        /// 显示报表对话框
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        /// <param name="p_isPdf">报表是否采用Pdf格式</param>
        /// <param name="p_title"></param>
        /// <returns>对话框</returns>
        public static async Task<Dlg> ShowDlg(RptInfo p_info, bool p_isPdf = false, string p_title = null)
        {
            Throw.IfNull(p_info, _assertMsg);

            if (await p_info.Init())
            {
                var dlg = new Dlg
                {
                    IsPinned = true,
                    Title = string.IsNullOrEmpty(p_title) ? p_info.Name : p_title,
                };
                if (!Kit.IsPhoneUI)
                {
                    dlg.Width = 1000;
                    dlg.Height = Kit.ViewHeight - 200;
                }

                RptTab rpt = new RptTab();
                if (p_isPdf)
                    rpt.IsPdf = true;
                dlg.LoadTab(rpt);
                // 释放
                dlg.Closed += (s, e) => rpt.Close();
                dlg.Show();
                rpt.LoadReport(p_info);
                return dlg;
            }
            else
            {
                Kit.Warn($"初始化报表模板[{p_info.Name}]出错！");
            }
            return null;
        }

        /// <summary>
        /// 打开新窗口显示报表组
        /// </summary>
        /// <param name="p_infos">报表组描述信息</param>
        /// <param name="p_isPdf">报表是否采用Pdf格式</param>
        /// <param name="p_winTitle">窗口标题</param>
        /// <param name="p_icon">图标</param>
        public static void Show(IList<RptInfo> p_infos, bool p_isPdf = false, string p_winTitle = null, Icons p_icon = Icons.折线图)
        {
            Throw.If(!IsValid(p_infos), _assertMsg);
            if (p_infos.Count == 1)
            {
                Show(p_infos[0], p_isPdf, p_winTitle, p_icon);
            }
            else
            {
                // 使用 RptInfoList 只为识别窗口用
                var ls = new RptInfoList();
                ls.AddRange(p_infos);
                Kit.OpenWin(
                    typeof(RptGroupWin),
                    string.IsNullOrEmpty(p_winTitle) ? "无标题" : p_winTitle,
                    p_icon,
                    new RptGroupWinParams { Infos = ls, IsPdf = p_isPdf });
            }
        }

        /// <summary>
        /// 打开报表设计窗口
        /// </summary>
        /// <param name="p_info">报表设计描述信息，null时使用临时空模板</param>
        /// <param name="p_winTitle">窗口标题，null时使用报表名称</param>
        /// <param name="p_icon">窗口图标</param>
        /// <returns></returns>
        public static async Task<bool> ShowDesign(RptDesignInfo p_info, string p_winTitle = null, Icons p_icon = Icons.Excel)
        {
            if (p_info == null)
            {
                Kit.OpenWin(typeof(RptDesignHome), p_winTitle, p_icon);
                return true;
            }

            if (await p_info.InitTemplate())
            {
                Kit.OpenWin(typeof(RptDesignHome), string.IsNullOrEmpty(p_winTitle) ? p_info.Name : p_winTitle, p_icon, p_info);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 反序列化报表模板
        /// </summary>
        /// <param name="p_define"></param>
        /// <returns></returns>
        internal static Task<RptRoot> DeserializeTemplate(string p_define)
        {
            return Task.Run(() =>
            {
                RptRoot root = new RptRoot();
                if (!string.IsNullOrEmpty(p_define))
                {
                    try
                    {
                        using (StringReader stream = new StringReader(p_define))
                        {
                            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true }))
                            {
                                reader.Read();
                                root.ReadXml(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Throw.Msg(string.Format("反序列化报表模板时异常：{0}", ex.Message));
                    }
                }
                return root;
            });
        }

        /// <summary>
        /// 序列化报表模板
        /// </summary>
        /// <param name="p_root"></param>
        /// <returns></returns>
        internal static string SerializeTemplate(RptRoot p_root)
        {
            if (p_root == null)
                return "";

            p_root.OnBeforeSerialize();
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                p_root.WriteXml(writer);
                writer.Flush();
            }
            p_root.OnAfterSerialize();
            return sb.ToString();
        }

        /// <summary>
        /// 查询报表数据
        /// </summary>
        /// <param name="p_svc">服务名称</param>
        /// <param name="p_sql">Sql语句</param>
        /// <param name="p_params">参数字典</param>
        /// <returns></returns>
        internal static Task<Table> Query(string p_svc, string p_sql, Dict p_params)
        {
            Throw.If(string.IsNullOrEmpty(p_sql), "查询报表数据时Sql不可为空！");

            // 按参数位置顺序整理查询参数
            Dict sqlDt = new Dict();
            Regex reg = new Regex(@"@[^\s,]+");
            MatchCollection matches = reg.Matches(p_sql);
            foreach (Match match in matches)
            {
                string name = match.Value.Substring(1);
                if (p_params != null && p_params.TryGetValue(name, out var val))
                {
                    sqlDt[name] = val;
                }
                else
                {
                    Throw.If(true, $"查询报表数据时未提供参数【{name}】的值！");
                }
            }

            // 参数值替换占位符
            string sql = p_sql;
            reg = new Regex(@"#[^\s#,]+#");
            matches = reg.Matches(p_sql);
            foreach (Match match in matches)
            {
                string name = match.Value.Trim('#');
                if (p_params != null && p_params.TryGetValue(name, out var val))
                {
                    sql = sql.Replace(match.Value, val == null ? "" : val.ToString());
                }
                else
                {
                    Throw.If(true, $"查询报表数据时未提供参数【{name}】的值！");
                }
            }

            return Kit.Rpc<Table>(
                string.IsNullOrEmpty(p_svc) ? At.CurrentSvc : p_svc,
                "Da.Query",
                sql,
                sqlDt
            );
        }

        /// <summary>
        /// 校验报表组描述信息
        /// </summary>
        /// <param name="p_infos"></param>
        /// <returns></returns>
        static bool IsValid(IList<RptInfo> p_infos)
        {
            bool valid = false;
            if (p_infos != null && p_infos.Count > 0)
            {
                valid = true;
                foreach (RptInfo info in p_infos)
                {
                    if (info == null || string.IsNullOrEmpty(info.Name))
                    {
                        valid = false;
                        break;
                    }
                }
            }
            return valid;
        }
    }
}
