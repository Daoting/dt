#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 按照Rpc Json数据结构进行序列化和反序列化接口
    /// </summary>
    public interface IRpcJson
    {
        /// <summary>
        /// 反序列化读取Rpc Json数据
        /// </summary>
        void ReadRpcJson(ref Utf8JsonReader p_reader);

        /// <summary>
        /// 将对象按照Rpc Json数据结构进行序列化
        /// </summary>
        void WriteRpcJson(Utf8JsonWriter p_writer);
    }
}
