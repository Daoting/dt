#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    [LvCall]
    public class MenuListUI
    {
        public static void Icon(Env e)
        {
            var tb = new TextBlock
            {
                Style = Res.LvTextBlock,
                FontFamily = Res.IconFont,
                TextAlignment = TextAlignment.Center,
            };
            e.UI = tb;

            e.Set += c =>
            {
                var val = c.CellVal;
                string txt = null;
                if (c.Data is MenuX m && m.IsGroup)
                {
                    txt = Res.GetIconChar(Icons.文件夹);
                }
                else if (val != null)
                {
                    if (val is int || val is byte)
                        txt = Res.GetIconChar((Icons)val);
                    else
                        txt = Res.ParseIconChar(val.ToString());
                }
                tb.Text = string.IsNullOrEmpty(txt) ? "" : txt;
                c.Dot.ToggleVisible(string.IsNullOrEmpty(txt));
            };
        }
    }
}