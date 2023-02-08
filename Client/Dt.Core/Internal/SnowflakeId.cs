#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Net;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Snowflake算法
    /// </summary>
    class SnowflakeId
    {
        const int _sequenceBits = 12;
        const long _sequenceMask = -1L ^ (-1L << _sequenceBits);

        const int _workerIdBits = 10;
        const long _maxWorkerId = -1L ^ (-1L << _workerIdBits);

        const int _timestampShift = _sequenceBits + _workerIdBits;

        // 起始时间：70年大庆
        readonly DateTime _startTime = new DateTime(2019, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        readonly object _syncRoot = new object();

        // 未采用本届IP段相加的和，只本地sqlite库用
        long _workerId = 0L;
        long _sequence = 0L;
        long _lastTimestamp = -1L;

        public long NextId()
        {
            lock (_syncRoot)
            {
                long timestamp = TimeGen();

                // 保证当前时间大于最后时间，时间回退会导致产生重复id
                if (timestamp < _lastTimestamp)
                {
                    throw new ApplicationException($"禁止时间回调，当前已回调 {_lastTimestamp - timestamp} 毫秒");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & _sequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0L;
                }

                _lastTimestamp = timestamp;
                return (timestamp << _timestampShift) | (_workerId << _sequenceBits) | _sequence;
            }
        }

        long TilNextMillis(long lastTimestamp)
        {
            long timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        long TimeGen()
        {
            return (long)(DateTime.UtcNow - _startTime).TotalMilliseconds;
        }
    }
}
