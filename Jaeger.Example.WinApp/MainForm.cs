﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jaeger.Example.WinApp.Helpers;

namespace Jaeger.Example.WinApp
{
    public partial class MainForm : AsyncForm
    {
        public MainForm()
        {
            InitializeComponent();
            MyInitializeComponent();
        }

        private void MyInitializeComponent()
        {
            this.WithPrefix = false;

            this.cbxCount.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxCount.Items.Add(1);
            this.cbxCount.Items.Add(2);
            this.cbxCount.Items.Add(5);
            for (int i = 1; i <= 10; i++)
            {
                this.cbxCount.Items.Add(i * 10);
            }
            this.cbxCount.SelectedIndex = 0;
            
            this.cbxSeconds.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbxSeconds.Items.Add(1);
            this.cbxSeconds.Items.Add(2);
            this.cbxSeconds.Items.Add(5);
            this.cbxSeconds.Items.Add(10);
            this.cbxSeconds.Items.Add(30);
            this.cbxSeconds.Items.Add(60);
            this.cbxSeconds.SelectedIndex = 3;
        }

        protected override Control GetInvoker()
        {
            return this.txtLogs;
        }

        public override void ShowCallbackMessage(string value)
        {
            this.txtLogs.AppendText(value);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.txtOps.Text = @"FooApi, FooDomain, FooData";
        }

        private async void btnCall_Click(object sender, EventArgs e)
        {
            var invokeCount = int.Parse(this.cbxCount.SelectedItem.ToString());
            var invokeWait = int.Parse(this.cbxSeconds.SelectedItem.ToString());
            this.btnCall.Enabled = false;
            this.txtOps.Enabled = false;

            var mockOpName = "MyOp";
            for (int i = 0; i < invokeCount; i++)
            {
                var theOpName = mockOpName + (i + 1).ToString("00");
                this.txtLogs.AppendText($"\r\n-----Call {theOpName} at {DateTime.Now}-----\r\n");
                this.txtLogs.AppendText(Environment.NewLine);

                var demoHelper = JaegerFactory.CreateDemoHelper();
                var opTxt = this.txtOps.Text.Trim();
                var ops = opTxt.Split(',', ' ', ';', '，', '；').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                demoHelper.InvokeOp(theOpName, 0, ops);
                await Task.Delay(TimeSpan.FromSeconds(invokeWait));
            }

            this.btnCall.Enabled = true;
            this.txtOps.Enabled = true;
        }
    }

}