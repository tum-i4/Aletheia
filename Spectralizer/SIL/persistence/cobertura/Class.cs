using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spectralizer.SIL.persistence.cobertura
{
    public enum SourceType
    {
        c,
        cpp,
        h,
        hpp,
        UNKNOWN
    }

    [XmlType("class")]
    public class Class
    {
        [XmlAttribute]
        public string name;

        [XmlAttribute]
        public string filename;

        //private Dictionary<Function, bool> sourceFileSpectra;

        [XmlArray("lines")]
        [XmlArrayItem("line", typeof(Line))]
        public Line[] LinesOfCode { get; set; }

        public string FileName
        {
            get { return this.name; }
        }

        public string FilePath
        {
            get { return this.filename; }
        }

        public SourceType SourceType
        {
            get
            {
                string fileType = this.FileName.Split('.')[1];

                switch (fileType)
                {
                    case "h": return SourceType.h;
                    case "c": return SourceType.c;
                    case "cpp": return SourceType.cpp;
                    case "hpp": return SourceType.hpp;
                    default: return SourceType.UNKNOWN;
                }
            }
        }
    }
}
