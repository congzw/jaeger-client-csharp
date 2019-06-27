namespace Jaeger.Example.WinApp.Traces
{
    public class MyLocalSpanConvert
    {
        public MyLocalSpan Convert(Span span)
        {
            //todo
            var myLocalSpan = new MyLocalSpan();
            myLocalSpan.TraceId = span.Context.TraceId.ToString();
            myLocalSpan.SpanId = span.Context.SpanId.ToString();
            myLocalSpan.OpName = span.OperationName;
            return myLocalSpan;
        }
    }
}