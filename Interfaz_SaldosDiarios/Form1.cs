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
using System.Threading;

namespace Interfaz_SaldosDiarios
{
    public partial class Form1 : Form
    { 
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
            main.Ls_fechaoperacion = txtFechaProceso.Text;
            main.Init(false, txtFechaProceso.Text);          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            main = new Main(ref txtConsola, ref pgbrCargaSaldos, ref pgbrCargaVencimientos, ref txtStatusInterfaz, ref lblNumSaldosHO, ref lblNumVencimHO, ref txtArchivoSaldos, ref txtArchivoVencimiento,  ref txtFechaArchivos, ref pgbrSaldosBD, ref pgbrVencimientosBD);

            main.EstableceParametros();
            PintarParametros();
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

       

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFechaProceso_Enter(object sender, EventArgs e)
        {
            if(txtFechaProceso.Text == "dd-mm-yyyy")
            {
                txtFechaProceso.Text = String.Empty;
            }

            txtFechaProceso.ForeColor = Color.Black;
        }

        private void txtFechaProceso_Leave(object sender, EventArgs e)
        {
            if (txtFechaProceso.Text == String.Empty)
            {
                txtFechaProceso.Text = "dd-mm-yyyy";
            }

            txtFechaProceso.ForeColor = Color.DimGray;
        }
    }
}
