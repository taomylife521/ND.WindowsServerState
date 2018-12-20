
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor.Alarm
{
    /// <summary>
    /// 邮件预警
    /// </summary>
    public class EmailAlarm : IArarm
    {
        public static ILog log = LogManager.GetLogger("MSMQMonitorService");
        public bool Alarm(AlarmOption option)
        {
            StringBuilder strMessage = new StringBuilder();
            try
            {
             
                strMessage.AppendLine("------------------预警开始(" + DateTime.Now + ":" + option.QueueOption.QueueName + ")---------------------------");
                string emailBody = "";
                StringBuilder messageTemplate= PackageMessage(option,ref emailBody);
                strMessage.AppendLine(messageTemplate.ToString());
                List<string> emailList = System.Configuration.ConfigurationManager.AppSettings["ToEmail_necessary"].Split(',').ToList();
                if (!string.IsNullOrEmpty(option.QueueOption.AlarmPerson))
                {
                    List<string> sendPersons = option.QueueOption.AlarmPerson.Split(',').ToList();
                    if (sendPersons.Count > 0)
                    {
                        emailList.AddRange(sendPersons);
                    }
                }
                string title = "机器:" + option.Ip + "下队列(" + option.QueueOption.QueueName + ")超过最大阀值(" + option.QueueOption.ThresholdMaxCount + "),请及时处理!";
                SendMail(title, emailBody, emailList.Distinct().ToList());
                return true;
            }
            catch(Exception ex)
            {
                log.Error("邮件预警异常(" + option.QueueOption.QueueName + "):" + ex.Message, ex);
                strMessage.AppendLine("邮件预警异常(" + option.QueueOption.QueueName + "):" + ex.Message);
                return false;
            }
            finally
            {
                strMessage.AppendLine("------------------预警结束(" + DateTime.Now + ":" + option.QueueOption.QueueName + ")---------------------------");
                log.Warn(strMessage.ToString());
            }
        }

        #region 封装消息
        /// <summary>
        /// 封装消息
        /// </summary>
        /// <param name="option"></param>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        private StringBuilder PackageMessage(AlarmOption option, ref string emailBody)
        {
            string nextAlarmTime = DateTime.Now.AddSeconds(option.QueueOption.AlarmInterval).ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder strMessage = new StringBuilder();
            StringBuilder strEmailBody = new StringBuilder();
            strMessage.AppendLine("机房ip:" + option.Ip);
            strMessage.AppendLine("队列名称:" + option.QueueOption.QueueName);
            strMessage.AppendLine("当前监控到的队列数量:" + option.CurrentQueueCount);
            strMessage.AppendLine("队列数量阀值:" + option.QueueOption.ThresholdMaxCount);
            strMessage.AppendLine("报警人员:" + option.QueueOption.AlarmPerson);
            strMessage.AppendLine("最大报警次数:" + option.QueueOption.AlarmMaxCount);
            strMessage.AppendLine("已报警次数:" + option.HaveAlarmCount);
            strMessage.AppendLine("下一次报警时间:" + nextAlarmTime);
            strMessage.AppendLine("达到阀值是否要清空队列:" + option.QueueOption.IsClearQueue);
            strMessage.AppendLine("是否已清空队列:" + option.isClearSucess);
            strMessage.AppendLine("报警主要参数:" + JsonConvert.SerializeObject(option));

            strEmailBody.AppendLine("<table border=1>");
            strEmailBody.AppendLine("<tr><td>机房ip:</td><td>" + option.Ip + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>队列名称:</td><td>" + option.QueueOption.QueueName + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>当前监控到的队列数量:</td><td>" + option.CurrentQueueCount + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>队列数量阀值:</td><td>" + option.QueueOption.ThresholdMaxCount + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>报警人员:</td><td>" + option.QueueOption.AlarmPerson + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>最大报警次数:</td><td>" + option.QueueOption.AlarmMaxCount + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>已报警次数:</td><td>" + option.HaveAlarmCount + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>下一次报警时间:</td><td>" + nextAlarmTime + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>达到阀值是否要清空队列:</td><td>" + option.QueueOption.IsClearQueue + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>是否已清空队列:</td><td>" + option.isClearSucess + "</td></tr>");
            strEmailBody.AppendLine("<tr><td>报警主要参数:</td><td>" + JsonConvert.SerializeObject(option) + "</td></tr>");
            strEmailBody.AppendLine("</table>");
            emailBody = strEmailBody.ToString();
            return strMessage;
        }
        #endregion

        #region 发送邮件
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="Boty">正文</param>
        /// <param name="Addressee">收件人地址</param>
        private  void SendMail(string Title, string Boty, List<string> Addressee)
        {
           // new SendEMail(Title, Boty, Addressee).Send();
        } 
        #endregion
    }
}
