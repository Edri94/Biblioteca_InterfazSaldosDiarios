using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Models
{
    public class Vencim
    {
        public string Ticket { get; set; }
        public string Agencia { get; set; }
        public string NumCuenta { get; set; }
        public string SufijoCuenta { get; set; }
        public decimal Saldo { get; set; }
        public DateTime FechaVen { get; set; }
        public int NumRegistros { get; set; }
        public float Intereses { get; set; }
        public string FinRegistro { get; set; }
    }
}
