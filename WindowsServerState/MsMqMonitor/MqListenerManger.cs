using WindowsServerState.MQMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 监听管理器
    /// </summary>
   public class MqListenerManger
    {
       static IMqListener _listener;

        //public MqListenerManger(IMqListener listener)
        //{
        //    _listener = listener;
        //}
        //public MqListenerManger(CancellationTokenSource cts)
        //{
        //    _listener = new DefaultMqListener(cts);
        //}



        /// <summary>
        /// 开启监听
        /// </summary>
        /// <param name="queueName"></param>
        public static void StartListener(CancellationTokenSource cts)
        {
            _listener = new DefaultMqListener(cts);
            _listener.StartListener();
        }

        /// <summary>
        /// 停止监听
        /// </summary>
       public static void StopListener()
        {
            _listener.StopListener();
        }
    }
}
