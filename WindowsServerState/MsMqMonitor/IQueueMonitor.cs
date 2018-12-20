using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 队列监控
    /// </summary>
   public interface IQueueMonitor
    {
        /// <summary>
        /// 获取所有队列名称
        /// </summary>
        /// <returns></returns>
        List<string> GetAllQueue();

        /// <summary>
        /// 获取消息队列数量
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        float GetCount(string queueName);

        /// <summary>
        /// 清空队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        bool ClearQueue(string queueName);
    }
}
