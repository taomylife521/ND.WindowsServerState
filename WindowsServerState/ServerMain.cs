using System;
using System.ServiceModel;
using System.Threading;
using WindowsServerState.Log;
using WindowsServerState.MsMqMonitor;

namespace WindowsServerState
{
    public class ServerMain
    {
        private readonly ServiceHost _hostService = new ServiceHost(typeof(Service));
        public CancellationTokenSource TokenSource = new CancellationTokenSource();
        public static LogFileFolder Log = new LogFileFolder("State");

        /// <summary>
        ///     服务启动
        /// </summary>
        public void Start()
        {
            try
            {
                ////从工厂中获取一个调度器实例化
                //var scheduler = StdSchedulerFactory.GetDefaultScheduler();
                //scheduler.Start(); //开启调度器
                MqListenerManger.StartListener(TokenSource);
                //c# 6.0 using static class
                ServerStateTask.DoTask(TokenSource);
                ServerStateTask.DoStateTask(TokenSource) /* 服务状态 */;
                _hostService.Opened += delegate { Log.Info("hostService服务 启动中."); };
                _hostService.Open();
            }
            catch (Exception e)
            {
                Log.Fatal(e.Message, e);
            }
        }

        /// <summary>
        ///     服务停止
        /// </summary>
        public void Stop()
        {
            TokenSource.Cancel();
            MqListenerManger.StopListener();
        }
    }
}