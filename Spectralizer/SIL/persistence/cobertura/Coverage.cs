using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Spectralizer.SIL.persistence.cobertura
{
    [XmlType("coverage")]
    public class Coverage
    {
        [XmlArray("packages")]
        [XmlArrayItem("package", typeof(Package))]
        public Package[] Package { get; set; }
    }
}
