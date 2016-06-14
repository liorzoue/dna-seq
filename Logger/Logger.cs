using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using slack_client;

namespace Logger
{
    public class Logger
    {
        private static ILog _log;

        public static Level Level()
        {
            return Level();
        }
        public Logger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        public void Log(log4net.Core.Level level, string message)
        {
            
        }
    }
}
