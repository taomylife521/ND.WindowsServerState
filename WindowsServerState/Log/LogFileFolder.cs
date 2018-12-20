using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.Log
{
    /// <summary>
    /// log4net非公共类
    /// 用于特定配置,存入特定文件及文件夹内
    /// </summary>
    public class LogFileFolder
    {
        /// <summary>
        /// 前缀,在配置文件中对应
        /// </summary>
        private string PrdfixName { get; set; }

        private ILog debug = null;
        private ILog info = null;
        private ILog error = null;
        private ILog warn = null;
        private ILog fatal = null;

        public LogFileFolder(string prdfixName)
        {
            PrdfixName = prdfixName;

            debug = LogManager.GetLogger(PrdfixName + "_DEBUG");
            info = LogManager.GetLogger(PrdfixName + "_INFO");
            error = LogManager.GetLogger(PrdfixName + "_ERROR");
            warn = LogManager.GetLogger(PrdfixName + "_WARN");
            fatal = LogManager.GetLogger(PrdfixName + "_FATAL");
        }

        public void Debug(string log, Exception ex = null)
        {
            debug.Debug(log, ex);
        }

        public void DebugFormat(string format, params object[] args)
        {
            debug.DebugFormat(format, args);
        }

        public void Info(string log, Exception ex = null)
        {
            info.Debug(log, ex);
        }

        public void InfoFormat(string format, params object[] args)
        {
            info.DebugFormat(format, args);
        }

        public void Error(string log, Exception ex = null)
        {
            error.Debug(log, ex);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            error.DebugFormat(format, args);
        }

        public void Warn(string log, Exception ex = null)
        {
            warn.Debug(log, ex);
        }

        public void WarnFormat(string format, params object[] args)
        {
            warn.DebugFormat(format, args);
        }

        public void Fatal(string log, Exception ex = null)
        {
            fatal.Debug(log, ex);
        }

        public void FatalFormat(string format, params object[] args)
        {
            fatal.DebugFormat(format, args);
        }
    }
}
