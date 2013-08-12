using System;
using log4net;
using log4net.Config;
using log4net.Core;

namespace ru_football
{
    public class LogWrapper : ILog
    {
        private readonly ILog log;

        ///<summary>
        /// default
        ///</summary>
        public LogWrapper()
            : this("logger")
        {
        }

        ///<summary>
        /// logger with custom name
        ///</summary>
        public LogWrapper(string name)
        {
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(name);
        }

        public ILogger Logger
        {
            get { return log.Logger; }
        }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        public void DebugFormat(string format, object arg0)
        {
            log.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            log.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            log.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.DebugFormat(provider, format, args);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            log.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            log.InfoFormat(format, args);
        }

        public void InfoFormat(string format, object arg0)
        {
            log.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            log.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            log.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            log.WarnFormat(format, args);
        }

        public void WarnFormat(string format, object arg0)
        {
            log.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            log.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            log.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.WarnFormat(provider, format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            log.ErrorFormat(format, args);
        }

        public void ErrorFormat(string format, object arg0)
        {
            log.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            log.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            log.FatalFormat(format, args);
        }

        public void FatalFormat(string format, object arg0)
        {
            log.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            log.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            log.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            log.FatalFormat(provider, format, args);
        }

        public bool IsDebugEnabled
        {
            get { return log.IsDebugEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return log.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return log.IsWarnEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return log.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return log.IsFatalEnabled; }
        }
    }
}