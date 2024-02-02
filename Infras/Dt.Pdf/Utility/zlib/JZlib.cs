#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Utility.zlib
{
    public sealed class JZlib
    {
        private const string version_ = "1.0.2";
        public const int Z_BEST_COMPRESSION = 9;
        public const int Z_BEST_SPEED = 1;
        public const int Z_BUF_ERROR = -5;
        public const int Z_DATA_ERROR = -3;
        public const int Z_DEFAULT_COMPRESSION = -1;
        public const int Z_DEFAULT_STRATEGY = 0;
        public const int Z_ERRNO = -1;
        public const int Z_FILTERED = 1;
        public const int Z_FINISH = 4;
        public const int Z_FULL_FLUSH = 3;
        public const int Z_HUFFMAN_ONLY = 2;
        public const int Z_MEM_ERROR = -4;
        public const int Z_NEED_DICT = 2;
        public const int Z_NO_COMPRESSION = 0;
        public const int Z_NO_FLUSH = 0;
        public const int Z_OK = 0;
        public const int Z_PARTIAL_FLUSH = 1;
        public const int Z_STREAM_END = 1;
        public const int Z_STREAM_ERROR = -2;
        public const int Z_SYNC_FLUSH = 2;
        public const int Z_VERSION_ERROR = -6;

        public static string version()
        {
            return "1.0.2";
        }
    }
}

