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
    public partial class CList
    {
        #region 静态内容
        public readonly static DependencyProperty ItemsXamlProperty = DependencyProperty.Register(
            "ItemsXaml",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null));
        #endregion

        /// <summary>
        /// 设计时用，xaml中定义的对象列表
        /// </summary>
        public string ItemsXaml
        {
            get { return (string)GetValue(ItemsXamlProperty); }
            set { SetValue(ItemsXamlProperty, value); }
        }
        
        protected override void ExportCustomXaml(XmlWriter p_xw)
        {
            if (!string.IsNullOrEmpty(ItemsXaml))
            {
                p_xw.WriteStartElement("a", "CList.Items", null);
                FvDesignKit.CopyXml(p_xw, ItemsXaml);
                p_xw.WriteEndElement();
            }
            
            if (Sql == null || string.IsNullOrEmpty(Sql.SqlStr))
                return;

            p_xw.WriteStartElement("a", "CList.Sql", null);
            p_xw.WriteStartElement("a", "Sql", null);

            if (!string.IsNullOrEmpty(Sql.LocalDb))
                p_xw.WriteAttributeString("LocalDb", Sql.LocalDb);
            if (!string.IsNullOrEmpty(Sql.Svc))
                p_xw.WriteAttributeString("Svc", Sql.Svc);
            p_xw.WriteString(Sql.SqlStr);

            p_xw.WriteEndElement();
            p_xw.WriteEndElement();
        }

        public override FvCell CreateDesignCell(CellPropertyInfo p_info)
        {
            if (p_info.Info.Name == "Ex")
            {
                return new CList
                {
                    ID = p_info.Info.Name,
                    IsEditable = true,
                    Items = { "Option#民族", "EnumData#Dt.Base.DlgPlacement,Dt.Base" },
                    ShowTitle = false,
                };
            }

            return base.CreateDesignCell(p_info);
        }

        public override void AddCustomDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "对象列表" };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "ItemsXaml",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
            };
            p_items.Add(text);

            var btn = new Button { Content = "+模板", HorizontalAlignment = HorizontalAlignment.Right };
            Menu m = new Menu { Placement = MenuPosition.BottomLeft };
            Mi mi = new Mi { ID = "字符串选项" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<x:String>选项一</x:String>");
            m.Add(mi);

            mi = new Mi { ID = "整数选项" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<x:Int32>1</x:Int32>");
            m.Add(mi);

            mi = new Mi { ID = "IDStr" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<a:IDStr ID=\"0\" Str=\"选项一\" />");
            m.Add(mi);
            Dt.Base.Ex.SetMenu(btn, m);
            bar.Content = btn;
            
            // 空时无法绑定
            if (Sql == null)
                Sql = new Sql();

            AddSqlDesignCells(p_items);
        }

        public override void LoadXamlString(XmlNode p_node)
        {
            for (int i = 0; i < p_node.ChildNodes.Count; i++)
            {
                var node = p_node.ChildNodes[i];
                if (node.LocalName == "CList.Items")
                {
                    ItemsXaml = FvDesignKit.GetNodeXml(node, true);
                }
            }
        }
        
        internal static void AddSqlDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "数据源Sql，语句可包含变量或占位符\n变量：@userid @username @[列名]\n占位符：#input#，输入的过滤串", RowSpan = 2 };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "Sql.SqlStr",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
                Placeholder = "SELECT\r\n\ttitle\r\nFROM\r\n\tdemo_tbl\r\nWHERE\r\n\tparent_id = @[parentid]\r\n    AND name LIKE '#input#%'\r\n    AND id = @RptValueCall.GetMaxID(demo_tbl)\r\n    AND owner = @userid",
            };
            p_items.Add(text);

            var btn = new Button { Content = "美化SQL", HorizontalAlignment = HorizontalAlignment.Right };
            btn.Click += (s, e) =>
            {
                var val = text.Val;
                if (val != null && !string.IsNullOrEmpty(text.Val.ToString()))
                {
                    text.Val = SqlFormatter.Format(text.Val.ToString());
                }
            };
            bar.Content = btn;

            var ct = new CText
            {
                ID = "Sql.LocalDb",
                ShowTitle = false,
                ColSpan = 0.5,
                Placeholder = "本地sqlite库名",
            };
            p_items.Add(ct);

            ct = new CText
            {
                ID = "Sql.Svc",
                ShowTitle = false,
                ColSpan = 0.5,
                Placeholder = "服务名，空为当前服务",
            };
            p_items.Add(ct);
        }
    }
}