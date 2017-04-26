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
        GraphClient client;
        Field field;
        List<Field> result;
        Row row;
        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.field = new Field();
            myProgram.row = new Row();

            myProgram.field.Name = "Marken";
            myProgram.row.Number = 42;

            myProgram.Create(myProgram.field, myProgram.row);
            
            myProgram.result = myProgram.Test(myProgram.field, myProgram.row);

            Console.ReadKey();
        }
        private void Connect()
        {
            client = new GraphClient(new Uri("http://localhost:7474/db/data"), "username", "password");
            client.Connect();
        }
        private void Disconnect()
        {
            client.Dispose();
        }

        public void Create(Field field, Row row)
        {
            try
            {
                Connect();
                client.Cypher
                        .Create("(field:Field {prop})")
                        .WithParam("prop", field)
                        .ExecuteWithoutResults();

                client.Cypher
                       .Match("(field:Field)")
                        .Where("field.Name = '" + field.Name + "'")
                        .Create("(field)-[:CONTAINS]->(row:Row {prop})")
                       .WithParam("prop", row)
                       .ExecuteWithoutResults();
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
        public void Delete(Field field)
        {

            try
            {
                Connect();
                
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
        public List<Field> Test(Field fields, Row row)
        {
            try
            {
                Connect();
                return client.Cypher
                    .Match("(field:Field)")
                    .Where("field.Name = '" + fields.Name + "'")
                    .Return(field => field.As<Field>())
                    .Results.ToList();
                
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
