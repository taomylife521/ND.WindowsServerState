using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState
{
   public class Constant
    {
        /// <summary>
        ///监控服务app 
        /// </summary>
        public static string BiHu_BaoXian_ServerStateService = "BiHu.BaoXian.ServerStateService";

        /// <summary>
        /// 要排除的消息队列
        /// </summary>
        public static string ExcludeMonitorMsmq = "ExcludeMonitorMsmq";

        /// <summary>
        /// 监控消息队列的工作时间段
        /// </summary>
        public static string MonitorMsmqWorkTime = "MonitorMsmqWorkTime";
    }
}
