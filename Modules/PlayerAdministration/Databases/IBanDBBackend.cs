using BZFlag.Game.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BZFS.PlayerAdministration.Databases
{
    public class BanRecord
    {
        public int ID = 0;

        public DateTime CreateTime = DateTime.MinValue;

        public string Author = string.Empty;
        public string Reason = string.Empty;
    }

    public interface IBanDBBackend
    {
        bool Setup(ServerConfig.SecurityInfo info);

        BanRecord FindIDBan(string ID);

        BanRecord FindIPAddressBan(string ban);

        BanRecord FindHostMaskBan(string ban);

        int AddBan(string bzID, string address, string host, string author, string reason, int lenght);

        bool ChecForExpiredBans();
    }
}
