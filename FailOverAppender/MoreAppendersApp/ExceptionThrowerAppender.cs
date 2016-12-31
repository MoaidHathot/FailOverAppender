using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace MoreAppenders
{
    //For testing purposes...throws exception once per <ThrowExceptionForCount>
    public class ExceptionThrowerAppender : ConsoleAppender
    {
        public int ThrowExceptionForCount { get; set; } = 1;
        private int _count;

        public ExceptionThrowerAppender()
        {
            ErrorHandler = new PropogateErrorHandler();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (0 == System.Threading.Interlocked.Increment(ref _count) % ThrowExceptionForCount)
            {
                throw new Exception($"Interval {ThrowExceptionForCount} is reached with counter {_count}");
            }

            base.Append(loggingEvent);
        }
    }
}
