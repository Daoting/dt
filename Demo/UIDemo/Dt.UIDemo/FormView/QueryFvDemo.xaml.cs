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
            _fvFull.Data = _fv;
            _fv.CellClick += (e) => _fvCell.Data = e;
            _fv.Query += OnQuery;
            LoadData();
        }

        void OnQuery(QueryClause e)
        {
            Kit.Msg("执行查询");
        }

        void LoadData()
        {
            var row = new Row();
            row.Add<string>("字符串");
            row.Add<double>("数值");
            row.Add<DateTime>("时间_min");
            row.Add<DateTime>("时间_max");
            row.Add<int>("值_min");
            row.Add<int>("值_max");
            row.Add<string>("忽略");
            _fv.Data = row;
        }
    }
}