#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Encodings.Web;
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Json序列化选项
    /// </summary>
    public static class JsonOptions
    {
        /// <summary>
        /// JsonWriter序列化时不对中文和转义字符进行编码
        /// </summary>
        public static readonly JsonWriterOptions UnsafeWriter = new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <summary>
        /// JsonWriter序列化时不对中文和转义字符进行编码，含有缩进
        /// </summary>
        public static readonly JsonWriterOptions IndentedWriter = new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = true };

        /// <summary>
        /// 序列化时不对中文和转义字符进行编码
        /// </summary>
        public static readonly JsonSerializerOptions UnsafeSerializer = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        /// <summary>
        /// 序列化时不对中文和转义字符进行编码，含有缩进
        /// </summary>
        public static readonly JsonSerializerOptions IndentedSerializer = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
    }
}
