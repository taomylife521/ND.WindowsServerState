using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 加载队列配置
    /// </summary>
    public interface IQueueMonitorOption
    {
        /// <summary>
        /// 加载待监控的配置选项
        /// </summary>
        /// <returns></returns>
        Dictionary<string, QueueMonitorOption> LoadOption();

        /// <summary>
        /// 获取默认的配置选项
        /// </summary>
        /// <returns></returns>
        QueueMonitorOption GetDefault();



    }
}
