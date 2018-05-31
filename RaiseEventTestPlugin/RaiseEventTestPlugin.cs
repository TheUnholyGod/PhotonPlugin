using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive;
using Photon.Hive.Plugin;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace TestPlugin
{
    public class RaiseEventTestPlugin : PluginBase
    {
        MySqlConnection conn;

        public string ServerString
        {
            get;
            private set;
        }
        public int CallsCount
        {
            get;
            private set;
        }
        public RaiseEventTestPlugin()
        {
            this.UseStrictMode = true;
            this.ServerString = "ServerMessage";
            this.CallsCount = 0;

            ConnectToSQL();
        }
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }
        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }
            if (1 == info.Request.EvCode)
            {
                string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                PackageDecoder pc = new PackageDecoder();
                string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");
                string Password = pc.GetValue(RecvdMessage, "Password");
                string sql = "SELECT accountdb.password FROM accountdb WHERE accountdb.account_name = " + PlayerName;
                string olwpw = ConnectToSQL(sql);
                if (Password != olwpw)
                {
                    DisconnectFromSQL();

                    sql = "INSERT INTO accountdb (account_name, password, date_created) VALUES('" + PlayerName + "', '" + Password + "', now())";
                    ConnectToSQL(sql);
                }
                DisconnectFromSQL();
            }
        }

        public void ConnectToSQL()
        {
            string connStr = "server=localhost;user=root;database=spaceships;port=3306;password=DM2341sidm;SslMode=none";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                
            }
        }

        public string ConnectToSQL(string _cmd)
        {
            string connStr = "server=localhost;user=root;database=spaceships;port=3306;password=DM2341sidm;SslMode=none";
            conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(_cmd, conn);
                cmd.ExecuteNonQuery();
                return cmd.ToString();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
            return "";
        }

        public void DisconnectFromSQL()
        {
            conn.Close();
        }
    }
}

