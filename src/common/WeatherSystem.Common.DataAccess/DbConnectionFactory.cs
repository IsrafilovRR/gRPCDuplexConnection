using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace WeatherSystem.Common.DataAccess
{
    /// <inheritdoc />
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _dbConnectionString;

        public DbConnectionFactory(IOptions<DbConfiguration> optionsConnectionString)
        {
            var config = optionsConnectionString?.Value ??
                         throw new ArgumentNullException(nameof(optionsConnectionString));

            _dbConnectionString = config.DbConnection;
            SqlMapper.Settings.CommandTimeout = config.CommandTimeout;
        }

        /// <inheritdoc />
        public async Task<DbConnection> CreateDbConnectionAsync(bool open = true)
        {
            var connection = new NpgsqlConnection(_dbConnectionString);
            if (open)
            {
                await connection.OpenAsync().ConfigureAwait(false);
            }

            return connection;
        }
    }
}