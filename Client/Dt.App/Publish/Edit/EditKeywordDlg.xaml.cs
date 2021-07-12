#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    public sealed partial class EditKeywordDlg : Dlg
    {
        bool _needRefresh;

        public EditKeywordDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(Keyword p_keyword)
        {
            if (p_keyword != null)
                _fv.Data = await AtPublish.GetByID<Keyword>(p_keyword.ID);
            else
                _fv.Data = Create();
            await ShowAsync();
            return _needRefresh;
        }

        Keyword Create()
        {
            return new Keyword(
                ID: "新关键字",
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }

        async void OnSave(object sender, Mi e)
        {
            if (await AtPublish.Save(_fv.Data.To<Keyword>()))
            {
                _needRefresh = true;
                _fv.Data = Create();
            }
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = Create();
        }

        protected override Task<bool> OnClosing(bool p_result)
        {
            if (_fv.Row.IsChanged)
                return Kit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }
    }
}
