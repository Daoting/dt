#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Oa
{
    public partial class 收文表单 : Win, IWfForm
    {
        readonly WfFormInfo _info;

        public 收文表单(WfFormInfo p_info)
        {
            InitializeComponent();

            _info = p_info;
            _tab.Menu = p_info.GetDefaultMenu(_fv);
            //p_info.ApplyMenuCmd(_tab.Menu, _fv);
            Load();
        }

        async void Load()
        {
            if (_info.IsNew)
            {
                _fv.Data = new 收文(
                    ID: _info.ID,
                    来文时间: AtSys.Now);
            }
            else
            {
                _fv.Data = await AtCm.GetByID<收文>(_info.ID);
            }

            switch (_info.State)
            {
                case "接收文件":
                    _fv.Hide("市场部经理意见", "综合部经理意见", "收文完成时间");
                    break;
                case "市场部":
                    _fv.Hide("综合部经理意见", "收文完成时间");
                    break;
                case "综合部":
                    _fv.Hide("市场部经理意见", "收文完成时间");
                    break;
                case "返回收文人":
                    _fv.Hide("收文完成时间");
                    break;
            }
        }

        public Task<bool> Save()
        {
            var data = _fv.Data.To<收文>();
            if (data.IsAdded || data.IsChanged)
                return AtCm.Save(data);

            return Task.FromResult(true);
        }

        public Task<bool> Delete()
        {
            throw new NotImplementedException();
        }

        public string GetPrcName()
        {
            return "请假单";
        }
    }
}