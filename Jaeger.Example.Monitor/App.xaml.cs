using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Jaeger.Example.Monitor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += App_Startup;
            this.Exit += App_Exit;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            StartupUri = new Uri("TraceWindow.xaml", UriKind.Relative);
        }
    }
}
