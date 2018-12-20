
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 队列监控配置管理器
    /// </summary>
   public class QueueMonitorOptionProvider
    {
        public static ILog log = LogManager.GetLogger("MSMQMonitorService");
        static IQueueMonitorOption _option = new LocalJsonQueueMonitorOption();
        static ObjectCache cache = MemoryCache.Default;
        static string cacheKey = "queuemonitoroptions";
        private static object lockObj = new object();
        //public QueueMonitorOptionProvider(IQueueMonitorOption option)
        //{
        //    _option = option;
        //}
        //public QueueMonitorOptionProvider():this(new LocalJsonQueueMonitorOption())
        //{
           
        //}

        /// <summary>
        /// 获取监控配置
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static QueueMonitorOption GetOption(string queueName)
        {
            lock (lockObj)
            {
                try
                {
                    queueName = queueName.Substring(queueName.LastIndexOf("\\")+1);
                    Dictionary<string, QueueMonitorOption> dics = new Dictionary<string, QueueMonitorOption>();
                    if (!cache.Contains(cacheKey))  //内存字典没值或缓存没有
                    {
                        //缓存处理
                        dics = _option.LoadOption();
                        if (dics.Count > 0)
                        {
                            
                            cache.Add(cacheKey, dics, DateTimeOffset.Now.AddSeconds(int.Parse(ConfigurationManager.AppSettings["OptionCacheInterval"])));
                        }
                    }
                    else
                    {
                        dics = (Dictionary<string, QueueMonitorOption>)cache[cacheKey];
                    }
                    if (dics.ContainsKey(queueName))
                    {
                        return dics[queueName];
                    }
                    return GetDefault(queueName);
                }
                catch (Exception ex)
                {
                    log.Error("获取队列监控配置异常("+queueName+"):"+ex.Message,ex);
                    return GetDefault(queueName);
                }
            }

        }

        /// <summary>
        /// 获取默认配置
        /// </summary>
        /// <returns></returns>
        public static QueueMonitorOption GetDefault(string queueName)
        {
            return new QueueMonitorOption() {
               AlarmInterval=900,
               AlarmMaxCount=2,
               AlarmType= AlarmType.Email,
               AlarmPerson =System.Configuration.ConfigurationManager.AppSettings["ToEmail_necessary"],
               ThresholdMaxCount=10000,
               QueueName= queueName
            };
        }
    }
}
