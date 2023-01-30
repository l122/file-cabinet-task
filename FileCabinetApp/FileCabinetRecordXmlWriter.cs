using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// File Cabinet XML Writer Class.
    /// </summary>
    internal class FileCabinetRecordXmlWriter
    {
        private readonly XmlWriter xmlWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> instance.</param>
        public FileCabinetRecordXmlWriter(TextWriter textWriter)
        {
            this.xmlWriter = XmlWriter.Create(textWriter);
        }

        /// <summary>
        /// Wirtes a <see cref="FileCabinetRecord"/> object to an XML file.
        /// </summary>
        /// <param name="record">The <see cref="FileCabinetRecord"/> instance.</param>
        public void Write(FileCabinetRecord record)
        {
            this.xmlWriter.WriteStartDocument();
            this.xmlWriter.WriteStartElement("records");
            this.xmlWriter.WriteStartElement("record");
            this.xmlWriter.WriteAttributeString("id", $"{record.Id}");
            this.xmlWriter.WriteStartElement("name");
            this.xmlWriter.WriteAttributeString("first", $"{record.FirstName}");
            this.xmlWriter.WriteAttributeString("last", $"{record.LastName}");
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.WriteElementString("dateOfBirth", record.DateOfBirth.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
            this.xmlWriter.WriteElementString("PlaceOfWork", $"{record.WorkPlaceNumber}");
            this.xmlWriter.WriteElementString("Salary", $"{record.Salary}");
            this.xmlWriter.WriteElementString("Department", $"{record.Department}");
            this.xmlWriter.WriteEndElement();
            this.xmlWriter.WriteEndDocument();
            this.xmlWriter.Flush();
        }
    }
}
