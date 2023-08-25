#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public sealed partial class HistoryLogList : Tab
    {
        public HistoryLogList()
        {
            InitializeComponent();
        }

        public Table Data
        {
            get
            {
                return (Table)_lv.Data;
            }
            set
            {
                _lv.Data = value;
            }
        }

        void OnOutputClick(object sender, ItemClickArgs e)
        {
            //var item = e.Row;
            //var d = new TraceLogData
            //{
            //    Time = item.Time,
            //    Level = item.Log.Level.ToString(),
            //    Detial = item.Detial,
            //};

            //if (item.Log.Properties.TryGetValue("SourceContext", out var val))
            //{
            //    d.Source = val.ToString("l", null);
            //}
            //else
            //{
            //    d.Source = "未知";
            //}

            //if (item.Log.Properties.TryGetValue("Title", out var vtitle)
            //    && vtitle.ToString("l", null) is string msg)
            //{
            //    d.Title = msg;
            //}

            //_win.Form.Update(d);
        }

        HistoryLogWin _win => OwnWin as HistoryLogWin;
    }
}
