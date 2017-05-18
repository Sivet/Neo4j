using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Crop
    {
        public string Name { get; set; }
        public Crop() { }
        public Crop(string name)
        {
            this.Name = name;
        }
    }
}
