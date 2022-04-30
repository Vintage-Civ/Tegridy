using ProtoBuf;
using System.Collections.Generic;

namespace Tegridy
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    internal class TegridyPacket
    {
        public List<TegridyReport> Reports = new List<TegridyReport>();

        internal void AddReport(TegridyReport report)
        {
            Reports.Add(report);
        }
    }
}
