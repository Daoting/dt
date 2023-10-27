#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class PerFuncForm : Tab
    {
        long _moduleID;

        public PerFuncForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id, long p_moduleID)
        {
            _moduleID = p_moduleID;
            if (p_id > 0)
            {
                Data = await PermissionFuncX.GetByID(p_id);
            }
            else
            {
                Create();
            }
        }

        public bool IsChanged { get; private set; }

        async void Create()
        {
            Data = await PermissionFuncX.New(ModuleID: _moduleID);
        }

        async void Save()
        {
            if (await Data.Save())
                IsChanged = true;
        }

        PermissionFuncX Data
        {
            get { return _fv.Data.To<PermissionFuncX>(); }
            set { _fv.Data = value; }
        }
    }
}
