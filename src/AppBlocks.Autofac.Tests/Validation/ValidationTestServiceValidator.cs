using AppBlocks.Autofac.Common;
using AppBlocks.Autofac.Support;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Tests.Validation
{
    [AppBlocksValidatorService("AppBlocks.Autofac.Tests.Validation.ValidationTestService")]
    public class ValidationTestServiceValidator : IServiceValidator
    {
        private static int inputParametersCallCount = 0;
        public static int GetInputParametersCallCount() => inputParametersCallCount;

        private static int resultCallCount = 0;

        internal static void ResetCount()
        {
            resultCallCount = 0;
            inputParametersCallCount = 0;
        }

        public static int GetResultCallCount() => resultCallCount;

        public void ValidateInputParameters(IInvocation invocation)
        {
            inputParametersCallCount++;
        }

        public void ValidateResult(IInvocation invocation)
        {
            resultCallCount++;
        }
    }
}
