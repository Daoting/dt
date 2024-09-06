#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dt.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Demo.Lob
{
    [View("业务样例")]
    public partial class DemoMain : Win
    {
        public DemoMain()
        {
            InitializeComponent();
            LoadBase();
        }

        void LoadBase()
        {
            var ds = new Nl<GroupData<Nav>>();
            var group = new GroupData<Nav>
            {
                new Nav("共享视图", Icons.命令) { Desc ="项目之间共享带有[View]标签的窗口或实现 IView 的类型", Callback = (s, n) => Kit.OpenView("基础") },
                new Nav("共享Tab", typeof(ShareTabWin), Icons.文件) { Desc ="项目之间共享带有[Share]标签的List Form等Tab类型" },
                new Nav("共享Form", Icons.命令) { Desc ="用共享Form显示数据，并禁止修改", Callback = async (s, n) =>
                {
                    var en = await 基础X.First(null);
                    if (en != null)
                    {
                        var form = Kit.GetShareObj<Form>("Crud基础Form");
                        form.Menu = null;
                        form.MainFv.IsReadOnly = true;
                        await form.Query(en.ID, true);
                    }
                } },
                
            };
            group.Title = "类型共享";
            ds.Add(group);
            
            _navBase.Data = ds;
        }
    }
}