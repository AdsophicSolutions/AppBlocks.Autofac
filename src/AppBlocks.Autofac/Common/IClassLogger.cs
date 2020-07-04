using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Common
{
    public interface IClassLogger
    {
        void PreMethodInvocationLog(IInvocation invocation);
        void PostMethodInvocationLog(IInvocation invocation);
    }
}
