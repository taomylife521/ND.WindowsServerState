/*----------------------------------------------------------------
// Copyright (C) 2015 壁虎养车
//
// 文件名：LogOnUserInfo.cs
// 文件功能描述：系统日志记录对象。
//
//----------------------------------------------------------------*/

using System.Diagnostics;
using System.Linq;
using log4net;
using System;
using log4net.Core;
using System.Text;

namespace BiHuManBu.BaoXian.Reconciliation.Helper.Log
{
    /// <summary>
    /// 日志记录对象
    /// </summary>
    public class Logger
    {
        private static readonly _Logger Log = new _Logger(LogManager.GetLogger("MyLogger"));
        private static readonly _Logger LogInfo = new _Logger(LogManager.GetLogger("info"));
        private static readonly _Logger LogMonitor = new _Logger(LogManager.GetLogger("Monitor"));
        /// <summary>
        /// 默认得构造函数
        /// </summary>
        //static Logger()
        //{
        //    log4net.Config.XmlConfigurator.Configure();
        //}

        /// <summary>
        /// 获取默认日志输出对象
        /// </summary>
        public static ILog Default
        {
            get { return Log; }
        }

        public static ILog TraceLog
        {
            get { return LogInfo; }
        }
        public static ILog Monitor
        {
            get { return LogMonitor; }
        }

        /// <summary>
        /// 设置日志被记录后的回调函数
        /// </summary>
        /// <param name="callback"></param>
        public static void SetLoggedCallbackMethod(LoggedCallbackDelegate callback)
        {
            Log.LoggedCallback = callback;
        }

        /// <summary>
        /// 
        /// </summary>
        private class _Logger : ILog
        {
            private readonly ILog _logger;

            /// <summary>
            /// 获取或设置设置日志的回调函数
            /// </summary>
            internal LoggedCallbackDelegate LoggedCallback { get; set; }

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="log"></param>
            internal _Logger(ILog log)
            {
                this._logger = log;

                LoggedCallback = delegate { };
            }

            /// <summary>
            /// 获取日志所需的对象
            /// </summary>
            /// <returns></returns>
            private static LoggedCallbackInfo GetLogStackTrace(string message, Exception baseException = null, LogTypeEnum lt = LogTypeEnum.Debug)
            {
                if (baseException != null)
                {
                    return new LoggedCallbackInfo
                               {
                                   Message = message,
                                   StackTrace = baseException.StackTrace,
                                   BaseException = baseException,
                                   LogType = lt
                               };
                }

                var txt = new StringBuilder();
                var st = new StackTrace(true);
                //Console.WriteLine(" Stack trace for current level: {0}", st.ToString());
                //var sf = st.GetFrame(1);
                foreach (var sf in st.GetFrames().Skip(3).Take(5).Where(sf => sf.GetFileName() != null))
                {
                    txt.AppendLine("方法:" + sf.GetMethod() + " 在 " + sf.GetFileName() + ":行 " + sf.GetFileLineNumber());
                }

                return new LoggedCallbackInfo
                {
                    Message = message,
                    StackTrace = txt.ToString(),
                    BaseException = baseException,
                    LogType = lt
                };
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lt"></param>
            /// <param name="message"></param>
            /// <param name="exception"></param>
            private void WriteLog(LogTypeEnum lt, object message, Exception exception = null)
            {
                if (exception == null)
                {
                    switch (lt)
                    {
                        case LogTypeEnum.Debug:
                            this._logger.Debug(message);
                            break;
                        case LogTypeEnum.Warn:
                            this._logger.Warn(message);
                            break;
                        case LogTypeEnum.Info:
                            this._logger.Info(message);
                            break;
                        case LogTypeEnum.Error:
                            this._logger.Error(message);
                            break;
                        case LogTypeEnum.Fatal:
                            this._logger.Fatal(message);
                            break;
                        case LogTypeEnum.Monitor:
                            this._logger.Info(message);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("lt");
                    }

                    LoggedCallback(message.GetType().IsSubclassOf(typeof(Exception))
                                       ? GetLogStackTrace(((Exception)message).Message, (Exception)message)
                                       : GetLogStackTrace(message.ToString()));
                }
                else
                {
                    switch (lt)
                    {
                        case LogTypeEnum.Debug:
                            this._logger.Debug(message, exception);
                            break;
                        case LogTypeEnum.Warn:
                            this._logger.Warn(message, exception);
                            break;
                        case LogTypeEnum.Info:
                            this._logger.Info(message, exception);
                            break;
                        case LogTypeEnum.Error:
                            this._logger.Error(message, exception);
                            break;
                        case LogTypeEnum.Fatal:
                            this._logger.Fatal(message, exception);
                            break;
                        case LogTypeEnum.Monitor:
                            this._logger.Info(message, exception);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("lt");
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lt"></param>
            /// <param name="provider"></param>
            /// <param name="format"></param>
            /// <param name="args"></param>
            private void WriteLogFormat(LogTypeEnum lt, IFormatProvider provider, string format, params object[] args)
            {
                if (provider == null)
                {
                    switch (lt)
                    {
                        case LogTypeEnum.Debug:
                            this._logger.DebugFormat(format, args);
                            break;
                        case LogTypeEnum.Warn:
                            this._logger.WarnFormat(format, args);
                            break;
                        case LogTypeEnum.Info:
                            this._logger.InfoFormat(format, args);
                            break;
                        case LogTypeEnum.Error:
                            this._logger.ErrorFormat(format, args);
                            break;
                        case LogTypeEnum.Fatal:
                            this._logger.FatalFormat(format, args);
                            break;
                        case LogTypeEnum.Monitor:
                            this._logger.InfoFormat(format, args);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("lt");
                    }

                    LoggedCallback(GetLogStackTrace(string.Format(format, args)));
                }
                else
                {
                    switch (lt)
                    {
                        case LogTypeEnum.Debug:
                            this._logger.DebugFormat(provider, format, args);
                            break;
                        case LogTypeEnum.Warn:
                            this._logger.WarnFormat(provider, format, args);
                            break;
                        case LogTypeEnum.Info:
                            this._logger.InfoFormat(provider, format, args);
                            break;
                        case LogTypeEnum.Error:
                            this._logger.ErrorFormat(provider, format, args);
                            break;
                        case LogTypeEnum.Fatal:
                            this._logger.FatalFormat(provider, format, args);
                            break;
                        case LogTypeEnum.Monitor:
                            this._logger.InfoFormat(provider, format, args);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("lt");
                    }
                }
            }

            #region ILoggerWrapper 成员

            public ILogger Logger
            {
                get { return _logger.Logger; }
            }

            #endregion

            #region ILog 成员

            public void Debug(object message, Exception exception)
            {
                WriteLog(LogTypeEnum.Debug, message, exception);
            }

            public void Debug(object message)
            {
                WriteLog(LogTypeEnum.Debug, message);
            }

            public void DebugFormat(IFormatProvider provider, string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Debug, provider, format, args);
            }

            public void DebugFormat(string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Debug, null, format, args);
            }

            public void Error(object message, Exception exception)
            {
                WriteLog(LogTypeEnum.Error, message, exception);
            }

            public void Error(object message)
            {
                WriteLog(LogTypeEnum.Error, message);
            }

            public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Error, provider, format, args);
            }

            public void ErrorFormat(string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Error, null, format, args);
            }

            public void Fatal(object message, Exception exception)
            {
                WriteLog(LogTypeEnum.Fatal, message, exception);
            }

            public void Fatal(object message)
            {
                WriteLog(LogTypeEnum.Fatal, message);
            }

            public void FatalFormat(IFormatProvider provider, string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Fatal, provider, format, args);
            }

            public void FatalFormat(string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Fatal, null, format, args);
            }

            public void Info(object message, Exception exception)
            {
                WriteLog(LogTypeEnum.Info, message, exception);
            }

            public void Info(object message)
            {
                WriteLog(LogTypeEnum.Info, message);
            }
            public void InfoFormat(IFormatProvider provider, string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Info, provider, format, args);
            }

            public void InfoFormat(string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Info, null, format, args);
            }

            public bool IsDebugEnabled
            {
                get { return this._logger.IsDebugEnabled; }
            }

            public bool IsErrorEnabled
            {
                get { return this._logger.IsErrorEnabled; }
            }

            public bool IsFatalEnabled
            {
                get { return this._logger.IsFatalEnabled; }
            }

            public bool IsInfoEnabled
            {
                get { return this._logger.IsInfoEnabled; }
            }

            public bool IsWarnEnabled
            {
                get { return this._logger.IsWarnEnabled; }
            }

            public void Warn(object message, Exception exception)
            {
                WriteLog(LogTypeEnum.Warn, message, exception);
            }

            public void Warn(object message)
            {
                WriteLog(LogTypeEnum.Warn, message);
            }

            public void WarnFormat(IFormatProvider provider, string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Warn, provider, format, args);
            }

            public void WarnFormat(string format, params object[] args)
            {
                WriteLogFormat(LogTypeEnum.Warn, null, format, args);
            }
            #endregion


            public void DebugFormat(string format, object arg0, object arg1, object arg2)
            {
                WriteLogFormat(LogTypeEnum.Debug, null, format, arg0, arg1, arg2);
            }

            public void DebugFormat(string format, object arg0, object arg1)
            {
                WriteLogFormat(LogTypeEnum.Debug, null, format, arg0, arg1);
            }

            public void DebugFormat(string format, object arg0)
            {
                WriteLogFormat(LogTypeEnum.Debug, null, format, arg0);
            }

            public void ErrorFormat(string format, object arg0, object arg1, object arg2)
            {
                WriteLogFormat(LogTypeEnum.Error, null, format, arg0, arg1, arg2);
            }

            public void ErrorFormat(string format, object arg0, object arg1)
            {
                WriteLogFormat(LogTypeEnum.Error, null, format, arg0, arg1);
            }

            public void ErrorFormat(string format, object arg0)
            {
                WriteLogFormat(LogTypeEnum.Error, null, format, arg0);
            }

            public void FatalFormat(string format, object arg0, object arg1, object arg2)
            {
                WriteLogFormat(LogTypeEnum.Fatal, null, format, arg0, arg1, arg2);
            }

            public void FatalFormat(string format, object arg0, object arg1)
            {
                WriteLogFormat(LogTypeEnum.Fatal, null, format, arg0, arg1);
            }

            public void FatalFormat(string format, object arg0)
            {
                WriteLogFormat(LogTypeEnum.Fatal, null, format, arg0);
            }

            public void InfoFormat(string format, object arg0, object arg1, object arg2)
            {
                WriteLogFormat(LogTypeEnum.Info, null, format, arg0, arg1, arg2);
            }

            public void InfoFormat(string format, object arg0, object arg1)
            {
                WriteLogFormat(LogTypeEnum.Info, null, format, arg0, arg1);
            }

            public void InfoFormat(string format, object arg0)
            {
                WriteLogFormat(LogTypeEnum.Info, null, format, arg0);
            }

            public void WarnFormat(string format, object arg0, object arg1, object arg2)
            {
                WriteLogFormat(LogTypeEnum.Warn, null, format, arg0, arg1, arg2);
            }

            public void WarnFormat(string format, object arg0, object arg1)
            {
                WriteLogFormat(LogTypeEnum.Warn, null, format, arg0, arg1);
            }

            public void WarnFormat(string format, object arg0)
            {
                WriteLogFormat(LogTypeEnum.Warn, null, format, arg0);
            }
        }
    }
}
