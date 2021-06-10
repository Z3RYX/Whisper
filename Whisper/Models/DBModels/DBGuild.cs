using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whisper.Models
{
    /// <summary>
    /// Guild Object that will be stored as a BsonDocument in the Database
    /// </summary>
    public class DBGuild
    {
        /// <summary>
        /// ID of the Guild as well as ID of the Database entry
        /// </summary>
        public long _id { get; set; }

        /// <summary>
        /// Prefix used in this Guild
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Configurations for this Guild
        /// </summary>
        public DBGuildConfig Config { get; set; }

        /// <summary>
        /// List of Mod Mail this Guild has received
        /// </summary>
        public List<DBModMail> ModMail { get; set; }

        /// <summary>
        /// List of mod logs for this Guild
        /// </summary>
        public List<DBLog> Logs { get; set; }

        /// <summary>
        /// List of currently muted Members
        /// </summary>
        public List<DBMute> Mutes { get; set; }

        /// <summary>
        /// List of temporary bans in this Guild
        /// </summary>
        public List<DBTempBan> TempBans { get; set; }

        /// <summary>
        /// List of all Members that have at least one warning
        /// </summary>
        public List<DBWarn> Warns { get; set; }
    }
}
