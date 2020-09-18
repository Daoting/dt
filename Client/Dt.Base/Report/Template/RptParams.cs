#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表参数定义
    /// </summary>
    internal class RptParams
    {
        public RptParams()
        {
            Data = new Table
            {
                { "id" },
                { "name" },
                { "type" },
                { "macro" },
                { "val" },
                { "ct" },
                { "note" },
                { "colspan", typeof(int) },
                { "rowspan", typeof(int) },
                { "readonly" },
                { "hide" },
                { "horstretch" },
                { "verstretch" },
                { "hidetitle" },
                { "verticaltitle" },
                { "titlewidth",  typeof(double)},
            };
        }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        public Table Data { get; }

        /// <summary>
        /// 获取宏参数名列表
        /// </summary>
        public List<string> Macros
        {
            get
            {
                List<string> ls = new List<string>();
                foreach (Row row in Data)
                {
                    if (row.Bool("macro"))
                        ls.Add(row.Str("id"));
                }
                if (ls.Count == 0)
                    return null;
                return ls;
            }
        }

        /// <summary>
        /// 根据参数名获取参数定义Row
        /// </summary>
        /// <param name="p_col"></param>
        /// <returns></returns>
        public Row this[string p_col]
        {
            get
            {
                return (from row in Data
                        where row.Str("id") == p_col
                        select row).FirstOrDefault();
            }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            Data.ReadXml(p_reader);
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteXml(XmlWriter p_writer)
        {
            Data.WriteXml(p_writer, "Params", "Param");
        }
    }
}
