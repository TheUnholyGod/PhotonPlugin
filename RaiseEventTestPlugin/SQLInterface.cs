using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace TestPlugin
{
    class SQLInterface
    {
        MySqlConnection conn;
        string connectionstring = "server=localhost;user=root;database=spaceships;port=3306;password=DM2341sidm;SslMode=none";

        public int ConnectAndRunNonQuery(string _query)
        {
            ConnectToSQL();
            int ret = ExecuteNonQuery(_query);
            DisconnectFromSQL();
            return ret;
        }

        public object ConnectAndRunScalar(string _query)
        {
            ConnectToSQL();
            object ret = ExecuteSingleQuery(_query);
            DisconnectFromSQL();
            return ret;
        }

        public bool ConnectToSQL()
        {
            if (connectionstring.Length <= 0)
                return false;
            conn = new MySqlConnection(connectionstring);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public bool ConnectToSQL(string _conn)
        {
            connectionstring = _conn;
            return ConnectToSQL();

                //MySqlCommand cmd = new MySqlCommand(_cmd, conn);
                //cmd.ExecuteNonQuery();
                //cmd.ExecuteReader();
                //return cmd.ToString();
            
        }

        public int ExecuteNonQuery(string _query)
        {
            if (_query.Length <= 0)
                return -1;

            MySqlCommand command = new MySqlCommand(_query, conn);
            return command.ExecuteNonQuery();
        }

        public object ExecuteSingleQuery(string _query)
        {
            if (_query.Length <= 0)
                return "";

            MySqlCommand command = new MySqlCommand(_query,conn);
            return (command.ExecuteScalar());
        }

        public List<string> ExecuteQuery(string _query)
        {
            if (_query.Length <= 0)
                return null;
            List<string> ret = new List<string>();

            MySqlCommand command = new MySqlCommand(_query, conn);
            MySqlDataReader reader = command.ExecuteReader();
            int rowcount = 0;
            bool end = false;
            while (!end)
            {
                reader.GetString(rowcount);
                ++rowcount;
                end = reader.NextResult();
            }
            return ret;
        }

        public void DisconnectFromSQL()
        {
            conn.Close();
        }
    }
}
