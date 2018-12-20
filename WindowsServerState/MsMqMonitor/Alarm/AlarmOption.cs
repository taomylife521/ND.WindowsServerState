using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor.Alarm
{
  public  class AlarmOption
    {
        /// <summary>
        /// 报警所在机器ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 队列监控配置
        /// </summary>
        public QueueMonitorOption QueueOption { get; set; }

        /// <summary>
        /// 当前队列数量
        /// </summary>
        public float CurrentQueueCount { get; set; }

        /// <summary>
        /// 已报警次数
        /// </summary>
        public int HaveAlarmCount { get; set; }

        /// <summary>
        /// 是否清除队列成功
        /// </summary>
        public bool isClearSucess { get; set; }



    }
}
