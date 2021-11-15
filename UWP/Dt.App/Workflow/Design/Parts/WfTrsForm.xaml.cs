#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 迁移属性表单
    /// </summary>
    public sealed partial class WfTrsForm : UserControl
    {
        Table<WfdTrsObj> _trss;
        WfdTrsObj _curTrs;

        public WfTrsForm()
        {
            InitializeComponent();
        }

        public void LoadData(Table<WfdTrsObj> p_trss, WfdTrsObj p_trs)
        {
            _trss = p_trss;
            _curTrs = p_trs;
            _cbBack.IsChecked = (from item in _trss.OfType<WfdTrsObj>()
                                 where item.TrsID == _curTrs.ID
                                 select item).Any();
        }

        async void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (_cbBack.IsChecked == true)
            {
                WfdTrsObj trs = new WfdTrsObj(
                    ID: await AtCm.NewID(),
                    PrcID: _curTrs.PrcID,
                    IsRollback: true,
                    SrcAtvID: _curTrs.TgtAtvID,
                    TgtAtvID: _curTrs.SrcAtvID,
                    TrsID: _curTrs.ID);
                _trss.Add(trs);
            }
            else
            {
                var trs = (from item in _trss.OfType<WfdTrsObj>()
                           where item.TrsID == _curTrs.ID
                           select item).FirstOrDefault();
                if (trs != null)
                {
                    _trss.Remove(trs);
                }
            }
        }
    }
}
