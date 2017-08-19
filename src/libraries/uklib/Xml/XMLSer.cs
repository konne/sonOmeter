using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace UKLib.Xml
{
    public class XMLSer<T> where T : new()
    {
        public XMLSer()
        {
            try
            {
                xs = new XmlSerializer(typeof(T));
            }
            catch (Exception ex)
            {

            }
        }

        public XMLSer(string defaultNameSpace)
        {
            try
            {
                xs = new XmlSerializer(typeof(T), defaultNameSpace);
            }
            catch (Exception ex)
            {

            }
        }

        public XMLSer(IEnumerable<Type> ExtendedTypes)
        {
            try
            {
                var tempList = new List<Type>();
                foreach (var item in ExtendedTypes)
                    tempList.Add(item);

                if (tempList.Count > 0)
                    xs = new XmlSerializer(typeof(T), tempList.ToArray());
                else
                    xs = new XmlSerializer(typeof(T));
            }
            catch (Exception ex)
            {

            }
        }

        XmlSerializer xs;

        #region Default Functions
        // Summary:
        //     Deserializes the XML document contained by the specified System.IO.TextReader.
        //
        // Parameters:
        //   textReader:
        //     The System.IO.TextReader that contains the XML document to deserialize.
        //
        // Returns:
        //     The System.Object being deserialized.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during deserialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public T Deserialize(System.IO.TextReader textReader)
        {
            return (T)xs.Deserialize(textReader);
        }

        //
        // Summary:
        //     Deserializes the XML document contained by the specified System.xml.XmlReader.
        //
        // Parameters:
        //   xmlReader:
        //     The System.xml.XmlReader that contains the XML document to deserialize.
        //
        // Returns:
        //     The System.Object being deserialized.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during deserialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public T Deserialize(System.Xml.XmlReader xmlReader)
        {
            return (T)xs.Deserialize(xmlReader);
        }

        //
        // Summary:
        //     Deserializes the XML document contained by the specified System.xml.XmlReader
        //     and encoding style.
        //
        // Parameters:
        //   xmlReader:
        //     The System.xml.XmlReader that contains the XML document to deserialize.
        //
        //   encodingStyle:
        //     The encoding style of the serialized XML.
        //
        // Returns:
        //     The deserialized object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during deserialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public T Deserialize(System.Xml.XmlReader xmlReader, string encodingStyle)
        {
            return (T)xs.Deserialize(xmlReader, encodingStyle);
        }

        //
        // Summary:
        //     Deserializes an XML document contained by the specified System.Xml.XmlReader
        //     and allows the overriding of events that occur during deserialization.
        //
        // Parameters:
        //   xmlReader:
        //     The System.Xml.XmlReader that contains the document to deserialize.
        //
        //   events:
        //     An instance of the System.Xml.Serialization.XmlDeserializationEvents class.
        //
        // Returns:
        //     The System.Object being deserialized.
        public T Deserialize(XmlReader xmlReader, XmlDeserializationEvents events)
        {
            return (T)xs.Deserialize(xmlReader, events);
        }

        //
        // Summary:
        //     Deserializes the object using the data contained by the specified System.Xml.XmlReader.
        //
        // Parameters:
        //   xmlReader:
        //     An instance of the System.Xml.XmlReader class used to read the document.
        //
        //   encodingStyle:
        //     The encoding used.
        //
        //   events:
        //     An instance of the System.Xml.Serialization.XmlDeserializationEvents class.
        //
        // Returns:
        //     The object being deserialized.
        public T Deserialize(XmlReader xmlReader, string encodingStyle, XmlDeserializationEvents events)
        {
            return (T)xs.Deserialize(xmlReader, encodingStyle, events);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.Stream.
        //
        // Parameters:
        //   stream:
        //     The System.IO.Stream used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(System.IO.Stream stream, T o)
        {
            xs.Serialize(stream, o);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.TextWriter.
        //
        // Parameters:
        //   textWriter:
        //     The System.IO.TextWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        public void Serialize(System.IO.TextWriter textWriter, T o)
        {
            xs.Serialize(textWriter, o);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.Xml.XmlWriter.
        //
        // Parameters:
        //   xmlWriter:
        //     The System.xml.XmlWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(XmlWriter xmlWriter, T o)
        {
            xs.Serialize(xmlWriter, o);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.Streamthat references the specified namespaces.
        //
        // Parameters:
        //   stream:
        //     The System.IO.Stream used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        //   namespaces:
        //     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(System.IO.Stream stream, T o, XmlSerializerNamespaces namespaces)
        {
            xs.Serialize(stream, o, namespaces);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.IO.TextWriter and references the specified namespaces.
        //
        // Parameters:
        //   textWriter:
        //     The System.IO.TextWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        //   namespaces:
        //     The System.Xml.Serialization.XmlSerializerNamespaces that contains namespaces
        //     for the generated XML document.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(System.IO.TextWriter textWriter, T o, XmlSerializerNamespaces namespaces)
        {
            xs.Serialize(textWriter, o, namespaces);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.Xml.XmlWriter and references the specified namespaces.
        //
        // Parameters:
        //   xmlWriter:
        //     The System.xml.XmlWriter used to write the XML document.
        //
        //   o:
        //     The System.Object to serialize.
        //
        //   namespaces:
        //     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces)
        {
            xs.Serialize(xmlWriter, o, namespaces);
        }

        //
        // Summary:
        //     Serializes the specified object and writes the XML document to a file using
        //     the specified System.Xml.XmlWriter and references the specified namespaces
        //     and encoding style.
        //
        // Parameters:
        //   xmlWriter:
        //     The System.xml.XmlWriter used to write the XML document.
        //
        //   o:
        //     The object to serialize.
        //
        //   namespaces:
        //     The System.Xml.Serialization.XmlSerializerNamespaces referenced by the object.
        //
        //   encodingStyle:
        //     The encoding style of the serialized XML.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     An error occurred during serialization. The original exception is available
        //     using the System.Exception.InnerException property.
        public void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, string encodingStyle)
        {
            xs.Serialize(xmlWriter, o, namespaces, encodingStyle);
        }

        //
        // Summary:
        //     Serializes the specified System.Object and writes the XML document to a file
        //     using the specified System.Xml.XmlWriter, XML namespaces, and encoding.
        //
        // Parameters:
        //   xmlWriter:
        //     The System.Xml.XmlWriter used to write the XML document.
        //
        //   o:
        //     The object to serialize.
        //
        //   namespaces:
        //     An instance of the XmlSerializaerNamespaces that contains namespaces and
        //     prefixes to use.
        //
        //   encodingStyle:
        //     The encoding used in the document.
        //
        //   id:
        //     For SOAP encoded messages, the base used to generate id attributes.
        public void Serialize(XmlWriter xmlWriter, T o, XmlSerializerNamespaces namespaces, string encodingStyle, string id)
        {
            xs.Serialize(xmlWriter, o, namespaces, encodingStyle, id);
        }
        #endregion

        public T Deserialize(string s)
        {
            return (T)xs.Deserialize(new System.IO.StringReader(s));
        }

        public void Serialize(string s, T o)
        {
            s = Serialize(o);
        }

        public string Serialize(T o)
        {
            var sw = new System.IO.StringWriter();
            xs.Serialize(sw, o);
            return sw.ToString();
        }
    }
}
