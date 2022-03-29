
namespace Interfaz_SaldosDiarios
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtServidorTicket = new System.Windows.Forms.TextBox();
            this.txtBaseDeDatos = new System.Windows.Forms.TextBox();
            this.txtDsnAS400 = new System.Windows.Forms.TextBox();
            this.txtBibliotecaAS400 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtArchivoSaldos = new System.Windows.Forms.TextBox();
            this.txtArchivoVencimiento = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtFechaProceso = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStatusInterfaz = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.pgbrCargaSaldos = new System.Windows.Forms.ProgressBar();
            this.pgbrCargaVencimientos = new System.Windows.Forms.ProgressBar();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtFechaArchivos = new System.Windows.Forms.TextBox();
            this.btnCargaSaldos = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.txtConsola = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtBibliotecaAS400);
            this.groupBox1.Controls.Add(this.txtDsnAS400);
            this.groupBox1.Controls.Add(this.txtBaseDeDatos);
            this.groupBox1.Controls.Add(this.txtArchivoVencimiento);
            this.groupBox1.Controls.Add(this.txtArchivoSaldos);
            this.groupBox1.Controls.Add(this.txtServidorTicket);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(669, 192);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configuracion";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Servidor Ticket:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Base de datos:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "DSN AS/400:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 152);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Biblioteca AS/400:";
            // 
            // txtServidorTicket
            // 
            this.txtServidorTicket.Location = new System.Drawing.Point(170, 41);
            this.txtServidorTicket.Name = "txtServidorTicket";
            this.txtServidorTicket.Size = new System.Drawing.Size(182, 26);
            this.txtServidorTicket.TabIndex = 1;
            // 
            // txtBaseDeDatos
            // 
            this.txtBaseDeDatos.Location = new System.Drawing.Point(170, 78);
            this.txtBaseDeDatos.Name = "txtBaseDeDatos";
            this.txtBaseDeDatos.Size = new System.Drawing.Size(182, 26);
            this.txtBaseDeDatos.TabIndex = 1;
            // 
            // txtDsnAS400
            // 
            this.txtDsnAS400.Location = new System.Drawing.Point(170, 115);
            this.txtDsnAS400.Name = "txtDsnAS400";
            this.txtDsnAS400.Size = new System.Drawing.Size(182, 26);
            this.txtDsnAS400.TabIndex = 1;
            // 
            // txtBibliotecaAS400
            // 
            this.txtBibliotecaAS400.Location = new System.Drawing.Point(170, 152);
            this.txtBibliotecaAS400.Name = "txtBibliotecaAS400";
            this.txtBibliotecaAS400.Size = new System.Drawing.Size(182, 26);
            this.txtBibliotecaAS400.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(439, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 20);
            this.label5.TabIndex = 2;
            this.label5.Text = "Archivo saldos houston:";
            // 
            // txtArchivoSaldos
            // 
            this.txtArchivoSaldos.Location = new System.Drawing.Point(426, 62);
            this.txtArchivoSaldos.Name = "txtArchivoSaldos";
            this.txtArchivoSaldos.Size = new System.Drawing.Size(210, 26);
            this.txtArchivoSaldos.TabIndex = 1;
            this.txtArchivoSaldos.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // txtArchivoVencimiento
            // 
            this.txtArchivoVencimiento.Location = new System.Drawing.Point(426, 146);
            this.txtArchivoVencimiento.Name = "txtArchivoVencimiento";
            this.txtArchivoVencimiento.Size = new System.Drawing.Size(210, 26);
            this.txtArchivoVencimiento.TabIndex = 1;
            this.txtArchivoVencimiento.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(422, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(218, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "Archivo vencimiento Houston:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSalir);
            this.groupBox2.Controls.Add(this.btnCargaSaldos);
            this.groupBox2.Controls.Add(this.txtFechaArchivos);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.txtStatusInterfaz);
            this.groupBox2.Controls.Add(this.txtFechaProceso);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(12, 224);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(669, 194);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Fecha de Proceso:";
            // 
            // txtFechaProceso
            // 
            this.txtFechaProceso.Location = new System.Drawing.Point(196, 26);
            this.txtFechaProceso.Name = "txtFechaProceso";
            this.txtFechaProceso.Size = new System.Drawing.Size(233, 26);
            this.txtFechaProceso.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 118);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "Status Interfaz:";
            // 
            // txtStatusInterfaz
            // 
            this.txtStatusInterfaz.Location = new System.Drawing.Point(21, 150);
            this.txtStatusInterfaz.Name = "txtStatusInterfaz";
            this.txtStatusInterfaz.Size = new System.Drawing.Size(615, 26);
            this.txtStatusInterfaz.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.pgbrCargaVencimientos);
            this.groupBox3.Controls.Add(this.pgbrCargaSaldos);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(12, 424);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(669, 150);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Carga de Informacion AS400 >> TKT";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 39);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(159, 20);
            this.label9.TabIndex = 0;
            this.label9.Text = "Carga de Saldos HO:";
            // 
            // pgbrCargaSaldos
            // 
            this.pgbrCargaSaldos.Location = new System.Drawing.Point(11, 73);
            this.pgbrCargaSaldos.Name = "pgbrCargaSaldos";
            this.pgbrCargaSaldos.Size = new System.Drawing.Size(308, 23);
            this.pgbrCargaSaldos.TabIndex = 1;
            // 
            // pgbrCargaVencimientos
            // 
            this.pgbrCargaVencimientos.Location = new System.Drawing.Point(343, 73);
            this.pgbrCargaVencimientos.Name = "pgbrCargaVencimientos";
            this.pgbrCargaVencimientos.Size = new System.Drawing.Size(308, 23);
            this.pgbrCargaVencimientos.TabIndex = 1;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(339, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(206, 20);
            this.label10.TabIndex = 2;
            this.label10.Text = "Carga de Vencimientos HO:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 109);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(160, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "Registros a procesar:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(339, 109);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(161, 20);
            this.label12.TabIndex = 4;
            this.label12.Text = "Registros a Procesar:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(463, 26);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(91, 20);
            this.label13.TabIndex = 3;
            this.label13.Text = "dd-mm-yyyy";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(21, 76);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(168, 20);
            this.label14.TabIndex = 4;
            this.label14.Text = "Fecha de los Archivos:";
            // 
            // txtFechaArchivos
            // 
            this.txtFechaArchivos.Location = new System.Drawing.Point(196, 73);
            this.txtFechaArchivos.Name = "txtFechaArchivos";
            this.txtFechaArchivos.Size = new System.Drawing.Size(233, 26);
            this.txtFechaArchivos.TabIndex = 5;
            // 
            // btnCargaSaldos
            // 
            this.btnCargaSaldos.Location = new System.Drawing.Point(476, 63);
            this.btnCargaSaldos.Name = "btnCargaSaldos";
            this.btnCargaSaldos.Size = new System.Drawing.Size(149, 33);
            this.btnCargaSaldos.TabIndex = 6;
            this.btnCargaSaldos.Text = "Cargar Saldos";
            this.btnCargaSaldos.UseVisualStyleBackColor = true;
            this.btnCargaSaldos.Click += new System.EventHandler(this.btnCargaSaldos_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.Location = new System.Drawing.Point(476, 102);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(149, 36);
            this.btnSalir.TabIndex = 6;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = true;
            // 
            // txtConsola
            // 
            this.txtConsola.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtConsola.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsola.ForeColor = System.Drawing.Color.LawnGreen;
            this.txtConsola.Location = new System.Drawing.Point(12, 595);
            this.txtConsola.Multiline = true;
            this.txtConsola.Name = "txtConsola";
            this.txtConsola.ReadOnly = true;
            this.txtConsola.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsola.Size = new System.Drawing.Size(669, 100);
            this.txtConsola.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 707);
            this.Controls.Add(this.txtConsola);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Interfaz Saldos EQ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBibliotecaAS400;
        private System.Windows.Forms.TextBox txtDsnAS400;
        private System.Windows.Forms.TextBox txtBaseDeDatos;
        private System.Windows.Forms.TextBox txtArchivoSaldos;
        private System.Windows.Forms.TextBox txtServidorTicket;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtArchivoVencimiento;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtStatusInterfaz;
        private System.Windows.Forms.TextBox txtFechaProceso;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ProgressBar pgbrCargaVencimientos;
        private System.Windows.Forms.ProgressBar pgbrCargaSaldos;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnCargaSaldos;
        private System.Windows.Forms.TextBox txtFechaArchivos;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtConsola;
    }
}

