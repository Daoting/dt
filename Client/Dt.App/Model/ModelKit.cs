#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 内核模型服务Api代理类（自动生成）
    /// </summary>
    public static class ModelKit
    {
        /// <summary>
        /// 提示需要更新模型
        /// </summary>
        /// <param name="p_msg">提示消息</param>
        public static void PromptForUpdateModel(string p_msg = null)
        {
            var notify = new NotifyInfo();
            notify.Message = string.IsNullOrEmpty(p_msg) ? "需要更新模型才能生效" : p_msg + "，需要更新模型才能生效";
            notify.DelaySeconds = 5;
            notify.Link = "更新模型";
            notify.LinkCallback = async (e) =>
            {
                if (await Kit.Confirm("确认要更新模型吗？"))
                {
                    if (await AtCm.UpdateModelDbFile())
                        Kit.Msg("更新模型成功，请重启应用！");
                    else
                        Kit.Warn("更新模型失败！");
                }
            };
            Kit.RunAsync(() => SysVisual.NotifyList.Add(notify));
        }
    }
}
