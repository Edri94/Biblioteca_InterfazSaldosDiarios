using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Processes
{
    public class Main
    {
        string gsFechaSQL;
        string pass_enc;
        string pass_desc;
        string SQLpwd;

        //Variables para la conexion con AS/400
        string msDSN400;
        string msLibAS400;
        string msUser400;
        string msPswd400;

        //Variables para la conexion con la base de datos
        string msDBuser;
        string msDBPswd;
        string msDBName;
        string msDBSrvr;

        //Variables para recuperacion de archivos de saldos y vencimientos
        string FileSaldoHO;
        string FileVenciHO;

        //Opciones de operacion
        string Bandera;
        string Reintentos;

        string Fecha_Int;


    }
}
