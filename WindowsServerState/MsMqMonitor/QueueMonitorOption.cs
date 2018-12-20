using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 队列监控配置
    /// </summary>
   public class QueueMonitorOption
    {

      
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 队列阀值最大数量
        /// </summary>
        public int ThresholdMaxCount { get; set; }

        /// <summary>
        /// 报警最大次数
        /// </summary>
        public int AlarmMaxCount { get; set; }

        /// <summary>
        /// 报警最大间隔时间(秒)
        /// </summary>
        public int AlarmInterval { get; set; }

        /// <summary>
        /// 报警方式
        /// </summary>
        public AlarmType AlarmType { get; set; }

        /// <summary>
        /// 报警人员,多个之间用逗号分隔
        /// </summary>
        public string AlarmPerson { get; set; }

        /// <summary>
        /// 当达到最大阀值时是否要清空队列
        /// </summary>
        public bool IsClearQueue { get; set; }

        /// <summary>
        /// 当前消息队列监控工作时间段
        /// </summary>
        public string WorkTime { get; set; }
    }

    public enum AlarmType
    {
        [Description("邮件预警")]
        Email=0,

        [Description("短信预警")]
        Sms =1
    }
}
