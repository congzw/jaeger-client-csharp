using System;
using System.Collections.Generic;
using System.Linq;

namespace Jaeger.MySpans
{
    public class MySpanConvert
    {
        public TempSpan ConvertToTempSpan(Span span, DateTime stopUtc)
        {
            var myLocalSpan = new MySpan();

            myLocalSpan.TraceId = span.Context.TraceId.ToString();
            myLocalSpan.SpanId = span.Context.SpanId.ToString();
            myLocalSpan.Flags = (int)span.Context.Flags;
            myLocalSpan.OpName = span.OperationName;

            var references = span.GetReferences();
            foreach (var reference in references)
            {
                var myLocalReference = new MyLocalReference();
                myLocalReference.Type = reference.Type;
                if (reference.Context != null)
                {
                    myLocalReference.SpanID = reference.Context.SpanId.ToString();
                    myLocalReference.TraceID = reference.Context.TraceId.ToString();
                }
                myLocalSpan.References.Add(myLocalReference);
            }

            myLocalSpan.StartTime = span.StartTimestampUtc;
            //public int Duration { get; set; }
            myLocalSpan.StopTime = stopUtc;

            var tags = span.GetTags();
            foreach (var tag in tags)
            {
                myLocalSpan.Tags.Add(tag.Key, tag.Value);
            }

            var logs = span.GetLogs();
            foreach (var log in logs)
            {
                myLocalSpan.Logs.Add(MyLogData.Create(log));
            }
            
            var myProcess = new MyProcess();
            myProcess.ServiceName = span.Tracer.ServiceName;
            foreach (var keyValuePair in span.Tracer.Tags)
            {
                myProcess.Tags.Add(keyValuePair.Key, keyValuePair.Value);
            }

            myLocalSpan.ProcessKey = myProcess.CreateKey();

            var temp = new TempSpan(myProcess, myLocalSpan);
            return temp;
        }

        public MyRecord ConvertToMyRecord(IList<TempSpan> tempSpans)
        {
            var myRecord = new MyRecord();
            foreach (var tempSpan in tempSpans)
            {
                myRecord.Spans.Add(tempSpan.Span);
                var theProcess = myRecord.Processes.SingleOrDefault(x => x.CreateKey() == tempSpan.Process.CreateKey());
                if (theProcess == null)
                {
                    myRecord.Processes.Add(tempSpan.Process);
                }
            }

            return myRecord;
        }
    }
}