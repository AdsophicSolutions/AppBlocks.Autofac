﻿using AppBlocks.Autofac.Common;
using System;

namespace AppBlocks.Autofac.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AppBlocksClassValidatorWriterServiceAttribute : AppBlocksServiceAttribute
    {
        public AppBlocksClassValidatorWriterServiceAttribute(string ClassFullName) :
            base(
                ClassFullName,
                typeof(IClassValidator),
                EnumAppBlocksInstanceLifetime.SingleInstance,
                new string[0],
                new string[0],
                true)
        {
            if (string.IsNullOrWhiteSpace(ClassFullName)) throw new Exception("ClassFullName Name cannot be null or whitespace");
            this.ClassFullName = ClassFullName;
        }

        public string ClassFullName { get; }
    }
}
