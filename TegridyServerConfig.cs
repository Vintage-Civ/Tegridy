﻿using HarmonyLib;
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
        //Only forward versions if new default value needs pushed to all configs
        private static readonly Dictionary<string, string> Versions = new Dictionary<string, string>()
        {
            { @"configVersionByField",      @"1.0.0"},
            { @"clientReportGraceSeconds",  @"1.0.0"},
            { @"extraDisconnectMessage",    @"1.0.0"},
            { @"allowedClientMods",         @"1.0.0"},
        };

        private ICoreServerAPI sapi;

        [JsonProperty]
        private int? clientReportGraceSeconds = 15;

        [JsonProperty]
        private string extraDisconnectMessage = @"Please contact the server owner with any problems or to request new mods be added to the whitelist.";

        [JsonProperty]
        private TegridyReport[] allowedClientMods = new TegridyReport[0];

        [JsonProperty]
        private Dictionary<string, string> configVersionByField = Versions;

        public TegridyServerConfig(ICoreServerAPI sapi)
        {
            this.sapi = sapi;
        }

        public int ClientReportGraceSeconds
        {
            get { Load(); return clientReportGraceSeconds.Value; }
            set { clientReportGraceSeconds = value; Save(); }
        }

        public Dictionary<string, string> ConfigVersionByField
        { 
            get { Load(); return configVersionByField; } 
            set { configVersionByField = value; Save(); } 
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
                var newConfig = new TegridyServerConfig(sapi);

                var conf = sapi.LoadModConfig<TegridyServerConfig>("tegridy/server.json") ?? newConfig;

                clientReportGraceSeconds = conf.clientReportGraceSeconds ?? newConfig.ClientReportGraceSeconds;
                extraDisconnectMessage = conf.extraDisconnectMessage ?? newConfig.extraDisconnectMessage;
                allowedClientMods = conf.allowedClientMods ?? newConfig.allowedClientMods;
                configVersionByField = conf.configVersionByField ?? newConfig.configVersionByField;
                var fieldNames = AccessTools.GetFieldNames(this);
                fieldNames.Remove("sapi");
                fieldNames.Remove("Versions");
                
                foreach (string field in fieldNames)
                {
                    if (Versions.TryGetValue(field, out string version0) && configVersionByField.TryGetValue(field, out string version1))
                    {
                        var v0 = Version.Parse(version0);
                        var v1 = Version.Parse(version1);
                        if (v0 > v1)
                        {
                            this.SetField(field, newConfig.GetField<object>(field));
                            conf.configVersionByField[field] = version0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sapi.Logger.Error("Malformed ModConfig file tegridy/server.json, Exception: \n {0}", ex.StackTrace);
            }
        }
    }
}
