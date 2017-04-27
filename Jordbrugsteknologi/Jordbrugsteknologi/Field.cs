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
        public List<Row> rows;

        public Field()
        {
            if (rows == null)
            {
                rows = new List<Row>();
            }
        }
    }
}
