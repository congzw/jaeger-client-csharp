using System;
using Jaeger.Common;
using Jaeger.MySpans;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders;
using Microsoft.Extensions.Logging;

namespace Jaeger.Example.WinApp.Helpers
{
    public class JaegerFactory
    {
        public static DemoHelper CreateDemoHelper()
        {
            var tracer = GetCurrentTracer();
            return new DemoHelper(tracer);
        }

        public static IJsonFileHelper CreateJsonFileHelper()
        {
            var jsonFileHelper = JsonFileHelper.Resolve();
            return jsonFileHelper;
        }

        private static MySpanStorage _storage = null;
        public static MySpanStorage GetMySpanStorage()
        {
            var jsonFileHelper = CreateJsonFileHelper();
            return _storage ?? (_storage = new MySpanStorage(jsonFileHelper, () => AppDomain.CurrentDomain.Combine("traces")));
        }


        private static ILoggerFactory _logFactory = null;
        public static ILoggerFactory GetLoggerFactory()
        {
            return _logFactory ?? (_logFactory = new MyLoggerFactory());
        }
        

        private static MySpanRecorder _recorder = null;
        public static MySpanRecorder GetMySpanRecorder()
        {
            var myRecordStorage = GetMySpanStorage();
            var myLogHelper = GetLoggerFactory();
            var convert = new MySpanConvert();
            return _recorder ?? (_recorder = new MySpanRecorder(myRecordStorage, TimeSpan.FromSeconds(3), convert, myLogHelper.CreateLogger(null)));
        }
        
        private static MySpanReporter _theSpanReporter = null;
        public static MySpanReporter GetMySpanReporter()
        {
            var recorder = GetMySpanRecorder();
            return _theSpanReporter ?? (_theSpanReporter = new MySpanReporter(recorder));
        }


        private static Tracer _tracer = null;
        public static Tracer GetCurrentTracer()
        {
            return _tracer ?? (_tracer = CreateTracer("http://localhost:14268/api/traces", "Demo-Service"));
        }
        public static Tracer CreateTracer(string endPoint, string serviceName)
        {
            var myLoggerFactory = GetLoggerFactory();

            var traceBuilder = new Tracer.Builder(serviceName)
                .WithSampler(new ConstSampler(true))
                .WithLoggerFactory(myLoggerFactory);

            //var loggerFactory = traceBuilder.LoggerFactory;
            var metrics = traceBuilder.Metrics;

            //try result: 16686:X 14268:OK
            var sender = new HttpSender(endPoint);

            var reporter = new RemoteReporter.Builder()
                .WithLoggerFactory(myLoggerFactory)
                .WithMetrics(metrics)
                .WithSender(sender)
                .Build();

            _theSpanReporter = GetMySpanReporter();
            var compositeReporter = new CompositeReporter(_theSpanReporter, reporter);

            var tracer = traceBuilder
                .WithReporter(compositeReporter)
                .Build();

            //set clock
            var storage = GetMySpanStorage();
            storage.GetClock = () => tracer.Clock.UtcNow();

            return tracer;
        }

        public static void Init()
        {
            var loggerFactory = GetLoggerFactory();
            var logger = loggerFactory.CreateLogger(null);
            StringLogExtensions.ShowFunc = msg => logger.Log(LogLevel.Information, msg);
        }
    }
}
