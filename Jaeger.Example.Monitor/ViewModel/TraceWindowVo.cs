using System;
using System.IO;
using System.Threading;
using System.Windows;
using EzTrace.UI;
using Jaeger.Example.Common;
using Jaeger.Example.Monitor.Jaegers;

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
