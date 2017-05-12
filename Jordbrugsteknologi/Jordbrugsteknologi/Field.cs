using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jordbrugsteknologi
{
    class Field
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public List<Row> rows = new List<Row>();

        public Field(string name, int year)
        {
            this.Name = name;
            this.Year = year;
        }
        public Field()
        {

        }
    }
}
