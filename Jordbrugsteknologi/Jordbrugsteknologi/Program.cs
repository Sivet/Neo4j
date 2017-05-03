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
        List<Field> result;

        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();
            
        }
        public void Run()
        {
            //Make the different types of weed
            Weed Crabgrass = new Weed("Crabgrass");
            Weed Quackgrass = new Weed("Quackgrass");
            Weed MoringGlory = new Weed("Moring Glory");
            Weed Pigweed = new Weed("Pigweed");

            //Make the different types of herbicide
            Herbicide Simazine = new Herbicide(5.25, "Simazine");
            Herbicide Terbuthylazine = new Herbicide(42, "Terbuthylazine");
            Herbicide Versatil = new Herbicide(2.55, "Versatil");

            //Make the different types of crop
            Crop Wheat = new Crop("Wheat");

            //Make the number of rows in the field
            Row row1 = new Row(1,Crabgrass, Wheat, Simazine);
            Row row2 = new Row(2, Crabgrass, Wheat, Terbuthylazine);
            Row row3 = new Row(3, Crabgrass, Wheat, Versatil);
            Row row4 = new Row(4, Quackgrass, Wheat, Simazine);
            Row row5 = new Row(5, Quackgrass, Wheat, Terbuthylazine);
            Row row6 = new Row(6, Quackgrass, Wheat, Versatil);
            Row row7 = new Row(7, MoringGlory, Wheat, Simazine);
            Row row8 = new Row(8, MoringGlory, Wheat, Terbuthylazine);
            Row row9 = new Row(9, MoringGlory, Wheat, Versatil);
            Row row10 = new Row(10, Pigweed, Wheat, Simazine);
            Row row11 = new Row(11, Pigweed, Wheat, Terbuthylazine);
            Row row12 = new Row(12, Pigweed, Wheat, Versatil);

            //Make the field
            Field field = new Field("Marken");

            //Add all the rows to the list in the field
            field.rows.Add(row1);
            field.rows.Add(row2);
            field.rows.Add(row3);
            field.rows.Add(row4);
            field.rows.Add(row5);
            field.rows.Add(row6);
            field.rows.Add(row7);
            field.rows.Add(row8);
            field.rows.Add(row9);
            field.rows.Add(row10);
            field.rows.Add(row11);
            field.rows.Add(row12);

            CreateCompleteField(field);

            //result = ReadCompleteField(field);

            //Console.ReadKey();
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
        public void CreateCompleteField(Field field)
        {
            List<Row> TempRows = new List<Row>();
            try
            {
                //Empties the list of row to a temp list
                foreach (Row row in field.rows)
                {
                    TempRows.Add(row);
                }
                field.rows = null;

                Connect();
                //Creates the field
                client.Cypher
                        .Create("(field:Field {Name})")
                        .WithParam("Name", field)
                        .ExecuteWithoutResults();

                //Matches the field with its rows and creates the relations
                foreach (Row row in TempRows)
                {
                    //laver temp objecter af Crop, Weed og Herbicide og tømmer den row
                    Crop tempCrop = row.Crop;
                    row.Crop = null;

                    Weed tempWeed = row.Weed;
                    row.Weed = null;

                    Herbicide tempHerbicide = row.Herbicide;
                    row.Herbicide = null;
                    //Opretter row med relation til fielden
                    client.Cypher
                           .Match("(field:Field)")
                           .Where("field.Name = '" + field.Name + "'")
                           .Create("(field)-[:CONTAINS]->(row:Row {Number})")
                           .WithParam("Number", row)
                           .ExecuteWithoutResults();
                    //Opretter crop med relation til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where("row.Number = '" + row.Number + "'")
                        .Create("(row)<-[:PLANTED_IN]-(crop:Crop {Name})")
                        .WithParam("Name", tempCrop)
                        .ExecuteWithoutResults();
                    //Opretter weed med relation til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where("row.Number = '" + row.Number + "'")
                        .Create("(row)<-[:PLANTED_IN]-(weed:Weed {Name})")
                        .WithParam("Name", tempWeed)
                        .ExecuteWithoutResults();
                    //Opretter herbicide til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where("row.Number = '" + row.Number + "'")
                        .Create("(row)<-[:USED_IN]-(herbicide:Herbicide {prop})")
                        .WithParam("Dose", tempHerbicide)
                        .WithParam("Name", tempHerbicide)
                        .ExecuteWithoutResults();
                }

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
        public void DeleteRow(Row row)
        {
            try
            {
                Connect();
                client.Cypher
                    .Match("row:Row")
                    .Where("row.Number = '" + row.Number + "'")
                    .DetachDelete("row")
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
        public void DeleteCrop(Crop crop)
        {

        }
        public void DeleteWeed(Weed weed)
        {

        }
        public void DeleteHerbicide(Herbicide herbicide)
        {

        }
        /// <summary>
        /// ReadCompleteField giver en null exception hvis det field opbject man finder ikke har nogle rows
        /// </summary>
        /// <param name="searchField"></param>
        /// <returns></returns>
        public List<Field> ReadCompleteField(Field searchField)
        {
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
                    item.Field.rows = item.Row.ToList(); //changing the IEnumerable of rows to a list
                    temp.Add(item.Field); //Changing and adding the IEnumerable of Fields a list
                }

                return temp;
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
