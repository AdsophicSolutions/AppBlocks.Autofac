using AppBlocks.Autofac.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.ServiceDependency
{
    [AppBlocksLiveService]
    public class LiveService : IService
    {
        private static int callCount;

        public static int GetCallCount() => callCount;
        public static void ResetCount() => callCount = 0;

        public int RunService()
        {
            callCount++;
            return 0;
        }
    }
}
