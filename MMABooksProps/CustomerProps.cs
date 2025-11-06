using System;
using MySql.Data.MySqlClient;
using MMABooksTools;

namespace MMABooksProps
{
    [Serializable]
    public class CustomerProps : IBaseProps, ICloneable
    {
        public int CustomerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public int ConcurrencyID { get; set; }

        public CustomerProps() { }

        public CustomerProps(CustomerProps p)
        {
            CustomerID = p.CustomerID;
            Name = p.Name;
            Address = p.Address;
            City = p.City;
            State = p.State;
            ZipCode = p.ZipCode;
            ConcurrencyID = p.ConcurrencyID;
        }

        public string GetState()
        {
            return $"CustomerID={CustomerID},Name={Name},Address={Address},City={City},State={State},Zip={ZipCode},ConcurrencyID={ConcurrencyID}";
        }

        public void SetState(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;
            var parts = s.Split(',', StringSplitOptions.TrimEntries);
            foreach (var part in parts)
            {
                var kv = part.Split('=', 2);
                if (kv.Length != 2) continue;
                var key = kv[0];
                var val = kv[1];
                switch (key)
                {
                    case "CustomerID": if (int.TryParse(val, out var id)) CustomerID = id; break;
                    case "Name": Name = val; break;
                    case "Address": Address = val; break;
                    case "City": City = val; break;
                    case "State": State = val; break;
                    case "Zip": ZipCode = val; break;
                    case "ConcurrencyID": if (int.TryParse(val, out var cc)) ConcurrencyID = cc; break;
                }
            }
        }

        public void SetState(MySqlDataReader dr)
        {
            CustomerID = (int)dr["CustomerID"];
            Name = dr["Name"].ToString();
            Address = dr["Address"].ToString();
            City = dr["City"].ToString();
            State = dr["State"].ToString();
            ZipCode = dr["ZipCode"].ToString();
            ConcurrencyID = (int)dr["ConcurrencyID"];
        }

        public object Clone() => new CustomerProps(this);
    }
}
