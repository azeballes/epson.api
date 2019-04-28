using System;
using log4net;
using log4net.Config;

namespace Scanner.Batch
{
    class Logging
    {
        static Logging() => XmlConfigurator.Configure();

        public static ILog Log(Type type) => LogManager.GetLogger(type);

    }
}
