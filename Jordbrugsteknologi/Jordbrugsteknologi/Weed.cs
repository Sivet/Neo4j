using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Weed
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Weed() { }
        public Weed(string id, string name)
        {
            this.ID = id.GetHashCode();
            this.Name = name;
        }
    }
}
