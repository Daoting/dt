#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class PerModuleForm : Tab
    {
        public PerModuleForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            if (p_id > 0)
            {
                Data = await PermissionModuleX.GetByID(p_id);
            }
            else
            {
                Create();
            }
        }

        public bool IsChanged { get; private set; }

        async void Create()
        {
            Data = await PermissionModuleX.New();
        }

        async void Save()
        {
            if (await Data.Save())
                IsChanged = true;
        }

        PermissionModuleX Data
        {
            get { return _fv.Data.To<PermissionModuleX>(); }
            set { _fv.Data = value; }
        }
    }
}
