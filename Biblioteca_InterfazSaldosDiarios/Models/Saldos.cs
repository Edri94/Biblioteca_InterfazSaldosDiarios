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
        public string SaldoIniDia { get; set; }
        public string InteresDia { get; set; }
        public string SwitchBloqueo { get; set; }
        public string FechaGenSaldo { get; set; }
        public string NumRegistros { get; set; }
        public string FinRegistro { get; set; }
    }
}
