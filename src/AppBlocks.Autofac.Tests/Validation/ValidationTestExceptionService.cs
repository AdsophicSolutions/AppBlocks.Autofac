using AppBlocks.Autofac.Support;
using log4net;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AppBlocks.Autofac.Tests.Validation
{
    [AppBlocksService]
    public class ValidationTestExceptionService : IValidationTestExceptionService
    {
        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        private readonly ILogger<ValidationTestExceptionService> logger;

        public ValidationTestExceptionService(ILogger<ValidationTestExceptionService> logger)
        {
            this.logger = logger;
        }

        internal static void ResetCount()
        {
            callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            logger.LogInformation($"{nameof(ValidationTestExceptionService)}.{nameof(Method1)} called successfully");
        }
    }
}