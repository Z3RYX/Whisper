using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Whisper.Utility
{
    /// <summary>
    /// Collection of all methods used to interact with the Mongo Database
    /// </summary>
    public static class DBUtils
    {
        #region Get

        /// <summary>
        /// Gets a guild document from the guilds collection
        /// </summary>
        /// <param name="guildID">ID of the guild</param>
        /// <returns>Guild as a BsonDocument</returns>
        public static async Task<BsonDocument> GetGuildAsync(ulong guildID)
        {
            // TODO Add default guild to DB if guild not found

            var result = await GetCollection("Guilds").FindAsync(EqFilter("_id", (long)guildID));
            return result.FirstOrDefault();
        }

        public static async Task<string> GetPrefixAsync(ulong guildID)
            => (await GetGuildAsync(guildID))["Prefix"].AsString;

        #endregion Get

        #region Utility

        /// <summary>
        /// Checks if at least one document with the specified field value exists
        /// </summary>
        /// <param name="field">Field to check value of</param>
        /// <param name="value">Value to check equality of</param>
        /// <param name="collection">Collection in which to search</param>
        /// <returns>Whether or not at least one document exists</returns>
        private static bool Exists(string field, object value, string collection)
            => GetCollection(collection).Find(EqFilter(field, value)).Any();

        /// <summary>
        /// Gets the specified collection
        /// </summary>
        /// <param name="collectionName">Name of the collection</param>
        /// <returns>The specified collection</returns>
        private static IMongoCollection<BsonDocument> GetCollection(string collectionName)
            => Config.DB.GetCollection<BsonDocument>(collectionName);

        /// <summary>
        /// Creates an equality filter
        /// </summary>
        /// <param name="field">The field to apply the filter to</param>
        /// <param name="value">The value that the field should be equal to</param>
        /// <returns>Filter Definition with specified values</returns>
        private static FilterDefinition<BsonDocument> EqFilter(string field, object value)
            => Builders<BsonDocument>.Filter.Eq(field, value);

        #endregion Utility
    }
}
