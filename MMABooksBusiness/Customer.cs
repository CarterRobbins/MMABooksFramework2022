using System;
using System.Collections.Generic;
using MMABooksTools;
using MMABooksProps;
using MMABooksDB;

namespace MMABooksBusiness
{
    public class Customer : BaseBusiness
    {
        public int CustomerID
        {
            get { return ((CustomerProps)mProps).CustomerID; }
        }

        public string Name
        {
            get { return ((CustomerProps)mProps).Name; }
            set
            {
                if (value != ((CustomerProps)mProps).Name)
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 40)
                    {
                        mRules.RuleBroken("Name", false);
                        ((CustomerProps)mProps).Name = value;
                        mIsDirty = true;
                    }
                    else
                        throw new ArgumentOutOfRangeException("Name must be between 1 and 40 characters.");
                }
            }
        }

        public string Address
        {
            get { return ((CustomerProps)mProps).Address; }
            set
            {
                if (value != ((CustomerProps)mProps).Address)
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 50)
                    {
                        mRules.RuleBroken("Address", false);
                        ((CustomerProps)mProps).Address = value;
                        mIsDirty = true;
                    }
                    else
                        throw new ArgumentOutOfRangeException("Address must be between 1 and 50 characters.");
                }
            }
        }

        public string City
        {
            get { return ((CustomerProps)mProps).City; }
            set
            {
                if (value != ((CustomerProps)mProps).City)
                {
                    if (value.Trim().Length >= 1 && value.Trim().Length <= 50)
                    {
                        mRules.RuleBroken("City", false);
                        ((CustomerProps)mProps).City = value;
                        mIsDirty = true;
                    }
                    else
                        throw new ArgumentOutOfRangeException("City must be between 1 and 50 characters.");
                }
            }
        }

        public string State
        {
            get { return ((CustomerProps)mProps).State; }
            set
            {
                if (value != ((CustomerProps)mProps).State)
                {
                    if (value.Trim().Length == 2)
                    {
                        mRules.RuleBroken("State", false);
                        ((CustomerProps)mProps).State = value;
                        mIsDirty = true;
                    }
                    else
                        throw new ArgumentOutOfRangeException("State must be a valid 2-letter code.");
                }
            }
        }

        public string ZipCode
        {
            get { return ((CustomerProps)mProps).ZipCode; }
            set
            {
                if (value != ((CustomerProps)mProps).ZipCode)
                {
                    if (value.Trim().Length >= 5 && value.Trim().Length <= 10)
                    {
                        mRules.RuleBroken("ZipCode", false);
                        ((CustomerProps)mProps).ZipCode = value;
                        mIsDirty = true;
                    }
                    else
                        throw new ArgumentOutOfRangeException("ZipCode must be 5 to 10 characters long.");
                }
            }
        }

        public override object GetList()
        {
            List<Customer> customers = new List<Customer>();
            List<CustomerProps> propsList = (List<CustomerProps>)mdbReadable.RetrieveAll();

            foreach (CustomerProps props in propsList)
            {
                Customer c = new Customer(props);
                customers.Add(c);
            }

            return customers;
        }

        protected override void SetDefaultProperties()
        {
            ((CustomerProps)mProps).Name = "";
            ((CustomerProps)mProps).Address = "";
            ((CustomerProps)mProps).City = "";
            ((CustomerProps)mProps).State = "";
            ((CustomerProps)mProps).ZipCode = "";
        }

        protected override void SetRequiredRules()
        {
            mRules.RuleBroken("Name", true);
            mRules.RuleBroken("Address", true);
            mRules.RuleBroken("City", true);
            mRules.RuleBroken("State", true);
            mRules.RuleBroken("ZipCode", true);
        }

        protected override void SetUp()
        {
            mProps = new CustomerProps();
            mOldProps = new CustomerProps();

            mdbReadable = new CustomerDB();
            mdbWriteable = new CustomerDB();
        }

        public Customer() : base() { }

        public Customer(int key) : base(key) { }

        private Customer(CustomerProps props) : base(props) { }
    }
}
