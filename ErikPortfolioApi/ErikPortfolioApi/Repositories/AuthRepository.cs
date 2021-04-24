using Dapper;
using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Repositories
{
    public class AuthRepository
    {
        private readonly string _connectionString;
        private IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public AuthRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddLogin(string username, string password)
        {
            using (var conn = Connection)
            {
                await conn.ExecuteAsync("INSERT INTO login VALUES (@username, @password)", new { username, password });
            }
        }

        public async Task<string> GetHashedPassword(string username)
        {
            string hashedPassword = null;

            using (var conn = Connection)
            {
                hashedPassword = (await conn.QueryAsync<string>("SELECT password FROM login WHERE username = @username", new { username })).First();
            }

            return hashedPassword ?? throw new ArgumentException($"username {username} did not have a valid password");
        }
    }
}
