using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;
using DBConnection = MySql.Data.MySqlClient.MySqlConnection;
using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using DBDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;

namespace MMABooksTools
{
    /// <summary>
    /// BaseSQLDB is the class from which all data access classes that access a SQLServer 2000 database will be derived.
    /// The core functionality of establishing a connection with the database and executing simple stored procedures is
    /// also provided by this class.
    /// </summary>
    public abstract class BaseSQLDB
    {
        protected DBConnection mConnection;
        private string mConnectionString = "";

        #region Constructors

        /// <summary>
        /// The default constructor, it gets the connection string from config file and
        /// instantiates a connection object.  The connection is not yet open.
        /// </summary>
        public BaseSQLDB()
        {
            mConnectionString = ConfigDB.GetMySqlConnectionString();
            mConnection = new DBConnection(mConnectionString);
        }

        public BaseSQLDB(DBConnection cn)
        {
            mConnectionString = cn.ConnectionString;
            mConnection = cn;
        }

        #endregion

        #region Properties

        protected string ConnectionString
        {
            get
            {
                return mConnectionString;
            }
        }
        #endregion

        #region Methods

        public DBDataReader RunSQL(string sql)
        {
            DBDataReader reader;

            mConnection.Open();
            DBCommand command = new DBCommand(sql, mConnection);
            command.CommandType = CommandType.Text;
            reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public DBDataReader RunProcedure(string spName)
        {
            DBDataReader reader;

            mConnection.Open();
            DBCommand command = new DBCommand(spName, mConnection);
            command.CommandType = CommandType.StoredProcedure;
            reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public DBDataReader RunProcedure(DBCommand command)
        {
            DBDataReader reader;

            mConnection.Open();
            command.Connection = mConnection;
            reader = command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public int RunNonQueryProcedure(DBCommand command)
        {
            mConnection.Open();
            command.Connection = mConnection;
            int rows = command.ExecuteNonQuery();
            mConnection.Close();
            return rows;
        }

        public DataSet RunProcedure(DBCommand command, string tableAlias)
        {
            DataSet ds = new DataSet();
            DBDataAdapter da = new DBDataAdapter();

            mConnection.Open();
            command.Connection = mConnection;
            da.SelectCommand = command;
            da.Fill(ds, tableAlias);
            mConnection.Close();

            return ds;
        }

        public void RunProcedure(DBCommand command, string tableAlias, DataSet ds)
        {
            DBDataAdapter da = new DBDataAdapter();

            if (this.mConnection.State == ConnectionState.Closed)
            {
                this.mConnection.Open();
            }
            command.Connection = this.mConnection;
            da.SelectCommand = command;
            da.Fill(ds, tableAlias);
            mConnection.Close();
        }

        #endregion
    }
}
