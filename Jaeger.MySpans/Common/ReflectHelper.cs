//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Reflection;

//namespace Jaeger.Common
//{
//    public class ReflectHelper
//    {
//        public ReflectHelper()
//        {
//            LoadedAssemblies = new ConcurrentDictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
//        }

//        public IDictionary<string, Assembly> LoadedAssemblies { get; set; }

//        public object Create(string dllPath, string typeName, params object[] args)
//        {
//            //Load the assembly using Assembly.LoadFile or another overload.
//            //Get the type using Assembly.GetType.
//            //Use the Activator.CreateInstance once you have the type.
//            //Cast it to dynamic(if using .net 4.0.), and call your method, or set your property. 

//            if (!LoadedAssemblies.ContainsKey(dllPath))
//            {
//                LoadedAssemblies.Add(dllPath, Assembly.LoadFile(dllPath));
//            }

//            var instance = LoadedAssemblies[dllPath].CreateInstance(
//                typeName,
//                true,
//                BindingFlags.NonPublic | BindingFlags.Instance,
//                null,
//                args,
//                null,
//                null);
//            return instance;
//        }
//    }
//}
