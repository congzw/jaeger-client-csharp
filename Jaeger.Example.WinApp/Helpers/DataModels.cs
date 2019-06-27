using System.Collections.Generic;
using Jaeger.Util;

namespace Jaeger.Example.WinApp.Helpers
{

    public class MyLocalSpan
    {
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
