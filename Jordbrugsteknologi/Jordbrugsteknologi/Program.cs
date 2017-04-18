using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;

namespace Jordbrugsteknologi
{
    class Program
    {
        static void Main(string[] args)
        {
            //Make connection
            GraphClient client = new GraphClient(new Uri("http://localhost:7474/db/data"), "Sivet", "glashoffhoff456");
            client.Connect();

            //Intantiation objects
            Person person1 = new Person();
            person1.Name = "Simon";
            person1.Height = 42;

            Person person2 = new Person();
            person2.Name = "Benny";
            person2.Height = 40;

            //Create objects
            client.Create(new Person() { Name = person1.Name });
            client.Create(person2);
        }
    }
}
