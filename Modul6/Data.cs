using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Policy;

namespace Modul6
{
    internal class Data
    {
        SqlConnection SqlConnection = new SqlConnection(@"Data Source=LAPTOP-B4N8CS4O;Initial Catalog=Mod6;Integrated Security=True");

        // Открывает соединение с базой данных, если оно закрыто.
        public void openConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
        }

        // Закрывает соединение с базой данных, если оно открыто.
        public void closeConnection()
        {
            if (SqlConnection.State == System.Data.ConnectionState.Open)
            {
                SqlConnection.Close();
            }
        }

        // Возвращает объект SqlConnection, представляющий соединение с базой данных.
        public SqlConnection GetConnection()
        {
            return SqlConnection;
        }
    }
}
