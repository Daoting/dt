#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-03-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.FormView
{
    public partial class SelectFvCellDlg : Dlg
    {
        readonly Row _row;
        
        public SelectFvCellDlg()
        {
            InitializeComponent();

            _row = new Row { { "Type", typeof(Type) }, { "ID", "" } };
            //_row[0] = typeof(CText);
            _fv.Data = _row;
            
            if (!Kit.IsPhoneUI)
            {
                Width = 400;
                Height = 350;
            }
        }

        public Row Row => _row;
        
        void OnLoadType(CList arg1, AsyncArgs arg2)
        {
            arg1.Data = new Nl<Type>
            {
                typeof(CBar),
                typeof(CText),
                typeof(CList),
                typeof(CNum),
                typeof(CDate),
                typeof(CBool),
                typeof(CPick),
                typeof(CTree),
                typeof(CTip),
                typeof(CIcon),
                typeof(CFile),
                typeof(CLink),
                typeof(CImage),
                typeof(CColor),
                typeof(CHtml),
                typeof(CMarkdown),
                typeof(CMask),
                typeof(CPassword),
            };
        }

        void OnSave()
        {
            if (_row[0] == null)
            {
                _fv[0].Warn("请选择格类型！");
                return;
            }
            
            
        }
    }
}