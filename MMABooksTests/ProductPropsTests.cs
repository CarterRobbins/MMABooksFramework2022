using NUnit.Framework;
using MMABooksProps;
using System;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductPropsTests
    {
        ProductProps props;

        [SetUp]
        public void Setup()
        {
            props = new ProductProps
            {
                ProductCode = "A4CS",
                Description = "C# 4.0 Intro",
                UnitPrice = 59.95m,
                OnHandQuantity = 12,
                ConcurrencyID = 1
            };
        }

        [Test]
        public void TestGetState()
        {
            string json = props.GetState();
            Console.WriteLine(json);
            Assert.IsTrue(json.Contains(props.ProductCode));
            Assert.IsTrue(json.Contains(props.Description));
            Assert.IsTrue(json.Contains(props.UnitPrice.ToString()));
            Assert.IsTrue(json.Contains(props.OnHandQuantity.ToString()));
            Assert.IsTrue(json.Contains(props.ConcurrencyID.ToString()));
        }

        [Test]
        public void TestSetState()
        {
            string json = props.GetState();
            ProductProps other = new ProductProps();
            other.SetState(json);

            Assert.AreEqual(props.ProductCode, other.ProductCode);
            Assert.AreEqual(props.Description, other.Description);
            Assert.AreEqual(props.UnitPrice, other.UnitPrice);
            Assert.AreEqual(props.OnHandQuantity, other.OnHandQuantity);
            Assert.AreEqual(props.ConcurrencyID, other.ConcurrencyID);
        }

        [Test]
        public void TestClone()
        {
            ProductProps copy = (ProductProps)props.Clone();

            Assert.AreEqual(props.ProductCode, copy.ProductCode);
            Assert.AreEqual(props.Description, copy.Description);
            Assert.AreEqual(props.UnitPrice, copy.UnitPrice);
            Assert.AreEqual(props.OnHandQuantity, copy.OnHandQuantity);
            Assert.AreEqual(props.ConcurrencyID, copy.ConcurrencyID);
            Assert.AreNotSame(props, copy);
        }
    }
}

