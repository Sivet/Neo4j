using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Herbicide
    {
        public double Dose { get; set; }
        public string Name { get; set; }
        public Herbicide() { }
        public Herbicide(double dose, string name)
        {
            this.Dose = dose;
            this.Name = name;
        }
    }
}
