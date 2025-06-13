#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public abstract partial class Stub
    {
        public Stub()
        {
            if (Inst != null)
                throw new Exception("Stub 为单例对象！若重启请使用 Reboot 方法");
            Inst = this;
            
            // 后台任务启动时可能为null
            var app = Application.Current;
            if (app != null)
            {
                var mi = app.GetType().GetMethod("InitDtDictionary", BindingFlags.Public | BindingFlags.Instance);
                if (mi == null)
                    throw new Exception(app.GetType().Name + " 中不包括 InitDtDictionary 方法！");
                mi.Invoke(app, new object[0]);
            }

            var svcs = new ServiceCollection();
            svcs.AddSingleton<ILogSetting, DefLogSetting>();
            svcs.AddSingleton<ITypeAlias, DefTypeAlias>();
            ConfigureServices(svcs);
            Kit.Init(svcs.BuildServiceProvider());
        }
        
        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected virtual void ConfigureServices(IServiceCollection p_svcs) { }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected virtual Task OnStartup() { return Task.CompletedTask; }

        /// <summary>
        /// 内部访问存根实例
        /// </summary>
        internal static Stub Inst { get; private set; }
    }
}