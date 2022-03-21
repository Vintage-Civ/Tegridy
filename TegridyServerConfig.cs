using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Server;

namespace Tegridy
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class TegridyServerConfig
    {
        private ICoreServerAPI sapi;

        [JsonProperty]
        private int clientReportGraceSeconds = 15;

        [JsonProperty]
        private string extraDisconnectMessage = "Please contact the server owner with any problems or to request new mods be added to the whitelist.";

        [JsonProperty]
        private TegridyReport[] allowedClientMods = new TegridyReport[0];

        public TegridyServerConfig(ICoreServerAPI sapi)
        {
            this.sapi = sapi;
        }

        public int ClientReportGraceSeconds { 
            get { Load(); return clientReportGraceSeconds; } 
            set { clientReportGraceSeconds = value; Save(); } 
        }

        public string ExtraDisconnectMessage
        {
            get { Load(); return extraDisconnectMessage; }
            set { extraDisconnectMessage = value; Save();}
        }

        public TegridyReport[] AllowedClientMods
        {
            get { Load(); return allowedClientMods; }   
            set { allowedClientMods = value; Save(); }
        }

        public void Save()
        {
            sapi.StoreModConfig(this, "tegridy/server.json");
        }

        public void Load()
        {
            try
            {
                var conf = sapi.LoadModConfig<TegridyServerConfig>("tegridy/server.json") ?? new TegridyServerConfig(sapi);

                clientReportGraceSeconds = conf.clientReportGraceSeconds;
                extraDisconnectMessage = conf.extraDisconnectMessage;
                allowedClientMods = conf.allowedClientMods;
            }
            catch (Exception ex)
            {
                sapi.Logger.Error("Malformed ModConfig file tegridy/server.json, Exception: \n {0}", ex.StackTrace);
            }
        }
    }
}
