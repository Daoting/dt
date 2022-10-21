#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 存根
    /// </summary>
    public class AppStub : DefaultStub
    {
        public AppStub()
        {
            Title = "搬运工";
            LogSetting.FileEnabled = true;
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            p_svcs.AddSingleton<IRpcConfig, RpcConfig>();
            p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
            p_svcs.AddTransient<IReceiveShare, ReceiveShare>();
            //p_svcs.AddTransient<ITheme, CustomTheme>();
        }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected override async Task OnStartup()
        {
            AtLocal.OpenDb();

            // 初次运行，显示用户协议和隐私政策对话框
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PolicyDlg().ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            ShowRoot(typeof(SamplesMain));
        }

        #region 自动生成
        // 本地库结构变化后，需通过《 win版app -> 系统日志 -> 存根 》重新生成！

        /// <summary>
        /// 合并本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// 先调用base.MergeSqliteDbs，不可覆盖上级的同名本地库
        /// </summary>
        /// <param name="p_sqliteDbs"></param>
        protected override void MergeSqliteDbs(Dictionary<string, SqliteTblsInfo> p_sqliteDbs)
        {
            base.MergeSqliteDbs(p_sqliteDbs);
            p_sqliteDbs["local"] = new SqliteTblsInfo
            {
                Version = "0a68f7fe86b78452e885c5e7394762ca",
                Tables = new List<Type>
                {
                    typeof(Dt.Sample.LocalDict),
                }
            };
        }
        #endregion
    }
}