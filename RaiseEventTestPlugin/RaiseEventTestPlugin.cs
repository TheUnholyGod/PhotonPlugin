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
            switch(info.Request.EvCode)
            {
                case 1:
                    {
                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        PackageDecoder pc = new PackageDecoder();
                        string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");
                        string Password = pc.GetValue(RecvdMessage, "Password");

                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"WELCOMETOFUCKYOU" } }, evCode: info.Request.EvCode, cacheOp: 0);

                        int usernameexists = Convert.ToInt32(connector.ConnectAndRunScalar("SELECT COUNT(accountdb.account_name) FROM accountdb WHERE account_name = '" + PlayerName + "'"));
                        if (Convert.ToInt32(usernameexists) == 0)
                        {
                            int rowsaffected = connector.ConnectAndRunNonQuery("INSERT INTO accountdb (account_name, password, date_created) VALUES('" + PlayerName + "', '" + Password + "', now())");
                            if (rowsaffected == 1)
                            {
                                this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"Signup Success" } }, evCode: info.Request.EvCode, cacheOp: 0);
                            }
                        }
                        else
                        {
                            string olwpw = Convert.ToString(connector.ConnectAndRunScalar("SELECT accountdb.password FROM accountdb WHERE accountdb.account_name = '" + PlayerName + "'"));
                            if (Password != olwpw)
                            {
                                this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"NotValid" } }, evCode: (byte)(69), cacheOp: 0);
                                return;
                            }


                            this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)"LoginSuccess" } }, evCode: (byte)(5), cacheOp: 0);

                        }
                        break;
                    }
                case 2:
                    {
                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        PackageDecoder pc = new PackageDecoder();
                        string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");
                        string posx = pc.GetValue(RecvdMessage, "posx");
                        string posy = pc.GetValue(RecvdMessage, "posy");
                        string posz = pc.GetValue(RecvdMessage, "posz");
                        connector.ConnectAndRunNonQuery("UPDATE accountdb SET posx = " + posx + ",posy=" + posy + ",posz=" + posz + " WHERE account_name = '" + PlayerName + "'");

                        break;
                    }
                case 3:
                    {
                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        PackageDecoder pc = new PackageDecoder();
                        string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");

                        string posx = Convert.ToString((int)connector.ConnectAndRunScalar("Select posx from accountdb where account_name = '" + PlayerName + "'"));
                        string posy = Convert.ToString((int)connector.ConnectAndRunScalar("Select posy from accountdb where account_name = '" + PlayerName + "'"));
                        string posz = Convert.ToString((int)connector.ConnectAndRunScalar("Select posz from accountdb where account_name = '" + PlayerName + "'"));
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)("<posx=" + posx + " posy=" + posy + " posz=" + posz + " >") } }, evCode: (byte)(3), cacheOp: 0);
                        break;
                    }
                case 6:
                    {
                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        PackageDecoder pc = new PackageDecoder();
                        string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");
                        string Guild = pc.GetValue(RecvdMessage, "Guild");
                        connector.ConnectAndRunNonQuery("UPDATE accountdb SET guild = '" + Guild + "' WHERE account_name = '" + PlayerName + "'");
                        break;
                    }
                case 7:
                    {
                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        PackageDecoder pc = new PackageDecoder();
                        string PlayerName = pc.GetValue(RecvdMessage, "PlayerName");
                        string guild = Convert.ToString(connector.ConnectAndRunScalar("SELECT guild FROM accountdb WHERE account_name = '" + PlayerName + "'"));
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, (string)("<Guild="+guild+" >") } }, evCode: (byte)(3), cacheOp: 0);

                        break;
                    }
            }
        }
    }
}

