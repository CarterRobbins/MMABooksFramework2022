using System;

using MMABooksTools;
using MMABooksProps;

using System.Data;

// *** I use an "alias" for the ado.net classes throughout my code
// When I switch to an oracle database, I ONLY have to change the actual classes here
using DBBase = MMABooksTools.BaseSQLDB;
using DBConnection = MySql.Data.MySqlClient.MySqlConnection;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using DBParameter = MySql.Data.MySqlClient.MySqlParameter;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using DBDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using DBDbType = MySql.Data.MySqlClient.MySqlDbType;
using System.Collections.Generic;

namespace MMABooksDB
{
    public class StateDB : DBBase, IReadDB, IWriteDB
    {

        public StateDB() : base() { }
        public StateDB(DBConnection cn) : base(cn) { }

        public IBaseProps Create(IBaseProps p)
        {
            int rowsAffected = 0;
            StateProps props = (StateProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_StateCreate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("code", props.Code);
            command.Parameters.AddWithValue("name", props.Name);

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID = 1;
                    return props;
                }
                else
                    throw new Exception("Unable to insert record. " + props.GetState());
            }
            catch (Exception e)
            {
                // log this error
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }

        public bool Delete(IBaseProps p)
        {
            StateProps props = (StateProps)p;
            int rowsAffected = 0;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_StateDelete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("code", DBDbType.VarChar);
            command.Parameters.Add("conCurrId", DBDbType.Int32);
            command.Parameters["code"].Value = props.Code;
            command.Parameters["conCurrId"].Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    return true;
                }
                else
                {
                    string message = "Record cannot be deleted. It has been edited by another user.";
                    throw new Exception(message);
                }

            }
            catch (Exception e)
            {
                // log this exception
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
            StateProps props = new StateProps();
            DBCommand command = new DBCommand();

            command.CommandText = "usp_StateSelect";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("code", DBDbType.VarChar);
            command.Parameters["code"].Value = key.ToString();

            try
            {
                data = RunProcedure(command);
                if (!data.IsClosed)
                {
                    if (data.Read())
                    {
                        props.SetState(data);
                    }
                    else
                        throw new Exception("Record does not exist in the database.");
                }
                return props;
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (data != null)
                {
                    if (!data.IsClosed)
                        data.Close();
                }
            }
        }

        public object RetrieveAll()
        {
            List<StateProps> list = new List<StateProps>();
            DBDataReader reader = null;
            StateProps props;

            try
            {
                reader = RunProcedure("usp_StateSelectAll");
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        props = new StateProps();
                        props.SetState(reader);
                        list.Add(props);
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                }
            }
        }

        public bool Update(IBaseProps p)
        {
            int rowsAffected = 0;
            StateProps props = (StateProps)p;

            DBCommand command = new DBCommand();
            command.CommandText = "usp_StateUpdate";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("code", DBDbType.VarChar);
            command.Parameters.Add("name", DBDbType.VarChar);
            command.Parameters.Add("conCurrId", DBDbType.Int32);
            command.Parameters["code"].Value = props.Code;
            command.Parameters["name"].Value = props.Name;
            command.Parameters["conCurrId"].Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID++;
                    return true;
                }
                else
                {
                    string message = "Record cannot be updated. It has been edited by another user.";
                    throw new Exception(message);
                }
            }
            catch (Exception e)
            {
                // log this exception
                throw;
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open)
                    mConnection.Close();
            }
        }
    }
}
