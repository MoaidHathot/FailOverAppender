using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace MoreAppenders
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var logger = LogManager.GetLogger(typeof(Program));

            for (var index = 0; index < int.MaxValue; ++index)
            {
                logger.Debug($"This is a debug for index {index} ");
            }

            Console.ReadLine();
        }
    }
}
