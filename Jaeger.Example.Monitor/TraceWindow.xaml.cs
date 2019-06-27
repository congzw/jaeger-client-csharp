﻿using System.Windows;
using Jaeger.Example.Monitor.ViewModel;

namespace EzTrace.UI
{
    /// <summary>
    /// TraceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TraceWindow : Window
    {
        public TraceWindow()
        {
            InitializeComponent();
            Vo = new TraceWindowVo(this);
            Vo.InitComponent();
        }

        public TraceWindowVo Vo { get; set; }
    }
}
