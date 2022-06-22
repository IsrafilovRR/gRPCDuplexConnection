namespace WeatherSystem.Common.DataAccess
{
    /// <summary>
    /// Db configuration
    /// </summary>
    public class DbConfiguration
    {
        /// <summary>
        /// Connection string
        /// </summary>
        public string DbConnection { get; set; }

        /// <summary>
        /// Timeout
        /// </summary>
        public int? CommandTimeout { get; set; }
    }
}