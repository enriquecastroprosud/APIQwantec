using QwantecNova.Conexion;
using System;
using System.IO;
using System.Net;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

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

            string fecha_inicio = DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd");
            string fecha_fin = DateTime.Now.AddDays(-4).ToString("yyyy-MM-dd");

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
                    rut = rut +"\""+ row["RUT"] +"\"" +",";
                }
                string enviar_json = "{\"apiKey\":\"17aa7f8785b58dee7096f3813655b60b\",\"inicio\":\"" + fecha_inicio + "T00:00:00\",  \"termino\":\"" + fecha_fin + "T23:59:59\",  \"identificador\":  ["+rut+"]}";
                streamWriter.Write(enviar_json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                //Console.WriteLine(responseText);

                DataSet dt = JsonConvert.DeserializeObject<DataSet>(responseText);
                var table = dt.Tables[0];


                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine(row[0]);
                    Console.WriteLine(row[1]);
                }

                    Console.ReadKey();

            }
        }
    }
}
