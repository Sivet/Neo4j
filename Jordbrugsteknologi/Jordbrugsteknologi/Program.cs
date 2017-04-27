using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.Collections;

namespace Jordbrugsteknologi
{
    class Program
    {
        GraphClient client;
        Field field;
        List<Field> result;
        Row row1;
        Row row2;
        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.field = new Field();
            myProgram.row1 = new Row();
            myProgram.row2 = new Row();

            myProgram.field.Name = "Marken";
            myProgram.row1.Number = 42;
            myProgram.row2.Number = 24;

            myProgram.CreateField(myProgram.field);
            myProgram.CreateRow(myProgram.row1, myProgram.field);
            myProgram.CreateRow(myProgram.row2, myProgram.field);

            myProgram.result = myProgram.Test(myProgram.field);

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
        public void CreateField(Field field)
        {
            try
            {
                Connect();
                client.Cypher
                        .Create("(field:Field {prop})")
                        .WithParam("prop", field)
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
        public void CreateRow(Row row, Field field) //Creates a row and makes the relation to a field
        {
            try
            {
                Connect();
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
        public void DeleteField(Field field) //Finds a field and deletes the field and all its relationships BUT not the objects
        {
            try
            {
                Connect();
                client.Cypher
                    .Match("field:Field")
                    .Where("field.Name = '" + field.Name + "'")
                    .DetachDelete("field")
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
        public List<Field> Test(Field searchField)
        {
            //Dictionary<string, List<Row>> temp = new Dictionary<string, List<Row>>();
            List<Field> temp = new List<Field>();
            try
            {
                Connect();
                var a = client.Cypher
                    .OptionalMatch("(field:Field)-[CONTAINS]-(rows:Row)")
                    .Where("field.Name = '" + searchField.Name + "'")
                    .Return((field, rows) => new { Field = field.As<Field>(), Row = rows.CollectAs<Row>() })
                    .Results;

                foreach (var item in a)
                {
                    //Field tempField = item.Field;
                    //tempField.rows = item.Row.ToList();
                    //temp.Add(tempField);
                    item.Field.rows = item.Row.ToList();
                    temp.Add(item.Field);
                    //temp.Add(item.Field.Name, item.Row.ToList());
                }

                return temp;
                //return client.Cypher
                //    .Match("(field:Field)")
                //    .Where("field.Name = '" + fields.Name + "'")
                //    .Return(field => field.As<Field>())
                //    .Results.ToList();



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
