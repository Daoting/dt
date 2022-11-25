#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace $rootnamespace$
{
    [MidVal]
    public class $clsname$ : IMidVal
    {
        public object Get(Mid m)
        {
            return m.Val;
        }

        public object Set(Mid m)
        {
            return m.Val;
        }
    }
}