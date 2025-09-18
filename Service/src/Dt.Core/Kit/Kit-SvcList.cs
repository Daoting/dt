#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.RabbitMQ;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 通过 RabbitMQ 获取微服务的方法
    /// </summary>
    public partial class Kit
    {
        static AppSvcList _svcList;

        static AppSvcList AppSvcList
        {
            get
            {
                if (EnableRabbitMQ && _svcList == null)
                    _svcList = new AppSvcList();
                return _svcList;
            }
        }

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的所有微服务列表，单体服务特殊，未启用RabbitMQ
        /// </summary>
        /// <param name="p_isSvcInst">true表示所有微服务副本实例，false表示所有微服务</param>
        /// <returns>微服务列表</returns>
        public static List<string> GetAllSvcs(bool p_isSvcInst)
        {
            if (EnableRabbitMQ)
                return AppSvcList.GetAllSvcs(p_isSvcInst);

            // 未启用RabbitMQ
            return SvcNames.ToList();
        }

        /// <summary>
        /// 获取当前服务的其他副本实例的id列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetOtherReplicaIDs()
        {
            if (EnableRabbitMQ)
                return AppSvcList.GetOtherReplicaIDs();
            return new List<string>();
        }

        /// <summary>
        /// 获取某微服务的所有副本实例的id列表
        /// </summary>
        /// <param name="p_svcName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetSvcReplicaIDs(string p_svcName = null)
        {
            if (EnableRabbitMQ)
                return AppSvcList.GetReplicaIDs(p_svcName);
            return new List<string>();
        }

        /// <summary>
        /// 通过RabbitMQ队列，获取应用内正在运行的某微服务的副本个数
        /// </summary>
        /// <param name="p_svcName">服务名称，null表示当前服务</param>
        /// <returns>副本个数</returns>
        public static int GetSvcReplicaCount(string p_svcName = null)
        {
            if (EnableRabbitMQ)
                return AppSvcList.GetReplicaCount(p_svcName);
            return 1;
        }

        internal static void UpdateSvcList()
        {
            if (EnableRabbitMQ)
                AppSvcList.Update();
        }
    }
}
