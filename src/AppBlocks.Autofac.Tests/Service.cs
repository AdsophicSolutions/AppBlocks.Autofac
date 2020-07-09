using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    [AppBlocksService]
    public class Service : IService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int RunService()
        {
            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(Service)}.{nameof(RunService)} called successfully");

            return 0;
        }
    }
}
