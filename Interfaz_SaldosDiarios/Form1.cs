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
            int lnDatos;
            int lnContador;
            int lnNumRegistros;
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


                List<Map> mapa_cuenta = new List<Map>();
                mapa_cuenta.Add(new Map { Key = "cuenta", Type = "int" });
                mapa_cuenta = as400.LLenarMapToQuery(mapa_cuenta, dr);

                lnNumRegistros = mapa_cuenta[0].GetIn32();

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

                if (lnNumRegistros > 0)
                {
                    switch (TipoInfo)
                    {
                        case 1:
                            msSQL400 = $"Select SDAB, SDAN, SDAS, SDBAL, SDINT , SDAI17, SDDTE1, SDNREG, SDFIN FROM {main.msLibAS400}.{lsArchivoAS400}";
                            break;
                        case 2:
                            msSQL400 = $"Select CDDLR, CDAB, CDAN, CDAS, CDBAL, CDDTE, CDREG, CDINT, CDFIN FROM {main.msLibAS400}.{lsArchivoAS400}";
                            break;
                    }


                    //Busca los registros por procesar
                    dr = as400.EjecutaSelect(msSQL400);

                    if (dr.HasRows)
                    {
                        maSaldos = new List<Saldos>();
                        maVencimientos = new List<Vencim>();

                        while (dr.Read())
                        {
                            switch (TipoInfo)
                            {
                                case 1:

                                    Saldos saldo = new Saldos
                                    {
                                        Agencia = Funcion.Left(dr.GetString(0) + Funcion.Space(4), 4),
                                        NumCuenta = Funcion.Left(dr.GetString(1) + Funcion.Space(6), 6),
                                        SufijoCuenta = Funcion.Left(dr.GetString(2) + Funcion.Space(3), 3),
                                        SaldoIniDia = dr.GetFloat(3),
                                        InteresDia = dr.GetInt32(4),
                                        SwitchBloqueo = dr.GetString(5),
                                        FechaGenSaldo = DateTime.Parse(dr.GetString(6)),
                                        NumRegistros = dr.GetInt32(7),
                                        FinRegistro = dr.GetString(8)

                                    };

                                    lnContador++;
                                    txtStatusInterfaz.Text = $"Recibiendo Saldos HO. ({lnContador} Registros)";
                                    ActualizaProgreso(ref pgbrCargaSaldos,10, 40, lnContador, lnNumRegistros);

                                    maSaldos.Add(saldo);
                                    break;
                                
                                case 2:
                                    Vencim vencim = new Vencim
                                    {
                                        Ticket = dr.GetString(0),
                                        Agencia = dr.GetString(1),
                                        NumCuenta = dr.GetString(2),
                                        SufijoCuenta = dr.GetString(3),
                                        Saldo = dr.GetFloat(4),
                                        FechaVen = DateTime.Parse(dr.GetString(5)),
                                        NumRegistros = dr.GetInt32(6),
                                        Intereses = dr.GetFloat(7),
                                        FinRegistro = dr.GetString(8)
                                    };

                                    lnContador++;
                                    txtStatusInterfaz.Text = $"Recibiendo Vencimientos HO. ({lnContador} Registros)";
                                    ActualizaProgreso(ref pgbrCargaVencimientos, 10, 40, lnContador, lnNumRegistros);

                                    maVencimientos.Add(vencim);
                                    break;
                            }

                            if(lnContador > 0)
                            {
                                //Ajusta el conteo por el ultimo registro
                                lnContador --;
                            }

                            gsFechaArchivo = Funcion.InvierteFecha(txtFechaProceso.Text, false);

                            //Se Analiza la estructura
                            switch (TipoInfo)
                            {
                                case 1:
                                    txtStatusInterfaz.Text = "Verificando integridad de los saldos...";
                                    if(IntegrityFile(maSaldos, maVencimientos, main.Fecha_Int, TipoInfo, lnContador))
                                    {

                                    }
                                    break;
                                case 2:
                                    txtStatusInterfaz.Text = "Verificando integridad de los vencimientos...";
                                    break;
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

        private bool IntegrityFile(List<Saldos> laSaldos, List<Vencim> laVencim, string Fecha, int TipoReg, int NumReg)
        {
            bool IntegrityFile; 
            int X;
            int noRegistros;
            int noRegistrosIni;
            string deal_reference;
            string agencia;
            string cuenta_cliente;
            string sufijo;
            string saldo;
            string interes;
            string cuenta_bloqueada;
            string fecha_generacion;
            string numero_registros;
            string fin_registro;

            try
            {
                IntegrityFile = false;
                noRegistros = 0;
                X = 0;
                
                switch (TipoReg)
                {
                    case 1:
                        Message("Revisando Información de saldos");

                        if(!RevisaArchivo(laSaldos, laVencim, 1, NumReg))
                        {
                            if(!ActValorTransfer(3, 1))
                            {
                                Message("Error al actualizar transfer_saldos_ho = 3");
                            }
                            return false;
                        }
                        Message("Revisando Integridad de la información de saldos");

                        do
                        {
                            //Revisa unicamente la primer linea del archivo para verificar si es DUMMY
                            if (X == 0)
                            {
                                if (ArmaRegistro(laSaldos, laVencim, 1).Trim().ToUpper().Contains("DUMMY"))
                                {
                                    Message("Información Vencimientos es DUMMY");

                                    if (!ActValorTransfer(5, 2))
                                    {
                                        Message("Error al actualizar trans_venc_ho = 5");
                                    }
                                    //GoTo FileDummy
                                }
                                X++;
                            }

                            //Agencia
                            agencia = laSaldos[noRegistros].Agencia;
                            if (agencia.Trim() == "" || !IsNumeric(agencia))
                            {
                                return false;
                            }
                            //Cuenta Cliente
                            cuenta_cliente = laSaldos[noRegistros].Agencia.Trim();

                            if (cuenta_cliente == "" || !IsNumeric(cuenta_cliente))
                            {
                                return false;
                            }
                            //Sufijo Cuenta
                            sufijo = laSaldos[noRegistros].SufijoCuenta.Trim();
                            if (sufijo == "" || !IsNumeric(sufijo))
                            {
                                return false;
                            }
                            //Saldo
                            saldo = laSaldos[noRegistros].SaldoIniDia.ToString();
                            saldo = saldo.Replace(" -.", "-0."); // Línea que corrige el problema de los números negativos
                            if (saldo == "" || (!IsNumeric(Funcion.Mid(saldo, 1, 14))) && Funcion.Mid(saldo, 1, 14).Trim() != "" || !IsNumeric(Funcion.Mid(saldo, 16, 2)) || Funcion.Mid(saldo, 15, 1) != ".")
                            {
                                return false;
                            }
                            //Interes
                            interes = laSaldos[noRegistros].InteresDia.ToString();
                            interes = interes.Replace(" -.", "-0.");
                            if (interes.Trim() == "" || !IsNumeric(Funcion.Mid(interes, 1, 14)) && Funcion.Mid(interes, 1, 14) != "" || !IsNumeric(Funcion.Mid(interes, 16, 2)) || Funcion.Mid(interes, 15, 1) != ".")
                            {
                                return false;
                            }
                            //Cuenta bloqueada
                            cuenta_bloqueada = laSaldos[noRegistros].SwitchBloqueo.Trim();
                            if (cuenta_bloqueada == "" || cuenta_bloqueada != "Y" && cuenta_bloqueada != "N")
                            {
                                return false;
                            }
                            //Fecha Agencia
                            fecha_generacion = laSaldos[noRegistros].FechaGenSaldo.ToString().Trim();
                            if (fecha_generacion == "")
                            {
                                return false;
                            }
                            double diferencia_fechas = (DateTime.Parse(Fecha) - DateTime.Parse(fecha_generacion)).TotalDays;
                            if (diferencia_fechas != 0)
                            {
                                Message("La información de saldos contiene Diferencias en Fechas");
                                if(!ActValorTransfer(2,1))
                                {
                                    Message("Error al actualizar el valor de trasfer_saldos_ho = 2");
                                }
                                return false;
                            }

                            //Numero de registros
                            numero_registros = laSaldos[noRegistros].NumRegistros.ToString();
                            if(numero_registros == "" || !IsNumeric(numero_registros))
                            {
                                return false;
                            }
                            if(noRegistros == 0)
                            {
                                noRegistros = laSaldos[noRegistros].NumRegistros;
                            }
                            //Fin de registros
                            fin_registro = Funcion.Right(laSaldos[noRegistros].FinRegistro, 1);
                            if(fin_registro.Trim() != "*")
                            {
                                return false;
                            }
                            ActualizaProgreso(ref pgbrCargaSaldos, 50, 70, noRegistros, NumReg);
                            noRegistros++;

                        } while (noRegistros <= NumReg);

                        break;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return false;
        }

        private string ArmaRegistro(List<Saldos> paSaldos, List<Vencim> paVenc, int tipoRegistro)
        {
            string ArmaRegistro = "";
            switch (tipoRegistro)
            {
                case 1:
                    ArmaRegistro += paSaldos[0].Agencia + " " + paSaldos[0].NumCuenta;
                    ArmaRegistro += " " + paSaldos[0].SufijoCuenta + " " + paSaldos[0].SaldoIniDia;
                    ArmaRegistro += " " + paSaldos[0].InteresDia + " " + paSaldos[0].SwitchBloqueo;
                    ArmaRegistro += " " + paSaldos[0].FechaGenSaldo + " " + paSaldos[0].NumRegistros;
                    ArmaRegistro += " " + paSaldos[0].FinRegistro;
                break;
                case 2:
                    ArmaRegistro = paVenc[0].Ticket + " " + paVenc[0].Agencia;
                    ArmaRegistro += " " + paVenc[0].Agencia + " " + paVenc[0].NumCuenta;
                    ArmaRegistro += " " + paVenc[0].SufijoCuenta + " " + paVenc[0].Saldo;
                    ArmaRegistro += " " + paVenc[0].FechaVen + " " + paVenc[0].NumRegistros;
                    ArmaRegistro += " " + paVenc[0].Intereses + " " + paVenc[0].FinRegistro;
                break;

            }

            return ArmaRegistro;
        }

        private bool RevisaArchivo(List<Saldos> revisaSaldo, List<Vencim> revisaVenc, int tipo, int RegExist)
        {
            int indice = 0;
            do
            {
                if(tipo == 2)
                {
                    if(!FiltraCadena(revisaVenc[indice].Ticket) || 
                        !FiltraCadena(revisaVenc[indice].Agencia) ||
                        !FiltraCadena(revisaVenc[indice].NumCuenta) || 
                        !FiltraCadena(revisaVenc[indice].SufijoCuenta) || 
                        !FiltraCadena(revisaVenc[indice].FechaVen.ToString()) || 
                        !FiltraCadena(revisaVenc[indice].FinRegistro))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!FiltraCadena(revisaSaldo[indice].Agencia) ||
                       !FiltraCadena(revisaSaldo[indice].NumCuenta) ||
                       !FiltraCadena(revisaSaldo[indice].SufijoCuenta) ||
                       !FiltraCadena(revisaSaldo[indice].SwitchBloqueo) ||
                       !FiltraCadena(revisaSaldo[indice].FechaGenSaldo.ToString()))
                    {
                        return false;
                    }
                }

                indice++;

            } while (indice <= RegExist);

            return true;

        }

        private bool FiltraCadena(string cadena)
        {
            bool FiltraCadena = false; 

            string lsNueva;
            string lsvalidos;
            int lnCaracter;
            try
            {
                if(cadena.Trim() == "")
                {
                    FiltraCadena = false;
                    return FiltraCadena;
                }

                FiltraCadena = new Regex(@"^[a-zA-Z0-9.,-/()]+$").Match(cadena).Success;
                
                if(!FiltraCadena)
                {
                    Message("El archivo contiene caracteres Inválidos");
                }

                return FiltraCadena;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return FiltraCadena;
            }
        }

        private void ActualizaProgreso(ref ProgressBar progBar, int PorcIniBar, int PorcMaxBar, int NumIteracion, int TotalItercion)
        {
            int x, xiter;

            if(TotalItercion > 0 && PorcMaxBar > 0 && NumIteracion > 0)
            {
                xiter = ((NumIteracion * 100) / TotalItercion);

                if(xiter > 0)
                {
                    if(PorcIniBar < 0)
                    {
                        return;
                    }
                    x = (PorcMaxBar - PorcIniBar) / (100) * xiter + PorcIniBar;

                    if(x > PorcIniBar && x <= PorcMaxBar && progBar.Value < x)
                    {
                        progBar.Value = x;
                    }
                }
            }

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

        public static bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }
    }
}
