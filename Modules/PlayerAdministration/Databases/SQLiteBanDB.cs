using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using BZFlag.Game.Host;

namespace BZFS.PlayerAdministration.Databases
{
    public class SQLiteBanDB : IBanDBBackend
    {
        private SQLiteConnection DBConnection = null;

        private bool ReadOnly = false;

        public bool Setup(ServerConfig.SecurityInfo info)
        {
            ReadOnly = info.BansReadOnly;

            bool exists = File.Exists(info.BanDBFile);
            string connect = "Data Source=" + info.BanDBFile + ";Vesion=3;New=" + (exists ? "False" : "True") + "Compress=True;";
            DBConnection = new SQLiteConnection(connect);

            if (DBConnection == null)
                return false;

            DBConnection.Open();

            if (!exists)
            {
                SQLiteCommand cmd = new SQLiteCommand(Resources.SQLiteCreateString, DBConnection);
                cmd.ExecuteNonQuery();
            }

            return true;
        }

        protected BanRecord ResultsToRecord(SQLiteDataReader results)
        {
            if (!results.HasRows || !results.Read())
                return null;

            BanRecord record = new BanRecord();
            record.ID = results.GetInt32(0);
            record.Author = results.GetString(4);
            record.Reason = results.GetString(5);
            record.CreateTime = results.GetDateTime(6);

            return null;
        }

        public BanRecord FindHostMaskBan(string ban)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM bans WHERE BanHost=@host AND Active=1", DBConnection);
            cmd.Parameters.Add(new SQLiteParameter("@host", ban));

            return ResultsToRecord(cmd.ExecuteReader());
        }

        public BanRecord FindIDBan(string ID)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM bans WHERE BanID=@id AND Active=1", DBConnection);
            cmd.Parameters.Add(new SQLiteParameter("@id", ID));

            return ResultsToRecord(cmd.ExecuteReader());
        }

        public BanRecord FindIPAddressBan(string ban)
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM bans WHERE BanAddress=@ban AND Active=1", DBConnection);
            cmd.Parameters.Add(new SQLiteParameter("@ban", ban));

            return ResultsToRecord(cmd.ExecuteReader());
        }

        public int AddBan(string bzID, string address, string host, string author, string reason, int lenght)
        {
            if (ReadOnly)
                return -1;

            SQLiteCommand insert = new SQLiteCommand("INSERT INTO bans (BanID, BanHost, BanAddress, Author, Reason, Active, CreateTime, EndTime) VALUES(@id, @host, @address, @author, @reason, 1, @create, @end);", DBConnection);
            insert.Parameters.Add(new SQLiteParameter("@id", bzID));
            insert.Parameters.Add(new SQLiteParameter("@host", host));
            insert.Parameters.Add(new SQLiteParameter("@address", address));
            insert.Parameters.Add(new SQLiteParameter("@author", author));
            insert.Parameters.Add(new SQLiteParameter("@reason", reason));

            insert.Parameters.Add(new SQLiteParameter("@create", DateTime.Now));
            if (lenght > 0)
                insert.Parameters.Add(new SQLiteParameter("@end", DateTime.Now + new TimeSpan(0,lenght,0)));
            else
                insert.Parameters.Add(new SQLiteParameter("@end", DateTime.MaxValue));

            insert.ExecuteNonQuery();

            // find the record we just added
            SQLiteCommand query = new SQLiteCommand("SELECT ID FROM bans WHERE BanID=@id AND BanHost=@host AND BanAddress=@address AND Author=@author AND Reason=@reason AND Active=1", DBConnection);
            query.Parameters.Add(new SQLiteParameter("@id", bzID));
            query.Parameters.Add(new SQLiteParameter("@host", host));
            query.Parameters.Add(new SQLiteParameter("@address", address));
            query.Parameters.Add(new SQLiteParameter("@author", author));
            query.Parameters.Add(new SQLiteParameter("@reason", reason));

            var results = query.ExecuteReader();

            if (!results.HasRows || !results.Read())
                return -1;
            return results.GetInt32(0);
        }

        public bool ChecForExpiredBans()
        {
            if (ReadOnly)
                return false;

            SQLiteCommand update = new SQLiteCommand("UPDATE bans SET Active=0 WHERE EndTime <= @now AND Active=1);", DBConnection);
            update.Parameters.Add(new SQLiteParameter("@now", DateTime.Now));
            update.ExecuteNonQuery();

            return true;
        }
    }
}
