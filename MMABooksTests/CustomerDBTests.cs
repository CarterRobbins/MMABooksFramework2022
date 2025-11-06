using NUnit.Framework;
using MMABooksProps;
using MMABooksDB;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;
using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    public class CustomerDBTests
    {
        CustomerDB db;

        [SetUp]
        public void ResetData()
        {
            db = new CustomerDB();
            var command = new DBCommand
            {
                CommandText = "usp_testingResetData",
                CommandType = CommandType.StoredProcedure
            };
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestCreateAndRetrieve()
        {
            var p = new CustomerProps
            {
                Name = "Test User",
                Address = "1 Main St",
                City = "Eugene",
                State = "OR",
                ZipCode = "97401"
            };

            var created = (CustomerProps)db.Create(p);
            Assert.Greater(created.CustomerID, 0);

            var fetched = (CustomerProps)db.Retrieve(created.CustomerID);
            Assert.AreEqual(created.GetState(), fetched.GetState());
        }

        [Test]
        public void TestRetrieveAll_CountIncreasesAfterCreate()
        {
            var before = (List<CustomerProps>)db.RetrieveAll();

            var p = new CustomerProps
            {
                Name = "Temp User",
                Address = "2 Main St",
                City = "Eugene",
                State = "OR",
                ZipCode = "97401"
            };
            db.Create(p);

            var after = (List<CustomerProps>)db.RetrieveAll();
            Assert.AreEqual(before.Count + 1, after.Count);
        }

        [Test]
        public void TestUpdate()
        {
            var p = new CustomerProps { Name = "Update Me", Address = "3 Main St", City = "Eugene", State = "OR", ZipCode = "97401" };
            var created = (CustomerProps)db.Create(p);

 
            created = (CustomerProps)db.Retrieve(created.CustomerID);

            created.Name = "Updated Name";
            created.ZipCode = "97402";
            Assert.True(db.Update(created));
        }

        [Test]
        public void TestDelete()
        {
            var p = new CustomerProps { Name = "Delete Me", Address = "5 Main St", City = "Eugene", State = "OR", ZipCode = "97401" };
            var created = (CustomerProps)db.Create(p);


            created = (CustomerProps)db.Retrieve(created.CustomerID);

            Assert.True(db.Delete(created));
            Assert.Throws<Exception>(() => db.Retrieve(created.CustomerID));
        }

        [Test]
        public void TestDeleteForeignKeyConstraint()
        {
            
            Assert.Inconclusive("Set a known referenced CustomerID to fully exercise FK delete.");
        }
    }
}
