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

        long _workerId;
        long _sequence = 0L;
        long _lastTimestamp = -1L;

        public SnowflakeId(long workerId)
        {
            if (workerId > _maxWorkerId || workerId < 0)
                throw new ArgumentException($"工作机器id在 0-{_maxWorkerId} 之间");
            _workerId = workerId;
        }

        public SnowflakeId()
        {
            _workerId = GetWorkerId();
        }

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

        static long GetWorkerId()
        {
            int workerId = 0;

            // 使用这种 IP 生成工作进程编号的方法，必须保证IP段相加不能重复！！！
            try
            {
                var ips = Dns.GetHostAddresses(Dns.GetHostName());
                for (int i = 0; i < ips.Length; i++)
                {
                    if (ips[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        // IPV4时采用IP段数值相加生成唯一的workerId，满足 < 1024，重复可能性小，凑合用！
                        byte[] data = ips[i].GetAddressBytes();
                        for (int j = 0; j < data.Length; j++)
                        {
                            workerId += data[j] & 0xFF;
                        }
                        break;
                    }
                    else if (ips[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        // IPV6时将每个 Bit 位的后6位相加
                        byte[] data = ips[i].GetAddressBytes();
                        for (int j = 0; j < data.Length; j++)
                        {
                            workerId += data[j] & 0B111111;
                        }
                        break;
                    }
                }
            }
            catch { }

            return workerId;
        }
    }
}
