using WindowsServerState.MsMqMonitor.Alarm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    public class DefaultMqListener : AbstractMqListener
    {
        public DefaultMqListener(IQueueMonitor queueMonitor, CancellationTokenSource cts) : base(queueMonitor, cts)
        {
        }
        public DefaultMqListener(CancellationTokenSource cts) : base(cts)
        {
        }

        /// <summary>
        /// 开启监听
        /// </summary>
        /// <param name="queueName"></param>
        public override void StartListener(List<string> queueName)
        {
            StringBuilder strMessage = new StringBuilder();
            try
            {
               
                bool isClearSucess = false;
                string msg = "";
                //string wkTime = "";
                foreach (var item in queueName)
                {
                    QueueMonitorOption option = QueueMonitorOptionProvider.GetOption(item);
                    if (!IsListenerWorkTime(option))
                    {
                        strMessage.AppendLine("queuename:" + item + ",未在监听时间段("+ option.WorkTime + ")");
                        continue;
                    }
                    float count = _queueMonitor.GetCount(item);
                    strMessage.AppendLine("queuename:" + item + ",count:" + count);
                  
                    if (option.ThresholdMaxCount <= count) //超出范围
                    {
                        if (option.IsClearQueue)   //是否要清空队列
                        {
                            isClearSucess = _queueMonitor.ClearQueue(item);
                        }
                        msg = "queueName:" + item + ",count:" + count + ",ThresholdMaxCount:" + option.ThresholdMaxCount + ",isClearSucess:" + isClearSucess;
                        strMessage.AppendLine(msg);
                        log.Error(msg);
                        AlarmManger.AlaramAsync(new AlarmOption()
                        {
                            Ip = ConfigurationManager.AppSettings["ServerIp"],
                            CurrentQueueCount = count,
                            QueueOption = option,
                            isClearSucess=isClearSucess

                        });
                    }
                    else
                    {
                        AlarmManger.ClearAlaramCache(item);
                    }
                }
               
              
            }
            catch(Exception ex)
            {
                log.Error("监听异常:" + ex.Message, ex);
                strMessage.AppendLine("监听异常:" + ex.Message);
            }
            finally
            {
                log.Info(strMessage.ToString());
            }

        }

      
    }
}
