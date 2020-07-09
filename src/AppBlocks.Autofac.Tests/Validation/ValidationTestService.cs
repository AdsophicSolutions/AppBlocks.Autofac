using AppBlocks.Autofac.Support;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.Validation
{
    [AppBlocksService]
    public class ValidationTestService : IValidationTestService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Method1()
        {
            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(ValidationTestService)}.{nameof(Method1)} called successfully");
        }
    }
}
