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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    public sealed partial class WfDeleteMenu : Menu
    {
        Sketch _sketch;

        public WfDeleteMenu(Sketch p_sketch)
        {
            InitializeComponent();
            _sketch = p_sketch;
        }

        void OnDel(object sender, Mi e)
        {
            _sketch.DeleteSelection();
        }
    }
}
