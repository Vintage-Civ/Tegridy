using ProtoBuf;
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
    }
}
