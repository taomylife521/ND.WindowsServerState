using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 读取本地json配置文件
    /// </summary>
    public class LocalJsonQueueMonitorOption : AbstractQueueMonitorOption
    {
        string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MsMqMonitor\\JsonQueueMonitorOptions.txt");
        public override Dictionary<string, QueueMonitorOption> LoadOption()
        {
           
            log.Fatal("-------------------读取本地json配置开始("+DateTime.Now+")---------------------------------");
            Dictionary<string, QueueMonitorOption> dic = new Dictionary<string, QueueMonitorOption>();
            QueueMonitorOption defaultOption= GetDefault();
            List<QueueMonitorOption> optionList= JsonConvert.DeserializeObject<List<QueueMonitorOption>>(File.ReadAllText(directory));
            foreach (var item in optionList)
            {
                item.AlarmMaxCount= item.AlarmMaxCount <= 0 ? defaultOption.AlarmMaxCount: item.AlarmMaxCount;
                item.AlarmInterval= item.AlarmInterval <= 0 ? defaultOption.AlarmInterval : item.AlarmInterval;
                item.ThresholdMaxCount= item.ThresholdMaxCount <= 0 ? defaultOption.ThresholdMaxCount : item.ThresholdMaxCount;
                
                if (dic.ContainsKey(item.QueueName))
                {
                    dic[item.QueueName] = item;
                }
                else
                {
                    dic.Add(item.QueueName, item);
                }

            }
            log.Fatal("刷新配置文件:" + JsonConvert.SerializeObject(optionList));
            log.Fatal("-------------------读取本地json配置结束(" + DateTime.Now + ")---------------------------------");
            //读取本地配置文件
            return dic;
        }
    }
}
