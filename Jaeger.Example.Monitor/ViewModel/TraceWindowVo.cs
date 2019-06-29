using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows;
using EzTrace.UI;
using Jaeger.Common;
using Jaeger.Example.Monitor.Helpers;
using Jaeger.Example.Monitor.Jaegers;
using Jaeger.MySpans;

namespace Jaeger.Example.Monitor.ViewModel
{
    public class TraceWindowVo
    {
        public TraceWindowVo(TraceWindow window)
        {
            TheWindow = window;
        }

        public TraceWindow TheWindow { get; set; }
        public JaegerRunner Runner { get; set; }

        public void InitComponent()
        {
            JaegerEnv.Instance.ClearEnv(EnvironmentVariableTarget.Process);
            TheWindow.BtnStart.Click += BtnStart_Click;
            TheWindow.BtnStop.Click += BtnStop_Click;
            TheWindow.BtnReset.Click += BtnReset_Click;
            TheWindow.BtnLoad.Click += BtnLoad_Click;
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            //todo clear current
            var spanLoader = MyFactory.GetMySpanLoader();
            var myRecords = spanLoader.GetMyRecords();

            var endPoint = @"http://localhost:14268/api/traces";
            var mockTracerFactory = MyFactory.GetMockTracerFactory(endPoint);
            var convert = MyFactory.CreateMySpanConvert();
            var loggerFactory = MyFactory.GetLoggerFactory();


            foreach (var myRecord in myRecords)
            {
                var tempSpans = convert.ConvertBackToTempSpans(myRecord);
                var spanDic = convert.ConvertBackToSpanDic(tempSpans, mockTracerFactory, loggerFactory);
                foreach (var spanItem in spanDic)
                {
                    var tempSpan = spanItem.Key;
                    var span = spanItem.Value;
                    Console.WriteLine(@"=> report: {0} with trace-span: {1}-{2}", tempSpan.Span.OpName, tempSpan.Span.TraceId, tempSpan.Span.SpanId);
                    //report to collector
                    span.Finish(tempSpan.Span.StopTime);
                }
            }
        }
        private object Create(Assembly assembly, string theTypeFullName, params object[] args)
        {
            var theOne = assembly.CreateInstance(theTypeFullName,
                true,
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                args,
                null,
                null);
            return theOne;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (Runner == null)
            {
                Runner = new JaegerRunner();
            }

            if (Runner.IsRunning())
            {
                MessageBox.Show("Jaeger is running!");
                return;
            }

            //jaeger-all-in-one --collector.zipkin.http-port=9411
            var jaegerExe = "jaeger-all-in-one.exe";
            var args = "--collector.zipkin.http-port=9411";
            var jaegerPath = AppDomain.CurrentDomain.Combine(jaegerExe);
            
            if (!File.Exists(jaegerPath))
            {
                MessageBox.Show($"[{jaegerPath}] not exist!");
                return;
            }

            Runner.Start(jaegerPath, args);
            System.Diagnostics.Process.Start("http://localhost:16686");
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Runner == null || !Runner.IsRunning())
            {
                MessageBox.Show("Jaeger is not running!");
                return;
            }

            Runner.Stop();
            Console.WriteLine(@"stop jaeger!");
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (Runner == null)
            {
                Runner = new JaegerRunner();
            }

            if (Runner.IsRunning())
            {
                Runner.Stop();
            }

            Thread.Sleep(2000);
            BtnStart_Click(sender, e);
        }
    }
}
