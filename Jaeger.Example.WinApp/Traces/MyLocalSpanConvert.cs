using System;

namespace Jaeger.Example.WinApp.Traces
{
    public class MyLocalSpanConvert
    {
        public MyLocalSpan Convert(Span span, DateTime stopUtc)
        {
            //todo
            var myLocalSpan = new MyLocalSpan();

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
                myLocalSpan.Logs.Add(new LogData(log.TimestampUtc, log.Message));
            }
            
            return myLocalSpan;
        }
        
        //public MyLocalSpan ConvertToMyLocalSpan(Span span)
        //{
        //    var convertSpan = JaegerThriftSpanConverter.ConvertSpan(span);
        //    Console.WriteLine(convertSpan.ToJson(false));

        //    var tags = span.GetTags();
        //    var dictionary = new Dictionary<string, object>();
        //    foreach (var tag in tags)
        //    {
        //        dictionary.Add(tag.Key, tag.Value);
        //    }

        //    var spanContext = (SpanContext)span.Context;

        //    //var parentIdString = SpanId.FromString("0");
        //    //Console.WriteLine(">>> => " + spanContext.ParentId.ToString());
        //    //Console.WriteLine(">>> => " + SpanId.FromString("0"));

        //    var context = new SpanContext(TraceId.FromString(spanContext.TraceId.ToString()),
        //        SpanId.FromString(spanContext.SpanId.ToString()),
        //        SpanId.FromString(spanContext.ParentId.ToString()),
        //        spanContext.Flags);

        //    var span1 = new Span(span.Tracer, span.OperationName + "-COPY", context, span.StartTimestampUtc, dictionary,
        //        span.GetReferences());

        //    span1.Tracer.Reporter.Report(span1);

        //    //set 

        //    return null;
        //}

        //public static Span CopySpan(Span span)
        //{
        //    var tags = span.GetTags();
        //    var dictionary = new Dictionary<string, object>();
        //    foreach (var tag in tags)
        //    {
        //        dictionary.Add(tag.Key, tag.Value);
        //    }

        //    var spanContext = (SpanContext)span.Context;
        //    var context = new SpanContext(TraceId.FromString(spanContext.TraceId.ToString()),
        //        SpanId.FromString(spanContext.SpanId.ToString()),
        //        SpanId.FromString(spanContext.ParentId.ToString()),
        //        spanContext.Flags);

        //    var span1 = new Span(span.Tracer, span.OperationName + "-COPY", context, span.StartTimestampUtc, dictionary,
        //        span.GetReferences());

        //    var logs = span.GetLogs().ToList();
        //    foreach (var log in logs)
        //    {
        //        span1.Log(log.TimestampUtc, log.Fields);
        //    }

        //    span1.FinishTimestampUtc = span.Tracer.Clock.UtcNow();
        //    //set finish time
        //    return span1;
        //}

    }
}