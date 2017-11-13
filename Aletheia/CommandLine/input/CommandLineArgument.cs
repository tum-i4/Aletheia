using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.CommandLine.input
{
    public class CommandLineArgument
    {
        private string key;
        private string value;

        public CommandLineArgument(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string Key
        {
            get { return key; }
        }

        public string Value
        {
            get { return value; }
        }
    }
}
