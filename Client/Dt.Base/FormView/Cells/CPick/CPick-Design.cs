#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Dt.Toolkit.Sql;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设计时
    /// </summary>
    public partial class CPick
    {
        /// <summary>
        /// 设计时用，行视图的xaml
        /// </summary>
        public string ViewXaml
        {
            get { return _lv.ViewXaml; }
            set { _lv.ViewXaml = value; }
        }

        /// <summary>
        /// 设计时用，xaml定义Sql
        /// </summary>
        public Sql SqlXaml
        {
            get { return Dt.Base.Ex.GetSqlXaml(this); }
            set { Dt.Base.Ex.SetSqlXaml(this, value); }
        }

        protected override void ExportCustomXaml(XmlWriter p_xw)
        {
            if (!string.IsNullOrEmpty(ViewXaml))
            {
                FvDesignKit.CopyXml(p_xw, ViewXaml);
            }

            if (SqlXaml != null && !string.IsNullOrEmpty(SqlXaml.SqlStr))
            {
                p_xw.WriteStartElement("a", "CPick.Sql", null);
                p_xw.WriteStartElement("a", "Sql", null);

                if (!string.IsNullOrEmpty(SqlXaml.LocalDb))
                    p_xw.WriteAttributeString("LocalDb", SqlXaml.LocalDb);
                if (!string.IsNullOrEmpty(SqlXaml.Svc))
                    p_xw.WriteAttributeString("Svc", SqlXaml.Svc);
                p_xw.WriteCData(SqlXaml.SqlStr);

                p_xw.WriteEndElement();
                p_xw.WriteEndElement();
            }
        }

        public override void AddCustomDesignCells(FvItems p_items)
        {
            CList.AddViewDesignCells(p_items);

            // 空时无法绑定
            if (SqlXaml == null)
                SqlXaml = new Sql();

            CList.AddSqlDesignCells(p_items);
        }

        public override void LoadXamlString(XmlNode p_node)
        {
            for (int i = 0; i < p_node.ChildNodes.Count; i++)
            {
                var node = p_node.ChildNodes[i];

                if (node.LocalName == "CPick.Sql"
                    && node.HasChildNodes
                    && node.ChildNodes[0].LocalName == "Sql")
                {
                    if (SqlXaml == null)
                        SqlXaml = new Sql();

                    var cn = node.ChildNodes[0];
                    foreach (var attr in cn.Attributes.OfType<XmlAttribute>())
                    {
                        if (attr.LocalName == "LocalDb")
                            SqlXaml.LocalDb = attr.Value;
                        else if (attr.LocalName == "Svc")
                            SqlXaml.Svc = attr.Value;
                        else if (attr.LocalName == "SqlStr")
                            SqlXaml.SqlStr = attr.Value;
                    }

                    if (cn.HasChildNodes)
                    {
                        SqlXaml.SqlStr = cn.ChildNodes[0].InnerText;
                    }
                }
                else if (!node.LocalName.StartsWith("CPick."))
                {
                    ViewXaml = FvDesignKit.GetNodeXml(node, false);
                }
            }
        }
    }
}