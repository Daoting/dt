#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设计时
    /// </summary>
    public partial class Lv
    {
        #region 静态内容
        public readonly static DependencyProperty ViewXamlProperty = DependencyProperty.Register(
            "ViewXaml",
            typeof(string),
            typeof(Lv),
            new PropertyMetadata(null));
        #endregion
        
        /// <summary>
        /// 设计时用，行视图的xaml
        /// </summary>
        public string ViewXaml
        {
            get { return (string)GetValue(ViewXamlProperty); }
            set { SetValue(ViewXamlProperty, value); }
        }

        /// <summary>
        /// 导出Lv的xaml
        /// </summary>
        /// <returns></returns>
        public string ExportXaml()
        {
            var sb = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, Indent = true }))
            {
                // 可能为QueryFv
                xw.WriteStartElement("a", "Lv", "dt");
                xw.WriteAttributeString("xmlns", "x", null, "xaml");

                if (ViewMode != ViewMode.Auto)
                    xw.WriteAttributeString("ViewMode", ViewMode.ToString());

                if (SelectionMode != SelectionMode.Single)
                    xw.WriteAttributeString("SelectionMode", SelectionMode.ToString());

                if (!ShowGroupHeader)
                    xw.WriteAttributeString("ShowGroupHeader", "false");
                
                if (!string.IsNullOrEmpty(GroupName))
                    xw.WriteAttributeString("GroupName", GroupName);

                if (ItemHeight != 0)
                    xw.WriteAttributeString("ItemHeight", ItemHeight.ToString());
                
                if (!ShowItemBorder)
                    xw.WriteAttributeString("ShowItemBorder", "false");
                
                if (ShowListHeader)
                    xw.WriteAttributeString("ShowListHeader", "true");
                if (ShowDotBorder)
                    xw.WriteAttributeString("ShowDotBorder", "true");
                
                if (AutoCreateCol)
                    xw.WriteAttributeString("AutoCreateCol", "true");
                
                if (MinItemWidth != 160)
                    xw.WriteAttributeString("MinItemWidth", MinItemWidth.ToString());
                
                if (PullToRefresh)
                    xw.WriteAttributeString("PullToRefresh", "true");
                
                if (AutoScrollBottom)
                    xw.WriteAttributeString("AutoScrollBottom", "true");
                
                if (ShowReportMenu)
                    xw.WriteAttributeString("ShowReportMenu", "true");

                if (AutoSaveCols)
                    xw.WriteAttributeString("AutoSaveCols", "true");

                if (!string.IsNullOrEmpty(ViewXaml))
                    FvDesignKit.CopyXml(xw, ViewXaml);
                
                xw.WriteEndElement();
                xw.Flush();
            }

            // 去掉冗余的命名空间，Kit.LoadXaml已自动添加
            return sb.Replace(" xmlns:a=\"dt\"", "").Replace(" xmlns:x=\"xaml\"", "").ToString();
        }
    }
}