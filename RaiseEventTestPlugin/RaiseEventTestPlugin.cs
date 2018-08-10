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
        MultithreadManager multithreadmanager = null;
        List<AI> ailist = new List<AI>();

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

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            multithreadmanager = MultithreadManager.GetInstance();
            //multithreadmanager.AddThread("aiManager", new System.Threading.ParameterizedThreadStart(UpdateAI), (object)this);
            //multithreadmanager.StartThread("aiManager");
            //host.TryRegisterType(typeof(CustomObject), (byte)EVENT_CODES.CO_CUSTOMOBJECT,
            //CustomObject.Serialize,
            //CustomObject.Deserialize);
            return base.SetupInstance(host, config, out errorMsg);
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
                        //string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        //string PlayerName = PackageDecoder.GetValue(RecvdMessage, "PlayerName");
                        //string Password = PackageDecoder.GetValue(RecvdMessage, "Password");

                        AccountdDetails ad = (AccountdDetails)AccountdDetails.Deserialize((byte[])info.Request.Data);
                        string PlayerName = ad.userid;
                        string Password = ad.pw;

                        CustomObject ret = new CustomObject();
                        ret.Init();
                        ret.message = "WELCOME";

                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, CustomObject.Serialize(ret) } }, evCode: info.Request.EvCode, cacheOp: 0);

                        int usernameexists = Convert.ToInt32(connector.ConnectAndRunScalar("SELECT COUNT(accountdb.account_name) FROM accountdb WHERE account_name = '" + PlayerName + "'"));
                        if (Convert.ToInt32(usernameexists) == 0)
                        {
                            int rowsaffected = connector.ConnectAndRunNonQuery("INSERT INTO accountdb (account_name, password, date_created) VALUES('" + PlayerName + "', '" + Password + "', now())");
                            if (rowsaffected == 1)
                            {
                                ret.message = "SIGNUP SUCCESS";
                                this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, CustomObject.Serialize(ret) } }, evCode: info.Request.EvCode, cacheOp: 0);
                            }
                        }
                        else
                        {
                            string olwpw = Convert.ToString(connector.ConnectAndRunScalar("SELECT accountdb.password FROM accountdb WHERE accountdb.account_name = '" + PlayerName + "'"));
                            if (Password != olwpw)
                            {
                                ret.message = "NOT VALID";

                                this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, CustomObject.Serialize(ret) } }, evCode: (byte)(69), cacheOp: 0);
                                return;
                            }
                        }
                        ret.message = "LOGIN SUCESS";

                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, CustomObject.Serialize(ret) } }, evCode: (byte)(5), cacheOp: 0);

                        break;
                    }
                case 2:
                    {
                        PlayerPos pp = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);

                        string RecvdMessage = Encoding.Default.GetString((byte[])info.Request.Data);
                        string PlayerName = pp.userid; //PackageDecoder.GetValue(RecvdMessage, "PlayerName");
                        string posx = Convert.ToString(pp.posx); //PackageDecoder.GetValue(RecvdMessage, "posx");
                        string posy = Convert.ToString(pp.posy);//PackageDecoder.GetValue(RecvdMessage, "posy");
                        string posz = Convert.ToString(pp.posz);//PackageDecoder.GetValue(RecvdMessage, "posz");
                        connector.ConnectAndRunNonQuery("UPDATE accountdb SET posx = " + posx + ",posy=" + posy + ",posz=" + posz + " WHERE account_name = '" + PlayerName + "'");

                        break;
                    }
                case 3:
                    {
                        PlayerPos oldpp = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);
                        string PlayerName = oldpp.userid;
                        PlayerPos pp = new PlayerPos();
                        pp.Init();
                        pp.userid = PackageDecoder.GetValue(PlayerName, "PlayerName");

                        pp.posx = (int)connector.ConnectAndRunScalar("Select posx from accountdb where account_name = '" + PlayerName + "'");
                        pp.posy = (int)connector.ConnectAndRunScalar("Select posy from accountdb where account_name = '" + PlayerName + "'");
                        pp.posz = (int)connector.ConnectAndRunScalar("Select posz from accountdb where account_name = '" + PlayerName + "'");
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, PlayerPos.Serialize(pp) } }, evCode: (byte)(3), cacheOp: 0);
                        break;
                    }
                case 6:
                    {
                        PlayerPos pp = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);
                        string PlayerName = pp.userid;
                        string Guild = pp.guild;
                        connector.ConnectAndRunNonQuery("UPDATE accountdb SET guild = '" + Guild + "' WHERE account_name = '" + PlayerName + "'");
                        break;
                    }
                case 7:
                    {
                        PlayerPos pp = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);
                        string PlayerName = pp.userid;
                        pp.guild = Convert.ToString(connector.ConnectAndRunScalar("SELECT guild FROM accountdb WHERE account_name = '" + PlayerName + "'"));
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, senderActor: 0, targetGroup: 0, data: new Dictionary<byte, object>() { { (byte)245, PlayerPos.Serialize(pp) } }, evCode: (byte)(7), cacheOp: 0);

                        break;
                    }
                case 99:
                    {
                        AccountdDetails ad = (AccountdDetails)AccountdDetails.Deserialize((byte[])info.Request.Data);
                        string PlayerName = ad.userid;
                        string Password = ad.pw;

                        connector.ConnectAndRunNonQuery("UPDATE accountdb SET password = '" + Password + "' WHERE account_name = '" + PlayerName + "'");

                        break;
                    }
                case 101:
                    {


                        MonsterInfo mi = (MonsterInfo)MonsterInfo.Deserialize((byte[])info.Request.Data);
                        AI ai = new TestPlugin.AI();
                        ai._info = mi;
                        ailist.Add(ai);

                        multithreadmanager.AddThread(ai._info.name, new System.Threading.ParameterizedThreadStart(ai.Update), this);
                        multithreadmanager.StartThread(ai._info.name);

                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr, 
                            senderActor: 0, 
                            targetGroup: 0, 
                            data: new Dictionary<byte, object>()
                            { { (byte)245, MonsterInfo.Serialize(mi) } }, 
                            evCode: (byte)(102), 
                            cacheOp: 0);

                        break;
                    }
                case 103:
                    {
                        PlayerPos mi = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);

                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr,
                            senderActor: 0,
                            targetGroup: 0,
                            data: new Dictionary<byte, object>()
                            { { (byte)245, PlayerPos.Serialize(mi) } },
                            evCode: (byte)(104),
                            cacheOp: 0);

                        break;
                    }
                case 150:
                    {
                        PlayerPos mi = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);
                        PartitioningInfo pi = new PartitioningInfo();
                        pi.Init();
                        pi.objname = mi.userid;
                        pi.currtile = MapPartitioningButNotPartitioningIsJustSplittingUpTheMap.ReturnTileViaPos(mi.posx, mi.posz);
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr,
                            senderActor: 0,
                            targetGroup: 0,
                            data: new Dictionary<byte, object>()
                            { { (byte)245, PartitioningInfo.Serialize(pi) } },
                            evCode: info.Request.EvCode,
                            cacheOp: 0);
                        break;
                    }
                case 151:
                    {
                        PlayerPos mi = (PlayerPos)PlayerPos.Deserialize((byte[])info.Request.Data);
                        ListPartitioningInfo pi = new ListPartitioningInfo();
                        pi.Init();
                        pi.playerposx = mi.posx;
                        pi.playerposz = mi.posz;

                        pi.objname = mi.userid;
                        pi.currtile = MapPartitioningButNotPartitioningIsJustSplittingUpTheMap.ReturnTileViaPos(mi.posx, mi.posz);
                        pi._multitile = MapPartitioningButNotPartitioningIsJustSplittingUpTheMap.ReturnSurroundingTilesViaCurrentTile(pi.currtile);
                        this.PluginHost.BroadcastEvent(target: (byte)info.ActorNr,
                            senderActor: 0,
                            targetGroup: 0,
                            data: new Dictionary<byte, object>()
                            { { (byte)245, ListPartitioningInfo.Serialize(pi) } },
                            evCode: info.Request.EvCode,
                            cacheOp: 0);
                        break;
                    }
                case 180:
                    {

                        MonsterInfo mi = (MonsterInfo)MonsterInfo.Deserialize((byte[])info.Request.Data);
                        AI updater = ailist.Find(x => x._info.name == mi.name);
                        updater._newinfo = mi;

                        break;
                    }
            }
        }

        public void SendMessage(int _evCode, byte[] _object)
        {
            this.PluginHost.BroadcastEvent(target: 0,
                            senderActor: 0,
                            targetGroup: 0,
                            data: new Dictionary<byte, object>()
                            { { (byte)245, _object } },
                            evCode: (byte)(_evCode),
                            cacheOp: 0);
        }

        //public void UpdateAI(object _plugin)
        //{
        //    CustomObject co = new CustomObject();
        //    co.Init();
        //    co.message = "UpdateAI";
        //    CustomObject co1 = new CustomObject();
        //    co1.Init();
        //    string message = "UpdateAI.";
        //    while (true)
        //    {
        //        this.SendMessage(169,
        //            CustomObject.Serialize(co)
        //            );
        //        foreach (AI ai in ailist)
        //        {
        //            co1.message = message + ai._info.name;
        //            this.SendMessage(169,
        //            CustomObject.Serialize(co1)
        //            );
        //            ai.Update(_plugin);
        //        }
        //        System.Threading.Thread.Sleep(100);
        //    }
        //}
    }
}

