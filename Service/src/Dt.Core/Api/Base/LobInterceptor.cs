#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Castle.DynamicProxy;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 业务线处理拦截器
    /// </summary>
    public class LobInterceptor : IInterceptor
    {
        public void Intercept(IInvocation p_invocation)
        {
            var lc = LobContext.Current;

            // 放行内嵌方法
            if (lc.IsIntercepted())
            {
                p_invocation.Proceed();
                return;
            }

            bool suc = true;
            var type = p_invocation.Method.ReturnType;
            try
            {
                p_invocation.Proceed();

                // 异步时等待外部调用结束，否则数据库连接过早关闭！
                if (type == typeof(Task) || typeof(Task).IsAssignableFrom(type))
                    ((Task)p_invocation.ReturnValue).Wait();
            }
            catch
            {
                suc = false;
                throw;
            }
            finally
            {
                lc.Complete(suc).Wait();
            }
        }
    }
}
