using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Jaeger.Example.Common;
using Jaeger.Example.WinApp.Helpers;

namespace Jaeger.Example.WinApp.Traces
{
    public interface IMySpanRecorder
    {
        Task RecordSpanAsync(Span span);
        void Flush();
    }

    public class MySpanRecorder : IMySpanRecorder
    {
        public Task RecordSpanAsync(Span span)
        {
            return Task.Run(() =>
            {
                var temp = Convert.ConvertToTempSpan(span, span.Tracer.Clock.UtcNow());
                TempSpans.Enqueue(temp);
            });
        }
        
        public Func<bool> ShouldRecording = () => true;

        private readonly MyLogHelper _logHelper;
        private readonly Task _flushTask;
        public TimeSpan FlushInterval { get; set; }
        public ConcurrentQueue<TempSpan> TempSpans { get; set; }
        public MySpanConvert Convert { get; set; }
        public IMySpanStorage Storage { get; set; }

        public MySpanRecorder(IMySpanStorage storage, TimeSpan flushInterval, MySpanConvert convert, MyLogHelper logHelper)
        {
            Storage = storage;
            _logHelper = logHelper;
            FlushInterval = flushInterval;
            Convert = convert;
            TempSpans = new ConcurrentQueue<TempSpan>();
            _flushTask = Task.Factory.StartNew(FlushLoop, TaskCreationOptions.LongRunning);
        }

        private void SaveTempSpans(IList<TempSpan> tempSpans)
        {
            if (tempSpans == null || tempSpans.Count == 0)
            {
                return;
            }

            try
            {
                var filePath = Storage.AutoCreateFilePath();
                var myRecord = Convert.ConvertToMyRecord(tempSpans);
                Storage.AddOrUpdate(filePath, myRecord);
            }
            catch (Exception ex)
            {
                _logHelper.InfoException(ex);
            }
        }

        public void Flush()
        {
            var tempSpans = new List<TempSpan>();
            do
            {
                TempSpans.TryDequeue(out var result);
                if (result != null)
                {
                    tempSpans.Add(result);
                }
            }
            while (!TempSpans.IsEmpty);

            if (tempSpans.Count > 0)
            {
                SaveTempSpans(tempSpans);
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
            Flush();
        }
    }
}
