using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using Biblioteca_InterfazSaldosDiarios.Helpers;
using Biblioteca_InterfazSaldosDiarios.Models;

namespace Biblioteca_InterfazSaldosDiarios.Data
{
    public class ConexionAS400
    {
        private string user;
        private string password;
        private string dsn;

        private OdbcConnection DbConnection;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dsn">nombre instancia ODBC</param>
        /// <param name="usuario">usuario ODBC</param>
        /// <param name="password">contrasena ODBC </param>
        public ConexionAS400(string dsn, string usuario, string password)
        {
            this.dsn = dsn;
            this.user = usuario;
            this.password = password;
        }

        /// <summary>
        /// Conectarse al AS400
        /// </summary>
        /// <returns>Verdadero si logra conectarse</returns>
        public bool Conectar()
        {
            try
            {
                string connection_str = $"DSN={this.dsn}; UID={this.user}; PWD={this.password};";
                
                if(DbConnection == null)
                {
                    DbConnection = new OdbcConnection(connection_str);
                }
                
                DbConnection.Open();
                return true;
            }
            catch (OdbcException ex)
            {
                Log.Escribe($"Fallo conexion a {this.dsn}", "Error");
                Log.Escribe(ex);
                return false;
            }
        }

        /// <summary>
        /// Ejecuta consulta a ODBC
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public OdbcDataReader EjecutaSelect(string query)
        {
            try
            {
                OdbcCommand DbCommand = DbConnection.CreateCommand();
                DbCommand.CommandText = query;
                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                return DbReader;
            }
            catch (OdbcException ex)
            {
                Log.Escribe($"Fallo consulta a {this.dsn}", "Error");
                Log.Escribe(ex);
                return null;
            }
        }

        /// <summary>
        /// Ejecuta consulta com parametros a ODBC
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parametetros"></param>
        /// <returns></returns>
        public OdbcDataReader EjecutaSelectConParametros(string query, OdbcParameter[] parametetros)
        {
            try
            {
                OdbcCommand DbCommand = DbConnection.CreateCommand();
                DbCommand.CommandText = query;
                DbCommand.Parameters.Add(parametetros);
                OdbcDataReader DbReader = DbCommand.ExecuteReader();

                return DbReader;
            }
            catch (OdbcException ex)
            {
                Log.Escribe($"Fallo consulta a {this.dsn}", "Error");
                Log.Escribe(ex);
                return null;
            }
        }

        /// <summary>
        /// Obtiene la informacion de un OdbcDataReader en una lista de Maps
        /// </summary>
        /// <param name="maps">definicion del mapa</param>
        /// <param name="dr">OdbcDataReader de la consulta</param>
        /// <returns></returns>
        public List<List<Map>> LLenarMapToQuery(List<Map> maps, OdbcDataReader dr)
        {

            List<List<Map>> lista = new List<List<Map>>();
            int fila = 0;
            try
            {
                fila = 0;
                while (dr.Read())
                {
                    if (dr.FieldCount == 1)
                    {
                        switch (maps[0].Type)
                        {
                            case "string":
                                maps[0].Value = dr.GetString(0);
                                break;

                            case "int":
                                maps[0].Value = dr.GetInt32(0);
                                break;

                            default:
                                maps[0].Value = dr.GetString(0);
                                break;
                        }

                    }
                    else if(dr.FieldCount > 1)
                    {
                        for(int columna = 0; columna < dr.FieldCount; columna++)
                        {
                            switch (maps[columna].Type)
                            {
                                case "string":
                                    maps[columna].Value = dr.GetString(columna);
                                    break;

                                case "int":
                                    maps[columna].Value = dr.GetInt32(columna);
                                    break;
                                
                                case "float":
                                    maps[columna].Value = dr.GetFloat(columna);
                                    break;

                                default:
                                    maps[columna].Value = dr.GetString(columna);
                                    break;
                            }
                        }
                    }
                    lista.Add(maps);
                    fila++;
                }
                dr.Close();

                return lista;
            }
            catch (Exception ex)
            {
                dr.Close();
                Log.Escribe($"Error er la fila {fila}", "Error");
                Log.Escribe(ex);
                return null;
            }
            
           
        }

        
    }
}
