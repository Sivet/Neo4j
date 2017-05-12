using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Herbicide
    {
        public int ID { get; set; }
        public double Dose { get; set; }
        public string Name { get; set; }
        public Herbicide() { }
        public Herbicide(string id, double dose, string name)
        {
            this.ID = id.GetHashCode();
            this.Dose = dose;
            this.Name = name;
        }
    }
}
