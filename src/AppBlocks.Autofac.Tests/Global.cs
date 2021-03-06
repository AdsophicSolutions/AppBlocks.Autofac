﻿using AppBlocks.Autofac.Common;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
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
            // set via test.runsettings. Tests can be run using serilog
            // or using log4net
            if ((string)testContext.Properties["logType"] == "serilog")
            {

                Log.Logger = new LoggerConfiguration()
                             .Enrich
                             .FromLogContext()
                             .WriteTo
                             .Console()
                             .CreateLogger();

                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddSerilog();
                }
                );

                AppBlocksLogging.Instance.SetLoggerFactory(loggerFactory);
            }
            else 
                AppBlocksLogging.Instance.UseLog4Net("log4net.config");
        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            // The test framework will call this method once -AFTER- each test run.
        }
    }
}
