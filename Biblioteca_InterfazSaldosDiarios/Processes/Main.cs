using Biblioteca_InterfazSaldosDiarios.Data;
using Biblioteca_InterfazSaldosDiarios.Helpers;
using Biblioteca_InterfazSaldosDiarios.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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


        //Funciones Bibliotecas
        Encriptacion encriptacion;
        FuncionesBD bd;
        ConexionAS400 as400;

        //Elementos Formulario
        TextBox txtConsola;
        ProgressBar pgbrCargaSaldos;
        ProgressBar pgbrCargaVencimientos;
        TextBox txtStatusInterfaz;
        Label lblNumSaldosHO;
        Label lblNumVencimHO;
        TextBox txtArchivoSaldos;
        TextBox txtArchivoVencimiento;
        TextBox txtFechaArchivos;

        public string Ls_fechaoperacion { get => ls_fechaoperacion; set => ls_fechaoperacion = value; }
        public byte MnFirstTime { get => mnFirstTime; set => mnFirstTime = value; }

        public Main(ref TextBox txtConsola, ref ProgressBar pgbrCargaSaldos, ref ProgressBar pgbrCargaVencimientos, ref TextBox txtStatusInterfaz, ref Label lblNumSaldosHO, ref Label lblNumVencimHO, ref TextBox txtArchivoSaldos, ref TextBox txtArchivoVencimiento, ref TextBox txtFechaArchivos)
        {
            this.txtConsola = txtConsola;
            this.pgbrCargaSaldos = pgbrCargaSaldos;
            this.pgbrCargaVencimientos = pgbrCargaVencimientos;
            this.txtStatusInterfaz = txtStatusInterfaz;
            this.lblNumSaldosHO = lblNumSaldosHO;
            this.lblNumVencimHO = lblNumVencimHO;
            this.txtArchivoSaldos = txtArchivoSaldos;
            this.txtArchivoVencimiento = txtArchivoVencimiento;         
            this.txtFechaArchivos = txtFechaArchivos;

            encriptacion = new Encriptacion();
        }

        public void Init(bool fecha_actual, string fecha = "")
        {
            if (EstableceConexionBD() == true && as400.Conectar() == true)
            {
                if (!fecha_actual)
                {
                    Ls_fechaoperacion = fecha;
                }
                else
                {
                    Ls_fechaoperacion = bd.obtenerFechaServidor();
                }

                var reg = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[-]([0]?[1-9]|[1][0-2])[-]([0-9]{4}|[0-9]{2})$").Match(Ls_fechaoperacion).Success;

                if (!reg)
                {
                    Message("El formato de la fecha de proceso esta mal. El formato correcto es dd-MM-yyyy");
                    return;
                }

                if (msDSN400 == "" || msUser400 == "" || msPswd400 == "" || msLibAS400 == "")
                {
                    Message("Verifique el archivo de inicio. Faltan datos para la conexion con AS400");
                    return;
                }

                if (msDBSrvr == "" || msDBuser == "" || msDBPswd == "" || msDBName == "")
                {
                    Message("Verifique el archivo de inicio. Faltan datos para la conexion con con SQL Server");
                    return;
                }

                MnFirstTime = 1;

                Message("Conectando...");

           
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

        public void EstableceParametros()
        {
            try
            {
                msDBName = encriptacion.Decrypt(Funcion.getValueAppConfig("Nombre", "BD"));
                msDBSrvr = encriptacion.Decrypt(Funcion.getValueAppConfig("Servidor", "BD"));
                msDBuser = encriptacion.Decrypt(Funcion.getValueAppConfig("Usuario", "BD"));
                msDBPswd = encriptacion.Decrypt(Funcion.getValueAppConfig("Password", "BD"));

                msDSN400 = encriptacion.Decrypt(Funcion.getValueAppConfig("DSN", "AS400"));
                msLibAS400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Biblioteca", "AS400"));
                msUser400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Usuario", "AS400"));
                msPswd400 = encriptacion.Decrypt(Funcion.getValueAppConfig("Password", "AS400"));

                FileSaldoHO = encriptacion.Decrypt(Funcion.getValueAppConfig("SaldosHouston", "ARCHIVO"));
                FileVenciHO = encriptacion.Decrypt(Funcion.getValueAppConfig("VencimientoHouston", "ARCHIVO"));

                as400 = new ConexionAS400(msDSN400, msUser400, msPswd400);

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }
        }

        public bool EstableceConexionBD()
        {
            int lnIntento = 1;
            bool EstableceConexion = false;


            try
            {
                string conn_str = $"Data source ={msDBSrvr}; uid ={msDBuser}; PWD ={msDBPswd}; initial catalog = {msDBName}";

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

        public bool CalculaFechaArchivos()
        {
            bool CalculaFechaArchivos = false;
            string ls_FechaDiaActual;
            string lsQuery;
            string TipoFecha;
            int DiasAgregados;
            int tempDiasFeriados;
            bool lnEsDíaFeriado;

            try
            {
                //Obtiene la fecha del servidor SQL
                lsQuery = "SELECT CONVERT(VARCHAR,DATEPART(dd, getdate()),2)+'/'+CONVERT(VARCHAR,DATEPART(month, getdate()),2)+'/'+CONVERT(VARCHAR,DATEPART(yy, getdate()),2)";
                SqlDataReader dr = bd.ejecutarConsulta(lsQuery);

                List<Map> maps = new List<Map>();
                maps.Add(new Map { Key = "Fecha_Actual", Type = "string" });
                maps = bd.LLenarMapToQuery(maps, dr);


                ls_FechaDiaActual = Funcion.InvierteFecha(maps[0].GetString(), false);
                Fecha_Int = DateTime.Parse(ls_fechaoperacion).ToString("MM-dd-yyyy");

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

                    lnEsDíaFeriado = (maps.Count > 0);

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

        public void RecibeArchivos()
        {
            //Saldos
            ActualizaProgreso(ref pgbrCargaSaldos, 5, 1, 1, 0);
            if (mnBanderaSHO == 0)
            {
                if (ProcesaInfo(1))
                {
                    Message("Carga de saldos completa...");

                    if (!ActValorTransfer(1, 1))
                    {
                        Message("Error al actualizar la bandera transfer_saldos_ho = 1");
                    }
                }
            }
            else
            {
                Message("Carga de saldos relizada previamente...");
            }
            ActualizaProgreso(ref pgbrCargaSaldos, 100, 1, 1, 0);

            //Vencimientos
            ActualizaProgreso(ref pgbrCargaVencimientos, 5, 1, 1, 0);
            if (mnBanderaVHO == 0)
            {
                if (ProcesaInfo(2))
                {
                    Message("Carga de vencimientos completa...");
                    if (!ActValorTransfer(1, 2))
                    {
                        Message("Error al actualizar la bandera transfer_venc_ho = 1");
                    }
                }
            }
            else
            {
                Message("Error al actualizar la bandera transfer_venc_ho = 1");
            }
            ActualizaProgreso(ref pgbrCargaVencimientos, 100, 1, 1, 0);

            Message("DECONECTADO");
        }

        private void ActualizaProgreso(ref ProgressBar progBar, int PorcMaxBar, int NumIteracion, int TotalItercion, int porc)
        {
            double valor_actual = progBar.Value;
            double porcentaje_relativo = (NumIteracion * PorcMaxBar) / TotalItercion;
            progBar.Value = porc + (int)porcentaje_relativo;
        }

        public void Message(string mensaje)
        {
            txtConsola.AppendText("$ " + mensaje);
            txtConsola.AppendText(Environment.NewLine);
        }

        private bool ProcesaInfo(int TipoInfo)
        {
            //TipoInfo = 2;
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

                msSQL400 = $"Select COUNT(*) FROM {msLibAS400}.{lsArchivoAS400}";

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
                        ActualizaProgreso(ref pgbrCargaSaldos, 10, 1, 1, 5);
                        break;
                    case 2:
                        lblNumVencimHO.Text = "Registros a procesar : " + lnNumRegistros;
                        ActualizaProgreso(ref pgbrCargaVencimientos, 10, 1, 1, 5);
                        break;
                }

                if (lnNumRegistros > 0)
                {
                    switch (TipoInfo)
                    {
                        case 1:
                            msSQL400 = $"Select SDAB, SDAN, SDAS, SDBAL, SDINT , SDAI17, SDDTE1, SDNREG, SDFIN FROM {msLibAS400}.{lsArchivoAS400}";
                            break;
                        case 2:
                            msSQL400 = $"Select CDDLR, CDAB, CDAN, CDAS, CDBAL, CDDTE, CDREG, CDINT, CDFIN FROM {msLibAS400}.{lsArchivoAS400}";
                            break;
                    }


                    //Busca los registros por procesar
                    dr = as400.EjecutaSelect(msSQL400);


                    //si hay registros de la consulta...
                    if (dr.HasRows)
                    {
                        maSaldos = new List<Saldos>();
                        maVencimientos = new List<Vencim>();

                        while (dr.Read())
                        {
                            //if (Funcion.Left(dr.GetString(1) + Funcion.Space(6), 6).Contains("225300"))
                            //{
                            //    bool encontrado = true;
                            //    var saldo = dr.GetDecimal(3);
                            //}
                            switch (TipoInfo)
                            {
                                case 1:
                                    Saldos saldo = new Saldos();
                                    saldo.Agencia = Funcion.Left(dr.GetString(0) + Funcion.Space(4), 4);
                                    saldo.NumCuenta = Funcion.Left(dr.GetString(1) + Funcion.Space(6), 6);
                                    saldo.SufijoCuenta = Funcion.Left(dr.GetString(2) + Funcion.Space(3), 3);
                                    saldo.SaldoIniDia = dr.GetDecimal(3);
                                    saldo.InteresDia = dr.GetInt32(4);
                                    saldo.SwitchBloqueo = dr.GetString(5);
                                    saldo.FechaGenSaldo = DateTime.Parse(Funcion.InvierteFecha(dr.GetString(6), false));
                                    saldo.NumRegistros = dr.GetInt32(7);
                                    saldo.FinRegistro = dr.GetString(8);

                                    lnContador++;
                                    txtStatusInterfaz.Text = $"Recibiendo Saldos HO. ({lnContador} Registros)";
                                    ActualizaProgreso(ref pgbrCargaSaldos, 35, lnContador, lnNumRegistros, 15);

                                    maSaldos.Add(saldo);
                                    break;

                                case 2:
                                    Vencim vencim = new Vencim();
                                    vencim.Ticket = dr.GetString(0);
                                    vencim.Agencia = dr.GetString(1);
                                    vencim.NumCuenta = dr.GetString(2);
                                    vencim.SufijoCuenta = dr.GetString(3);
                                    vencim.Saldo = dr.GetDecimal(4);
                                    vencim.FechaVen = DateTime.Parse(Funcion.InvierteFecha(dr.GetString(5), false));
                                    vencim.NumRegistros = dr.GetInt32(6);
                                    vencim.Intereses = dr.GetFloat(7);
                                    vencim.FinRegistro = dr.GetString(8);

                                    lnContador++;
                                    txtStatusInterfaz.Text = $"Recibiendo Vencimientos HO. ({lnContador} Registros)";
                                    ActualizaProgreso(ref pgbrCargaVencimientos, 35, lnContador, lnNumRegistros, 15);

                                    maVencimientos.Add(vencim);
                                    break;
                            }
                        }

                        if (lnContador > 0)
                        {
                            //Ajusta el conteo por el ultimo registro
                            lnContador--;
                        }

                        gsFechaArchivo = Funcion.InvierteFecha(Ls_fechaoperacion, false);

                        //Se Analiza la estructura
                        switch (TipoInfo)
                        {

                            case 1:
                                txtStatusInterfaz.Text = "Verificando integridad de los saldos...";
                                if (IntegrityFile(maSaldos, maVencimientos, Funcion.InvierteFecha(Fecha_Int, false), TipoInfo, lnContador))
                                {
                                    if (!LimpiaTablasSV(TipoInfo))
                                    {
                                        ErrorRecepcion();
                                    }
                                    pgbrCargaSaldos.Value = 70;
                                    if (!CargaArchivosHO(maSaldos, maVencimientos, TipoInfo, lnContador))
                                    {
                                        ErrorRecepcion();
                                    }
                                }
                                else
                                {
                                    lblNumSaldosHO.Text = ("Registros procesados : 0");
                                    Message("Error en la integridad de los saldos");
                                    return ErrorProceso();
                                }
                                break;
                            case 2:
                                txtStatusInterfaz.Text = "Verificando integridad de los vencimientos...";

                                if (IntegrityFile(maSaldos, maVencimientos, Funcion.InvierteFecha(gsFechaArchivo, false), TipoInfo, lnContador))
                                {
                                    if (!LimpiaTablasSV(TipoInfo))
                                    {
                                        ErrorRecepcion();
                                    }
                                    pgbrCargaVencimientos.Value = 70;
                                    if (!CargaArchivosHO(maSaldos, maVencimientos, TipoInfo, lnContador))
                                    {
                                        ErrorRecepcion();
                                    }
                                }
                                else
                                {
                                    lblNumVencimHO.Text = "Registros procesados : 0";
                                    Message("Error en la integridad de los vencimientos");
                                    return ErrorProceso();
                                }
                                break;
                        }

                    }
                    else
                    {
                        ErrorProceso();
                    }
                }
                ProcesaInfo = true;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
            }

            return ProcesaInfo;
        }

        private bool LimpiaTablasSV(int tipo)
        {
            bool LimpiaTablasSV = false;
            string lsQuery = "";
            int afectados = 0;
            try
            {
                switch (tipo)
                {
                    case 1:
                        Message("Limpiando Tabla de Saldos...");
                        lsQuery = "DELETE FROM TICKET.Transferencia.SALDOS_KAPITI WHERE 1 = 1";
                        afectados = bd.ejecutarDelete(lsQuery);
                        Message("SALDOS_KAPITI Limpia.");
                        lsQuery = "DELETE FROM TICKET.Transferencia.SALDOS_KAPITI_1 WHERE 1 = 1";
                        afectados = bd.ejecutarDelete(lsQuery);
                        Message("SALDOS_KAPITI_1 Limpia.");
                        LimpiaTablasSV = true;
                        break;

                    case 2:
                        Message("Limpiando Tabla de Vencimientos...");
                        lsQuery = "DELETE FROM TICKET.Transferencia.SALDOS_CD_KAPITI WHERE 1 = 1";
                        afectados = bd.ejecutarDelete(lsQuery);
                        Message("SALDOS_CD_KAPITI Limpia.");
                        lsQuery = "DELETE FROM TICKET.Transferencia.SALDOS_CD_KAPITI_1 WHERE 1 = 1";
                        afectados = bd.ejecutarDelete(lsQuery);
                        Message("SALDOS_CD_KAPITI_1 Limpia.");
                        LimpiaTablasSV = true;
                        break;

                }

                return LimpiaTablasSV;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return LimpiaTablasSV;
            }
        }

        public static bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        private bool IntegrityFile(List<Saldos> laSaldos, List<Vencim> laVencim, string Fecha, int TipoReg, int NumReg)
        {
            bool IntegrityFile;
            int X;
            int noRegistros;
            int noRegistrosIni = 0;
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

                        if (!RevisaArchivo(laSaldos, laVencim, 1, NumReg))
                        {
                            if (!ActValorTransfer(3, 1))
                            {
                                Message("Error al actualizar transfer_saldos_ho = 3");
                            }
                            return IntegrityFile;
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
                                    return InfoDummy();
                                }
                                X++;
                            }

                            //Agencia
                            agencia = laSaldos[noRegistros].Agencia;
                            if (agencia.Trim() == "" || !IsNumeric(agencia))
                            {
                                return IntegrityFile;
                            }
                            //Cuenta Cliente                          
                            cuenta_cliente = laSaldos[noRegistros].NumCuenta.Trim();
                            //if (cuenta_cliente.Contains("225300"))
                            //{
                            //    bool bandera = true;
                            //}

                            if (cuenta_cliente == "" || !IsNumeric(cuenta_cliente))
                            {
                                return IntegrityFile;
                            }
                            //Sufijo Cuenta
                            sufijo = laSaldos[noRegistros].SufijoCuenta.Trim();
                            if (sufijo == "" || !IsNumeric(sufijo))
                            {
                                return IntegrityFile;
                            }
                            //Saldo
                            saldo = laSaldos[noRegistros].SaldoIniDia.ToString();
                            saldo = saldo.Replace(" -.", "-0."); // Línea que corrige el problema de los números negativos
                            //if (saldo == "" || (!IsNumeric(Funcion.Mid(saldo, 1, 14)) && Funcion.Mid(saldo, 1, 14).Trim() != "") || !IsNumeric(Funcion.Mid(saldo, 16, 2)) || Funcion.Mid(saldo, 15, 1) != ".")
                            //{
                            //    return IntegrityFile;
                            //}
                            //Interes
                            interes = laSaldos[noRegistros].InteresDia.ToString();
                            interes = interes.Replace(" -.", "-0.");
                            //if (interes.Trim() == "" || (!IsNumeric(Funcion.Mid(interes, 1, 14)) && Funcion.Mid(interes, 1, 14) != "") || !IsNumeric(Funcion.Mid(interes, 16, 2)) || Funcion.Mid(interes, 15, 1) != ".")
                            //{
                            //    return IntegrityFile;
                            //}
                            //Cuenta bloqueada
                            cuenta_bloqueada = laSaldos[noRegistros].SwitchBloqueo.Trim();
                            if (cuenta_bloqueada == "" || (cuenta_bloqueada != "Y" && cuenta_bloqueada != "N"))
                            {
                                return IntegrityFile;
                            }
                            //Fecha Agencia
                            fecha_generacion = laSaldos[noRegistros].FechaGenSaldo.ToString().Trim();
                            if (fecha_generacion == "")
                            {
                                return IntegrityFile;
                            }
                            double diferencia_fechas = (DateTime.Parse(Fecha) - DateTime.Parse(fecha_generacion)).TotalDays;
                            if (diferencia_fechas != 0)
                            {
                                Message("La información de saldos contiene Diferencias en Fechas");
                                if (!ActValorTransfer(2, 1))
                                {
                                    Message("Error al actualizar el valor de trasfer_saldos_ho = 2");
                                }
                                return IntegrityFile;
                            }

                            //Numero de registros
                            numero_registros = laSaldos[noRegistros].NumRegistros.ToString();
                            if (numero_registros == "" || !IsNumeric(numero_registros))
                            {
                                return IntegrityFile;
                            }
                            if (noRegistros == 0)
                            {
                                noRegistrosIni = laSaldos[noRegistros].NumRegistros;
                            }
                            //Fin de registros
                            fin_registro = Funcion.Right(laSaldos[noRegistros].FinRegistro, 1);
                            if (fin_registro.Trim() != "*")
                            {
                                return IntegrityFile;
                            }
                            ActualizaProgreso(ref pgbrCargaSaldos, 10, noRegistros, NumReg, 50);
                            noRegistros++;

                        } while (noRegistros <= NumReg);

                        if (noRegistros != noRegistrosIni)
                        {
                            Message("Numero de registros incorrecto...");
                            if (!ActValorTransfer(4, 1))
                            {
                                Message("Error al actualizar del parametro trans_saldos_ho = 4");
                            }
                            return IntegrityFile;
                        }
                        break;
                    case 2:
                        Message("Revisando Información de vencimientos");
                        if (!RevisaArchivo(laSaldos, laVencim, 2, NumReg))
                        {
                            if (!ActValorTransfer(3, 2))
                            {
                                Message("Error al actualizar transfer_venc_ho = 3");
                            }
                        }
                        Message("Revisando Integridad de la información de vencimientos");

                        do
                        {
                            //Revisa unicamente la primer linea del archivo para verificar si es DUMMY
                            if (X == 0)
                            {
                                if (ArmaRegistro(laSaldos, laVencim, 2).Trim().ToUpper().Contains("DUMMY"))
                                {
                                    Message("Información Vencimientos es DUMMY");
                                    if (!ActValorTransfer(5, 2))
                                    {
                                        Message("Error al actualizar trans_venc_ho = 5");
                                    }
                                    return InfoDummy();
                                }
                                X++;
                            }

                            //Ticket
                            deal_reference = laVencim[noRegistros].Ticket.Trim();
                            if (deal_reference == "")
                            {
                                return IntegrityFile;
                            }
                            //Agencia
                            agencia = laVencim[noRegistros].Agencia.Trim();
                            if (agencia == "" || !IsNumeric(agencia))
                            {
                                return IntegrityFile;
                            }

                            //Cuenta cliente
                            cuenta_cliente = laVencim[noRegistros].NumCuenta.Trim();
                            if (cuenta_cliente == "" || !IsNumeric(cuenta_cliente))
                            {
                                return IntegrityFile;
                            }

                            //Sufijo Cuenta
                            sufijo = laVencim[noRegistros].SufijoCuenta.Trim();
                            if (sufijo == "" || !IsNumeric(sufijo))
                            {
                                return IntegrityFile;
                            }

                            ////Saldo
                            saldo = laVencim[noRegistros].Saldo.ToString();
                            saldo = saldo.Replace(" -.", "-0."); //Línea que corrige el problema de los números negativos

                            //if(saldo == "" || (!IsNumeric(Funcion.Mid(saldo, 1, 14)) && Funcion.Mid(saldo, 1, 14) != "") || !IsNumeric(Funcion.Mid(saldo, 16, 2)) || Funcion.Mid(saldo, 15, 1) != ".")
                            //{
                            //    return IntegrityFile;
                            //}

                            // Fecha Generacion
                            fecha_generacion = laVencim[noRegistros].FechaVen.ToString();
                            if (fecha_generacion == "")
                            {
                                return IntegrityFile;
                            }
                            double diferencia_fechas = (DateTime.Parse(Fecha) - DateTime.Parse(fecha_generacion)).TotalDays;
                            if (diferencia_fechas != 0)
                            {
                                Message("La información de vencimientos contiene Diferencias en Fechas");
                                if (!ActValorTransfer(2, 2))
                                {
                                    Message("Error al actualizar el valor de trasfer_venc_ho = 2");
                                }
                                return IntegrityFile;
                            }

                            //Numero de Registros
                            numero_registros = laVencim[noRegistros].NumRegistros.ToString();
                            if (numero_registros == "" || !IsNumeric(numero_registros))
                            {
                                return IntegrityFile;
                            }

                            if (noRegistros == 0)
                            {
                                noRegistrosIni = laVencim[noRegistros].NumRegistros;
                            }

                            //Intereses
                            interes = laVencim[noRegistros].Intereses.ToString();
                            //interes = interes.Replace(" -.", "-0."); //Línea que corrige el problema de los números negativos
                            //if(interes == "" || (!IsNumeric(Funcion.Mid(interes,1,14)) && Funcion.Mid(interes, 1, 14) != "") || !IsNumeric(Funcion.Mid(interes, 16, 2)) || Funcion.Mid(interes, 15, 1) != ".")
                            //{
                            //    return IntegrityFile;
                            //}
                            // Fin de registros
                            fin_registro = laVencim[noRegistros].FinRegistro.Trim();
                            if (fin_registro != "*")
                            {
                                return IntegrityFile;
                            }

                            ActualizaProgreso(ref pgbrCargaVencimientos, 10, noRegistros, NumReg, 50);
                            noRegistros++;

                        } while (noRegistros <= NumReg);

                        if (noRegistros != noRegistrosIni)
                        {
                            Message("Numero de registros incorrecto...");
                            if (!ActValorTransfer(4, 2))
                            {
                                Message("Error al actualizar del parametro trans_venc_ho = 4");
                            }
                            return IntegrityFile;
                        }

                        break;
                }
                IntegrityFile = true;
                return IntegrityFile;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                Message("Error al verificar el integridad del archivo ");
                return false;
            }
        }

        private bool RevisaArchivo(List<Saldos> revisaSaldo, List<Vencim> revisaVenc, int tipo, int RegExist)
        {
            int indice = 0;
            do
            {
                if (tipo == 2)
                {
                    if (!FiltraCadena(revisaVenc[indice].Ticket) ||
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
                if (cadena.Trim() == "")
                {
                    FiltraCadena = false;
                    return FiltraCadena;
                }

                FiltraCadena = new Regex(@"^[a-zA-Z0-9., -/:()]+$").Match(cadena).Success;

                if (!FiltraCadena)
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
        private bool InfoDummy()
        {
            Message("La información es Dummy...");
            return true;
        }

        private bool CargaArchivosHO(List<Saldos> cargaSaldo, List<Vencim> cargaVenc, int tipo, int NumRegistros)
        {
            int lnIndice = 0;
            string lsQuery;
            bool CargaArchivosHO = false;
            List<QueryParametro> querys = new List<QueryParametro>();

            try
            {
                if (tipo == 1)
                {
                    if (ArchivoDummy(cargaSaldo, cargaVenc, "FINTLA20", 1, "8000"))
                    {
                        return FileDummy();
                    }
                    lnIndice = 0;
                    Message("Cargando Archivo de Saldos de Houston...");


                    do
                    {
                        lsQuery = "INSERT INTO TICKET.Transferencia.SALDOS_KAPITI_1(agencia, cuenta_cliente, sufijo, saldo, interes, cuenta_bloqueada, fecha_generacion, numero_registros, fin_registro) VALUES (@agencia, @cuenta_cliente, @sufijo, @saldo, @interes, @cuenta_bloqueada, @fecha_generacion, @numero_registros, @fin_registro);";

                        SqlParameter[] parameters = new SqlParameter[] {
                                new SqlParameter("@agencia",SqlDbType.VarChar, 4) { Value = cargaSaldo[lnIndice].Agencia.ToString() },
                                new SqlParameter("@cuenta_cliente",SqlDbType.VarChar, 6) { Value = cargaSaldo[lnIndice].NumCuenta.ToString() },
                                new SqlParameter("@sufijo",SqlDbType.VarChar, 3) { Value = cargaSaldo[lnIndice].SufijoCuenta.ToString()},
                                new SqlParameter("@saldo",SqlDbType.VarChar, 17) { Value = cargaSaldo[lnIndice].SaldoIniDia.ToString() },
                                new SqlParameter("@interes",SqlDbType.VarChar, 11) { Value = cargaSaldo[lnIndice].InteresDia.ToString() },
                                new SqlParameter("@cuenta_bloqueada",SqlDbType.VarChar, 1) { Value = cargaSaldo[lnIndice].SwitchBloqueo.ToString() },
                                new SqlParameter("@fecha_generacion",SqlDbType.VarChar, 10) { Value = cargaSaldo[lnIndice].FechaGenSaldo.ToString("MM-dd-yyyy") },
                                new SqlParameter("@numero_registros",SqlDbType.VarChar, 8) { Value = cargaSaldo[lnIndice].NumRegistros.ToString()},
                                new SqlParameter("@fin_registro",SqlDbType.VarChar, 1) { Value = cargaSaldo[lnIndice].FinRegistro.ToString() },
                        };

                        querys.Add(new QueryParametro { Query = lsQuery, Parametros = parameters });
                        Message($"Añadiendo el saldo {lnIndice}  a la cola para cargar a la base de datos");
                        lnIndice++;

                    } while (lnIndice <= NumRegistros);

                    txtStatusInterfaz.Text = "Cargando Saldos a la base de datos";
                    CargaArchivosHO = (transaccionInsert(querys, ref pgbrCargaSaldos) != -1) ? true : false;

                    //Task.Run(async () =>
                    //{
                    //    CargaArchivosHO = (await Task.Run(() => bd.transaccionInsert(querys)) > 1)? true: false;
                    //}).GetAwaiter().GetResult();


                    lblNumSaldosHO.Text = $"Registros procesados: {lnIndice.ToString("00000")}";
                    Message("Carga de Saldos de Houston Terminada...");
                }
                else if (tipo == 2)
                {
                    if (ArchivoDummy(cargaSaldo, cargaVenc, "FINTLA21", 2, "8000"))
                    {
                        return FileDummy();
                    }
                    lnIndice = 0;
                    Message("Cargando Archivo de Vencimientos de Houston...");


                    do
                    {
                        lsQuery = "INSERT INTO TICKET.Transferencia.SALDOS_CD_KAPITI_1(deal_reference, agencia, cuenta_cliente, sufijo, saldo, fecha_generacion, numero_registros, intereses, fin_registro) VALUES (@deal_reference, @agencia, @cuenta_cliente, @sufijo, @saldo, @fecha_generacion, @numero_registros, @intereses, @fin_registro);";

                        SqlParameter[] parameters = new SqlParameter[] {
                                new SqlParameter("@deal_reference",SqlDbType.VarChar, 7) { Value = cargaVenc[lnIndice].Ticket.ToString() },
                                new SqlParameter("@agencia",SqlDbType.VarChar, 4) { Value = cargaVenc[lnIndice].Agencia.ToString() },
                                new SqlParameter("@cuenta_cliente",SqlDbType.VarChar, 6) { Value = cargaVenc[lnIndice].NumCuenta.ToString() },
                                new SqlParameter("@sufijo",SqlDbType.VarChar, 3) { Value = cargaVenc[lnIndice].SufijoCuenta.ToString() },
                                new SqlParameter("@saldo",SqlDbType.VarChar, 17) { Value = cargaVenc[lnIndice].Saldo.ToString() },
                                new SqlParameter("@fecha_generacion",SqlDbType.VarChar, 10) { Value = cargaVenc[lnIndice].FechaVen.ToString("MM-dd-yyyy") },
                                new SqlParameter("@numero_registros",SqlDbType.VarChar, 8) { Value = cargaVenc[lnIndice].NumRegistros.ToString() },
                                new SqlParameter("@intereses",SqlDbType.VarChar, 17) { Value = cargaVenc[lnIndice].Intereses.ToString() },
                                new SqlParameter("@fin_registro",SqlDbType.VarChar, 1) { Value = cargaVenc[lnIndice].FinRegistro.ToString() },
                            };

                        querys.Add(new QueryParametro { Query = lsQuery, Parametros = parameters });
                        Message($"Añadiendo el saldo {lnIndice}  a la cola para cargar a la base de datos");
                        lnIndice++;

                    } while (lnIndice <= NumRegistros);

                    txtStatusInterfaz.Text = "Cargando Vencimientos a la base de datos";
                    CargaArchivosHO = (transaccionInsert(querys, ref pgbrCargaVencimientos) != -1) ? true : false;

                    lblNumVencimHO.Text = $"Registros procesados: {lnIndice.ToString("00000")}";
                    Message("Carga de Vencimientos de Houston Terminada...");
                }
                return CargaArchivosHO;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return CargaArchivosHO;
            }
        }

        public int transaccionInsert(List<QueryParametro> querys, ref ProgressBar pgbr)
        {
            int registros_procesados = 0;
            using (SqlConnection connection = new SqlConnection(bd.connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                transaction = connection.BeginTransaction("transaccionInsert");

                command.Connection = connection;
                command.Transaction = transaction;


                try
                {

                    foreach (QueryParametro query in querys)
                    {
                        Message($"Cargando el saldo {registros_procesados}  a la base de datos");

                        command.Parameters.Clear();
                        command.CommandText = query.Query;
                        foreach (SqlParameter parametro in query.Parametros)
                        {
                            command.Parameters.Add(parametro);
                        }
                        registros_procesados++;
                        //if (registros_procesados > 3) break; //[pruebas]
                        command.ExecuteNonQuery();

                        ActualizaProgreso(ref pgbr, 30, registros_procesados, querys.Count, 70);


                    }

                    transaction.Commit();
                    Log.Escribe("records are written to database.");
                    return registros_procesados;
                }
                catch (Exception ex)
                {
                    Log.Escribe("Commit Exception");
                    Log.Escribe(ex);
                    try
                    {
                        transaction.Rollback();
                        return -1;
                    }
                    catch (Exception ex2)
                    {
                        Log.Escribe("Rollback Exception");
                        Log.Escribe(ex2);
                        return -1;
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

                ActValorTransfer = (resultado > 0) ? true : false;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                ActValorTransfer = true;
            }

            return ActValorTransfer;
        }

        private bool ArchivoDummy(List<Saldos> FileSaldo, List<Vencim> FileVenc, string Titulo, int tipo, string agencia)
        {
            bool lbDummy = false;
            string Ls_Query;

            try
            {
                switch (tipo)
                {
                    case 1:
                        if (ArmaRegistro(FileSaldo, FileVenc, 1).Trim().ToUpper().Trim().IndexOf("DUMMY") > 0)
                        {
                            lbDummy = true;
                        }
                        break;
                    case 2:
                        if (ArmaRegistro(FileSaldo, FileVenc, 2).Trim().ToUpper().Trim().IndexOf("DUMMY") > 0)
                        {
                            lbDummy = true;
                        }
                        break;
                }

                if (lbDummy)
                {

                    if (tipo == 1)  //Es archivo de Saldos
                    {
                        Ls_Query = "Insert into TICKET.Transferencia.SALDOS_KAPITI_1 (agencia, cuenta_cliente, sufijo, saldo, interes, cuenta_bloqueada, fecha_generacion, numero_registros, fin_registro) values (@agencia, @cuenta_cliente, @sufijo, @saldo, @interes, @cuenta_bloqueada, @fecha_generacion, @numero_registros, @fin_registro);";

                        OdbcParameter[] parameters = new OdbcParameter[] {
                            new OdbcParameter("@agencia", agencia),
                            new OdbcParameter("@cuenta_cliente", "DUMMY"),
                            new OdbcParameter("@sufijo", "0"),
                            new OdbcParameter("@saldo", "0"),
                            new OdbcParameter("@interes","0"),
                            new OdbcParameter("@cuenta_bloqueada", "0"),
                            new OdbcParameter("@fecha_generacion", Funcion.InvierteFecha(gsFechaArchivo, false)),
                            new OdbcParameter("@numero_registros", "0"),
                            new OdbcParameter("@fin_registro", "*"),
                        };

                        as400.EjecutaActualizacion(Ls_Query, parameters);
                    }
                    else //Es archivo de Vencimientos
                    {
                        Ls_Query = "Insert into TICKET.Transferencia.SALDOS_CD_KAPITI_1 (agencia, cuenta_cliente, sufijo, saldo, fecha_generacion, numero_registros, intereses, fin_registro) values (@agencia, @cuenta_cliente, @sufijo, @saldo, @fecha_generacion, @numero_registros, @intereses, @fin_registro);";

                        OdbcParameter[] parameters = new OdbcParameter[] {
                            new OdbcParameter("@agencia", agencia),
                            new OdbcParameter("@cuenta_cliente", "DUMMY"),
                            new OdbcParameter("@sufijo", "0"),
                            new OdbcParameter("@saldo", "0"),
                            new OdbcParameter("@fecha_generacion",Funcion.InvierteFecha(gsFechaArchivo, false)),
                            new OdbcParameter("@numero_registros", "0"),
                            new OdbcParameter("@intereses","0"),
                            new OdbcParameter("@fin_registro", "*"),
                        };

                        as400.EjecutaActualizacion(Ls_Query, parameters);
                    }


                }
                return lbDummy;
            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return lbDummy;
            }
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


        private bool ErrorProceso()
        {
            return false;
        }

        private void ErrorRecepcion()
        {
            Message("Interfaz de Movimientos de las Agencias");
            Message("Error Inesperado de Operación. Transferencia Terminada.");
        }

        private bool FileDummy()
        {
            Message("Carga de archivo Dummy");
            return true;
        }



    }
}
