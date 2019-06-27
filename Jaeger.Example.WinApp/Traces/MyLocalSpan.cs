using System;
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
        public string TraceId { get; set; }
        public string SpanId { get; set; }
        public int Flags { get; set; }
        public string OpName { get; set; }
        public IList<MyLocalReference> References { get; set; }
        public DateTime StartTime { get; set; }
        //public int Duration { get; set; }
        public DateTime StopTime { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public IList<LogData> Logs { get; set; }
        
        //public string ProcessID { get; set; }
        //public string Warning { get; set; }
    }
    
    public class MyLocalReference
    {
        public string Type { get; set; }
        public string TraceID { get; set; }
        public string SpanID { get; set; }
    }
}
