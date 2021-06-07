#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 模型工具类
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
                if (await AtKit.Confirm("确认要更新模型吗？"))
                {
                    if (await new UnaryRpc("cm", "ModelMgr.更新模型").Call<bool>())
                        AtKit.Msg("更新模型成功，请重启应用！");
                    else
                        AtKit.Warn("更新模型失败！");
                }
            };
            AtKit.RunAsync(() => SysVisual.NotifyList.Add(notify));
        }
    }
}
