using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class CsvFormatter<T> : IFormatter where T: class
{
    private readonly char _delimiter;
    private readonly bool _firstLineIsHeaders;
    public CsvFormatter(char delimiter, bool firstLineIsHeaders = false)
    {
        _delimiter = delimiter;
        _firstLineIsHeaders = firstLineIsHeaders;
    }
    public ISurrogateSelector SurrogateSelector { get; set; }
    public SerializationBinder Binder { get; set; }
    public StreamingContext Context { get; set; }
    
    public object Deserialize(Stream serializationStream)
    {
        IList list;
        using (var sr = new StreamReader(serializationStream))
        {
            // Optional if reading headers! Example: UserId, FirstName, Title
            if (_firstLineIsHeaders)
            {
                string[] headers = GetHeader(sr);
            }
            var listType = typeof (List<>);
            var constructedListType = listType.MakeGenericType(typeof(T));
            list = (IList) Activator.CreateInstance(constructedListType);
            while (sr.Peek() >= 0)
            {
                var line = sr.ReadLine();
                var fieldData = line.Split(_delimiter);
                var obj = FormatterServices.GetUninitializedObject(typeof (T));
                var members = FormatterServices.GetSerializableMembers(obj.GetType(), Context);
                object[] data = new object[members.Length];
                for (int i = 0; i < members.Length; ++i)
                {
                    FieldInfo fi = ((FieldInfo)members[i]);
                    data[i] = Convert.ChangeType(fieldData.ElementAt(i), fi.FieldType, CultureInfo.InvariantCulture);
                }
                list.Add((T)FormatterServices.PopulateObjectMembers(obj, members, data));
            }
        }
        return list;
    }
    private string[] GetHeader(StreamReader sr)
    {
        string line = sr.ReadLine();
        return line.Split(_delimiter)
            .ToList()
            .Select(e=> e.Trim())
            .ToArray();
    }
    public void Serialize(Stream serializationStream, object graph)
    {
        if (!(graph is IEnumerable))
            throw new Exception("This serialize will only work on IEnumerable.");
        var headings = typeof(T).GetProperties();
        var headerNames = headings.Select(e => e.Name.ToString()).ToArray();
        var headers = String.Join(new String(_delimiter, 1), headerNames);
        using (var stream = new StreamWriter(serializationStream))
        {
            if (_firstLineIsHeaders)
            {
                stream.WriteLine(headers);
                stream.Flush();
            }
            var members = FormatterServices.GetSerializableMembers(typeof(T), Context);
            foreach (var item in (IEnumerable)graph)
            {
                var objs = FormatterServices.GetObjectData(item, members);
                var valueList = objs.Select(e => e.ToString()).ToArray();
                var values = String.Join(new String(_delimiter, 1), valueList);
                stream.WriteLine(values);
            }
            stream.Flush();
        }
    }
}