using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp.FileCabinetService
{
    /// <summary>
    /// Helper class for reading data from xml.
    /// </summary>
    public class FileCabinetRecordXmlReader
    {
        private readonly StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="streamReader">A <see cref="StreamReader"/> instance.</param>
        public FileCabinetRecordXmlReader(StreamReader streamReader)
        {
            this.reader = streamReader;
        }

        /// <summary>
        /// Reads xml data into a list.
        /// </summary>
        /// <returns>The <see cref="IList{T}"/> instance of <see cref="FileCabinetRecord"/> items.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var xmlRoot = new XmlRootAttribute
            {
                ElementName = "records",
                IsNullable = true,
            };

            var serializer = new XmlSerializer(typeof(List<FileCabinetRecord>), xmlRoot);

            try
            {
                using var xmlReader = XmlReader.Create(this.reader);
                var data = serializer.Deserialize(xmlReader);
                if (data != null)
                {
                    return (List<FileCabinetRecord>)data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in deserialization: {0}", e.ToString());
            }

            return new List<FileCabinetRecord>();
        }
    }
}
