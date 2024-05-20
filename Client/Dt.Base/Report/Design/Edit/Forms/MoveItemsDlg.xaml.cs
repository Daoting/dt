#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.Report
{
    public partial class MoveItemsDlg : Dlg
    {
        public MoveItemsDlg()
        {
            InitializeComponent();

            if (!Kit.IsPhoneUI)
            {
                Width = 300;
                Height = 200;
            }
            _fv.Data = new Row { { "x", 0 }, { "y", 0 } };
        }

        public int DeltaX => _fv.Row.Int("x");

        public int DeltaY => _fv.Row.Int("y");
        
        void OnOk()
        {
            Close(true);
        }
    }
}