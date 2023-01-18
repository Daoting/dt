#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using System.Text;
#endregion

namespace Dt.UIDemo
{
    public partial class QueryFvDemo : Win
    {
        public QueryFvDemo()
        {
            InitializeComponent();
            _fv.CellClick += (s, e) => _fvCell.Data = e;
            _fv.Query += OnQuery;
            LoadData();
        }

        void OnQuery(object sender, QueryClause e)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            foreach (var item in e.Params)
            {
                sb.AppendLine($"{item.Key}：{item.Value}");
            }
            Kit.Msg(e.Where + sb.ToString());
        }

        void LoadData()
        {
            var row = new Row();
            row.AddCell<string>("字符串");
            row.AddCell<double>("数值");
            row.AddCell<DateTime>("时间_min");
            row.AddCell<DateTime>("时间_max");
            row.AddCell<int>("值_min");
            row.AddCell<int>("值_max");
            row.AddCell<string>("忽略");
            _fv.Data = row;
        }
    }
}