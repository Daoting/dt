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
    /*
    /// <summary>
    /// 生成ID算法
    /// </summary>
    public static class Id
    {
        static readonly SnowflakeId _snowflake = new SnowflakeId(GetWorkerId());
        //static readonly FlagId _flag = new FlagId(GetWorkerId());

        /// <summary>
        /// 产生新ID
        /// </summary>
        /// <returns></returns>
        public static long New()
        {
            return _snowflake.NextId();
        }

        // 不再使用带3位标志位的ID！
        ///// <summary>
        ///// 产生含3位标志位的新ID
        ///// </summary>
        ///// <param name="p_flag">ID标志，取值范围0-7</param>
        ///// <returns></returns>
        //public static long New(int p_flag)
        //{
        //    if (p_flag < 0 || p_flag > 7)
        //        throw new Exception("ID标志超出范围(0-7)");
        //    return _flag.NextId(p_flag);
        //}

        ///// <summary>
        ///// 获取ID的标志值(低3位)
        ///// </summary>
        ///// <param name="p_id"></param>
        ///// <returns></returns>
        //public static int GetFlag(long p_id)
        //{
        //    return (int)(p_id & 7);
        //}

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
    */
}
