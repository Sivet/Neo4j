using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Row
    {
        public int Number { get; set; }
        public int ID { get; set; }
        public Weed Weed { get; set; }
        public Crop Crop { get; set; }
        public Herbicide Herbicide { get; set; }
        public Row(string id, int number, Weed weed, Crop crop, Herbicide herbicide)
        {
            this.ID = id.GetHashCode();
            this.Number = number;
            this.Weed = weed;
            this.Crop = crop;
            this.Herbicide = herbicide;
        }
        public Row()
        {

        }
    }
}
