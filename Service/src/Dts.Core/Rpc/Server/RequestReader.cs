#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 读取客户端请求流
    /// </summary>
    public class RequestReader
    {
        LobContext _lc;
        string _originalVal;

        internal RequestReader(LobContext p_lc)
        {
            _lc = p_lc;
        }

        public async Task<bool> MoveNext()
        {
            var reader = _lc.Context.Request.BodyReader;
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;
            if (buffer.IsEmpty)
                return false;

            byte[] data = buffer.ToArray();
            _originalVal = Encoding.UTF8.GetString(data, 0, data.Length);
            reader.AdvanceTo(buffer.End);
            return true;
        }

        public T GetVal<T>()
        {
            throw new NotImplementedException();
        }
    }
}
