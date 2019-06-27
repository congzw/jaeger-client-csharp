using System;
using System.Threading;
using System.Threading.Tasks;
using Jaeger.Reporters;

namespace Jaeger.Example.WinApp.Helpers
{
    public class MyLocalReporter : IReporter
    {
        private readonly MyLogHelper _logHelper;
        public MyLocalSpanCache Cache { get; set; }
        public MyLocalSpanConvert Convert { get; set; }

        public MyLocalReporter(MyLocalSpanCache cache, MyLocalSpanConvert convert, MyLogHelper logHelper)
        {
            _logHelper = logHelper;
            Cache = cache;
            Convert = convert;
        }

        public void Report(Span span)
        {
            _logHelper.Info(@"!!! MyLocalReporter Report Span: " + span.OperationName);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _logHelper.Info(@">>> MyLocalReporter CloseAsync " );
            return Task.FromResult(0);
        }
    }
}
