#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表查询基类
    /// </summary>
    public abstract partial class RptSearchMv : Mv
    {
        protected readonly RptInfo _info;
        protected readonly Row _row;

        public RptSearchMv(RptInfo p_info)
        {
            Title = "查询";
            _info = p_info;
            _row = _info.BuildParamsRow();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<RptInfo> Query;

        /// <summary>
        /// 查询命令
        /// </summary>
        protected void OnQuery(object sender, Mi e)
        {
            _info.UpdateParams(_row);
            Query?.Invoke(this, _info);
        }

        /// <summary>
        /// 重置命令
        /// </summary>
        protected void OnReset(object sender, Mi e)
        {
            _row.RejectChanges();
        }
    }
}