using AppBlocks.Autofac.Support;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Autofac.Tests.Logging
{
    [AppBlocksService]
    public class LoggingElevatedToWarnService : ILoggingElevatedToWarn
    {
        private readonly ILogger<LoggingElevatedToWarnService> logger;

        public LoggingElevatedToWarnService(ILogger<LoggingElevatedToWarnService> logger)
        {
            this.logger = logger;
        }

        public void Method1(string inputValue) =>
            logger.LogInformation($"{nameof(LoggingElevatedToWarnService)}.{nameof(Method1)} called successfully");
        
    }
}
