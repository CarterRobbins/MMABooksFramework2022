using System;

using MMABooksTools;
using DBDataReader = MySql.Data.MySqlClient.MySqlDataReader;

using System.Text.Json;
using System.Text.Json.Serialization;


namespace MMABooksProps
{
    [Serializable()]
    public class StateProps : IBaseProps
    {
        #region Auto-implemented Properties
        public string Code { get; set; } = "";

        public string Name { get; set; } = "";

        /// <summary>
        /// ConcurrencyID. Don't manipulate directly.
        /// </summary>
        public int ConcurrencyID { get; set; } = 0;
        #endregion
        public object Clone()
        {
            StateProps p = new StateProps();
            p.Code = this.Code;
            p.Name = this.Name;
            p.ConcurrencyID = this.ConcurrencyID;
            return p;
        }

        // this is always the same ... so I should have made IBaseProps and abstract class
        public string GetState()
        {
            string jsonString;
            jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }

        public void SetState(string jsonString)
        {
            StateProps p = JsonSerializer.Deserialize<StateProps>(jsonString);
            this.Code = p.Code;
            this.Name = p.Name;
            this.ConcurrencyID = p.ConcurrencyID;
        }

        public void SetState(DBDataReader dr)
        {
            this.Code = ((string)dr["StateCode"]).Trim();
            this.Name = (string)dr["StateName"];
            this.ConcurrencyID = (Int32)dr["ConcurrencyID"];
        }
    }
}
