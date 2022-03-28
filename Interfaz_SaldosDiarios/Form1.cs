using Biblioteca_InterfazSaldosDiarios.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            
            if(msd)
        }
    }
}
