using NUnit.Framework;

using MMABooksBusiness;
using MMABooksProps;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    [TestFixture]
    public class StateTests
    {

        [SetUp]
        public void TestResetDatabase()
        {
            StateDB db = new StateDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetStateData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewStateConstructor()
        {
            // not in Data Store - no code
            State s = new State();
            Assert.AreEqual(string.Empty, s.Abbreviation);
            Assert.AreEqual(string.Empty, s.Name);
            Assert.IsTrue(s.IsNew);
            Assert.IsFalse(s.IsValid);
        }


        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            // retrieves from Data Store
            State s = new State("OR");
            Assert.AreEqual("OR", s.Abbreviation);
            Assert.IsTrue(s.Name.Length > 0);
            Assert.IsFalse(s.IsNew);
            Assert.IsTrue(s.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            State s = new State();
            s.Abbreviation = "??";
            s.Name = "Where am I";
            s.Save();
            State s2 = new State("??");
            Assert.AreEqual(s2.Abbreviation, s.Abbreviation);
            Assert.AreEqual(s2.Name, s.Name);
        }

        [Test]
        public void TestUpdate()
        {
            State s = new State("OR");
            s.Name = "Edited Name";
            s.Save();

            State s2 = new State("OR");
            Assert.AreEqual(s2.Abbreviation, s.Abbreviation);
            Assert.AreEqual(s2.Name, s.Name);
        }

        [Test]
        public void TestDelete()
        {
            State s = new State("HI");
            s.Delete();
            s.Save();
            Assert.Throws<Exception>(() => new State("HI"));
        }

        [Test]
        public void TestGetList()
        {
            State s = new State();
            List<State> states = (List<State>)s.GetList();
            Assert.AreEqual(53, states.Count);
            Assert.AreEqual("Alabama", states[0].Name);
            Assert.AreEqual("AL", states[0].Abbreviation);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            // not in Data Store - abbreviation and name must be provided
            State s = new State();
            Assert.Throws<Exception>(() => s.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            // not in Data Store - abbreviation and name must be provided
            State s = new State();
            Assert.Throws<Exception>(() => s.Save());
            s.Abbreviation = "??";
            Assert.Throws<Exception>(() => s.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            State s = new State();
            Assert.Throws<ArgumentOutOfRangeException>(() => s.Abbreviation = "???");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            State s1 = new State("OR");
            State s2 = new State("OR");

            s1.Name = "Updated first";
            s1.Save();

            s2.Name = "Updated second";
            Assert.Throws<Exception>(() => s2.Save());
        }
    }
}