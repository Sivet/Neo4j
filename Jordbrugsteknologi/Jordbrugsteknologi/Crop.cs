using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Crop
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Crop() { }
        public Crop(string id, string name)
        {
            this.ID = id.GetHashCode();
            this.Name = name;
        }
    }
}
