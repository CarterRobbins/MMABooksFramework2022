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
    public class ProductDB : DBBase, IReadDB, IWriteDB
    {
        public ProductDB() : base() { }
        public ProductDB(DBConnection cn) : base(cn) { }

        public IBaseProps Create(IBaseProps p)
        {
            int rowsAffected = 0;
            ProductProps props = (ProductProps)p;

            DBCommand command = new DBCommand
            {
                CommandText = "usp_ProductCreate",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("prodcode_p", DBDbType.VarChar).Value = props.ProductCode;
            command.Parameters.Add("description_p", DBDbType.VarChar).Value = props.Description;
            command.Parameters.Add("unitprice_p", DBDbType.Decimal).Value = props.UnitPrice;
            command.Parameters.Add("onhandqty_p", DBDbType.Int32).Value = props.OnHandQuantity;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID = 1;
                    return props;
                }
                else
                {
                    throw new Exception("Unable to insert record. " + props.GetState());
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
            var props = (ProductProps)p;

            var fresh = (ProductProps)Retrieve(props.ProductCode);
            props.ConcurrencyID = fresh.ConcurrencyID;

            int rows = 0;
            var command = new DBCommand
            {
                CommandText = "usp_ProductDelete",
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("prodcode_p", DBDbType.VarChar).Value = props.ProductCode;
            command.Parameters.Add("conCurrId", DBDbType.Int32).Value = props.ConcurrencyID;

            try
            {
                rows = RunNonQueryProcedure(command);
                if (rows == 1) return true;
                throw new Exception("Record cannot be deleted; it has been edited by another user.");
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open) mConnection.Close();
            }
        }

        public IBaseProps Retrieve(object key)
        {
            DBDataReader data = null;
            ProductProps props = new ProductProps();

            DBCommand command = new DBCommand
            {
                CommandText = "usp_ProductSelect",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("prodcode_p", DBDbType.VarChar).Value = key.ToString();

            try
            {
                data = RunProcedure(command);
                if (!data.IsClosed)
                {
                    if (data.Read())
                        props.SetState(data);
                    else
                        throw new Exception("Record does not exist in the database.");
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
            List<ProductProps> list = new List<ProductProps>();
            DBDataReader reader = null;

            try
            {
                reader = RunProcedure("usp_ProductSelectAll");
                if (!reader.IsClosed)
                {
                    while (reader.Read())
                    {
                        ProductProps props = new ProductProps();
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
            var props = (ProductProps)p;

            
            var fresh = (ProductProps)Retrieve(props.ProductCode);
            props.ConcurrencyID = fresh.ConcurrencyID;

            int rowsAffected = 0;
            var command = new DBCommand
            {
                CommandText = "usp_ProductUpdate",
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("prodcode_p", DBDbType.VarChar).Value = props.ProductCode;
            command.Parameters.Add("description_p", DBDbType.VarChar).Value = props.Description;
            command.Parameters.Add("unitprice_p", DBDbType.Decimal).Value = props.UnitPrice;
            command.Parameters.Add("onhandqty_p", DBDbType.Int32).Value = props.OnHandQuantity;
            command.Parameters.Add("conCurrId", DBDbType.Int32).Value = props.ConcurrencyID;

            try
            {
                rowsAffected = RunNonQueryProcedure(command);
                if (rowsAffected == 1)
                {
                    props.ConcurrencyID++;
                    return true;
                }
                throw new Exception("Record cannot be updated; it has been edited by another user.");
            }
            finally
            {
                if (mConnection.State == ConnectionState.Open) mConnection.Close();
            }
        }
    }
}
