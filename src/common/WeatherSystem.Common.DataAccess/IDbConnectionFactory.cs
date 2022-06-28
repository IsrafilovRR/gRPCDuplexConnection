using System.Data.Common;

namespace WeatherSystem.Common.DataAccess
{
    /// <summary>
    /// Factory for <see cref="DbConnection"/> 
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Create db connection
        /// </summary>
        Task<DbConnection> CreateDbConnectionAsync(bool open = true);
    }
}