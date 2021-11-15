#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// json字符串处理
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 按照系统规则序列化对象
        /// </summary>
        /// <param name="p_object"></param>
        /// <returns></returns>
        public static string Serialize(object p_object)
        {
            Throw.IfNull(p_object);

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, JsonOptions.UnsafeWriter))
                {
                    JsonRpcSerializer.Serialize(p_object, writer);
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 按照系统规则反序列化json串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_content"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string p_content)
        {
            Throw.IfNullOrEmpty(p_content);

            Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_content));
            reader.Read();
            return JsonRpcSerializer.Deserialize<T>(ref reader);
        }
    }
}