#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-31 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    public sealed partial class WfDeleteMenu : Menu
    {
        Sketch _sketch;

        public WfDeleteMenu(Sketch p_sketch)
        {
            InitializeComponent();
            _sketch = p_sketch;
        }

        void OnDel(Mi e)
        {
            _sketch.DeleteSelection();
        }
    }
}
