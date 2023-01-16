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
#endregion

namespace $rootnamespace$
{
    [CListEx]
    public class $clsname$ : CListEx
    {
        public override Task<INotifyList> GetData()
        {
            return Task.FromResult(null);
        }
    }
}