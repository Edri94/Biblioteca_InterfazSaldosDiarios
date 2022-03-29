using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Processes
{
    public class Main
    {
        public string gsFechaSQL;
        public string pass_enc;
        public string pass_desc;
        public string SQLpwd;
         
         //Variables para la conexion con AS/400
        public string msDSN400;
        public string msLibAS400;
        public string msUser400;
        public string msPswd400;
         
         //Variables para la conexion con la base de datos
        public string msDBuser;
        public string msDBPswd;
        public string msDBName;
        public string msDBSrvr;
         
         //Variables para recuperacion de archivos de saldos y vencimientos
        public string FileSaldoHO;
        public string FileVenciHO;
         
         //Opciones de operacion
        public string Bandera;
        public string Reintentos;
         
        public string Fecha_Int;


    }
}
