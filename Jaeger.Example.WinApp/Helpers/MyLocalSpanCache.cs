using System.Collections.Concurrent;

namespace Jaeger.Example.WinApp.Helpers
{
    public class MyLocalSpanCache
    {
        public ConcurrentQueue<MyLocalSpan> Spans { get; set; }

        public MyLocalSpanCache()
        {
            Spans = new ConcurrentQueue<MyLocalSpan>();
        }
    }

    public class MyLocalSpanConvert
    {
        public MyLocalSpan Convert(Span span)
        {
            //todo
            return new MyLocalSpan();
        }
    }
}