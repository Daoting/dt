#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端缓存管理者，服务端CacheAccess的代理
    /// </summary>
    public class CacheAccess
    {
        #region 构造方法
        readonly string _svc;

        public CacheAccess(string p_svc)
        {
            _svc = p_svc;
        }
        #endregion

        /// <summary>
        /// 根据键查询字符串类型的缓存值
        /// </summary>
        /// <param name="p_key">完整缓存键，形如"prefix:key"</param>
        /// <returns>缓存字符串</returns>
        public Task<string> StringGet(string p_key)
        {
            return Kit.Rpc<string>(
                _svc,
                "CacheAccess.StringGet",
                p_key
            );
        }

        /// <summary>
        /// 将键值添加到缓存
        /// </summary>
        /// <param name="p_key">完整缓存键，形如"prefix:key"，非空</param>
        /// <param name="p_value">待缓存内容</param>
        /// <returns></returns>
        public Task<bool> StringSet(string p_key, string p_value)
        {
            return Kit.Rpc<bool>(
                _svc,
                "CacheAccess.StringSet",
                p_key,
                p_value
            );
        }

        /// <summary>
        /// 先根据 p_key 查到实体的主键值，再根据主键前缀 p_priKeyPrefix 和主键值查询实体json
        /// </summary>
        /// <param name="p_key">实体非主键列键名：“表名:列名:列值”</param>
        /// <param name="p_priKeyPrefix">查询实体json的键名：“表名:主键列名”</param>
        /// <returns></returns>
        public Task<string> GetEntityJson(string p_key, string p_priKeyPrefix)
        {
            return Kit.Rpc<string>(
                _svc,
                "CacheAccess.GetEntityJson",
                p_key,
                p_priKeyPrefix
            );
        }

        /// <summary>
        /// 根据键名删除缓存内容
        /// </summary>
        /// <param name="p_key">键名</param>
        /// <returns></returns>
        public Task<bool> KeyDelete(string p_key)
        {
            return Kit.Rpc<bool>(
                _svc,
                "CacheAccess.KeyDelete",
                p_key
            );
        }

        /// <summary>
        /// 根据键名列表批量删除缓存
        /// </summary>
        /// <param name="p_keys">键名列表</param>
        /// <returns></returns>
        public Task BatchKeyDelete(List<string> p_keys)
        {
            return Kit.Rpc<object>(
                _svc,
                "CacheAccess.BatchKeyDelete",
                p_keys
            );
        }
    }
}
