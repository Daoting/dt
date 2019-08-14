#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(true, "功能测试", AgentMode.Generic)]
    public class TestDomain : BaseApi
    {
        [Transaction(false)]
        public virtual Task<string> NoTrans()
        {
            return GetSql();
        }

        /// <summary>
        /// 不拦截内嵌方法
        /// </summary>
        /// <returns></returns>
        public virtual Task<string> CallInline()
        {
            return GetSql();
        }

        /// <summary>
        /// 外层不拦截，拦截内嵌方法
        /// </summary>
        /// <returns></returns>
        public Task<string> NotIntercept()
        {
            return GetSql();
        }

        /// <summary>
        /// 调用过程异常
        /// </summary>
        /// <returns></returns>
        public virtual async Task ThrowException()
        {
            string sql = await GetSql();
            throw new Exception("普通异常");
        }

        public virtual Task<string> GetSql()
        {
            return _c.Db.Scalar<string>($"select `sql` from {Glb.SvcName}_sql");
        }
    }
}
