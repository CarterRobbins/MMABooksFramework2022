using System;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;

namespace MMABooksTools
{
    /// <summary>
    /// IBaseProps is the "middle tier" of the framework.
    /// It converts data tier attribute values into business tier attribute values.
    /// </summary>
    public interface IBaseProps : ICloneable
    {
        string GetState();

        void SetState(string jsonString);
        void SetState(DBDataReader dr);
    }
}
