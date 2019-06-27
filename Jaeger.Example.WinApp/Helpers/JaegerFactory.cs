using System;
using Jaeger.Example.Common;
using Jaeger.Example.WinApp.Traces;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;

namespace Jaeger.Example.WinApp.Helpers
{
    public class JaegerFactory
    {
        private static Tracer _tracer = null;
        public static Tracer GetCurrentTracer()
        {
            return _tracer ?? (_tracer = CreateTracer("http://localhost:14268/api/traces", "Demo-Service"));
        }
        public static Tracer CreateTracer(string endPoint, string serviceName)
        {
            var traceBuilder = new Tracer.Builder(serviceName)
                .WithSampler(new ConstSampler(true));

            var loggerFactory = traceBuilder.LoggerFactory;
            var metrics = traceBuilder.Metrics;

            //try result: 16686:X 14268:OK
            var sender = new HttpSender(endPoint);
            
            var reporter = new RemoteReporter.Builder()
                .WithLoggerFactory(loggerFactory)
                .WithMetrics(metrics)
                .WithSender(sender)
                .Build();

            _theLocalReporter = GetLocalReporter();
            var compositeReporter = new CompositeReporter(_theLocalReporter, reporter);

            var tracer = traceBuilder
                .WithReporter(compositeReporter)
                .Build();
            
            return tracer;
        }

        public static DemoHelper CreateDemoHelper()
        {
            var tracer = GetCurrentTracer();
            return new DemoHelper(tracer);
        }

        private static MyLocalFileFlusher _flusher = null;
        public static MyLocalFileFlusher GetMyLocalFileFlusher()
        {
            return _flusher ?? (_flusher = new MyLocalFileFlusher(TimeSpan.FromSeconds(30), new MyLocalSpanConvert(), GetMyLogHelper()));
        }

        private static MyLogHelper _myLogHelper = null;
        public static MyLogHelper GetMyLogHelper()
        {
            return _myLogHelper ?? (_myLogHelper = new MyLogHelper("[Default]"){WithPrefix = false});
        }

        private static MyLocalReporter _theLocalReporter = null;
        public static MyLocalReporter GetLocalReporter()
        {
            var myLocalFileFlusher = GetMyLocalFileFlusher();
            return _theLocalReporter ?? (_theLocalReporter = new MyLocalReporter(myLocalFileFlusher));
        }

        public static void Init()
        {
            var myLogHelper = GetMyLogHelper();
            StringLogExtensions.ShowFunc = msg => myLogHelper.Info(msg);
        }
    }
}
