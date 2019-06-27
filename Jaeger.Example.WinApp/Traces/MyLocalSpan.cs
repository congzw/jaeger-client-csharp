using System.Collections.Generic;
using Jaeger.Util;

namespace Jaeger.Example.WinApp.Traces
{
    //todo: short name for smaller json content size
    public class MyLocalSpan
    {
        public MyLocalSpan()
        {
            Logs = new List<LogData>();
            Tags = new Dictionary<string, object>();
            References = new List<MyLocalReference>();
        }
        public string OpName { get; set; }
        public string SpanId { get; set; }
        public string TraceId { get; set; }
        public IList<LogData> Logs { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public IList<MyLocalReference> References { get; set; }
    }
    
    public sealed class MyLocalReference : ValueObject
    {
        public string Type { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Type;
        }
    }
}
