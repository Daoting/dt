#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// 单体服务时本地直接调用
    /// </summary>
    class NativeApiInvoker
    {
        object _result;

        public async Task<T> Call<T>(string p_methodName, params object[] p_params)
        {
            ApiMethod sm = Silo.GetMethod(p_methodName);
            if (sm == null)
                throw new Exception($"未找到Api[{p_methodName}]");

            var mi = sm.Method;
            var tgt = Kit.GetObj(mi.DeclaringType) as BaseApi;
            if (tgt == null)
                throw new Exception($"无法创建服务实例，类型[{mi.DeclaringType.Name}]");

            // 本地调用标识
            tgt.UserID = 112;
            tgt.IsTransactional = sm.IsTransactional;
            bool suc = await CallMethod(mi, tgt, p_params);
            // Api调用结束后释放资源
            await tgt.Close(suc);

            return ParseResult<T>();

        }

        async Task<bool> CallMethod(MethodInfo mi, BaseApi tgt, object[] p_params)
        {
            bool suc = true;
            _result = null;
            try
            {
                if (mi.ReturnType == typeof(Task))
                {
                    // 异步无返回值时
                    var task = (Task)mi.Invoke(tgt, p_params);
                    await task;
                }
                else if (typeof(Task).IsAssignableFrom(mi.ReturnType))
                {
                    // 异步有返回值
                    var task = (Task)mi.Invoke(tgt, p_params);
                    await task;
                    _result = task.GetType().GetProperty("Result").GetValue(task);
                }
                else
                {
                    // 调用同步方法
                    _result = mi.Invoke(tgt, p_params);
                }
            }
            catch
            {
                suc = false;
            }

            return suc;
        }

        T ParseResult<T>()
        {
            if (_result == null)
                return default(T);

            Type tp = _result.GetType();
            if (typeof(T) == tp)
            {
                // 结果对象与给定对象类型相同时
                return (T)_result;
            }

            // 特殊处理，将 Row 转 Entity
            if (tp == typeof(Row) && typeof(T).IsSubclassOf(typeof(Entity)))
            {
                // T 是返回值的子类，如 T 为Entity, result为Row
                object entity = ((Row)_result).CloneTo(typeof(T));
                return (T)entity;
            }

            object val;
            try
            {
                val = Convert.ChangeType(_result, typeof(T));
            }
            catch
            {
                throw new Exception(string.Format("无法将【{0}】转换到【{1}】类型！", _result, typeof(T)));
            }
            return (T)val;
        }
    }
}