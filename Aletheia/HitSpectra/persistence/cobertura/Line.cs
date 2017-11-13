using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Aletheia.HitSpectra.persistence.cobertura
{
    [XmlType("line")]
    public class Line
    {
        [XmlAttribute]
        public int number;

        [XmlAttribute]
        public int hits;

    }
}
