using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Models
{
    public class Map
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }

        public int GetIn32()
        {
            if(Value.GetType().Name == "Int32")
            {
                return Int32.Parse(Value.ToString());
            }
            else
            {
                return 0;
            }
            
        }

        public string GetString()
        {
            if (Value.GetType().Name == "String")
            {
                return Value.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        public long GetLong()
        {
            if (Value.GetType().Name == "long")
            {
                return long.Parse(Value.ToString());
            }
            else
            {
                return 0;
            }
        }
    }
}
