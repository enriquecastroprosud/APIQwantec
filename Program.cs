using QwantecNova.Conexion;
using System;
using System.IO;
using System.Net;

namespace QwantecNova
{
    class Program
    {
        static void Main(string[] args)
        {
            //Conexion.ConexionBD con = new ConexionBD();
            //con.conn();

            string webAddr = "https://app.relojcontrol.com/api/consultaMarcaciones/consulta";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

  
            string fecha_inicio = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            string fecha_fin = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            string rut_usuario = "17.778.088-K";

            string enviar_json = "{\"apiKey\":\"17aa7f8785b58dee7096f3813655b60b\",\"inicio\":\""+fecha_inicio+"T00:00:00\",  \"termino\":\""+fecha_fin+"T23:59:59\",  \"identificador\":  [ \""+rut_usuario+"\"]}";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = enviar_json;

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                Console.WriteLine(responseText);
                Console.ReadKey();


                //Now you have your response.
                //or false depending on information in the response     
            }
        }
    }
}
