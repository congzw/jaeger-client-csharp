using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jaeger.Example.Common;
using Jaeger.Example.WinApp.Helpers;

namespace Jaeger.Example.WinApp.Traces
{
    public class MyLocalFileFlusher: IDisposable
    {
        public async Task AppendSpanAsync(Span span)
        {
            await Task.Run(() =>
            {
                //_logHelper.Info(@"!!! AppendSpanAsync: " + span.OperationName);
                var myLocalSpan = Convert.Convert(span);
                MyLocalSpans.Enqueue(myLocalSpan);
            });
        }

        public Func<bool> ShouldRecording = () => true;

        private readonly MyLogHelper _logHelper;
        private readonly Task _flushTask;
        public TimeSpan FlushInterval { get; set; }
        public ConcurrentQueue<MyLocalSpan> MyLocalSpans { get; set; }
        public MyLocalSpanConvert Convert { get; set; }
        
        public MyLocalFileFlusher(TimeSpan flushInterval, MyLocalSpanConvert convert, MyLogHelper logHelper)
        {
            _logHelper = logHelper;
            FlushInterval = flushInterval;
            Convert = convert;
            MyLocalSpans = new ConcurrentQueue<MyLocalSpan>();
            _flushTask = Task.Factory.StartNew(FlushLoop, TaskCreationOptions.LongRunning);
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
                if (!_disposing)
                {
                    _logHelper.Info($"try save {toSaveSpans.Count} spans to file: {filePath}");
                }

                File.AppendAllText(AppDomain.CurrentDomain.Combine($"trace_flush.txt"), 
                    string.Format("{0} flush count: {1}{2}", 
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                        toSaveSpans.Count, 
                        Environment.NewLine));
            }
            catch (Exception ex)
            {
                if (!_disposing)
                {
                    _logHelper.InfoException(ex);
                }
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

        private bool _disposing = false;
        public void Dispose()
        {
            try
            {
                _disposing = true;
                Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                File.AppendAllText(AppDomain.CurrentDomain.Combine($"trace_ex.txt"),  e.Message);
            }
            _flushTask?.Dispose();
        }
    }
}
