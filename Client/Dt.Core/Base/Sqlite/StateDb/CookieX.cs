﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端Cookie字典
    /// </summary>
    public partial class CookieX
    {
        /// <summary>
        /// 查询本地存储的Cookie值，值为null时返回string.Empty
        /// </summary>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static async Task<string> Get(string p_key)
        {
            var cc = await GetByID(p_key);
            return cc == null || cc.Val == null ? string.Empty : cc.Val;
        }

        /// <summary>
        /// 保存Cookie
        /// </summary>
        /// <param name="p_key">键</param>
        /// <param name="p_val">值</param>
        /// <returns></returns>
        public static async Task<bool> Save(string p_key, string p_val)
        {
            var c = await GetByID(p_key);
            if (c == null)
            {
                c = new CookieX(p_key, p_val);
            }
            else
            {
                c.Val = p_val;
            }
            if (c.IsAdded || c.IsChanged)
            {
                return await c.Save(false);
            }
            return true;
        }

        /// <summary>
        /// 是否启用后台作业，默认 true
        /// </summary>
        public static async Task<bool> IsEnableBgJob()
        {
            var cc = await GetByID("DisableBgJob");
            return cc == null;
        }

        /// <summary>
        /// 设置是否启用后台作业
        /// </summary>
        /// <param name="p_enable"></param>
        public static async Task SetEnableBgJob(bool p_enable)
        {
            var cc = await GetByID("DisableBgJob");
            if (cc == null)
            {
                if (!p_enable)
                {
                    await Save("DisableBgJob", "true");
                }
            }
            else if (p_enable)
            {
                await DelByID("DisableBgJob", true, false);
            }
        }

        /// <summary>
        /// 查询自启动信息
        /// </summary>
        /// <returns></returns>
        internal static async Task<AutoStartInfo> GetAutoStart()
        {
            try
            {
                var cc = await GetByID("AutoStart");
                if (cc != null && !string.IsNullOrEmpty(cc.Val))
                    return JsonSerializer.Deserialize<AutoStartInfo>(cc.Val);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 保存自启动信息
        /// </summary>
        /// <param name="p_info"></param>
        internal static async Task SaveAutoStart(AutoStartInfo p_info)
        {
            if (p_info != null)
            {
                string json = JsonSerializer.Serialize(p_info, JsonOptions.UnsafeSerializer);
                var cc = await GetByID("AutoStart");
                if (cc == null)
                {
                    await CookieX.Save("AutoStart", json);
                }
                else
                {
                    cc.Val = json;
                    await cc.Save(false);
                }
            }
        }

        /// <summary>
        /// 删除自启动信息
        /// </summary>
        internal static async Task DelAutoStart()
        {
            await DelByID("AutoStart", true, false);
        }
    }
}
