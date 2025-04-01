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
        /// <summary>
        /// 设计时用，xaml中定义的对象列表
        /// </summary>
        public string ItemsXaml
        {
            get { return Dt.Base.Ex.GetItemsXaml(this); }
            set { Dt.Base.Ex.SetItemsXaml(this, value); }
        }

        /// <summary>
        /// 设计时用，xaml定义Sql
        /// </summary>
        public Sql SqlXaml
        {
            get { return Dt.Base.Ex.GetSqlXaml(this); }
            set { Dt.Base.Ex.SetSqlXaml(this, value); }
        }

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

            if (!string.IsNullOrEmpty(ItemsXaml))
            {
                p_xw.WriteStartElement("a", "CList.Items", null);
                FvDesignKit.CopyXml(p_xw, ItemsXaml);
                p_xw.WriteEndElement();
            }

            if (SqlXaml != null && !string.IsNullOrEmpty(SqlXaml.SqlStr))
            {
                p_xw.WriteStartElement("a", "CList.Sql", null);
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
            AddViewDesignCells(p_items);
            AddItemsDesignCells(p_items);

            // 空时无法绑定
            if (SqlXaml == null)
                SqlXaml = new Sql();

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
                else if (node.LocalName == "CList.Sql"
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
                else if (!node.LocalName.StartsWith("CList."))
                {
                    ViewXaml = FvDesignKit.GetNodeXml(node, false);
                }
            }
        }

        internal static void AddViewDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "行视图" };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "ViewXaml",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
            };
            p_items.Add(text);

            var btn = new Button { Content = "+模板", HorizontalAlignment = HorizontalAlignment.Right };
            Menu m = new Menu { Placement = MenuPosition.BottomLeft };
            Mi mi = new Mi { ID = "Cols定义" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<a:Cols>\n  <a:Col ID=\"xm\" Title=\"姓名\" />\n</a:Cols>");
            m.Add(mi);

            mi = new Mi { ID = "Col" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<a:Col ID=\"xm\" Title=\"姓名\" />");
            m.Add(mi);

            mi = new Mi { ID = "行模板" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text,
@"<DataTemplate>
    <Grid Padding=""6"">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""50"" />
            <ColumnDefinition Width=""*"" />
        </Grid.ColumnDefinitions>
        <a:Dot ID=""icon"" />
        <StackPanel Margin=""10,0,0,0"" Grid.Column=""1"">
            <a:Dot ID=""xm"" />
            <a:Dot ID=""xb"" />
        </StackPanel>
    </Grid>
</DataTemplate>");
            m.Add(mi);

            Dt.Base.Ex.SetMenu(btn, m);
            bar.Content = btn;
        }

        void AddItemsDesignCells(FvItems p_items)
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
        }

        internal static void AddSqlDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "数据源Sql，可包含变量或占位符" };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "SqlXaml.SqlStr",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
                Placeholder = "1. 变量以@开头，以Sql参数方式查询\n2. 占位符首尾添加#，查询前自动替换占位符\n3. @属性名  #属性名#    取Fv数据源的属性值\n4. @{userid}  #{userid}#    当前用户ID\n5. @{userid}  #{userid}#    当前用户ID\n6. @{input}  #{input}#    CPick中输入的过滤串\n7. @类名.方法(参数或无)    #类名.方法(参数或无)#\n    调用有ValueCall标签的类的静态方法取值",
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
                ID = "SqlXaml.LocalDb",
                ShowTitle = false,
                ColSpan = 0.5,
                Placeholder = "本地sqlite库名",
            };
            p_items.Add(ct);

            ct = new CText
            {
                ID = "SqlXaml.Svc",
                ShowTitle = false,
                ColSpan = 0.5,
                Placeholder = "服务名，空为当前服务",
            };
            p_items.Add(ct);
        }
    }
}