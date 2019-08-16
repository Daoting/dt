#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 系统参数管理类
    /// </summary>
    public static class AtParam
    {
        /*
        /// <summary>
        /// 获取指定参数标识的字符串值，为null时返回string.Empty！！！
        /// </summary>
        /// <param name="p_paramID">参数标识</param>
        /// <returns>字符串值</returns>
        public static Task<string> Str(string p_paramID)
        {
            return GetVal<string>(p_paramID);
        }

        /// <summary>
        /// 获取指定参数标识的bool值
        /// </summary>
        /// <param name="p_paramID">参数标识</param>
        /// <returns>参数值为1时返回true，其他值返回false</returns>
        public static async Task<bool> Bool(string p_paramID)
        {
            return (await GetVal<string>(p_paramID)) == "1";
        }

        /// <summary>
        /// 获取指定参数标识的double值，为null时返回零即default(double)！！！
        /// </summary>
        /// <param name="p_paramID">参数标识</param>
        /// <returns>double值</returns>
        public static Task<double> Double(string p_paramID)
        {
            return GetVal<double>(p_paramID);
        }

        /// <summary>
        /// 获取指定参数标识的整数值，为null时返回零即default(int)！！！
        /// </summary>
        /// <param name="p_paramID">参数标识</param>
        /// <returns>整数值</returns>
        public static Task<int> Int(string p_paramID)
        {
            return GetVal<int>(p_paramID);
        }

        /// <summary>
        /// 获取指定参数标识的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        /// <param name="p_paramID">参数标识</param>
        /// <returns>日期值</returns>
        public static Task<DateTime> Date(string p_paramID)
        {
            return GetVal<DateTime>(p_paramID);
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static async Task<T> GetVal<T>(string p_paramID)
        {
            if (string.IsNullOrEmpty(p_paramID))
                AtKit.Throw("参数标识不能为空！");
            string val = await GetParamVal(AtUser.ID, p_paramID);
            if (string.IsNullOrEmpty(val))
                return default(T);

            Type type = typeof(T);
            if (type == typeof(string))
                return (T)(object)val;

            object result = null;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception(string.Format("参数【{0}】的值转换异常：无法将【{1}】转换到【{2}】类型！", p_paramID, val, type));
            }
            return (T)result;
        }

        #region Params
        /// <summary>
        /// 获取用户的参数值，可能为用户参数或全局参数
        /// </summary>
        /// <param name="p_userID">用户标识</param>
        /// <param name="p_paramID">参数标识</param>
        /// <returns></returns>
        public static Task<string> GetParamVal(string p_userID, string p_paramID)
        {
            return Call<string>(
                "Bus",
                "Params.GetParamVal",
                p_userID,
                p_paramID
            );
        }

        /// <summary>
        /// 获取参数分组内所有参数列表，若为用户分组，自动合并参数值
        /// </summary>
        /// <param name="p_userID">用户标识</param>
        /// <param name="p_groupID">参数分组标识</param>
        /// <param name="p_isSys">是否为全局参数</param>
        /// <returns></returns>
        public static Task<Table> GetParamsByGroup(string p_userID, string p_groupID, bool p_isSys)
        {
            return Call<Table>(
                "Bus",
                "Params.GetParamsByGroup",
                p_userID,
                p_groupID,
                p_isSys
            );
        }

        /// <summary>
        /// 保存参数值
        /// </summary>
        /// <param name="p_isSys">是否为全局参数</param>
        /// <param name="p_paramID">参数标识</param>
        /// <param name="p_val">参数值</param>
        /// <param name="p_userID">用户标识</param>
        /// <returns></returns>
        public static Task<bool> SaveParamVal(bool p_isSys, string p_paramID, string p_val, string p_userID)
        {
            return Call<bool>(
                "Bus",
                "Params.SaveParamVal",
                p_isSys,
                p_paramID,
                p_val,
                p_userID
            );
        }

        /// <summary>
        /// 获取参数组
        /// </summary>
        /// <param name="p_type">0 系统参数组，1 用户参数组，其他 所有参数组</param>
        /// <returns></returns>
        public static Task<Table> GetGroups(int p_type)
        {
            return Call<Table>(
                "Bus",
                "Params.GetGroups",
                p_type
            );
        }
        #endregion
        */
    }
}
