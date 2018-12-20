using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.BaoXian.Reconciliation.Helper.Log
{
   public class LoggedCallbackInfo
    {
        /// <summary>
        /// 获取使用
        /// </summary>
        internal Exception BaseException { get; set; }

        /// <summary>
        /// 获取调用堆栈
        /// </summary>
        public string StackTrace { get; internal set; }

        /// <summary>
        /// 获取日志的记录消息
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// 获取日志的类型
        /// </summary>
        public LogTypeEnum LogType { get; internal set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        internal LoggedCallbackInfo()
        {


        }
    }
}
