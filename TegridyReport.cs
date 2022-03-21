using ProtoBuf;
using System;
using Vintagestory.API.Common;

namespace Tegridy
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TegridyReport
    {
        public string Id;
        public string Name;
        public string Version;
        public string FileName;
        public int SourceType;
        public string Fingerprint;

        private const string unformattedString = @"[TegridyReport] - Type: {0} - Name: {1} - ID: {2} - Version: {3} - FileName: {4}, SHA256Hash: {5}";
        public static TegridyReport Create(Mod mod)
        {
            return new TegridyReport()
            {
                Id = mod.Info.ModID,
                Name = mod.Info.Name,
                Version = mod.Info.Version,
                FileName = mod.FileName,
                SourceType = (int)mod.SourceType,
                Fingerprint = ExtraMath.Sha256HashMod(mod)
            };
        }

        public string GetString()
        {
            return string.Format(unformattedString, Enum.GetName(typeof(EnumModSourceType), SourceType), Name, Id, Version, FileName, Fingerprint);
        }
    }
}
