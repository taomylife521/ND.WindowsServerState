using WindowsServerState.MQMonitor;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    public abstract class AbstractMqListener : IMqListener
    {
        public static ILog log = LogManager.GetLogger("MSMQMonitorService");
        public IQueueMonitor _queueMonitor;
        protected  CancellationTokenSource _cts;
        private static object lockObj = new object();
      
        public AbstractMqListener(IQueueMonitor queueMonitor, CancellationTokenSource cts)
        {
            _cts = cts;
            _queueMonitor = queueMonitor;
           
        }

        public AbstractMqListener(CancellationTokenSource cts) :this(new DefaultQueueMonitor(),cts)
        {
        }

        #region 多线程分发
        public void Dispatch(List<string> queueNames)
        {
            log.Info("--------------------开启多线程分配监听:("+DateTime.Now+")---------------------------");
            log.Info("监听队列数量:" + queueNames.Count);
            log.Info("监听队列:" + string.Join(",", queueNames));
            int maxTaskCount = 20;//最大开20个线程去跑
            int maxSingleTaskCount = 10;   //每个线程最多监控10个队列
            int channelCount = queueNames.Count;
            ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
            Stopwatch st = new Stopwatch();
            st.Start();
            Task.Factory.StartNew(() =>   //开启管理分配任务Task
            {
                int currentTaskCount = 0;

                while (queueNames.Count > 0)
                {

                    while (currentTaskCount >= maxTaskCount)
                    {

                        int index = Task.WaitAny(tasks.ToArray());//等待任何一个task完成 
                        currentTaskCount--;

                    }
                    List<string> childData = new List<string>();
                    childData = queueNames.Take(maxSingleTaskCount).ToList();

                    if (childData.Count > 0)
                    {
                        queueNames.RemoveRange(0, childData.Count);
                        var subTask = Task.Factory.StartNew(() =>
                        {
                            while (true)
                            {
                                //如果线程取消,则此处会抛出异常
                                _cts.Token.ThrowIfCancellationRequested();
                                StartListener(childData);
                            }
                        });
                        currentTaskCount++;
                        tasks.Add(subTask);
                    }
                    else //如果分配完毕currentTaskCount<maxTaskCount,则让maxTaskCount=currentTaskCount
                    {

                        Console.WriteLine("全部分配完毕");
                    }
                }
                log.Info("任务全部分配完毕，共开启线程数量:" + tasks.Count.ToString());
                log.Info("--------------------多线程分配完毕:(" + DateTime.Now + ")---------------------------");
            });
          

        }
        #endregion


        #region 开启监听
        public virtual void StartListener()
        {
            try
            {
                string wkTime = "";
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        _cts.Token.ThrowIfCancellationRequested();
                        bool isInWorkTime = true; //是否在工作时间段内
                        try
                            {
                                
                                List<string> newQueueNames = new List<string>();
                                log.Info("------------------开启监听:(" + DateTime.Now + ")-----------------------");
                                if (IsListenerWorkTime(ref wkTime))
                                {
                                    List<string> queueNames = _queueMonitor.GetAllQueue();
                                    log.Info("扫描到监听队列数量:" + queueNames.Count);
                                    List<string> excludeQueueNames= GetExcludeQueueName();//要排除监控的队列名称
                                    if (excludeQueueNames.Count > 0)
                                    {
                                        log.Info("忽略监控的队列列表:" + string.Join(",", excludeQueueNames));
                                        #region Filter QueueName
                                        foreach (var item in queueNames)
                                        {
                                            bool isIn = false;
                                            foreach (var subItem in excludeQueueNames)
                                            {
                                                if (item.IndexOf(subItem) > -1)
                                                {
                                                    isIn = true;
                                                }
                                            }
                                            if (!isIn)
                                            {
                                                newQueueNames.Add(item);
                                            }
                                        } 
                                        #endregion
                                        log.Info("过滤后需监听队列数量:" + newQueueNames.Count);
                                    }
                                    else
                                    {
                                        newQueueNames = queueNames;
                                    }
                                    StartListener(newQueueNames);

                                }
                                else
                                {
                                    isInWorkTime = false;
                                    log.Info("未在监听时间段(" + wkTime + ")");
                                }

                            }
                            catch (Exception ex)
                            {
                                log.Error("Listener监听异常:" + ex.Message, ex);
                                log.Info("Listener监听异常:" + ex.Message);
                            }
                            finally
                            {
                                 int sleepTime=int.Parse(ConfigurationManager.AppSettings["ListenerInterval"]) * 1000;
                                 if (!isInWorkTime)//如果不在工作时间段内
                                 {
                                    sleepTime = sleepTime * 5;//多休眠5倍
                                 }
                                log.Info("------------------监听结束:(" + DateTime.Now + ")-----------------------\r\n\r\n");
                                Thread.Sleep(sleepTime);
                            }
                        
                    }
                }, _cts.Token);
            }
            catch (Exception ex)
            {
                log.Error("监听消息队列异常:" + ex.Message, ex);
            }

            // Dispatch(queueNames);
        } 
        #endregion

        public abstract void StartListener(List<string> queueName);
        

        public virtual void StopListener()
        {
            _cts.Cancel();
        }

        /// <summary>
        /// 是否在监听工作时间段
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public virtual bool IsListenerWorkTime(ref string wkTime)
        {
            try
            {
                // wkTime = KeyValueConfig.Instance.GetValue(Constant.BiHu_BaoXian_ServerStateService, "", Constant.MonitorMsmqWorkTime);
                //List<string> workTime = wkTime.Split('-').ToList();
                //if (workTime != null && workTime.Count > 0)
                //{
                //   DateTime dtStartTime= Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + workTime[0]);
                //    DateTime dtEndTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + workTime[1]);
                //    return
                //        workTime.Any(
                //            workTimes =>
                //                DateTime.Now.Ticks >= dtStartTime.Ticks && DateTime.Now.Ticks <= dtEndTime.Ticks);
                //}
                return true;
            }
            catch(Exception ex)
            {
                log.Error("Listener.IsListenerWorkTime获取是否在工作时间段异常:" + ex.Message, ex);
                return false;
            }

        }

        /// <summary>
        /// 是否在监听工作时间段
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public virtual bool IsListenerWorkTime(QueueMonitorOption option)
        {
            try
            {
                //if (string.IsNullOrEmpty(option.WorkTime))  //如果当前工作时间段为空（未配置）,默认都在工作时间段内
                //{
                //    return true;
                //}
                //List<string> workTime = option.WorkTime.Split('-').ToList();
                //if (workTime != null && workTime.Count > 0)
                //{
                //    DateTime dtStartTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + workTime[0]);
                //    DateTime dtEndTime = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + workTime[1]);
                //    return
                //        workTime.Any(
                //            workTimes =>
                //                DateTime.Now.Ticks >= dtStartTime.Ticks && DateTime.Now.Ticks <= dtEndTime.Ticks);
                //}
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Listener.IsListenerWorkTime获取("+option.QueueName+")是否在工作时间段("+option.WorkTime+")异常:" + ex.Message, ex);
                return false;
            }

        }




        public virtual List<string> GetExcludeQueueName()
        {
            try
            {
                //string queueNames = KeyValueConfig.Instance.GetValue(Constant.BiHu_BaoXian_ServerStateService, "", Constant.ExcludeMonitorMsmq);
                //if (string.IsNullOrEmpty(queueNames))
                //{
                //    return new List<string>();
                //}
                //List<string> excludeQueueNames =JsonConvert.DeserializeObject<List<string>>(queueNames);
                //if (excludeQueueNames.Count <= 0)
                //{
                //    return new List<string>();
                //}
                return new List<string>();
            }
            catch (Exception ex)
            {
                log.Error("Listener.GetExcludeQueueName获取要排除的消息队列异常:" + ex.Message, ex);
                return new List<string>();
            }
        }
    }
}
