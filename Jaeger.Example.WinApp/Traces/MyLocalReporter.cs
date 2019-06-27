using System.Threading;
using System.Threading.Tasks;
using Jaeger.Reporters;

namespace Jaeger.Example.WinApp.Traces
{
    public class MyLocalReporter : IReporter
    {
        public MyLocalFileFlusher Flusher { get; set; }

        public MyLocalReporter(MyLocalFileFlusher flusher)
        {
            Flusher = flusher;
        }
        public void Report(Span span)
        {
            //async
            var appendSpanAsync = Flusher.AppendSpanAsync(span);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            //flush any way?
            //_logHelper.Info(@"!!! MyLocalReporter CloseAsync ");
            return Task.FromResult(0);
        }
    }
}
