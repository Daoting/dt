#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class FvData : Win
    {
        public FvData()
        {
            InitializeComponent();
        }

        void OnNewLocal(object sender, Mi e)
        {
            _fv1.Data = new ClientLog(Content: "hdt", Ctime: DateTime.Now);
        }

        async void OnLocalSave(object sender, Mi e)
        {
            if (await AtState.Save((ClientLog)_fv1.Data, false))
            {
                _fv1.AcceptChanges();
                Kit.Msg("本地库保存成功！");
            }
            else
            {
                Kit.Msg("本地库保存失败！");
            }
        }

        void OnQueryLocal(object sender, Mi e)
        {
            Table tbl = AtState.Query("select * from ClientLog limit 1");
            if (tbl.Count > 0)
                _fv1.Data = tbl[0];
            else
                Kit.Msg("本地库无数据！");
        }

        async void OnLocalDel(object sender, Mi e)
        {
            if (await Kit.Confirm("确认要删除码？"))
            {
                if (await AtState.Delete((ClientLog)_fv1.Data))
                    _fv1.Data = null;
            }
        }
    }
}