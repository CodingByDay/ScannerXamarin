using QRScanner.Appclases;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace QRScanner.Core.App
{
    public class CompactSerializer
    {
        private static Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();

        public static string Serialize<T>(T value)
        {
            //Log.Write(new LogEntry("CompactSerializer.Serialize for " + typeof(T).FullName + " started."));
            var startedAt = DateTime.Now;
            try
            {
                if (value == null)
                {
                    //Log.Write(new LogEntry("CompactSerializer.Serialize for " + typeof(T).FullName + " completed due to null value."));
                    return null;
                }

                XmlSerializer serializer;
                if (serializers.ContainsKey(typeof(T)))
                {
                    serializer = serializers[typeof(T)];
                    //Log.Write(new LogEntry("CompactSerializer.Serialize using existing serializer"));
                }
                else
                {
                    serializer = new XmlSerializer(typeof(T));
                    serializers.Add(typeof(T), serializer);
                    //Log.Write(new LogEntry("CompactSerializer.Serialize using new serializer"));
                }
                
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false, false);
                settings.Indent = false;
                settings.OmitXmlDeclaration = false;
                using (StringWriter textWriter = new StringWriter())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                    {
                        serializer.Serialize(xmlWriter, value);
                    }
                    return textWriter.ToString();
                }
            }
            finally
            {
                QRScanner.Appclases.Log.Write(new QRScanner.Appclases.LogEntry("END REQUEST: [Device/CompactSerializer.Serialize];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
                //Log.Write(new LogEntry("CompactSerializer.Serialize for " + typeof(T).FullName + " completed in " + (Environment.TickCount - startedAt).ToString() + " ticks."));
            }
        }

        public static T Deserialize<T>(string xml)
        {
            //Log.Write(new LogEntry("CompactSerializer.Deserialize for " + typeof(T).FullName + " started."));
            var startedAt = DateTime.Now;
            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    //Log.Write(new LogEntry("CompactSerializer.Deserialize for " + typeof(T).FullName + " completed due to null value."));
                    return default(T);
                }

                XmlSerializer serializer;
                if (serializers.ContainsKey(typeof(T)))
                {
                    serializer = serializers [typeof(T)];
                    //Log.Write(new LogEntry("CompactSerializer.Deserialize using existing serializer"));
                } else {
                    serializer = new XmlSerializer(typeof(T));
                    serializers.Add (typeof(T), serializer);
                    //Log.Write(new LogEntry("CompactSerializer.Deserialize using new serializer"));
                }                 

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.CheckCharacters = false;
                using (StringReader textReader = new StringReader(xml))
                {
                    using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                    {
                        return (T)serializer.Deserialize(xmlReader);
                    }
                }
            }
            finally
            {
                QRScanner.Appclases.Log.Write(new QRScanner.Appclases.LogEntry("END REQUEST: [Device/CompactSerializer.Deserialize];" + (DateTime.Now - startedAt).TotalMilliseconds.ToString()));
                //Log.Write(new LogEntry("CompactSerializer.Deserialize for " + typeof(T).FullName + " completed in " + (Environment.TickCount - startedAt).ToString() + " ticks."));
            }
        }
    }
}
