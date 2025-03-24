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

        protected override void ExportCustomXaml(XmlWriter p_xw)
        {
            if (!string.IsNullOrEmpty(ViewXaml))
            {
                FvDesignKit.CopyXml(p_xw, ViewXaml);
            }

            if (Sql == null || string.IsNullOrEmpty(Sql.SqlStr))
                return;

            p_xw.WriteStartElement("a", "CPick.Sql", null);
            p_xw.WriteStartElement("a", "Sql", null);

            if (!string.IsNullOrEmpty(Sql.LocalDb))
                p_xw.WriteAttributeString("LocalDb", Sql.LocalDb);
            if (!string.IsNullOrEmpty(Sql.Svc))
                p_xw.WriteAttributeString("Svc", Sql.Svc);
            p_xw.WriteString(Sql.SqlStr);

            p_xw.WriteEndElement();
            p_xw.WriteEndElement();
        }

        public override void AddCustomDesignCells(FvItems p_items)
        {
            CList.AddViewDesignCells(p_items);

            // 空时无法绑定
            if (Sql == null)
                Sql = new Sql();

            CList.AddSqlDesignCells(p_items);
        }

        public override void LoadXamlString(XmlNode p_node)
        {
            for (int i = 0; i < p_node.ChildNodes.Count; i++)
            {
                var node = p_node.ChildNodes[i];
                if (!node.LocalName.StartsWith("CPick."))
                {
                    ViewXaml = FvDesignKit.GetNodeXml(node, false);
                }
            }
        }
    }
}