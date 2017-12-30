using BZFlag.Game.Host;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BZFS.PlayerAdministration
{
    internal static class BanDatabase
    {
        internal class BanResults
        {
            internal int ID = -1;
            internal bool Active = false;
            internal string Reason = string.Empty;

            internal static readonly BanResults None = new BanResults();
        }

        private static Databases.IBanDBBackend Backend = null;

        internal static void Init(ServerConfig.SecurityInfo info)
        {
            if (Backend != null)
                return;

            API.RegisterDBBackend("YAML", new Databases.YAMLFlatFileDB());
            string backend = info.BanDBBackend.ToUpperInvariant();

            lock (API.Backends)
            {
                if (API.Backends.ContainsKey(backend))
                    Backend = API.Backends[backend];
            }

            if (Backend != null)
                Backend.Setup(info);
        }

        internal static BanResults CheckAddressBan(string address)
        {
            if (Backend == null || address == string.Empty)
                return BanResults.None;

            if (address.Contains(":"))
            {
                string mask = string.Empty;
                foreach (string subnet in address.Split(":".ToCharArray()))
                {
                    if (mask == string.Empty)
                        mask = subnet;
                    else
                        mask += ":" + subnet;

                    BanResults results = ResultsFromRecord(Backend.FindIPAddressBan(mask));
                    if (results != BanResults.None)
                        return results;
                }
            }
            else
            {
                string mask = string.Empty;
                foreach (string subnet in address.Split(".".ToCharArray()))
                {
                    if (mask == string.Empty)
                        mask = subnet;
                    else
                        mask += "." + subnet;

                    BanResults results = ResultsFromRecord(Backend.FindIPAddressBan(mask));
                    if (results != BanResults.None)
                        return results;
                }
            }
           

            return BanResults.None;
        }

        internal static BanResults CheckHostBan(string hostname)
        {
            if (Backend == null || hostname == string.Empty)
                return BanResults.None;

            string mask = string.Empty;
            foreach (string subnet in hostname.Split(".".ToCharArray()))
            {
                if (mask == string.Empty)
                    mask = subnet;
                else
                    mask += "." + subnet;

                BanResults results = ResultsFromRecord(Backend.FindHostMaskBan(mask));
                if (results != BanResults.None)
                    return results;
            }

            return BanResults.None;
        }

        internal static BanResults CheckIDBan(string id)
        {
            if (Backend == null)
                return BanResults.None;

            return ResultsFromRecord(Backend.FindIDBan(id));
        }

        private static BanResults ResultsFromRecord(Databases.BanRecord record)
        {
            if (record == null)
                return BanResults.None;

            BanResults results = new BanResults();
            results.ID = record.ID;
            results.Reason = record.Reason;
            results.Active = true;

            return results;
        }

        internal static int AddBan(string id, string address, string host, string reason, string author, int days, int minutes)
        {
            if (Backend == null)
                return -1;

            int endTime = days * 1440 + minutes;
            return Backend.AddBan(id, address, host, reason, author, endTime);
        }

        internal static void CheckExpired()
        {
           Backend?.ChecForExpiredBans();
        }
    }
}
