#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    [View("普通")]
    public partial class 普通Win : Win
    {
        readonly 普通Form _parentForm;
        普通大儿Form _大儿Form;
        普通小儿Form _小儿Form;
    
        public 普通Win()
        {
            InitializeComponent();
            _parentForm = new 普通Form { OwnWin = this };
            Attach();
        }
        
        void Attach()
        {
            _query.Query += e =>
            {
                _parentList.Query(e);
                NaviTo(_parentList.Title);
            };
            
            _parentList.Msg += e => _ = _parentForm.Query(e);
            _parentList.Navi += () => NaviTo(_大儿List.Title + "," + _小儿List.Title);

            _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
            _parentForm.UpdateRelated += e => 
            {
                _大儿List.Query(e.ID);
                _小儿List.Query(e.ID);
            };

            _大儿List.Msg += e =>
            {
                if (e.Action == FormAction.Open && _大儿Form == null)
                {
                    _大儿Form = new 普通大儿Form { OwnWin = this };
                    _大儿Form.UpdateList += e => _ = _大儿List.Refresh(e.ID);
                }
                _ = _大儿Form?.Query(e);
            };

            _小儿List.Msg += e =>
            {
                if (e.Action == FormAction.Open && _小儿Form == null)
                {
                    _小儿Form = new 普通小儿Form { OwnWin = this };
                    _小儿Form.UpdateList += e => _ = _小儿List.Refresh(e.ID);
                }
                _ = _小儿Form?.Query(e);
            };
        }
    }
}