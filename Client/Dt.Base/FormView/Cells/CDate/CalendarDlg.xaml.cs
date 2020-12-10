#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.FormView
{
    public sealed partial class CalendarDlg : Dlg
    {
        public CalendarDlg()
        {
            InitializeComponent();
        }

        public CDate Owner { get; internal set; }

        public void ShowDlg()
        {
            _cv.SelectedDatesChanged -= OnSelectedDatesChanged;
            _cv.SelectedDates.Clear();
            if (Owner.Value != default)
            {
                _cv.SelectedDates.Add(Owner.Value);
                _cv.SetDisplayDate(Owner.Value);
            }
            _cv.SelectedDatesChanged += OnSelectedDatesChanged;
            Show();
        }

        void OnSelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            var val = Owner.Value;
            if (sender.SelectedDates.Count > 0)
                Owner.Value = sender.SelectedDates.FirstOrDefault().Date + (val - val.Date);
            Close();
        }
    }
}
