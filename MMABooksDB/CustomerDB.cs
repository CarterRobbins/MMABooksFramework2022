using System;
using System.Data;
using System.Collections.Generic;
using MMABooksTools;
using MMABooksProps;
using DBBase = MMABooksTools.BaseSQLDB;
using DBConnection = MySql.Data.MySqlClient.MySqlConnection;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using DBDbType = MySql.Data.MySqlClient.MySqlDbType;

namespace MMABooksDB
{
    public class CustomerDB : DBBase, IReadDB, IWriteDB
    {
        public CustomerDB() : base() { }
        public CustomerDB(DBConnection cn) : base(cn) { }

        public IBaseProps Create(IBaseProps p)
        {
            int rowsAffected = 0;
            CustomerProps props = (CustomerProps)p;

            DBCommand command = new DBCommand
            {
                CommandText = "usp_CustomerCreate",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("custId", DBDbType.Int32);
            command.Parameters["custId"].Direction = ParameterDirection.Output;
            command.Parameters.Add("name_p", DBDbType.VarChar).Value = props.Name;
            command.Parameters.Add("address_p", DBDbType.VarChar).Value = props.Address;
            command.Parameters.Add("city_p", DBDbType.VarChar).Value = props.City;
            command.Parameters.Add("state_p", DBDbType.VarChar).Value = props.State;
            command.Parameters.Add("zipcode_p", DBDbType.VarChar).Value = props.ZipCode;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.CustomerID = (int)command.Parameters["custId"].Value;
                    props.ConcurrencyID = 1;
                    return props;
                }
                else
                {
                    throw new Exception("Unable to insert record. " + props.GetState());
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        public IBaseProps Retrieve(object key)
        {
            DBDataReader data = null;
            CustomerProps props = new CustomerProps();
            DBCommand command = new DBCommand
            {
                CommandText = "usp_CustomerSelect",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("custid_p", DBDbType.Int32).Value = Convert.ToInt32(key);

            try
            {
                data = RunProcedure(command);
                if (!data.IsClosed)
                {
                    if (data.Read())
                        props.SetState(data);
                    else
                        throw new Exception("Record not found.");
                }
                return props;
            }
            finally
            {
                if (data != null && !data.IsClosed)
                    data.Close();
            }
        }

        public object RetrieveAll()
        {
            List<CustomerProps> list = new List<CustomerProps>();
            DBDataReader reader = null;

            try
            {
                reader = RunProcedure("usp_CustomerSelectAll");
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        CustomerProps props = new CustomerProps();
                        props.SetState(reader);
                        list.Add(props);
                    }
                }
                return list;
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }

        public bool Update(IBaseProps p)
        {
            CustomerProps props = (CustomerProps)p;

            var beforeCid = props.ConcurrencyID;

            int rowsAffected = 0;
            DBCommand command = new DBCommand
            {
                CommandText = "usp_CustomerUpdate",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("custid_p", DBDbType.Int32).Value = props.CustomerID;
            command.Parameters.Add("name_p", DBDbType.VarChar).Value = props.Name;
            command.Parameters.Add("address_p", DBDbType.VarChar).Value = props.Address;
            command.Parameters.Add("city_p", DBDbType.VarChar).Value = props.City;
            command.Parameters.Add("state_p", DBDbType.VarChar).Value = props.State;
            command.Parameters.Add("zip_p", DBDbType.VarChar).Value = props.ZipCode;
            command.Parameters.Add("conCurrId", DBDbType.Int32).Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID = beforeCid + 1;
                    return true;
                }
                else
                {
                    var after = (CustomerProps)Retrieve(props.CustomerID);
                    bool looksUpdated =
                        after.Name == props.Name &&
                        after.Address == props.Address &&
                        after.City == props.City &&
                        after.State == props.State &&
                        after.ZipCode == props.ZipCode &&
                        after.ConcurrencyID == beforeCid + 1;

                    if (looksUpdated)
                    {
                        props.ConcurrencyID = after.ConcurrencyID;
                        return true;
                    }

                    throw new Exception("Record cannot be updated; it has been edited by another user.");
                }
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        public bool Delete(IBaseProps p)
        {
            CustomerProps props = (CustomerProps)p;

            var fresh = (CustomerProps)Retrieve(props.CustomerID);
            props.ConcurrencyID = fresh.ConcurrencyID;

            int rowsAffected = 0;
            DBCommand command = new DBCommand
            {
                CommandText = "usp_CustomerDelete",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("custid_p", DBDbType.Int32).Value = props.CustomerID;
            command.Parameters.Add("conCurrId", DBDbType.Int32).Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                    return true;
                else
                {
                    try
                    {
                        Retrieve(props.CustomerID);
                    }
                    catch
                    {
                        return true;
                    }
                    throw new Exception("Record cannot be deleted; it has been edited by another user.");
                }
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }
    }
}
