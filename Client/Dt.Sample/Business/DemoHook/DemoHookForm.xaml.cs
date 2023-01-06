#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Sample
{
    public partial class DemoHookForm : Mv
    {
        public DemoHookForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                _fv.Data = await DemoHookObj.GetByID(p_id);
            }
            else
            {
                NewData();
            }
        }

        void OnNoBinding(object sender, RoutedEventArgs e)
        {
            _fv.Data.To<DemoHookObj>().NoBinding = new Random().Next(100);
        }

        void OnNoHook(object sender, RoutedEventArgs e)
        {
            _fv.Data.To<DemoHookObj>().NoHook = new Random().Next(100);
        }

        async void NewData()
        {
            _fv.Data = await DemoHookObj.New();
        }

        void OnAdd(object sender, Mi e)
        {
            NewData();
        }

        async void OnSave(object sender, Mi e)
        {
            var d = _fv.Data.To<DemoHookObj>();
            if (await AtCm.Save(d))
            {
                _win.List.Update();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<DemoHookObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                _fv.Data = null;
                return;
            }

            if (await AtCm.Delete(d))
            {
                _fv.Data = null;
                _win.List.Update();
            }
        }

        DemoHookWin _win => (DemoHookWin)_tab.OwnWin;
    }
}