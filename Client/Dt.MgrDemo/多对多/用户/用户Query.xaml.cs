#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public sealed partial class 用户Query : Tab
    {
        public 用户Query()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.Add<string>("手机号");
            row.Add<string>("姓名");
            row.Add<string>("密码");

            _fv.Data = row;
        }
        #endregion
    }
}
