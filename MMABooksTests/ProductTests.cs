using NUnit.Framework;

using MMABooksBusiness;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            var db = new ProductDB();
            var command = new DBCommand
            {
                CommandText = "usp_testingResetProductData",
                CommandType = CommandType.StoredProcedure
            };
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewProductConstructor()
        {
            var p = new Product();
            Assert.AreEqual(string.Empty, p.ProductCode);
            Assert.AreEqual(string.Empty, p.Description);
            Assert.AreEqual(0m, p.UnitPrice);
            Assert.AreEqual(0, p.OnHandQuantity);
            Assert.IsTrue(p.IsNew);
            Assert.IsFalse(p.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreConstructor()
        {
            // From reset proc seed: ('A4CS','C# 4.0 Intro',59.95,12)
            var p = new Product("A4CS");
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.AreEqual("C# 4.0 Intro", p.Description);
            Assert.AreEqual(59.95m, p.UnitPrice);
            Assert.AreEqual(12, p.OnHandQuantity);
            Assert.IsFalse(p.IsNew);
            Assert.IsTrue(p.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            var p = new Product();
            p.ProductCode = "TS01";
            p.Description = "Test Sample";
            p.UnitPrice = 9.99m;
            p.OnHandQuantity = 5;
            p.Save();

            var p2 = new Product("TS01");

            Assert.AreEqual(p.ProductCode, p2.ProductCode);
            Assert.AreEqual(p.Description, p2.Description);
            Assert.AreEqual(p.UnitPrice, p2.UnitPrice);
            Assert.AreEqual(p.OnHandQuantity, p2.OnHandQuantity);
        }

        [Test]
        public void TestUpdate()
        {
            var p = new Product("JAVA1");
            p.Description = "Intro Java (Updated)";
            p.UnitPrice = 49.95m;
            p.OnHandQuantity = 21;

            Assert.DoesNotThrow(() => p.Save());

            var after = new Product("JAVA1");
            Assert.AreEqual("Intro Java (Updated)", after.Description);
            Assert.AreEqual(49.95m, after.UnitPrice);
            Assert.AreEqual(21, after.OnHandQuantity);
        }

        [Test]
        public void TestDelete()
        {
            var p = new Product("PY310");
            p.Delete();
            p.Save();

            Assert.Throws<Exception>(() => new Product("PY310"));
        }

        [Test]
        public void TestGetList()
        {
            var p = new Product();
            var list = (List<Product>)p.GetList();
            Assert.GreaterOrEqual(list.Count, 3);
            Assert.AreEqual("A4CS", list[0].ProductCode);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            var p = new Product();
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            var p = new Product();
            p.ProductCode = "X1";
            // Missing Description/UnitPrice/OnHandQuantity -> not valid
            Assert.Throws<Exception>(() => p.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            var p = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Description = new string('x', 60));
            Assert.Throws<ArgumentOutOfRangeException>(() => p.UnitPrice = -1m);
            Assert.Throws<ArgumentOutOfRangeException>(() => p.OnHandQuantity = -5);
        }


        [Test]
        public void TestConcurrencyIssue()
        {
            var p1 = new Product("JAVA1");
            var p2 = new Product("JAVA1");

            p1.Description = "Updated first";
            p1.Save();

            p2.Description = "Updated second";
            Assert.Throws<Exception>(() => p2.Save());
        }
    }
}
