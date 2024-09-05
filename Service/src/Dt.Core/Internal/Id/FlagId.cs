#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 改进Snowflake算法，将12位序列号的后三位用作标志位
    /// </summary>
    public class FlagId
    {
        const int _flagBits = 3;
        const long _flagMask = -1L ^ (-1L << _flagBits);

        const int _sequenceBits = 9;
        const long _sequenceMask = -1L ^ (-1L << _sequenceBits);

        const int _workerIdBits = 10;
        const long _maxWorkerId = -1L ^ (-1L << _workerIdBits);

        const int _workerIdShift = _flagBits + _sequenceBits;
        const int _timestampShift = _flagBits + _sequenceBits + _workerIdBits;

        // 起始时间：70年大庆
        readonly DateTime _startTime = new DateTime(2019, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        readonly object _syncRoot = new object();

        long _workerId;
        long _sequence = 0L;
        long _lastTimestamp = -1L;

        internal FlagId(long workerId)
        {
            if (workerId > _maxWorkerId || workerId < 0)
                throw new ArgumentException($"工作机器id在 0-{_maxWorkerId} 之间");
            _workerId = workerId;
        }

        internal long NextId(int p_flag)
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
                return (timestamp << _timestampShift) | (_workerId << _workerIdShift) | (_sequence << _flagBits) | (p_flag & _flagMask);
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
