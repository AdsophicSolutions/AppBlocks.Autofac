using AppBlocks.Autofac.Support;
using log4net;
using System.Reflection;

namespace AppBlocks.Autofac.Tests.Validation
{
    [AppBlocksService]
    public class ValidationTestExceptionService : IValidationTestExceptionService
    {
        private static readonly ILog logger =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int callCount = 0;
        public static int GetCallCount() => callCount;

        internal static void ResetCount()
        {
            callCount = 0;
        }

        public void Method1()
        {
            callCount++;

            if (logger.IsInfoEnabled)
                logger.Info($"{nameof(ValidationTestExceptionService)}.{nameof(Method1)} called successfully");
        }
    }
}