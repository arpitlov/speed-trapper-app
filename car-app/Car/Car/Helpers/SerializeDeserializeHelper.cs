using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Car.Helpers
{
    public class SerializeDeserializeHelper
    {
        /// <summary>
        /// Reconstruct an object from an XML string
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="xml"> </param>
        /// <param name="isUtf8Encoding"></param>
        /// <returns> </returns>
        public static T DeserializeFromXml<T>(string xml, bool isUtf8Encoding = true)
        {
            T result;
            var serializer = new XmlSerializer(typeof(T));
            if (isUtf8Encoding)
            {
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    result = (T)serializer.Deserialize(stream);
                }
            }
            else
            {
                using (TextReader reader = new StringReader(xml))
                {
                    result = (T)serializer.Deserialize(reader);
                }
            }
            return result;
        }

        /// <summary>
        /// Serialize an object into an XML string
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="obj"> </param>
        /// <param name="isUtf8Encoding"></param>
        /// <returns> </returns>
        public static string SerializeToXml<T>(T obj, bool isUtf8Encoding = true)
        {
            StringWriter output = null;
            if (isUtf8Encoding)
            {
                output = new Utf8StringWriter(new StringBuilder());
            }
            else
            {
                output = new StringWriter(new StringBuilder());
            }

            var serializer = new XmlSerializer(typeof(T));

            serializer.Serialize(output, obj);
            return output.ToString();
        }
    }
}