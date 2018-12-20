
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    public abstract class AbstractQueueMonitorOption : IQueueMonitorOption
    {
        protected  ILog log = LogManager.GetLogger("MSMQMonitorService");
        /// <summary>
        /// 读取本地配置
        /// </summary>
        /// <returns></returns>
        public abstract Dictionary<string, QueueMonitorOption> LoadOption();
        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <returns></returns>
        public virtual QueueMonitorOption GetDefault()
        {
            return new QueueMonitorOption()
            {
                AlarmInterval = 900,
                AlarmMaxCount = 2,
                AlarmType = AlarmType.Email,
                AlarmPerson = System.Configuration.ConfigurationManager.AppSettings["ToEmail_necessary"],
                ThresholdMaxCount = 10000,
                QueueName = "all" ,
                IsClearQueue=false
            };
        }

    }
}
