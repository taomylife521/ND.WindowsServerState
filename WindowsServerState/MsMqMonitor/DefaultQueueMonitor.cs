using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    public class DefaultQueueMonitor : IQueueMonitor
    {
        public static ILog log = LogManager.GetLogger("MSMQMonitorService");
        public bool ClearQueue(string queueName)
        {
            try
            {
                queueName = Environment.MachineName + "\\" + queueName;
                //queueName = Environment.MachineName + "\\private$\\" + queueName;
                // Connect to a queue on the local computer.
                MessageQueue queue = new MessageQueue(queueName);

                // Delete all messages from the queue.
                queue.Purge();
                return true;
            }
            catch(Exception ex)
            {
                log.Error("DefaultQueueMonitor.ClearQueue异常:" + ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 获取所有的消息队列名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllQueue()
        {
            List<string> queueNameList = new List<string>();
            try
            {
              
                // Get a list of queues with the specified category.
                MessageQueue[] QueueList = MessageQueue.GetPrivateQueuesByMachine(Environment.MachineName);
                // Display the paths of the queues in the list.
                foreach (MessageQueue queueItem in QueueList)
                {
                    queueNameList.Add(queueItem.QueueName);
                }
              
            }
            catch(Exception ex)
            {
                log.Error("DefaultQueueMonitor.GetAllQueue异常:" + ex.Message, ex);
            }
            return queueNameList;
        }

        /// <summary>
        /// 获取指定消息队列的数量
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public float GetCount(string queueName)
        {
            try
            {
                queueName = Environment.MachineName + "\\" + queueName;
                PerformanceCounter singleCount = new PerformanceCounter("MSMQ Queue", "Messages in Queue", queueName);
                singleCount.InstanceName = queueName;
                return singleCount.NextValue();
            }
            catch(InvalidOperationException ex)
            {
                return 0;
            }
            catch(Exception ex)
            {
                log.Error("DefaultQueueMonitor.GetCount异常("+queueName+"):"+ex.Message,ex);
                return 0;
            }
        }
    }
}
