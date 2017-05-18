using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4jClient;
using System.Collections;
using System.Diagnostics;

namespace Jordbrugsteknologi
{
    class Program
    {
        GraphClient client;
        List<string> fieldNames;
        Field resultField;
        Row resultRow;
        Stopwatch timer;


        static void Main(string[] args)
        {
            Program myProgram = new Program();
            myProgram.Run();

        }
        public void Run()
        {
            timer = new Stopwatch();
            
                //Make the field
                Field field = new Field("Marken1", 2017);

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

            timer.Start();
            //CreateCompleteField(field);

            //Row row13 = new Row(field.Name, 13, Quackgrass, Wheat, Versatil);
            //CreateRowInField(row13, field);

            //resultRow = ReadRowInField(row5.Number, field.Name);

            //resultField = ReadCompleteField(field.Name);

            fieldNames = GetAllFieldNames();

            //Console.ReadKey();
            
            timer.Stop();
            Console.WriteLine(timer.Elapsed);
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
                        .Match("(row:Row)")
                        .Where((Row row) => row.Number == Thisrow.Number && row.ID == Thisrow.ID)
                        .Create("(row)<-[:PLANTED_IN]-(crop:Crop {tempCrop})")
                        .WithParam("tempCrop", tempCrop)
                        .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Merge("(crop:Crop {ID:{ID}, Name:{Name} })")
                    //    .OnCreate()
                    //    .Set("crop = {tempCrop}")
                    //    .WithParams(new { ID = tempCrop.ID, Name = tempCrop.Name, tempCrop })
                    //    .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Match("(row:Row)", "(crop:Crop)")
                    //    .Where((Row row) => row.Number == Thisrow.Number)
                    //    .AndWhere((Crop crop) => crop.ID == tempCrop.ID)
                    //    .CreateUnique("(crop)-[:PLANTED_IN]->(row)")
                    //    .ExecuteWithoutResults();

                    //Opretter weed med relation til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where((Row row) => row.Number == Thisrow.Number && row.ID == Thisrow.ID)
                        .Create("(row)<-[:ATTACKING]-(weed:Weed {tempWeed})")
                        .WithParam("tempWeed", tempWeed)
                        .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Merge("(weed:Weed {ID:{ID}, Name:{Name} })")
                    //    .OnCreate()
                    //    .Set("weed = {tempWeed}")
                    //    .WithParams(new { ID = tempWeed.ID, Name = tempWeed.Name, tempWeed })
                    //    .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Match("(row:Row)", "(weed:Weed)")
                    //    .Where((Row row) => row.Number == Thisrow.Number)
                    //    .AndWhere((Weed weed) => weed.ID == tempWeed.ID)
                    //    .CreateUnique("(weed)-[:ATTACKING]->(row)")
                    //    .ExecuteWithoutResults();

                    //Opretter herbicide til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where((Row row) => row.Number == Thisrow.Number && row.ID == Thisrow.ID)
                        .Create("(row)<-[:USED_IN]-(herbicide:Herbicide {tempHerbicide})")
                        .WithParam("tempHerbicide", tempHerbicide)
                        .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Merge("(herbicide:Herbicide {ID:{ID}, Dose:{Dose}, Name:{Name} })")
                    //    .OnCreate()
                    //    .Set("herbicide = {tempHerbicide}")
                    //    .WithParams(new { ID = tempHerbicide.ID, Dose = tempHerbicide.Dose, Name = tempHerbicide.Name, tempHerbicide })
                    //    .ExecuteWithoutResults();

                    //client.Cypher
                    //    .Match("(row:Row)", "(herbicide:Herbicide)")
                    //    .Where((Row row) => row.Number == Thisrow.Number)
                    //    .AndWhere((Herbicide herbicide) => herbicide.ID == tempHerbicide.ID)
                    //    .CreateUnique("(herbicide)-[:USED_IN]->(row)")
                    //    .ExecuteWithoutResults();
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
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)-[:PLANTED_IN]->(crop:Crop {tempCrop})")
                    .WithParam("tempCrop", tempCrop)
                    .ExecuteWithoutResults();
                //Opretter weed med relation til den row vi er på
                client.Cypher
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)<-[:ATTACKING]-(weed:Weed {tempWeed})")
                    .WithParam("tempWeed", tempWeed)
                    .ExecuteWithoutResults();
                //Opretter herbicide til den row vi er på
                client.Cypher
                    .Match("(row:Row)")
                    .Where((Row row) => row.Number == Thisrow.Number)
                    .Create("(row)<-[:USED_IN]-(herbicide:Herbicide {tempHerbicide})")
                    .WithParam("tempHerbicide", tempHerbicide)
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

        } //Not yet implemented
        public Field ReadCompleteField(string FieldName)
        {
            Field tempField = new Field();
            tempField.Name = FieldName;
            try
            {
                Connect();
                long count = 0;
                var number = client.Cypher
                    .OptionalMatch("(field:Field)-[CONTAINS]-(row:Row)")
                    .Where((Field field) => field.Name == FieldName)
                    .Return((row) => new { NumberOfRows = row.Count() })
                    .Results;
                foreach (var item in number)
                {
                    count = item.NumberOfRows;
                }

                for (int i = 1; i <= count; i++)
                {
                    tempField.rows.Add(ReadRowInField(i, FieldName));
                }
                
                return tempField;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Disconnect();
            }
        } //Finds a field, counts its rows and read all the relations on each row
        public Row ReadRowInField(int RowNumber, string FieldName) //Finds a single Row in a Field
        {
            Row tempRow = new Row();
            tempRow.Number = RowNumber;
            tempRow.ID = FieldName.GetHashCode();

            try
            {
                Connect();
                var a = client.Cypher
                    .Match("(field:Field)-[CONTAINS]->(row)<-[PLANTED_IN]-(crop)")
                    .Where((Field field) => field.Name == FieldName)
                    .AndWhere((Row row) => row.Number == RowNumber)
                    .Return((crop) =>
                    new { Herbicide = crop.As<Herbicide>(), Weed = crop.As<Weed>(), Crop = crop.As<Crop>() })
                    .Results;

                int i = 0;
                foreach (var item in a) //for(foreach) loop, der nok skal fixes.
                {
                    if (i == 0)
                    {
                        tempRow.Herbicide = item.Herbicide;
                    }
                    if (i == 1)
                    {
                        tempRow.Weed = item.Weed;
                    }
                    if (i == 2)
                    {
                        tempRow.Crop = item.Crop;
                    }
                    i++;
                }

                return tempRow;
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
        public List<string> GetAllFieldNames()
        {
            List<string> temp = new List<string>();
            try
            {
                Connect();
                var a = client.Cypher
                    .Match("(field:Field)")
                    .Return(field => field.As<Field>())
                    .Results;

                foreach (var item in a)
                {
                    temp.Add(item.Name);
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
