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
using System.Threading.Tasks;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数据源包装类
    /// </summary>
    public static class AtRpt
    {
        #region 成员变量
        const string _assertMsg = "报表描述信息不完整！";
        
        #endregion

        /// <summary>
        /// 打开新窗口显示报表
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        /// <param name="p_icon">图标</param>
        public static void Show(RptInfo p_info, string p_winTitle = null, Icons p_icon = Icons.折线图)
        {
            Throw.IfNull(p_info, _assertMsg);
            AtApp.OpenWin(typeof(RptViewWin), string.IsNullOrEmpty(p_winTitle) ? p_info.Name : p_winTitle, p_icon, p_info);
        }

        /// <summary>
        /// 打开新窗口显示报表组
        /// </summary>
        /// <param name="p_infos">报表组描述信息</param>
        /// <param name="p_winTitle">窗口标题</param>
        /// <param name="p_icon">图标</param>
        public static void Show(List<RptInfo> p_infos, string p_winTitle = null, Icons p_icon = Icons.折线图)
        {
            Throw.If(!IsValid(p_infos), _assertMsg);
            if (p_infos.Count == 1)
            {
                Show(p_infos[0], p_winTitle, p_icon);
            }
            else
            {
                // 使用 RptInfoList 只为识别窗口用
                var ls = new RptInfoList();
                ls.AddRange(p_infos);
                AtApp.OpenWin(typeof(RptViewWin), string.IsNullOrEmpty(p_winTitle) ? "无标题" : p_winTitle, p_icon, ls);
            }
        }

        /// <summary>
        /// 打开报表设计窗口
        /// </summary>
        /// <param name="p_info">报表设计描述信息</param>
        /// <param name="p_winTitle">窗口标题，null时使用报表名称</param>
        /// <param name="p_icon">窗口图标</param>
        /// <returns></returns>
        public static async Task<bool> ShowDesign(RptDesignInfo p_info, string p_winTitle = null, Icons p_icon = Icons.Excel)
        {
            Throw.IfNull(p_info, _assertMsg);
            if (await p_info.InitTemplate())
            {
                AtApp.OpenWin(typeof(RptDesignHome), string.IsNullOrEmpty(p_winTitle) ? p_info.Name : p_winTitle, p_icon, p_info);
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
                            using (XmlReader reader = XmlReader.Create(stream, AtKit.ReaderSettings))
                            {
                                reader.Read();
                                root.ReadXml(reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("反序列化报表模板时异常：{0}", ex.Message));
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
                return string.Empty;

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
        /// 根据参数默认值创建初始查询参数（自动查询时用）
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        internal static Dict CreateDefaultParams(RptInfo p_info)
        {
            if (p_info == null || p_info.Root == null || p_info.Root.Params == null || p_info.Root.Params.Data == null)
                return null;

            Dict dict = new Dict();
            foreach (var row in p_info.Root.Params.Data)
            {
                //dict.Add(row.Str("id"), cell.Val);
            }
            return dict;
        }

        
        /// <summary>
        /// 校验报表组描述信息
        /// </summary>
        /// <param name="p_infos"></param>
        /// <returns></returns>
        static bool IsValid(List<RptInfo> p_infos)
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
