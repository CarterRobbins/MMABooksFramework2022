using NUnit.Framework;

using MMABooksBusiness;
using MMABooksProps;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System.Collections.Generic;
using System;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            CustomerDB db = new CustomerDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetCustomer1Data";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewCustomerConstructor()
        {
            Customer c = new Customer();
            Assert.AreEqual(string.Empty, c.Name);
            Assert.AreEqual(string.Empty, c.Address);
            Assert.AreEqual(string.Empty, c.City);
            Assert.AreEqual(string.Empty, c.State);
            Assert.AreEqual(string.Empty, c.ZipCode);
            Assert.IsTrue(c.IsNew);
            Assert.IsFalse(c.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreConstructor()
        {
            Customer c = new Customer(1);
            Assert.Greater(c.CustomerID, 0);
            Assert.IsTrue(c.Name.Length > 0);
            Assert.IsFalse(c.IsNew);
            Assert.IsTrue(c.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Customer c = new Customer();
            c.Name = "Where Am I";
            c.Address = "1 Main St";
            c.City = "Eugene";
            c.State = "OR";
            c.ZipCode = "97401";
            c.Save();

            Customer c2 = new Customer(c.CustomerID);
            Assert.AreEqual(c2.CustomerID, c.CustomerID);
            Assert.AreEqual(c2.Name, c.Name);
            Assert.AreEqual(c2.Address, c.Address);
            Assert.AreEqual(c2.City, c.City);
            Assert.AreEqual(c2.State, c.State);
            Assert.AreEqual(c2.ZipCode, c.ZipCode);
        }

        [Test]
        public void TestUpdate()
        {
            Customer c = new Customer(1);
            string newName = c.Name + " Updated";
            c.Name = newName;
            c.Save();

            Customer c2 = new Customer(1);
            Assert.AreEqual(newName, c2.Name);
        }

        [Test]
        public void TestDelete()
        {
            Customer c = new Customer();
            c.Name = "Delete Me";
            c.Address = "5 Main St";
            c.City = "Eugene";
            c.State = "OR";
            c.ZipCode = "97401";
            c.Save();

            int id = c.CustomerID;

            c.Delete();
            c.Save();

            Assert.Throws<Exception>(() => new Customer(id));
        }

        [Test]
        public void TestGetList()
        {
            Customer c = new Customer();
            List<Customer> customers = (List<Customer>)c.GetList();
            Assert.GreaterOrEqual(customers.Count, 1);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            Customer c = new Customer();
            Assert.Throws<Exception>(() => c.Save());
            c.Name = "Partial";
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Customer c = new Customer();
            Assert.Throws<ArgumentOutOfRangeException>(() => c.State = "XXX");
            Assert.Throws<ArgumentOutOfRangeException>(() => c.ZipCode = "12");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Customer c1 = new Customer(1);
            Customer c2 = new Customer(1);

            c1.Name = c1.Name + " A";
            c1.Save();

            c2.Name = c2.Name + " B";
            Assert.Throws<Exception>(() => c2.Save());
        }
    }
}

