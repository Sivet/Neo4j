﻿using System;
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
        List<Field> resultField;
        Row resultRow;

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
            Row row1 = new Row(1, Crabgrass, Wheat, Simazine);
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

            Row row13 = new Row(13, Quackgrass, Wheat, Versatil);

            CreateRowInField(row13, field);

            //resultRow = ReadRowInField(row5.Number, field.Name);

            //resultField = ReadCompleteField(field.Name);

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
                        .Create("(field:Field {Name})")
                        .WithParam("Name", Thisfield)
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
                        .Match("(field:Field)")
                        .Where((Field field) => field.Name == Thisfield.Name)
                        .Create("(field)-[:CONTAINS]->(row:Row {Number})")
                        .WithParam("Number", Thisrow)
                        .ExecuteWithoutResults();
                    //Opretter crop med relation til den row vi er på
                    client.Cypher
                        .Match("(row:Row)")
                        .Where((Row row) => row.Number == Thisrow.Number)
                        .Create("(row)<-[:PLANTED_IN]-(crop:Crop {Name})")
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
        /// ReadCompleteField giver en null exception hvis det field opbject man finder ikke har nogle rows
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
                    .OptionalMatch("(field:Field)-[CONTAINS]-(row:Row)") //tilføj at finde alle rows "venner" der ikke er field
                    .Where((Field field) => field.Name == FieldName)
                    .Return((field, row) =>
                    new { Field = field.As<Field>(), Row = row.CollectAs<Row>() }) //Find ud af hvilken form det skal returnes i
                    .Results;

                foreach (var item in a)
                {
                    item.Field.rows = item.Row.ToList(); //changing the IEnumerable of rows to a list
                    temp.Add(item.Field); //Changing and adding the IEnumerable of Fields to a list
                    //Tilføj overførelse af crop, weed, herbicide til den korrekte Row
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
        public Row ReadRowInField(int RowNumber, string FieldName) //Mangler udflaskning af crop, weed, herbicide
        {
            try
            {
                Connect();
                var a = client.Cypher
                    .OptionalMatch("(field:Field)-[CONTAINS]-(row:Row)")
                    .Where((Field field) => field.Name == FieldName)
                    .Return((row) =>
                    new { Row = row.As<Row>() })
                    .Results;
                foreach (var item in a)
                {
                    if (item.Row.Number == RowNumber)
                    {
                        return item.Row;
                    }
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
