using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jaeger.Example.Common;
using Jaeger.Reporters;

namespace Jaeger.Example.WinApp.Helpers
{
    public class MyLocalReporter : IReporter
    {
        public static Func<bool> ShouldRecording = () => true;

        private readonly MyLogHelper _logHelper;
        private readonly Task _flushTask;
        public TimeSpan FlushInterval { get; set; }
        public ConcurrentQueue<MyLocalSpan> MyLocalSpans { get; set; }

        public MyLocalReporter(TimeSpan flushInterval, MyLocalSpanConvert convert, MyLogHelper logHelper)
        {
            _logHelper = logHelper;
            FlushInterval = flushInterval;
            Convert = convert;
            MyLocalSpans = new ConcurrentQueue<MyLocalSpan>();
           _flushTask = Task.Factory.StartNew(FlushLoop, TaskCreationOptions.LongRunning);
        }
        
        public MyLocalSpanConvert Convert { get; set; }

        public void Report(Span span)
        {
            //_logHelper.Info(@"!!! MyLocalReporter Report Span: " + span.OperationName);
            var myLocalSpan = Convert.Convert(span);
            MyLocalSpans.Enqueue(myLocalSpan);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            //flush any way?
            //_logHelper.Info(@"!!! MyLocalReporter CloseAsync ");
            return Task.FromResult(0);
        }
        
        private void SaveToFile(IList<MyLocalSpan> myLocalSpans)
        {
            if (myLocalSpans == null || myLocalSpans.Count == 0)
            {
                return;
            }

            try
            {
                var toSaveSpans = new List<MyLocalSpan>();
                var jsonFileHelper = JsonFileHelper.Resolve();
                var filePath = AppDomain.CurrentDomain.Combine($"trace_{DateTime.Now:yyyy-MM-dd_HH}.json");
                if (File.Exists(filePath))
                {
                    var oldSpans = jsonFileHelper.Load<IList<MyLocalSpan>>(filePath);
                    toSaveSpans.AddRange(oldSpans);
                }
                toSaveSpans.AddRange(myLocalSpans);
                jsonFileHelper.Save(toSaveSpans, filePath);
                _logHelper.Info($"try save {toSaveSpans} spans to file: {filePath}");

                File.AppendAllText(AppDomain.CurrentDomain.Combine($"trace_flush.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss\r\n"));
            }
            catch (Exception ex)
            {
                _logHelper.InfoException(ex);
            }
        }

        private void Flush()
        {
            Console.WriteLine(@"!!!Flush!!!");
            var localSpans = new List<MyLocalSpan>();
            do
            {
                MyLocalSpans.TryDequeue(out var result);
                if (result != null)
                {
                    localSpans.Add(result);
                }
            }
            while (!MyLocalSpans.IsEmpty);

            if (localSpans.Count > 0)
            {
                SaveToFile(localSpans);
            }
        }

        private async void FlushLoop()
        {
            while (ShouldRecording())
            {
                // First flush should happen later so we start with the delay
                await Task.Delay(FlushInterval).ConfigureAwait(false);
                Flush();
            }
        }
    }
}
