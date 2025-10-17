#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业，公共部分
    /// </summary>
    public static partial class BgJob
    {
        const string _bgJobType = "BackgroundJobType";

        /// <summary>
        /// 后台任务运行入口
        /// 此方法不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        /// <returns></returns>
        public static async Task Run()
        {
            // 因后台任务独立运行，类型需要从State库获取！
            IBackgroundJob bgJob = null;
            string tpName = await CookieX.Get(_bgJobType);
            if (!string.IsNullOrEmpty(tpName))
            {
                Type tp = Type.GetType(tpName);
                if (tp != null)
                    bgJob = Activator.CreateInstance(tp) as IBackgroundJob;
            }

            if (bgJob != null)
            {
                try
                {
                    // 可能已初始化，比如android刚启动时
                    if (string.IsNullOrEmpty(GlobalConfig.Server))
                    {
                        await GlobalConfig.Load(false);
                        At.InitConfig(false);
                        BgJobKit.Log("后台加载配置");
                    }

                    await bgJob.Run();
                    BgJobKit.Log("后台结束");
                }
                catch (Exception ex)
                {
                    BgJobKit.Log($"后台异常：\n{ex.Message}\n");
                }
            }
            else
            {
                Unregister();
                BgJobKit.Log("无后台内容，已注销！");
            }
        }
    }
}