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

        Saldos[] maSaldos;
        Vencim[] maVencimientos;

        string gsFechaArchivo;
        string ls_fechaoperacion;

        Encriptacion encriptacion;
        FuncionesBD bd; 
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
            
            if(!reg)
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

            if(EstableceConexion())
            {
                Message("En linea...");
                Message("Calculando fecha de los archivos a recuperar...");

                if(CalculaFechaArchivos())
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
            if(mnBanderaSHO == 0)
            {
                Message("Carga de saldos completa...");
                
                if(!ActValorTransfer(1, 1))
                {
                   
                }
            }
        }

        private bool ActValorTransfer(int status, int tipo_bandera)
        {
            bool ActValorTransfer = false;
            
            try
            {
                string ls_QueryActualiza = "UPDATE PARAMETROS SET ";

                switch(tipo_bandera)
                {
                    case 1:
                        ls_QueryActualiza = ls_QueryActualiza + "transfer_saldos_ho = " + status;
                    break;
                    case 2:
                        ls_QueryActualiza = ls_QueryActualiza + "transfer_venc_ho = " + status;
                    break;
                    case 3:
                        ls_QueryActualiza = ls_QueryActualiza + "transfer_saldos_ho = " + status + ", transfer_venc_ho = " + status;
                    break;
                }
                //ejecutar consulta aqui
                ActValorTransfer = true;

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
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

               
                ls_FechaDiaActual = Funcion.InvierteFecha(maps[0].Value.ToString(), false);
                main.Fecha_Int =DateTime.Parse(ls_fechaoperacion).ToString("MM-dd-yy");

                //Verifica si hoy es festivo...
                string tmp_fecha = DateTime.Parse(ls_fechaoperacion).ToString("yyyy-MM-dd") + " 00:00:00.000";
                lsQuery = $"SELECT COUNT(*) FROM CATALOGOS..DIAS_FERIADOS WHERE fecha = '{tmp_fecha}'";
                
                dr = bd.ejecutarConsulta(lsQuery);
                maps = new List<Map>();
                maps.Add(new Map { Key = "Dias_Feriados", Type = "int" });
                maps = bd.LLenarMapToQuery(maps, dr);

                if(Int32.Parse(maps[0].Value.ToString()) != 0)
                {
                    //Inicializamos el tipo día feriado
                    TipoFecha = "3";
                    lsQuery = $"SELECT  CAST(ISNULL(tipo_dia_feriado, 3) AS INT) as [tipo_dia_feriado] FROM CATALOGOS..DIAS_FERIADOS WHERE fecha = '{tmp_fecha}'";

                    dr = bd.ejecutarConsulta(lsQuery);
                    maps = new List<Map>();
                    maps.Add(new Map { Key = "tipo_dia_feriado", Type = "int" });
                    maps = bd.LLenarMapToQuery(maps, dr);

                    TipoFecha = maps[0].Value.ToString();

                    // Si es festivo en Houston o es sábado o Domingo, calcula el día hábil siguiente en la agencia
                    if (TipoFecha != "")
                    {
                        //Si es día festivo en Houston, obtiene día hábil siguiente
                        if(TipoFecha == "2" || TipoFecha == "3")
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
                                lsQuery = $"SELECT COUNT(1) as [dias] FROM CATALOGOS..DIAS_FERIADOS WHERE fecha = '{tmp_fecha}' and tipo_dia_feriado in(2, 3)";

                                dr = bd.ejecutarConsulta(lsQuery);
                                maps = new List<Map>();
                                maps.Add(new Map { Key = "dias", Type = "int" });
                                maps = bd.LLenarMapToQuery(maps, dr);
                                tempDiasFeriados = Int32.Parse(maps[0].Value.ToString());

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

        private bool EstableceConexion()
        {
            int lnIntento = 1;
            bool EstableceConexion = false;

            Message("Estableciendo conexion con AS400...");


            try
            {
                string conn_str = $"Data source ={main.msDBSrvr}; uid ={main.msDBuser}; PWD ={main.msDBPswd}; initial catalog = {main.msDBName}";
                
                if(bd == null)
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

                main.FileSaldoHO = encriptacion.Decrypt(Funcion.getValueAppConfig("SaldosHouston", "ARCHIVO"));
                main.FileVenciHO = encriptacion.Decrypt(Funcion.getValueAppConfig("VencimientoHouston", "ARCHIVO"));

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
    }
}
