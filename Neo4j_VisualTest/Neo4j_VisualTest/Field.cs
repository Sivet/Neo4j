using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo4j_VisualTest
{
    class Field
    {
        public string Name { get; set; }
        public Field(string Name)
        {
            this.Name = Name;
        }
    }
}
