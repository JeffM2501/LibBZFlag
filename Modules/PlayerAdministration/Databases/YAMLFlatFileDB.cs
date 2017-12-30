using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using YamlDotNet.Serialization;

using BZFlag.Game.Host;

namespace BZFS.PlayerAdministration.Databases
{
    public class YAMLFlatFileDB : IBanDBBackend
    {
        public class YAMLDB
        {
            public class BanRecord
            {
                public UInt64 ID { get; set; } = 0;

                public string BZID { get; set; } = string.Empty;
                public string Address { get; set; } = string.Empty;
                public string Host { get; set; } = string.Empty;
                public string Author { get; set; } = string.Empty;
                public string Reason { get; set; } = string.Empty;

                public DateTime CreateTime { get; set; } = DateTime.MinValue;
                public DateTime EndTime { get; set; } = DateTime.MinValue;

                public bool Active { get; set; } = true;
            }

            public int FileVersion { get; set; } = 1;
            public string FormatVersion { get; set; } = "BZFS.BANS.YAML.V1";

            public List<BanRecord> Records { get; set; } = new List<BanRecord>();

            public UInt64 LastID { get; set; } = 0;

            public void Clear()
            {
                Records.Clear();
            }
        }

        public YAMLDB DB = new YAMLDB();

        public Dictionary<string, YAMLDB.BanRecord> BZIDCache = new Dictionary<string, YAMLDB.BanRecord>();
        public Dictionary<string, List<YAMLDB.BanRecord>> AddressCache = new Dictionary<string, List<YAMLDB.BanRecord>>();
        public Dictionary<string, List<YAMLDB.BanRecord>> HostCache = new Dictionary<string, List<YAMLDB.BanRecord>>();

        public SortedDictionary<DateTime, List<YAMLDB.BanRecord>> ExpirtingBans = new SortedDictionary<DateTime, List<YAMLDB.BanRecord>>();

        protected FileInfo DBFile = null;
        
        private FileSystemWatcher DBWatcher = null;

        protected void LoadFile()
        {
            lock (DB)
            {
                YAMLDB oldDB = DB;

                if (DBFile == null)
                    return;

                StreamReader fs = null;
                try
                {
                    fs = DBFile.OpenText();

                    Deserializer yaml = new Deserializer();
                    DB = yaml.Deserialize<YAMLDB>(fs);
                    fs.Close();
                    fs = null;
                }
                catch (Exception /*ex*/)
                {
                    DB = oldDB; // do the best we can
                }
                if (fs != null)
                    fs.Close();

                BuildCache();
            }
        }

        private void BuildCache()
        {
            BZIDCache.Clear();
            AddressCache.Clear();
            HostCache.Clear();

            foreach (YAMLDB.BanRecord r in DB.Records)
                CacheRecord(r);
        }

        private void CacheRecord (YAMLDB.BanRecord record)
        {
            if (!record.Active)
                return;

            if (record.BZID != string.Empty &&  !BZIDCache.ContainsKey(record.BZID))
                BZIDCache.Add(record.BZID, record);

            if (record.Address != string.Empty)
            {
                if (!AddressCache.ContainsKey(record.Address))
                    AddressCache.Add(record.Address, new List<YAMLDB.BanRecord>());
                AddressCache[record.Address].Add(record);
            }
           
            if (record.Host != string.Empty)
            {
                if (!HostCache.ContainsKey(record.Host))
                    HostCache.Add(record.Host, new List<YAMLDB.BanRecord>());
                HostCache[record.Host].Add(record);
            }

            if (record.EndTime != DateTime.MaxValue)
            {
                if (!ExpirtingBans.ContainsKey(record.EndTime))
                    ExpirtingBans.Add(record.EndTime, new List<YAMLDB.BanRecord>());

                ExpirtingBans[record.EndTime].Add(record);
            }
        }

        private void FlushRecordFromCache(YAMLDB.BanRecord record)
        {
            if (record.Active)
                return;

            if (record.BZID != string.Empty && BZIDCache.ContainsKey(record.BZID) && BZIDCache[record.BZID] == record)
                BZIDCache.Remove(record.BZID);

            if (record.Address != string.Empty)
            {
                if (AddressCache.ContainsKey(record.Address))
                    AddressCache[record.Address].Remove(record);
            }

            if (record.Host != string.Empty)
            {
                if (HostCache.ContainsKey(record.Host))
                    HostCache[record.Host].Remove(record);
            }
        }

        protected void SaveFile()
        {
            lock(DB)
            {
                if (DBFile == null)
                    return;

                DBWatcher.EnableRaisingEvents = false;

                StreamWriter fs = null;
                try
                {
                    fs = DBFile.CreateText();
                    Serializer yaml = new Serializer();

                    yaml.Serialize(fs, DB, typeof(ServerConfig));
                    fs.Close();
                    fs = null;
                }
                catch (Exception /*ex*/)
                {
                }

                if (fs != null)
                    fs.Close();

                DBWatcher.EnableRaisingEvents = true;
            }
        }

        protected void StartUpdate()
        {
            new Thread(new ThreadStart(SaveFile)).Start();
        }

        public bool Setup(ServerConfig.SecurityInfo info)
        {
            if (DBWatcher != null)
            {
                DBWatcher.Dispose();
                DBWatcher = null;
            }

            DBFile = null;

            if (info.BanDBFile != string.Empty)
            {
                DBFile = new FileInfo(info.BanDBFile);
                if (DBFile.Directory.Exists && !DBFile.Exists)
                    DBFile.Create().Close();
            }

            LoadFile();

            if (info.BansReadOnly || !File.Exists(DBFile.FullName))
            {
                DBFile = null;
                return true;
            }

            if (DBFile.Directory.Exists)
            {
                DBWatcher = new FileSystemWatcher(DBFile.Directory.FullName);
                DBWatcher.Changed += DBWatcher_Changed;
                DBWatcher.EnableRaisingEvents = true;
            }

            return File.Exists(DBFile.FullName);
        }

        private void DBWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath != DBFile.FullName)
                return;

            LoadFile();
        }

        private Databases.BanRecord ConvertBanRecord(YAMLDB.BanRecord inRec)
        {
            Databases.BanRecord outRec = new BanRecord();

            outRec.ID = (int)inRec.ID;
            outRec.CreateTime = inRec.CreateTime;
            outRec.Author = inRec.Author;
            outRec.Reason = inRec.Reason;
            return outRec;
        }

        public Databases.BanRecord FindIDBan(string ID)
        {
            lock(DB)
            {
                if (BZIDCache.ContainsKey(ID))
                    return ConvertBanRecord(BZIDCache[ID]);
            }

            return null;
        }

        public Databases.BanRecord FindIPAddressBan(string ban)
        {
            lock (DB)
            {
                if (AddressCache.ContainsKey(ban))
                    return ConvertBanRecord(AddressCache[ban][0]);
            }

            return null;
        }

        public Databases.BanRecord FindHostMaskBan(string ban)
        {
            lock (DB)
            {
                if (HostCache.ContainsKey(ban))
                    return ConvertBanRecord(HostCache[ban][0]);
            }

            return null;
        }

        public int AddBan(string bzID, string address, string host, string author, string reason, int lenght)
        {
            YAMLDB.BanRecord record = new YAMLDB.BanRecord();

            record.Active = true;
            record.BZID = bzID;
            record.Address = address;
            record.Host = host;
            record.Author = author;
            record.Reason = reason;
            record.CreateTime = DateTime.Now;
            if (lenght > 0)
                record.EndTime = DateTime.Now + new TimeSpan(0, lenght, 0);
            else
                record.EndTime = DateTime.MaxValue;

            lock (DB)
            {
                DB.LastID++;
                record.ID = DB.LastID;
                DB.Records.Add(record);
                CacheRecord(record);
            }
            StartUpdate();

            return (int)record.ID;
        }

        public bool ChecForExpiredBans()
        {
            bool hadAny = false;

            lock (DB)
            {
                if (ExpirtingBans.Count == 0)
                    return false;

                List<DateTime> deads = new List<DateTime>();
                foreach ( var record in ExpirtingBans)
                {
                    if (record.Key > DateTime.Now)
                        break;

                    deads.Add(record.Key);
                    foreach (var r in record.Value)
                    {
                        r.Active = false;
                        FlushRecordFromCache(r);
                    }
                }

                if (hadAny)
                {
                    foreach (var k in deads)
                        ExpirtingBans.Remove(k);
                }
            }

            if (hadAny)
                StartUpdate();

            return hadAny;
        }
    }
}
