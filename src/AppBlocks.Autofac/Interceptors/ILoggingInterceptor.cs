﻿using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppBlocks.Autofac.Interceptors
{
    public interface ILoggingInterceptor
    {
        void PreMethodInvoke(IInvocation invocation);
        void PostMethodInvoke(IInvocation invocation);
    }
}