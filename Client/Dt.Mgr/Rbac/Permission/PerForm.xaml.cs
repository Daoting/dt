#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class PerForm : Tab
    {
        long _funcID;

        public PerForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id, long p_funcID)
        {
            _funcID = p_funcID;
            if (p_id > 0)
            {
                Data = await PermissionX.GetByID(p_id);
            }
            else
            {
                Create();
            }
        }

        public bool IsChanged { get; private set; }

        async void Create()
        {
            Data = await PermissionX.New(FuncID: _funcID);
        }

        async void Save()
        {
            if (await Data.Save())
                IsChanged = true;
        }

        PermissionX Data
        {
            get { return _fv.Data.To<PermissionX>(); }
            set { _fv.Data = value; }
        }
    }
}
