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
        SQLInterface connector = new SQLInterface();

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

                this.PluginHost.BroadcastEvent(target: ReciverGroup.All, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"WELCOMETOFUCKYOU" } }, evCode: info.Request.EvCode, cacheOp: 0);

                string usernameexists = connector.ConnectAndRunScalar("SELECT COUNT(accountdb.account_name) FROM accountdb WHERE account_name = '" + PlayerName + "'");
                if (Convert.ToInt32(usernameexists) == 0)
                {
                    int rowsaffected = connector.ConnectAndRunNonQuery("INSERT INTO accountdb (account_name, password, date_created) VALUES('" + PlayerName + "', '" + Password + "', now())");
                    if (rowsaffected == 1)
                    {
                        this.PluginHost.BroadcastEvent(target: ReciverGroup.All, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"Signup Success" } }, evCode: info.Request.EvCode, cacheOp: 0);
                    }
                }
                else
                {
                    string olwpw = connector.ConnectAndRunScalar("SELECT accountdb.password FROM accountdb WHERE accountdb.account_name = '" + PlayerName + "'");
                    if (Password != olwpw)
                    {
                        this.PluginHost.BroadcastEvent(target: ReciverGroup.All, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"NotValid" } }, evCode: info.Request.EvCode, cacheOp: 0);
                        return;
                    }
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"LoginSuccess" } }, evCode: info.Request.EvCode, cacheOp: 0);
                }
            }
        }
    }
}

