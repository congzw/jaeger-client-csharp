using System;
using System.Collections.Generic;
using Jaeger.Common;
using Jaeger.MySpans;

namespace Jaeger.Example.Monitor.Helpers
{
    public class MyFactory
    {
        public static IJsonFileHelper CreateJsonFileHelper()
        {
            var jsonFileHelper = JsonFileHelper.Resolve();
            return jsonFileHelper;
        }


        private static MySpanStorage _storage = null;
        public static MySpanStorage GetMySpanStorage()
        {
            var jsonFileHelper = CreateJsonFileHelper();
            return _storage ?? (_storage = new MySpanStorage(jsonFileHelper, () => AppDomain.CurrentDomain.Combine("traces")));
        }
    }

    public class MySpanLoader
    {
        public MySpanStorage Storage { get; set; }

        public MySpanLoader(MySpanStorage storage)
        {
            Storage = storage;
        }

        public IList<MyRecord> GetMyRecords()
        {
            return null;
        }

        public IList<Span> LoadSpans(params MyRecord[] myRecords)
        {
            return new List<Span>();
        }
    }
}
