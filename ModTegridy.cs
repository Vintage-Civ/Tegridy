using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

[assembly: ModInfo("Tegridy")]
namespace Tegridy
{
    public class TegridyPacket
    {
        public List<TegridyReport> Reports = new List<TegridyReport>();
    }

    public class ModTegridy : ModSystem
    {
        public override double ExecuteOrder() => double.NegativeInfinity;

        internal INetworkChannel channel;
        internal IClientNetworkChannel cChannel { get => channel as IClientNetworkChannel; }
        internal IServerNetworkChannel sChannel { get => channel as IServerNetworkChannel; }

        public override void StartPre(ICoreAPI api)
        {
            channel = api.Network.RegisterChannel("tegridy").RegisterMessageType(typeof(TegridyPacket));

            switch (api.Side)
            {
                case EnumAppSide.Server:
                    StartPreServer(api as ICoreServerAPI);
                    break;
                case EnumAppSide.Client:
                    StartPreClient(api as ICoreClientAPI);
                    break;
                case EnumAppSide.Universal:
                    break;
                default:
                    break;
            }
        }

        public void StartPreServer(ICoreServerAPI api)
        {

        }

        public void StartPreClient(ICoreClientAPI api)
        {
            TegridyPacket packet = new TegridyPacket();
            foreach (var mod in api.ModLoader.Mods)
            {
                TegridyReport report = TegridyReport.Create(mod);
                packet.Reports.Add(report);
            }

            api.Event.BlockTexturesLoaded += () =>
            {
                cChannel.SendPacket(packet);
            };
        }
    }
}
