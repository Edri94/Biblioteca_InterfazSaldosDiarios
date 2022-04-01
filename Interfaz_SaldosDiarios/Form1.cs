using Biblioteca_InterfazSaldosDiarios.Data;
using Biblioteca_InterfazSaldosDiarios.Helpers;
using Biblioteca_InterfazSaldosDiarios.Models;
using Biblioteca_InterfazSaldosDiarios.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Odbc;


namespace Interfaz_SaldosDiarios
{
    public partial class Form1 : Form
    {
        bool mbConexion;
        bool conectado;

        byte mnFirstTime;

        string msSQL;
        string msSQL400;
        int mnBanderaSHO;
        int mnBanderaVHO;
        string msPathFile;

        List<Saldos> maSaldos;
        List<Vencim> maVencimientos;

        string gsFechaArchivo;
        string ls_fechaoperacion;

        Encriptacion encriptacion;
        FuncionesBD bd;
        ConexionAS400 as400;
        Main main;


        public Form1()
        {
            InitializeComponent();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCargaSaldos_Click(object sender, EventArgs e)
        {
            ls_fechaoperacion = txtFechaProceso.Text;

            var reg = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[-]([0]?[1-9]|[1][0-2])[-]([0-9]{4}|[0-9]{2})$").Match(ls_fechaoperacion).Success;

            if (!reg)
            {
                Message("El formato de la fecha de proceso esta mal. El formato correcto es dd-MM-yyyy");
                return;
            }

            if (main.msDSN400 == "" || main.msUser400 == "" || main.msPswd400 == "" || main.msLibAS400 == "")
            {
                Message("Verifique el archivo de inicio. Faltan datos para la conexion con AS400");
                return;
            }

            if (main.msDBSrvr == "" || main.msDBuser == "" || main.msDBPswd == "" || main.msDBName == "")
            {
                Message("Verifique el archivo de inicio. Faltan datos para la conexion con con SQL Server");
                return;
            }

            mnFirstTime = 1;

            Message("Conectando...");

            if (EstableceConexionBD() == true && as400.Conectar() == true)
            {
                Message("En linea...");
                Message("Calculando fecha de los archivos a recuperar...");

                if (CalculaFechaArchivos())
                {
                    txtFechaArchivos.Text = ls_fechaoperacion;
                    mnBanderaSHO = 0;
                    mnBanderaVHO = 0;
                    RecibeArchivos();
                }
            }
        }

        private void RecibeArchivos()
        {
            pgbrCargaSaldos.Value = 5;
            if (mnBanderaSHO == 0)
            {
                if (ProcesaInfo(1))
                {
                    Message("Carga de saldos completa...");

                    if (!ActValorTransfer(1, 1))
                    {

                    }
                }
            }
        }

        private bool ProcesaInfo(int TipoInfo)
        {
            long lnDatos;
            long lnContador;
            long lnNumRegistros;
            string lsArchivoAS400 = "";

            bool ProcesaInfo = false;

            try
            {
                switch (TipoInfo)
                {
                    case 1:
                        Message("Recibiendo Información de saldos Houston");
                        lsArchivoAS400 = txtArchivoSaldos.Text;
                        break;
                    case 2:
                        Message("Recibiendo Información de vencimientos Houston");
                        lsArchivoAS400 = txtArchivoVencimiento.Text;
                        break;
                    default:
                        break;
                }

                msSQL400 = $"Select COUNT(*) FROM {main.msLibAS400}.{lsArchivoAS400}";

                OdbcDataReader dr = as400.EjecutaSelect(msSQL400);

                

                List<List<Map>> maps = new List<List<Map>>();
                List<Map> mapa_definicion = new List<Map>();

                mapa_definicion.Add(new Map { Key = "cuenta", Type = "int" });
                maps = as400.LLenarMapToQuery(mapa_definicion, dr);

                lnNumRegistros = maps[0][0].GetIn32();
                lnContador = 0;
                lnDatos = 0;

                switch (TipoInfo)
                {
                    case 1:
                        lblNumSaldosHO.Text = "Registros a procesar : " + lnNumRegistros;
                        pgbrCargaSaldos.Value = 10;
                    break;
                    case 2:
                        lblNumVencimHO.Text = "Registros a procesar : " + lnNumRegistros;
                        pgbrCargaVencimientos.Value = 10;
                    break;
                }
                
                if(lnNumRegistros > 0)
                {
                    switch (TipoInfo)
                    {
                        case 1:
                            msSQL400 = $"Select SDAB, SDAN, SDAS, SDBAL, SDINT , SDAI17, SDDTE1, SDNREG, SDFIN FROM {main.msLibAS400}.{lsArchivoAS400}";
                            mapa_definicion = new List<Map>();
                            mapa_definicion.Add(new Map { Key = "SDAB", Type = "string" });
                            mapa_definicion.Add(new Map { Key = "SDAN", Type = "string" });
                            mapa_definicion.Add(new Map { Key = "SDAS", Type = "string" });
                            mapa_definicion.Add(new Map { Key = "SDBAL", Type = "float" });
                            mapa_definicion.Add(new Map { Key = "SDINT", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "SDAI17", Type = "string" });
                            mapa_definicion.Add(new Map { Key = "SDDTE1", Type = "string" });
                            mapa_definicion.Add(new Map { Key = "SDNREG", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "SDFIN", Type = "string" });
                        break;
                        case 2:
                            msSQL400 = $"Select CDDLR, CDAB, CDAN, CDAS, CDDLR, CDAB, CDAN, CDAS, CDFIN FROM {main.msLibAS400}.{lsArchivoAS400}";
                            mapa_definicion.Add(new Map { Key = "CDDLR", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAB", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAN", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAS", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDDLR", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAB", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAN", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDAS", Type = "int" });
                            mapa_definicion.Add(new Map { Key = "CDFIN", Type = "int" });
                        break;
                    }


                    //Busca los registros por procesar
                    dr = as400.EjecutaSelect(msSQL400);
                    maps = as400.LLenarMapToQuery(mapa_definicion, dr);

                    if(maps != null)
                    {
                        maSaldos = new List<Saldos>();

                        if (maps.Count == lnNumRegistros)
                        {
                            for(int fila = 0; fila < lnNumRegistros; fila++)
                            {
                                switch (TipoInfo)
                                {
                                    case 1:

                                        Saldos saldo = new Saldos { 
                                            Agencia = Funcion.Left(maps[fila][0].GetString() + Funcion.Space(4), 4),
                                            NumCuenta = "",
                                            SufijoCuenta = "",
                                            SaldoIniDia = 0,
                                            InteresDia  = 0,
                                            SwitchBloqueo = "",
                                            FechaGenSaldo = "",
                                            NumRegistros = 0,
                                            FinRegistro = ""

                                        };
                                         maSaldos.Add(saldo);


                                    break;
                                }
                            }
                            
                        }
                    }
                    

                    

                }


            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }

            return ProcesaInfo;
        }

        private bool ActValorTransfer(int status, int tipo_bandera)
        {
            bool ActValorTransfer = false;

            try
            {
                string ls_QueryActualiza = "UPDATE PARAMETROS SET ";

                switch (tipo_bandera)
                {
                    case 1:
                        ls_QueryActualiza += "transfer_saldos_ho = @status";
                        break;
                    case 2:
                        ls_QueryActualiza += "transfer_venc_ho = @status";
                        break;
                    case 3:
                        ls_QueryActualiza += "transfer_saldos_ho = @status , transfer_venc_ho = @status";
                        break;
                }

                SqlParameter[] parametros_1 =
                {
                    new SqlParameter("@status",SqlDbType.Int, 1) { Value = status }
                };

                int resultado = bd.ejecutarActualizacionParametros(ls_QueryActualiza, parametros_1);

                ActValorTransfer = (resultado > 0) ? false : true;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                ActValorTransfer = true;
            }

            return ActValorTransfer;
        }

        private bool CalculaFechaArchivos()
        {
            bool CalculaFechaArchivos = false;
            string ls_FechaDiaActual;
            string lsQuery;
            string TipoFecha;
            int DiasAgregados;
            int tempDiasFeriados;
            int lnEsDíaFeriado;

            try
            {
                //Obtiene la fecha del servidor SQL
                lsQuery = "SELECT CONVERT(VARCHAR,DATEPART(dd, getdate()),2)+'/'+CONVERT(VARCHAR,DATEPART(month, getdate()),2)+'/'+CONVERT(VARCHAR,DATEPART(yy, getdate()),2)";
                SqlDataReader dr = bd.ejecutarConsulta(lsQuery);

                List<Map> maps = new List<Map>();
                maps.Add(new Map { Key = "Fecha_Actual", Type = "string" });
                maps = bd.LLenarMapToQuery(maps, dr);


                ls_FechaDiaActual = Funcion.InvierteFecha(maps[0].GetString(), false);
                main.Fecha_Int = DateTime.Parse(ls_fechaoperacion).ToString("MM-dd-yy");

                //Verifica si hoy es festivo...
                string tmp_fecha = DateTime.Parse(ls_fechaoperacion).ToString("yyyy-MM-dd") + " 00:00:00.000";
                lsQuery = "SELECT COUNT(*) FROM CATALOGOS..DIAS_FERIADOS WHERE fecha = @fechaOperacion";

                SqlParameter[] parametros_1 =
                {
                    new SqlParameter("@fechaOperacion",SqlDbType.VarChar, 18) { Value = tmp_fecha }
                };

                dr = bd.ejecutarConsultaParametros(lsQuery, parametros_1);
                maps = new List<Map>();
                maps.Add(new Map { Key = "Dias_Feriados", Type = "int" });
                maps = bd.LLenarMapToQuery(maps, dr);

                if (maps[0].GetIn32() != 0)
                {
                    //Inicializamos el tipo día feriado
                    TipoFecha = "3";
                    lsQuery = "SELECT  CAST(ISNULL(tipo_dia_feriado, 3) AS INT) as [tipo_dia_feriado] FROM CATALOGOS..DIAS_FERIADOS WHERE fecha = @fechaOperacion";

                    SqlParameter[] parametros_2 =
                    {
                        new SqlParameter("@fechaOperacion",SqlDbType.VarChar, 18) { Value = tmp_fecha }
                    };


                    dr = bd.ejecutarConsultaParametros(lsQuery, parametros_2);
                    maps = new List<Map>();
                    maps.Add(new Map { Key = "tipo_dia_feriado", Type = "int" });
                    maps = bd.LLenarMapToQuery(maps, dr);

                    TipoFecha = maps[0].GetIn32().ToString();

                    // Si es festivo en Houston o es sábado o Domingo, calcula el día hábil siguiente en la agencia
                    if (TipoFecha != "")
                    {
                        //Si es día festivo en Houston, obtiene día hábil siguiente
                        if (TipoFecha == "2" || TipoFecha == "3")
                        {
                            DiasAgregados = 1;
                            tempDiasFeriados = 1;

                            do
                            {
                                DateTime tmp_fecha_invertida = DateTime.Parse(Funcion.InvierteFecha(ls_FechaDiaActual, false));
                                //ls_fechaoperacion = Funcion.InvierteFecha(tmp_fecha_invertida.AddDays(DiasAgregados).ToString("dd-MM-yyyy"), false);
                                ls_fechaoperacion = tmp_fecha_invertida.AddDays(DiasAgregados).ToString("dd-MM-yyyy");

                                //Calcula día habil siguiente con base en a CATALOGOS..DIAS FERIADOS
                                tmp_fecha = DateTime.Parse(ls_fechaoperacion).ToString("yyyy-MM-dd") + " 00:00:00.000";
                                lsQuery = "SELECT COUNT(1) as [dias] FROM CATALOGOS..DIAS_FERIADOS WHERE fecha =  @fechaOperacion and tipo_dia_feriado in(2, 3)";

                                SqlParameter[] parametros_3 =
                                {
                                    new SqlParameter("@fechaOperacion",SqlDbType.VarChar, 18) { Value = tmp_fecha }
                                };

                                dr = bd.ejecutarConsultaParametros(lsQuery, parametros_3);
                                maps = new List<Map>();
                                maps.Add(new Map { Key = "dias", Type = "int" });
                                maps = bd.LLenarMapToQuery(maps, dr);
                                tempDiasFeriados = maps[0].GetIn32();

                                DiasAgregados = DiasAgregados + 1;


                            } while (tempDiasFeriados != 0);
                        }
                    }

                }



                gsFechaArchivo = ls_fechaoperacion;
                CalculaFechaArchivos = true;

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                CalculaFechaArchivos = false;
            }
            return CalculaFechaArchivos;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            main = new Main();
            encriptacion = new Encriptacion();

            EstableceParametros();
            PintarParametros();
        }

        private void EstableceParametros()
        {
            try
            {
                main.msDBName = encriptacion.Decrypt(Funcion.getValueAppConfig("Nombre", "BD"));
                main.msDBSrvr = encriptacion.Decrypt(Funcion.getValueAppConfig("Servidor", "BD"));
                main.msDBuser = encriptacion.Decrypt(Funcion.getValueAppConfig("Usuario", "BD"));
                main.msDBPswd = encriptacion.Decrypt(Funcion.getValueAppConfig("Password", "BD"));

                main.msDSN400 = encriptacion.Decrypt(Funcion.getValueAppConfig("DSN", "AS400"));
                main.msLibAS400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Biblioteca", "AS400"));
                main.msUser400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Usuario", "AS400"));
                main.msPswd400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Password", "AS400"));

                main.FileSaldoHO = encriptacion.Decrypt(Funcion.getValueAppConfig("SaldosHouston", "ARCHIVO"));
                main.FileVenciHO = encriptacion.Decrypt(Funcion.getValueAppConfig("VencimientoHouston", "ARCHIVO"));

                as400 = new ConexionAS400(main.msDSN400, main.msUser400, main.msPswd400);

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
        }

        private void PintarParametros()
        {
            txtBaseDeDatos.Text = main.msDBName;
            txtServidorTicket.Text = main.msDBSrvr;

            txtDsnAS400.Text = main.msDSN400;
            txtBibliotecaAS400.Text = main.msLibAS400;

            txtArchivoSaldos.Text = main.FileSaldoHO;
            txtArchivoVencimiento.Text = main.FileVenciHO;

        }

        private void Message(string mensaje)
        {
            txtConsola.AppendText("$ " + mensaje);
            txtConsola.AppendText(Environment.NewLine);
        }


        private bool EstableceConexionBD()
        {
            int lnIntento = 1;
            bool EstableceConexion = false;

            Message("Estableciendo conexion con AS400...");


            try
            {
                string conn_str = $"Data source ={main.msDBSrvr}; uid ={main.msDBuser}; PWD ={main.msDBPswd}; initial catalog = {main.msDBName}";

                if (bd == null)
                {
                    bd = new FuncionesBD(conn_str);
                    bd.ActiveConnection = true;
                    EstableceConexion = true;
                }

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                EstableceConexion = true;
            }

            return EstableceConexion;
        }
    }
}
