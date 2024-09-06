#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 物资目录X;
    
    public sealed partial class 物资目录Form : Form
    {
        long? _flID;
        
        public 物资目录Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await A.New(分类id: _args.ParentID ?? _flID);
        }

        protected override async Task OnGet()
        {
            var x = await A.View1.GetByID(_args.ID);
            _flID = x?.分类id;
            _fv.Data = x;
        }
    }
}