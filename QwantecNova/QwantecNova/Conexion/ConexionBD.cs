using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; 

namespace QwantecNova.Conexion
{
    class ConexionBD
    {
        public SqlConnection procesadorabd()
        {
            SqlConnection conn = new SqlConnection("Data Source=192.168.1.69;Initial Catalog=procesadorabd;uid=sa;Password=procesadora1");
            conn.Open();
            return conn; 
        }

        public SqlConnection prosud_bi()
        {
            SqlConnection conn = new SqlConnection("Data Source=192.168.1.68;Initial Catalog=Prosud_BI;uid=sa;Password=procesadora1;");
            conn.Open();

            return conn;
        }
    }
}
