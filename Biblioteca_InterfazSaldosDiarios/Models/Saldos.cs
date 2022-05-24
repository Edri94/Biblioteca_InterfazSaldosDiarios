using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Models
{
    public class Saldos
    {
        public string Agencia { get; set; }
        public string NumCuenta { get; set; }
        public string SufijoCuenta { get; set; }
        public decimal SaldoIniDia { get; set; }
        public int InteresDia { get; set; }
        public string SwitchBloqueo { get; set; }
        public DateTime FechaGenSaldo { get; set; }
        public int NumRegistros { get; set; }
        public string FinRegistro { get; set; }
    }
}
