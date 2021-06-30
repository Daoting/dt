#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Castle.DynamicProxy;
using Dt.Core;
using Serilog;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试", Interceptors = new Type[] { typeof(Interceptor1), typeof(Interceptor2) })]
    public class TestIntercept : BaseApi
    {
        public virtual Task<string> NoTrans()
        {
            return GetSql();
        }

        /// <summary>
        /// 不拦截内嵌方法
        /// </summary>
        /// <returns></returns>
        [Transaction]
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
            return _dp.GetScalar<string>($"select `sql` from {Kit.SvcName}_sql");
        }
    }

    /// <summary>
    /// 拦截器
    /// </summary>
    public class Interceptor1 : IInterceptor
    {
        bool isIntercepted = false;

        public async void Intercept(IInvocation p_invocation)
        {
            // 只拦截一次
            if (isIntercepted)
            {
                Log.Information("1号拦截器放行 " + p_invocation.Method.Name);
                p_invocation.Proceed();
                return;
            }

            isIntercepted = true;
            Log.Information("1号拦截器已拦截 " + p_invocation.Method.Name + " 用户 " + Kit.ContextUserID);
            var type = p_invocation.Method.ReturnType;
            try
            {
                p_invocation.Proceed();
                // 异步时等待外部调用结束
                if (type == typeof(Task) || typeof(Task).IsAssignableFrom(type))
                    await ((Task)p_invocation.ReturnValue);
            }
            catch
            {
                throw;
            }
            finally
            {
                Log.Information("1号拦截器结束 " + p_invocation.Method.Name);
            }
        }
    }

    public class Interceptor2 : IInterceptor
    {
        bool isIntercepted = false;

        public async void Intercept(IInvocation p_invocation)
        {
            if (isIntercepted)
            {
                Log.Information("2号拦截器放行 " + p_invocation.Method.Name);
                p_invocation.Proceed();
                return;
            }

            isIntercepted = true;
            Log.Information("2号拦截器已拦截 " + p_invocation.Method.Name);
            var type = p_invocation.Method.ReturnType;
            try
            {
                p_invocation.Proceed();
                // 异步时等待外部调用结束
                if (type == typeof(Task) || typeof(Task).IsAssignableFrom(type))
                    await ((Task)p_invocation.ReturnValue);
            }
            catch
            {
                throw;
            }
            finally
            {
                Log.Information("2号拦截器结束 " + p_invocation.Method.Name);
            }
        }
    }
}
