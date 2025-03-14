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
            
            p_xw.WriteEndElement();
        }
    }
}