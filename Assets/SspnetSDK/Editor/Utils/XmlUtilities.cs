using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SspnetSDK.Editor.Unfiled
{
    public abstract class XmlUtilities
    {
        public static int Num;

        internal static bool ParseXmlTextFileElements(
            string filename,
            ParseElement parseElement)
        {
            if (!File.Exists(filename))
                return false;
            try
            {
                using var xmlTextReader = new XmlTextReader(new StreamReader(filename));
                var elementNameStack = new List<string>();
                Func<string> func = () => elementNameStack.Count > 0 ? elementNameStack[0] : "";
                var reader = new Reader(xmlTextReader);
                while (reader.Reading)
                {
                    var name = xmlTextReader.Name;
                    var parentElementName = func();
                    if (xmlTextReader.NodeType == XmlNodeType.Element)
                    {
                        if (parseElement(xmlTextReader, name, true, parentElementName, elementNameStack))
                            elementNameStack.Insert(0, name);
                        if (reader.XmlReaderIsAhead)
                        {
                            reader.Read();
                            continue;
                        }
                    }

                    if ((xmlTextReader.NodeType == XmlNodeType.EndElement ||
                         (xmlTextReader.NodeType == XmlNodeType.Element && xmlTextReader.IsEmptyElement)) &&
                        !string.IsNullOrEmpty(parentElementName))
                    {
                        if (elementNameStack[0] == name)
                            elementNameStack.RemoveAt(0);
                        else
                            elementNameStack.Clear();
                        Num = parseElement(xmlTextReader, name, false, func(), elementNameStack) ? 1 : 0;
                    }

                    reader.Read();
                }
            }
            catch (XmlException ex)
            {
                Debug.Log($"Failed while parsing XML file {filename}\n{ex}\n");
                return false;
            }

            return true;
        }

        public static void FormatXml(string inputXml)
        {
            var document = new XmlDocument();
            document.Load(inputXml);
            using var writer = new XmlTextWriter(inputXml, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            document.Save(writer);
        }

        private class Reader
        {
            private int _lineNumber;
            private int _linePosition;
            private readonly XmlTextReader _reader;

            public Reader(XmlTextReader xmlReader)
            {
                _reader = xmlReader;
                Reading = _reader.Read();
                _lineNumber = _reader.LineNumber;
                _linePosition = _reader.LinePosition;
            }

            public bool Reading { private set; get; }

            public bool XmlReaderIsAhead
            {
                get
                {
                    if (_lineNumber == _reader.LineNumber)
                        return _linePosition != _reader.LinePosition;
                    return true;
                }
            }

            public void Read()
            {
                if (Reading && !XmlReaderIsAhead) Reading = _reader.Read();

                _lineNumber = _reader.LineNumber;
                _linePosition = _reader.LinePosition;
            }
        }

        internal delegate bool ParseElement(
            XmlTextReader reader,
            string elementName,
            bool isStart,
            string parentElementName,
            List<string> elementNameStack);
    }
}