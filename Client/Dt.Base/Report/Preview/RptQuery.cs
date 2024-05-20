#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表查询面板
    /// </summary>
    public partial class RptQuery : UserControl
    {
        protected Row _row;
        QueryFv _fv;

        public RptQuery()
        { }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<Row> Query;

        public virtual QueryFv Fv => _fv;

        internal void LoadData(Row p_row, QueryFv p_fv)
        {
            _row = p_row;

            // 根据xaml创建的查询面板
            if (p_fv != null)
            {
                _fv = p_fv;
                Content = _fv;
            }
            
            var fv = Fv;
            fv.Data = _row;
            fv.Query += OnQuery;
            OnInit();
        }

        protected virtual void OnInit()
        { }

        void OnQuery(QueryClause clause)
        {
            Query?.Invoke(_row);
        }
    }
}