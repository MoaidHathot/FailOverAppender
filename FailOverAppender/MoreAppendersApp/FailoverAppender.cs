using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;

namespace MoreAppenders
{
    public class FailoverAppender : AppenderSkeleton
    {
        private AppenderSkeleton _primaryAppender;
        private AppenderSkeleton _failOverAppender;

        //Public setters are necessary for configuring
        //the appender using a config file
        public AppenderSkeleton PrimaryAppender
        {
            get { return _primaryAppender; }
            set
            {
                _primaryAppender = value;
                SetAppenderErrorHandler(value);
            }
        }

        public AppenderSkeleton FailOverAppender
        {
            get { return _failOverAppender; }
            set
            {
                _failOverAppender = value;
                SetAppenderErrorHandler(value);
            }
        }

        public IErrorHandler DefaultErrorHandler { get; set; }

        //Whether to use the failover Appender or not
        public bool LogToFailOverAppender { get; private set; }

        public FailoverAppender()
        {
            //The ErrorHandler property is defined in
            //AppenderSkeleton
            DefaultErrorHandler = ErrorHandler;
            ErrorHandler = new FailOverErrorHandler(this);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (LogToFailOverAppender)
            {
                _failOverAppender?.DoAppend(loggingEvent);
            }
            else
            {
                try
                {
                    _primaryAppender?.DoAppend(loggingEvent);
                }
                catch
                {
                    ActivateFailOverMode();
                    Append(loggingEvent);
                }
            }
        }

        private void SetAppenderErrorHandler(AppenderSkeleton appender) 
            => appender.ErrorHandler = new PropogateErrorHandler();

        internal void ActivateFailOverMode()
        {
            ErrorHandler = DefaultErrorHandler;
            LogToFailOverAppender = true;
        }
    }

    /*
This is important. 
By default the AppenderSkeleton's ErrorHandler doesn't
propagate exceptions
*/
    class PropogateErrorHandler : IErrorHandler
    {
        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            throw new AggregateException(e);
        }

        public void Error(string message, Exception e)
        {
            throw new AggregateException(e);
        }

        public void Error(string message)
        {
            throw new LogException($"Error logging an event: {message}");
        }
    }

/*
This is just in case something happens. It signals 
the FailoverAppender to use the failback appender.
*/
class FailOverErrorHandler : IErrorHandler
    {
        public FailoverAppender FailoverAppender { get; set; }

        public FailOverErrorHandler(FailoverAppender failoverAppender)
        {
            FailoverAppender = failoverAppender;
        }

        public void Error(string message, Exception e, ErrorCode errorCode) 
            => FailoverAppender.ActivateFailOverMode();

        public void Error(string message, Exception e) 
            => FailoverAppender.ActivateFailOverMode();

        public void Error(string message) 
            => FailoverAppender.ActivateFailOverMode();
    }
}
