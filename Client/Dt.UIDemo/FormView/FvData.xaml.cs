#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.UIDemo
{
    public partial class FvData : Win
    {
        public FvData()
        {
            InitializeComponent();
        }

        void OnNewLocal(object sender, Mi e)
        {
            _fv1.Data = new Cookie("键名");
        }

        async void OnLocalSave(object sender, Mi e)
        {
            if (await ((Cookie)_fv1.Data).Save(false))
            {
                _fv1.AcceptChanges();
                Kit.Msg("本地库保存成功！");
            }
            else
            {
                Kit.Msg("本地库保存失败！");
            }
        }

        async void OnQueryLocal(object sender, Mi e)
        {
            var tbl = await AtState.Query<Cookie>("select * from Cookie limit 1");
            if (tbl.Count > 0)
                _fv1.Data = tbl[0];
            else
                Kit.Msg("本地库无数据！");
        }

        async void OnLocalDel(object sender, Mi e)
        {
            if (await Kit.Confirm("确认要删除码？"))
            {
                if (await ((Cookie)_fv1.Data).Delete())
                    _fv1.Data = null;
            }
        }
    }
}