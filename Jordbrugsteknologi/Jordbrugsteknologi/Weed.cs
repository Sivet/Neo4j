using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Weed
    {
        public string Name { get; set; }
        public Weed() { }
        public Weed(string id, string name)
        {
            this.Name = name;
        }
    }
}
