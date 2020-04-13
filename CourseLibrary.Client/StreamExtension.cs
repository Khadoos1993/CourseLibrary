using CourseLibrary.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CourseLibrary.Client
{
    public static class StreamExtension
    {
        public static IEnumerable<T> ReadAndDeserializeWithStream<T>(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException();
            if (!stream.CanRead)
                throw new NotSupportedException("Can't read from this stream");


            using (var streamReader = new StreamReader(stream))
            {
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var jsonSerializer = new JsonSerializer();
                    return jsonSerializer.Deserialize<IEnumerable<T>>(jsonTextReader);
                }
            }
        }
    }
}
