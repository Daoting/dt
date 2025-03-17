#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
using System.Text;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 设计时
    /// </summary>
    public partial class FvCell
    {
        /// <summary>
        /// 导出xaml
        /// </summary>
        /// <param name="p_xw"></param>
        public void ExportXaml(XmlWriter p_xw)
        {
            if (p_xw == null)
                return;

            Type tp = GetType();
            p_xw.WriteStartElement("a", tp.Name, null);

            if (ID != null)
                p_xw.WriteAttributeString("ID", ID);
            if (Title != null)
                p_xw.WriteAttributeString("Title", Title);

            foreach (var prop in FvDesignKit.GetCellProps(tp))
            {
                var val = prop.Info.GetValue(this);
                // 默认值不写入xaml
                if (!object.Equals(val, prop.DefaultValue))
                    p_xw.WriteAttributeString(prop.Info.Name, val.ToString());
            }
            ExportCustomXaml(p_xw);
            
            p_xw.WriteEndElement();
        }

        protected virtual void ExportCustomXaml(XmlWriter p_xw)
        {
        }
        
        /// <summary>
        /// 创建当前单元格设计时的属性单元格
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        public virtual FvCell CreateDesignCell(CellPropertyInfo p_info)
        {
            var fc = Fv.CreateCell(p_info.Info.PropertyType, p_info.Info.Name);
            if (p_info.Info.PropertyType == typeof(bool))
            {
                fc.Title = p_info.Title;
                fc.ShowTitle = false;
                fc.ColSpan = 0.5;
            }
            else
            {
                var length = Kit.GetByteCount(p_info.Title);
                if (length > 26)
                {
                    fc.ShowTitle = false;
                }
                else if (length > 13)
                {
                    fc.Title = p_info.Title;
                    fc.TitleWidth = 240;
                }
                else
                {
                    fc.Title = p_info.Title;
                }
            }
            return fc;
        }

        /// <summary>
        /// 添加自定义设计时属性单元格
        /// </summary>
        /// <param name="p_items"></param>
        public virtual void AddCustomDesignCells(FvItems p_items)
        {
        }
    }
}