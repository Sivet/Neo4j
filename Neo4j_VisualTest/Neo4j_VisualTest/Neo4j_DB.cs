using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace Neo4j_VisualTest
{
    class Neo4j_DB
    {
        GraphClient client;
        private void Connect()
        {
            //Make connection
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "Sivet", "glashoffhoff456");
            client.Connect();
        }
        private void Disconnect()
        {
            this.client.Dispose();
        }
        public void Create(Field field, Row row, Herbicide herbicide)
        {
            try
            {
                Connect();
                var temp1 = client.Create(field);
                var temp2 = client.Create(row);
                var tmep3 = client.Create(herbicide);

                //client.CreateRelationship(field,new FieldRelationship(temp2) { flightNumber = "2" });
                client.Cypher
                    .Match("p(Simon")
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Disconnect();
            }

        }
        public void AddRow(Row row)
        {
            try
            {
                Connect();
                client.Create(row);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
