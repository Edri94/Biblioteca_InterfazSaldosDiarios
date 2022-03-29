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
        string mnBanderaSHO;
        string mnBanderaVHO;
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
            
            if(main.msDSN400 == "" || main.msUser400 == "" || main.msPswd400 == "" || main.msLibAS400 == "")
            {
                Message("Verifique el archivo de inicio. Faltan datos para la conexion con AS400");
            }

            if (main.msDBSrvr == "" || main.msDBuser == "" || main.msDBPswd == "" || main.msDBName == "")
            {
                Message("Verifique el archivo de inicio. Faltan datos para la conexion con con SQL Server");
            }

            mnFirstTime = 1;

            Message("Conectando...");

            if(EstableceConexion())
            {
                Message("En linea...");
            }
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
