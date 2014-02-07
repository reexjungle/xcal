using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace reexmonkey.crosscut.io
{

    public static class XmlSerializationExtensions
    {

        private static void SerializeToXml<TValue>(this TValue value, XmlWriter xw, XmlSerializer xs, bool warning, bool flush )
        { 
             if (warning) xw.WriteComment("warning");
             xs.Serialize(xw, value);
             if (flush) xw.Flush();
        }

        public static Stream WriteToXml<TValue>(this TValue value, Stream stream)
        {
           var type = typeof(TValue);
           try
           {
               var xs = new XmlSerializer(type);
               var settings = new XmlWriterSettings { Indent = true, IndentChars = "    " };
               var xw = XmlWriter.Create(stream, settings);
               value.SerializeToXml(xw, xs, true, true);
               
           }
           catch (SerializationException ex)
           { 
               System.Diagnostics.Debug.WriteLine(ex.Message);           
           }
           catch (Exception ex)
           {
               System.Diagnostics.Debug.WriteLine(ex.Message);
           }

           return stream;
        }

        public static TextWriter WriteToXml<TValue>(this TValue value, TextWriter writer)
        {
            var type = typeof(TValue);
            try
            {
                var xs = new XmlSerializer(type);
                var settings = new XmlWriterSettings { Indent = true, IndentChars = "    " };
                var xw = XmlWriter.Create(writer, settings);
                value.SerializeToXml(xw, xs, true, true);

            }
            catch (SerializationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return writer;
        }

        public static void WriteToXml<TValue>(this TValue value, string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write)) value.WriteToXml(fs);
            }
            catch (UnauthorizedAccessException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }



    }
}
