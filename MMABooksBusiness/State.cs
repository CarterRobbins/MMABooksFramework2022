using System;

using MMABooksTools;
using MMABooksProps;
using MMABooksDB;

using System.Collections.Generic;

namespace MMABooksBusiness
{
    public class State : BaseBusiness
    {
        /// <summary>
        /// Read/Write property. 
        /// </summary>
        //  Notice that I used a name for the business object property that I thought would be more intuitive for the 
        //  application programmer.  It doesn't have to match the database.
        public string Abbreviation
        {
            get
            {
                return ((StateProps)mProps).Code;
            }

            set
            {
                if (!(value == ((StateProps)mProps).Code))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 2)
                    {
                        mRules.RuleBroken("Abbreviation", false);
                        ((StateProps)mProps).Code = value;
                        mIsDirty = true;
                    }

                    else
                    {
                        throw new ArgumentOutOfRangeException("Abbreviation must be no more than 2 characters long.");
                    }
                }
            }
        }

        public String Name
        {
            get
            {
                return ((StateProps)mProps).Name;
            }

            set
            {
                if (!(value == ((StateProps)mProps).Name))
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 20)
                    {
                        mRules.RuleBroken("Name", false);
                        ((StateProps)mProps).Name = value;
                        mIsDirty = true;
                    }

                    else
                    {
                        throw new ArgumentOutOfRangeException("Name must be no more than 20 characters long.");
                    }
                }
            }
        }

        public override object GetList()
        {
            List<State> states = new List<State>();
            List<StateProps> props = new List<StateProps>();


            props = (List<StateProps>)mdbReadable.RetrieveAll();
            foreach (StateProps prop in props)
            {
                State s = new State(prop);
                states.Add(s);
            }

            return states;
        }

        protected override void SetDefaultProperties()
        {
        }

        /// <summary>
        /// Sets required fields for a record.
        /// </summary>
        protected override void SetRequiredRules()
        {
            mRules.RuleBroken("Abbreviation", true);
            mRules.RuleBroken("Name", true);
        }

        /// <summary>
        /// Instantiates mProps and mOldProps as new Props objects.
        /// Instantiates mbdReadable and mdbWriteable as new DB objects.
        /// </summary>
        protected override void SetUp()
        {
            mProps = new StateProps();
            mOldProps = new StateProps();

            mdbReadable = new StateDB();
            mdbWriteable = new StateDB();
        }

        #region constructors
        /// <summary>
        /// Default constructor - gets the connection string - assumes a new record that is not in the database.
        /// </summary>
        public State() : base()
        {
        }

        /// <summary>
        /// Calls methods SetUp() and Load().
        /// Use this constructor when the object is in the database AND the connection string is in a config file
        /// </summary>
        /// <param name="key">ID number of a record in the database.
        /// Sent as an arg to Load() to set values of record to properties of an 
        /// object.</param>
        public State(string key)
            : base(key)
        {
        }

        private State(StateProps props)
            : base(props)
        {
        }

        #endregion
    }
}
