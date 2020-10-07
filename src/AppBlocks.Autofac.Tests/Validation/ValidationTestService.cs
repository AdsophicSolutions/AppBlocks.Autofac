using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests.Validation
{
    [AppBlocksService]
    public class ValidationTestService : IValidationTestService
    {
        private readonly ILogger<ValidationTestService> logger;

        public ValidationTestService(ILogger<ValidationTestService> logger)
        {
            this.logger = logger;
        }

        public void Method1()
        {
            logger.LogInformation($"{nameof(ValidationTestService)}.{nameof(Method1)} called successfully");
        }
    }
}
