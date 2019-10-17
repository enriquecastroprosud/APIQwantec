using QwantecNova.Conexion;
using System;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace QwantecNova
{
    class Program
    {
        static void Main(string[] args)
        {
            Conexion.ConexionBD con = new ConexionBD();

            string webAddr = "https://app.relojcontrol.com/api/consultaMarcaciones/consulta";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            string fecha_inicio = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string fecha_fin = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string usuarios_query = "select distinct dbo.Fn_FormatearRut(ltrim(EmplRutF)) as RUT from Empleados where EmprCod = '1'and len(ltrim(rtrim(emplrutf))) > 8 and emplrutf<> ' 099805013'";
                SqlCommand comando = new SqlCommand(usuarios_query, con.procesadorabd());

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = comando;
                DataTable usuarios_rut = new DataTable();
                adapter.Fill(usuarios_rut);
                string rut = "";

                foreach (DataRow row in usuarios_rut.Rows)
                {
                    rut = rut + "\"" + row["RUT"] + "\"" + ",";
                }

                string enviar_json = "{\"apiKey\":\"17aa7f8785b58dee7096f3813655b60b\",\"inicio\":\"" + fecha_inicio + "T00:00:00\",  \"termino\":\"" + fecha_fin + "T23:59:59\",  \"identificador\":  [" + rut + "]}";
                streamWriter.Write(enviar_json);
                streamWriter.Flush();

                //Account account = JsonConvert.DeserializeObject<Account>(enviar_json);
                //Console.WriteLine(account.RUT);


            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Console.WriteLine(responseText);

                //DataSet dt = JsonConvert.DeserializeObject<DataSet>(responseText);
                //Marcacion persona = JsonConvert.DeserializeObject<Marcacion>(responseText);
                //Console.WriteLine(persona.codigo);
                //Console.WriteLine(persona.nombres);


                JsonConverted_respuesta(responseText);


            }
        }

        public static void JsonConverted_respuesta(string jsonData)
        {
            var objData = (JObject)JsonConvert.DeserializeObject(jsonData); // Deserialize json data

            dynamic jObject = new JObject();
            //jObject.respuesta = objData.Value<JArray>("marcaciones");
            string json_inicial = objData.Value<JArray>("marcaciones").ToString();
            string ultimo_json = json_inicial.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Replace("    ", "").Replace("  ", "");
            Marcaciones(ultimo_json);

        }

        public static void Marcaciones(string jsonData)
        {
            Conexion.ConexionBD con = new ConexionBD();


            var objData = JsonConvert.DeserializeObject<List<Marcacion>>(jsonData);
            foreach (Marcacion Persona in objData)
            {
                string sqlConnectionString = @"Data Source=192.168.1.68;Initial Catalog=PROSUD_BI;user=sa;pwd=procesadora1";

                using (SqlConnection connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();
                    string query = "insert into MarcacionesQwantec  ([codigo],[codigoFicha],[nombres],[apellidos] ,[departamento],[sucursal],[fechauhora],[tipo],[numeroReloj]) values ('" + Persona.codigo + "','" +
                    Persona.codigoFicha + "','" +
                    Persona.nombres + "','" +
                    Persona.apellidos + "','" +
                    Persona.departamento + "','" +
                    Persona.sucursal + "','" +
                    Persona.fechaHora + "','" +
                    Persona.tipo + "','" +
                    Persona.numeroReloj + "');";


                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }

            }

        }
    }
}
