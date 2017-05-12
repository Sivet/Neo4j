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
        //static int id = 0;
        GraphClient client;
        List<Field> resultField;
        Row resultRow;

        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();

        }
        public void Run()
        {
            //Make the field
            Field field = new Field("Marken", 2017);

            //Make the different types of weed
            Weed Crabgrass = new Weed(field.Name, "Crabgrass");
            Weed Quackgrass = new Weed(field.Name, "Quackgrass");
            Weed MoringGlory = new Weed(field.Name, "Moring Glory");
            Weed Pigweed = new Weed(field.Name, "Pigweed");

            //Make the different types of herbicide
            Herbicide Simazine = new Herbicide(field.Name, 5.25, "Simazine");
            Herbicide Terbuthylazine = new Herbicide(field.Name, 42, "Terbuthylazine");
            Herbicide Versatil = new Herbicide(field.Name, 2.55, "Versatil");

            //Make the different types of crop
            Crop Wheat = new Crop(field.Name, "Wheat");

            //Make the number of rows in the field
            Row row1 = new Row(field.Name, 1, Crabgrass, Wheat, Simazine);
            Row row2 = new Row(field.Name, 2, Crabgrass, Wheat, Terbuthylazine);
            Row row3 = new Row(field.Name, 3, Crabgrass, Wheat, Versatil);
            Row row4 = new Row(field.Name, 4, Quackgrass, Wheat, Simazine);
            Row row5 = new Row(field.Name, 5, Quackgrass, Wheat, Terbuthylazine);
            Row row6 = new Row(field.Name, 6, Quackgrass, Wheat, Versatil);
            Row row7 = new Row(field.Name, 7, MoringGlory, Wheat, Simazine);
            Row row8 = new Row(field.Name, 8, MoringGlory, Wheat, Terbuthylazine);
            Row row9 = new Row(field.Name, 9, MoringGlory, Wheat, Versatil);
            Row row10 = new Row(field.Name, 10, Pigweed, Wheat, Simazine);
            Row row11 = new Row(field.Name, 11, Pigweed, Wheat, Terbuthylazine);
            Row row12 = new Row(field.Name, 12, Pigweed, Wheat, Versatil);

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

            //Row row13 = new Row(13, Quackgrass, Wheat, Versatil);

            //CreateRowInField(row13, field);

            //resultRow = ReadRowInField(row5.Number, field.Name);

            //resultField = ReadCompleteField(field.Name);

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
        public void CreateCompleteField(Field Thisfield)
        {
            List<Row> TempRows = new List<Row>();
            try
            {
                //Empties the list of row to a temp list
                foreach (Row row in Thisfield.rows)
                {
                    TempRows.Add(row);
                }
                Thisfield.rows = null;

                Connect();
                //Creates the field
                client.Cypher
                    .Merge("(field:Field {Name:{Name}, Year:{Year} })")
                    .OnCreate()
                    .Set("field = {Thisfield}")
                    .WithParams(new { Name = Thisfield.Name, Year = Thisfield.Year, Thisfield })
                    .ExecuteWithoutResults();

                //Matches the field with its rows and creates the relations
                foreach (Row Thisrow in TempRows)
                {
                    //laver temp objecter af Crop, Weed og Herbicide og tømmer dem i row'en
                    Crop tempCrop = Thisrow.Crop;
                    Thisrow.Crop = null;

                    Weed tempWeed = Thisrow.Weed;
                    Thisrow.Weed = null;

                    Herbicide tempHerbicide = Thisrow.Herbicide;
                    Thisrow.Herbicide = null;

                    //Opretter row med relation til fielden
                    client.Cypher
                        .Merge("(row:Row {ID:{ID}, Number:{Number} })")
                        .OnCreate()
                        .Set("row = {Thisrow}")
                        .WithParams(new { ID = Thisrow.ID, Number = Thisrow.Number, Thisrow })
                        .ExecuteWithoutResults();

                    client.Cypher
                        .Match("(field:Field)", "(row:Row)")
                        .Where((Field field) => field.Name == Thisfield.Name)
                        .AndWhere((Row row) => row.ID == Thisrow.ID)
                        .CreateUnique("(field)-[:CONTAINS]->(row)")
                        .ExecuteWithoutResults();

                    //Opretter crop med relation til den row vi er på
                    client.Cypher
                        .Merge("(crop:Crop {ID:{ID}, Name:{Name} })")
                        .OnCreate()
                        .Set("crop = {tempCrop}")
                        .WithParams(new { ID = tempCrop.ID, Name = tempCrop.Name, tempCrop })
                        .ExecuteWithoutResults();

                    client.Cypher
                        .Match("(row:Row)", "(crop:Crop)")
                        .Where((Row row) => row.ID == Thisrow.ID && row.Number == Thisrow.Number)
                        .AndWhere((Crop crop) => crop.ID == tempCrop.ID)
                        .CreateUnique("(crop)-[:PLANTED_IN]->(row)")
                        .ExecuteWithoutResults();

                    //Opretter weed med relation til den row vi er på
                    client.Cypher
                        .Merge("(weed:Weed {ID:{ID}, Name:{Name} })")
                        .OnCreate()
                        .Set("weed = {tempWeed}")
                        .WithParams(new { ID = tempWeed.ID, Name = tempWeed.Name, tempWeed })
                        .ExecuteWithoutResults();

                    client.Cypher
                        .Match("(row:Row)", "(weed:Weed)")
                        .Where((Row row) => row.ID == Thisrow.ID && row.Number == Thisrow.Number)
                        .AndWhere((Weed weed) => weed.ID == tempWeed.ID)
                        .CreateUnique("(weed)-[:ATTACKING]->(row)")
                        .ExecuteWithoutResults();

                    //Opretter herbicide til den row vi er på
                    client.Cypher
                        .Merge("(herbicide:Herbicide {ID:{ID}, Dose:{Dose}, Name:{Name} })")
                        .OnCreate()
                        .Set("herbicide = {tempHerbicide}")
                        .WithParams(new { ID = tempHerbicide.ID, Dose = tempHerbicide.Dose, Name = tempHerbicide.Name, tempHerbicide })
                        .ExecuteWithoutResults();

                    client.Cypher
                        .Match("(row:Row)", "(herbicide:Herbicide)")
                        .Where((Row row) => row.ID == Thisrow.ID && row.Number == Thisrow.Number)
                        .AndWhere((Herbicide herbicide) => herbicide.ID == tempHerbicide.ID)
                        .CreateUnique("(herbicide)-[:USED_IN]->(row)")
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
        } //Creates a field with all imbedded objects
        public void CreateRowInField(Row Thisrow, Field Thisfield) //Creates a row and makes the relation to a field
        {
            try
            {
                Connect();
                //laver temp objecter af Crop, Weed og Herbicide og tømmer dem i row'en
                Crop tempCrop = Thisrow.Crop;
                Thisrow.Crop = null;

                Weed tempWeed = Thisrow.Weed;
                Thisrow.Weed = null;

                Herbicide tempHerbicide = Thisrow.Herbicide;
                Thisrow.Herbicide = null;

                //Opretter row med relation til fielden
                client.Cypher
                    .Match("(field:Field)")
                    .Where((Field field) => field.Name == Thisfield.Name)
                    .Create("(field)-[:CONTAINS]->(row:Row {Number})")
                    .WithParam("Number", Thisrow)
                    .ExecuteWithoutResults();
                //Opretter crop med relation til den row vi er på
                client.Cypher
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)-[:PLANTED_IN]->(crop:Crop {Name})")
                    .WithParam("Name", tempCrop)
                    .ExecuteWithoutResults();
                //Opretter weed med relation til den row vi er på
                client.Cypher
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)<-[:PLANTED_IN]-(weed:Weed {Name})")
                    .WithParam("Name", tempWeed)
                    .ExecuteWithoutResults();
                //Opretter herbicide til den row vi er på
                client.Cypher
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)<-[:USED_IN]-(herbicide:Herbicide {Dose})")
                    .WithParam("Dose", tempHerbicide)
                    //.WithParam("Name", tempHerbicide.Name)
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
        public void DeleteRowInField(Row Thisrow, Field Thisfield)
        {

        }
        /// <summary>
        /// ReadCompleteField brænder muligvis hvis den finder 2 marker med samme navn. Skal entet gøres noget ved return typen,
        /// ellers skal foreach looped justeres så der er en work around.
        /// </summary>
        /// <param name="searchField"></param>
        /// <returns></returns>
        public List<Field> ReadCompleteField(string FieldName)
        {
            List<Field> temp = new List<Field>();
            try
            {
                Connect();
                var a = client.Cypher
                    .Match("(field:Field)-[CONTAINS]->(row)<-[PLANTED_IN]-(crop), (row)<-[ATTACKING]-(weed), (row)<-[USED_IN]-(herbicide)")
                    .Where((Field field) => field.Name == FieldName)
                    .Return((field, row, crop, weed, herbicide) =>
                    new { Field = field.As<Field>(), Row = row.As<Row>(), Crop = crop.As<Crop>(), Weed = weed.As<Weed>(), Herbicide = herbicide.As<Herbicide>() })
                    .Results;

                foreach (var item in a)
                {
                    item.Row.Crop = item.Crop;
                    item.Row.Weed = item.Weed;
                    item.Row.Herbicide = item.Herbicide;

                    //item.Field.rows = item.Row.ToList(); //changing the IEnumerable of rows to a list
                    //temp.Add(item.Field); //Changing and adding the IEnumerable of Fields to a list
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
        public Row ReadRowInField(int RowNumber, string FieldName) //Giver mere end 1 row
        {
            try
            {
                Connect();
                var a = client.Cypher
                    .Match("(field:Field)-[CONTAINS]->(row)<-[PLANTED_IN]-(crop), (row)<-[ATTACKING]-(weed), (row)<-[USED_IN]-(herbicide)")
                    .Where((Field field) => field.Name == FieldName)
                    .AndWhere((Row row) => row.Number == RowNumber)
                    .Return((crop, weed, herbicide) =>
                    new { Crop = crop.As<Crop>(), Weed = weed.As<Weed>(), Herbicide = herbicide.As<Herbicide>() })
                    .Results;

                foreach (var item in a)
                {
                    
                }
                return null;
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
