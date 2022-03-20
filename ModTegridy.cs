using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vintagestory.API.Common;

[assembly: ModInfo("Tegridy")]
namespace Tegridy
{
    public class TegridyPacket
    {
        public List<TegridyReport> Reports = new List<TegridyReport>();
    }

    public class ModTegridy : ModSystem
    {

    }
}
