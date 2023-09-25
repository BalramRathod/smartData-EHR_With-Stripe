using Microsoft.Data.SqlClient;
using System.Data;

namespace EHR_project.Data
{
    public class DapperContext
    {

        private readonly IConfiguration _configuration;
        private readonly string _ConnectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _ConnectionString = configuration.GetConnectionString("conn");
        }

        public IDbConnection CreateConnection() => new SqlConnection(_ConnectionString);
    }
}
