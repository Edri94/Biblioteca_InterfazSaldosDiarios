using Biblioteca_InterfazSaldosDiarios.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_InterfazSaldosDiarios.Helpers
{
    public static class Funcion
    {
        public static string getValueAppConfig(string key, string section = "")
        {
            if (section.Length >= 1)
            {
                return ConfigurationManager.AppSettings[$"{section}.{key}"];
            }
            else
            {
                return ConfigurationManager.AppSettings[$"{key}"];
            }

        }

        public static bool SetParameterAppSettings(string key, string value, string section = "")
        {
            string nombre_appconfig = "Interfaz_SaldosDiarios.exe.config";

            bool bandera_archivo_existe = false;
            try
            {
                string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string[] appPath_arr = appPath.Split('\\');


                if (File.Exists(System.IO.Path.Combine(appPath, nombre_appconfig)))
                {
                    string configFile = System.IO.Path.Combine(appPath, nombre_appconfig);
                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                    configFileMap.ExeConfigFilename = configFile;
                    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    if (section.Length > 0)
                    {
                        config.AppSettings.Settings[$"{section}.{key}"].Value = value;
                    }
                    else
                    {
                        config.AppSettings.Settings[key].Value = value;
                    }
                    config.Save();
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return false;
            }

        }

        public static bool SetParameterTransfer(string key, string value, string archivo, string ruta_archivo)
        {
            try
            {
                string appPath = ruta_archivo + archivo;

                if (File.Exists(appPath))
                {
                    string buscar = $"{key}=";
                    string remplazar = $"{key}={value}";
                    string text = File.ReadAllText(appPath).Replace(buscar, remplazar);
                    File.WriteAllText(appPath, text);

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Log.Escribe(ex);
                return false;
            }

        }

        public static string Left(string cadena, int posiciones)
        {
            return cadena.Substring(0, posiciones);
        }


        public static string Mid(string cadena, int start, int length)
        {
            return cadena.Substring(start, length);
        }

        public static string Right(string cadena, int posiciones)
        {
            return cadena.Substring((cadena.Length - posiciones), posiciones);
        }

        public static string InvierteFecha(string fecha, bool con_hora)
        {
            string fecha_invertida = "";
            fecha = fecha.Replace("/", "-");
            string[] fecha_dividida = fecha.ToString().Split('-');

            

            if (con_hora)
            {
                string tmp_hora = Funcion.Mid(fecha_dividida[2], 5, 2);
                string tmp_minuto = Funcion.Mid(fecha_dividida[2], 8, 2);
                string tmp_anio = Funcion.Mid(fecha_dividida[2], 0, 4);
                string tmp_mes = Int32.Parse(fecha_dividida[1]).ToString("00");
                string tmp_dia = Int32.Parse(fecha_dividida[0]).ToString("00");

                fecha_invertida = $"{tmp_mes}-{tmp_dia}-{tmp_anio} {tmp_hora}:{tmp_minuto}";
            }
            else
            {
                string tmp_anio = Funcion.Mid(fecha_dividida[2], 0, 4);
                string tmp_mes = Int32.Parse(fecha_dividida[1]).ToString("00");
                string tmp_dia = Int32.Parse(fecha_dividida[0]).ToString("00");

                fecha_invertida = $"{tmp_mes}-{tmp_dia}-{tmp_anio}";
            }
           

            return fecha_invertida;

        }

        public static string Space(int n_espacios)
        {
            char espacio = ' ';
            string cadena = "";

            for (int i = 0; i < n_espacios; i++)
            {
                cadena +=  espacio;
            }

            return cadena;
        }
    }
}
