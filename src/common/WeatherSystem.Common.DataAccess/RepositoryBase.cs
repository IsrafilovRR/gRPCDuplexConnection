namespace WeatherSystem.Common.DataAccess
{
    /// <summary>
    /// Abstract class for repository
    /// </summary>
    public abstract class RepositoryBase
    {
        /// <summary>
        /// Connection factory
        /// </summary>
        protected readonly IDbConnectionFactory ConnectionFactory;

        protected RepositoryBase(IDbConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }
    }
}