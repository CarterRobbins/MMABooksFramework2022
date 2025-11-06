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
    public class ProductDBTests
    {
        ProductDB db;

        [SetUp]
        public void ResetData()
        {
            db = new ProductDB();
            var command = new DBCommand();
            command.CommandText = "usp_testingResetProductData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestRetrieve()
        {
            ProductProps p = (ProductProps)db.Retrieve("A4CS");
            Assert.AreEqual("A4CS", p.ProductCode);
            Assert.AreEqual("C# 4.0 Intro", p.Description);
            Assert.AreEqual(59.95m, p.UnitPrice);
            Assert.AreEqual(12, p.OnHandQuantity);
        }

        [Test]
        public void TestRetrieveAll()
        {
            List<ProductProps> list = (List<ProductProps>)db.RetrieveAll();
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void TestDelete()
        {
            ProductProps p = (ProductProps)db.Retrieve("PY310");
            Assert.True(db.Delete(p));
            Assert.Throws<Exception>(() => db.Retrieve("PY310"));
        }

        [Test]
        public void TestDeleteForeignKeyConstraint()
        {
            Assert.Inconclusive("Set a known referenced ProductCode to fully exercise FK delete.");

        }

        [Test]
        public void TestUpdate()
        {
            ProductProps p = (ProductProps)db.Retrieve("JAVA1");
            p.Description = "Intro Java (Updated)";
            p.UnitPrice = 49.95m;
            p.OnHandQuantity = 21;
            Assert.True(db.Update(p));

            var after = (ProductProps)db.Retrieve("JAVA1");
            Assert.AreEqual("Intro Java (Updated)", after.Description);
            Assert.AreEqual(49.95m, after.UnitPrice);
            Assert.AreEqual(21, after.OnHandQuantity);
        }

        [Test]
        public void TestUpdateFieldTooLong()
        {
            ProductProps p = (ProductProps)db.Retrieve("A4CS");
            p.Description = new string('X', 60);
            Assert.Throws<MySqlException>(() => db.Update(p));
        }

        [Test]
        public void TestCreate()
        {
            var p = new ProductProps
            {
                ProductCode = "TS01",
                Description = "Test Sample",
                UnitPrice = 9.99m,
                OnHandQuantity = 5
            };

            db.Create(p);
            var p2 = (ProductProps)db.Retrieve(p.ProductCode);
            Assert.AreEqual(p.GetState(), p2.GetState());
        }

        [Test]
        public void TestCreatePrimaryKeyViolation()
        {
            var p = new ProductProps
            {
                ProductCode = "A4CS",
                Description = "Dup Key",
                UnitPrice = 1.00m,
                OnHandQuantity = 1
            };
            Assert.Throws<MySqlException>(() => db.Create(p));
        }
    }
}

