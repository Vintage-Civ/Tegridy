using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Tegridy
{
    internal class AllowList
    {
        internal Dictionary<string, List<TegridyReport>> allowedReportsById = new Dictionary<string, List<TegridyReport>>();

        internal void AddReport(TegridyReport report)
        {
            if (!allowedReportsById.ContainsKey(report.Id))
            {
                allowedReportsById.Add(report.Id, new List<TegridyReport>());
            }
            allowedReportsById[report.Id].Add(report);
        }

        internal EnumProblemFlags GetReportProblem(TegridyReport report)
        {
            EnumProblemFlags flags = EnumProblemFlags.All;

            if (allowedReportsById.TryGetValue(report.Id, out var tegridyReports))
            {
                foreach (var allowed in tegridyReports)
                {
                    flags = EnumProblemFlags.All;
                    flags &= ~EnumProblemFlags.UnrecognizedModId;
                    flags &= ~EnumProblemFlags.EmptyReports;

                    if (allowed.Fingerprint == report.Fingerprint)
                    {
                        flags &= ~EnumProblemFlags.UnrecognizedFingerprint;
                    }

                    if (allowed.Version == report.Version)
                    {
                        flags &= ~EnumProblemFlags.UnrecognizedVersion;
                    }

                    if (allowed.SourceType == report.SourceType)
                    {
                        flags &= ~EnumProblemFlags.UnrecognizedSourceType;
                    }

                    if (flags == 0) break;
                }
            }

            return flags;
        }

        internal const string bannedMod = @"Unrecognized or banned mod '{0}' — please disable this mod using the in-game Mod Manager.";
        internal const string bannedVersion = @"Unrecognized or banned version '{0}' for mod '{1}' — please update to a known good version, such as: {2}";
        internal const string bannedType = @"Unrecognized or banned source type '{0}' for mod '{1}' — please update this mod to use a known good source type, such as: {2}";
        internal const string bannedFingerprint = @"Unrecognized or banned fingerprint for mod '{0}' — please update this mod with a freshly downloaded copy.";

        internal bool HasErrors(TegridyReport report, out string errors)
        {
            errors = null;
            EnumProblemFlags flags = GetReportProblem(report);
            
            if (flags != EnumProblemFlags.None)
            {
                errors = GetFriendlyError(report, flags);
                return true;
            }

            return false;
        }

        internal string GetFriendlyError(TegridyReport report, EnumProblemFlags flags)
        {
            StringBuilder builder = new StringBuilder();
            if (flags != EnumProblemFlags.None)
            {
                if ((((int)flags >> 0) & 1) > 0)
                {
                    builder.AppendLine(Lang.Get(bannedMod, report.Name));
                }

                if ((((int)flags >> 1) & 1) > 0)
                {
                    builder.AppendLine(Lang.Get(bannedVersion, report.Version, report.Name, string.Join(", ", GetAllowedVersionsForMod(report.Id))));
                }

                if ((((int)flags >> 2) & 1) > 0)
                {
                    string type = Enum.GetName(typeof(EnumModSourceType), report.SourceType);
                    builder.AppendLine(Lang.Get(bannedType, type, report.Name, string.Join(", ", GetAllowedSourceTypesForMod(report.Id))));
                }

                if ((((int)flags >> 3) & 1) > 0)
                {
                    builder.AppendLine(Lang.Get(bannedFingerprint, report.Name));
                }
            }
            else
            {
                builder.Append("No Errors.");
            }
            return builder.ToString();
        }

        internal IEnumerable<string> GetAllowedVersionsForMod(string modId)
        {
            if (allowedReportsById.TryGetValue(modId, out var tegridyReports))
            {
                return tegridyReports.Select((rp) => rp.Version).Distinct();
            }
            return Enumerable.Empty<string>();
        }

        internal IEnumerable<int> GetAllowedSourceTypesForMod(string modId)
        {
            if (allowedReportsById.TryGetValue(modId, out var tegridyReports))
            {
                return tegridyReports.Select((rp) => rp.SourceType).Distinct();
            }
            return Enumerable.Empty<int>();
        }
    }
}
