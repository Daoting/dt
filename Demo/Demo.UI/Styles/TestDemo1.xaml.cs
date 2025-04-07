#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Dt.Base.Views;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();
        }


        void OnTest1(object sender, RoutedEventArgs e)
        {
            OneToManyCfg cfg = new OneToManyCfg();
            cfg.ParentCfg = new EntityCfg { Cls = "Demo.Base.父表X,Demo.Base" };
            cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.大儿X,Demo.Base", ParentID = "parent_id" });
            cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.小儿X,Demo.Base", ParentID = "group_id" });
            Kit.OpenView("通用一对多视图", "Test", p_params: cfg);
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            //OneToManyCfg cfg = new OneToManyCfg();
            //cfg.ParentCfg = new EntityCfg { Cls = "Demo.Base.父表X,Demo.Base" };
            //cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.大儿X,Demo.Base" });
            //cfg.ChildCfgs.Add(new EntityCfg { Cls = "Demo.Base.小儿X,Demo.Base" });
            //OneToManyWin win = new OneToManyWin(cfg);
            //Dlg dlg = new Dlg { IsPinned = true };
            //dlg.LoadWin(win);
            //dlg.Show();
            //new TestDlg().Show();
            CList ls = new CList { Items = { "111", "222", "333" } };
            //new ListDlg(ls).Show();
        }
    }
    
    
}