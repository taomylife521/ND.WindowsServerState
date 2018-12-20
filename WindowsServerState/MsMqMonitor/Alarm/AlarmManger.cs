using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor.Alarm
{
    /// <summary>
    /// 报警管理器
    /// </summary>
   public class AlarmManger
    {
        public static ILog log = LogManager.GetLogger("MSMQMonitorService");
        static ObjectCache cache = MemoryCache.Default;
        static string countSuffix = "$count";
        private static object lockObj = new object();
       static Dictionary<AlarmType, IArarm> alarmDic = new Dictionary<AlarmType, IArarm>()
        {
               {AlarmType.Email,new EmailAlarm()},
               {AlarmType.Sms,new SmsAlarm()},
        };
        #region 异步报警
        public static void AlaramAsync(AlarmOption alarmOption)
        {
            lock (lockObj)
            {
                Task.Factory.StartNew(() =>
                {
                    //获取上次有没有报警,报警时间
                    if (cache.Contains(alarmOption.QueueOption.QueueName))  //说明已报警
                    {
                        return;
                    }
                    string countCaheKey = string.Concat(alarmOption.QueueOption.QueueName, countSuffix);
                    if (cache.Contains(countCaheKey))  //判断是否超过最大报警次数
                    {
                        alarmOption.HaveAlarmCount = int.Parse(cache[countCaheKey].ToString());//已报警次数
                        if (int.Parse(cache[countCaheKey].ToString()) >= alarmOption.QueueOption.AlarmMaxCount)
                        {
                            log.Debug("queueName:" + alarmOption.QueueOption.QueueName + "报警已超过最大报警次数:(" + alarmOption.QueueOption.AlarmMaxCount + "),不再报警!");
                            return;
                        }
                    }
                    if (alarmDic.ContainsKey(alarmOption.QueueOption.AlarmType))
                    {
                        bool r = alarmDic[alarmOption.QueueOption.AlarmType].Alarm(alarmOption);
                        if (r)
                        {
                            if (!cache.Contains(countCaheKey))
                            {
                                double cacheSeconds = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 23:59:59").Subtract(DateTime.Now).TotalSeconds;
                                cache.Add(countCaheKey, 1, DateTimeOffset.Now.AddSeconds(cacheSeconds));
                            }
                            else
                            {
                                cache[countCaheKey] = int.Parse(cache[countCaheKey].ToString()) + 1;
                            }
                            cache.Add(alarmOption.QueueOption.QueueName, DateTime.Now, DateTimeOffset.Now.AddSeconds(alarmOption.QueueOption.AlarmInterval));
                        }
                    }

                });
            }
        }
        #endregion

        #region 清除报警缓存
        /// <summary>
        /// 当队列恢复正常时,清除报警缓存
        /// </summary>
        /// <param name="queueName"></param>
        public static void ClearAlaramCache(string queueName)
        {
            try
            {
                if (cache.Contains(queueName))  //说明已报警
                {
                    cache.Remove(queueName);
                    string countCaheKey = string.Concat(queueName, countSuffix);
                    if (cache.Contains(countCaheKey))
                    {
                        cache.Remove(countCaheKey);
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("清除报警缓存异常(" + queueName + "):" + ex.Message, ex);
            }
        }
        #endregion


        }
}
