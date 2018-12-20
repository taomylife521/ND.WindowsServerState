using WindowsServerState.MsMqMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MQMonitor
{
    /// <summary>
    /// 消息队列监听器
    /// </summary>
   public interface IMqListener
    {
        /// <summary>
        /// 开启监听
        /// </summary>
        /// <param name="queueName"></param>
        void StartListener();

        /// <summary>
        ///判断是否在监听工作时间段
        /// </summary>
        bool IsListenerWorkTime(QueueMonitorOption option);

        /// <summary>
        ///获取要排除的队列名称
        /// </summary>
        List<string> GetExcludeQueueName();

        /// <summary>
        /// 停止监听
        /// </summary>
        void StopListener();
    }
}
