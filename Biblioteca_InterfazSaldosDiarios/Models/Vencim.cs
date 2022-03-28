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
        public string Saldo { get; set; }
        public string FechaVen { get; set; }
        public string NumRegistros { get; set; }
        public string Intereses { get; set; }
        public string FinRegistro { get; set; }
    }
}
