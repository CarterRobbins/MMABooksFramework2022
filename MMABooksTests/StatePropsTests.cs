using NUnit.Framework;

using MMABooksProps;
using System;

namespace MMABooksTests
{
    [TestFixture]
    public class StatePropsTests
    {
        StateProps props;
        [SetUp]
        public void Setup()
        {
            props = new StateProps();
            props.Code = "11";
            props.Name = "This is a test";
        }

        [Test]
        public void TestGetState()
        {
            string jsonString = props.GetState();
            Console.WriteLine(jsonString);
            Assert.IsTrue(jsonString.Contains(props.Code));
            Assert.IsTrue(jsonString.Contains(props.Name));
        }

        [Test]
        public void TestSetState()
        {
            string jsonString = props.GetState();
            StateProps newProps = new StateProps();
            newProps.SetState(jsonString);
            Assert.AreEqual(props.Code, newProps.Code);
            Assert.AreEqual(props.Name, newProps.Name);
            Assert.AreEqual(props.ConcurrencyID, newProps.ConcurrencyID);
        }

        [Test]
        public void TestClone()
        {
            StateProps newProps = (StateProps)props.Clone();
            Assert.AreEqual(props.Code, newProps.Code);
            Assert.AreEqual(props.Name, newProps.Name);
            Assert.AreEqual(props.ConcurrencyID, newProps.ConcurrencyID);
        }
    }
}