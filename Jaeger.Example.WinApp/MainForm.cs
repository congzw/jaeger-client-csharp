using System;
using System.Linq;
using System.Windows.Forms;
using Jaeger.Example.WinApp.Helpers;

namespace Jaeger.Example.WinApp
{
    public partial class MainForm : AsyncForm
    {
        public MainForm()
        {
            InitializeComponent();
            this.WithPrefix = false;
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

        private void btnCall_Click(object sender, EventArgs e)
        {
            var demoHelper = JaegerFactory.CreateDemoHelper();
            var opTxt = this.txtOps.Text.Trim();
            var ops = opTxt.Split(',', ' ', ';', '，', '；').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            demoHelper.InvokeOp("SomeOpCall", 0, ops);
        }
    }

}