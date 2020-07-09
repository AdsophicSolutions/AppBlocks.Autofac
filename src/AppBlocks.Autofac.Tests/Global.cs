﻿using log4net;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AppBlocks.Autofac.Tests
{
    [TestClass]
    class Global
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            // The test framework will call this method once -AFTER- each test run.
        }
    }
}