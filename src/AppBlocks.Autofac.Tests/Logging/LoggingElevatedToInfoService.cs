using AppBlocks.Autofac.Support;
using Microsoft.Extensions.Logging;

namespace AppBlocks.Autofac.Tests.Logging
{
    [AppBlocksService]
    public class LoggingElevatedToInfoService : ILoggingElevatedToInfo
    {
        private readonly ILogger<LoggingElevatedToInfoService> logger;

        public LoggingElevatedToInfoService(ILogger<LoggingElevatedToInfoService> logger)
        {
            this.logger = logger;
        }

        public void Method1(string inputValue) =>
            logger.LogInformation($"{nameof(LoggingElevatedToInfoService)}.{nameof(Method1)} called successfully");
        
    }
}
