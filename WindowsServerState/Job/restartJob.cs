using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Quartz;
using WindowsServerState.Log;

namespace WindowsServerState.Job
{
    public class restartJob : IJob
    {
        public static LogFileFolder log = new LogFileFolder("State");
        public static ServerName restartConf  = ConfHlep.restartConfig.GetSectionBlock<ServerName>("ServerName");
        public static Dictionary<string, string> Confs
        {
            get {return restartConf.KeyValues.Cast<kvSetting>().ToDictionary(k => k.Key, v => v.Value); }
        }
        public void Execute(IJobExecutionContext context)
        {
            foreach (var item in Confs)
            {
                var service = new ServiceController(item.Key);
                try
                {
                    log.InfoFormat("{0}服务执行重启,描述：{1} 正在重启", item.Key, Confs[item.Key]);
                    var st = Stopwatch.StartNew();
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped);
                    }
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    st.Stop();
                    log.InfoFormat("服务：{0} 描述：{1} 重启成功,耗时：{2}", item.Key, Confs[item.Key], st.ElapsedMilliseconds);
                }
                catch (Exception exception)
                {
                    log.Fatal(string.Format("重启服务：{0} 重启失败！", item.Key), exception);
                }
                finally
                {
                    service.Close();
                }
            }
        }
    }
}
