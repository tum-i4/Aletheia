using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aletheia.HitSpectra.persistence.cobertura
{
    [XmlType("package")]
    public class Package
    {
        [XmlArray("classes")]
        [XmlArrayItem("class", typeof(Class))]
        public Class[] SourceFiles { get; set; }

        [XmlAttribute]
        public string name;
    }
}
