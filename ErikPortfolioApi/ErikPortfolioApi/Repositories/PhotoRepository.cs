using Dapper;
using ErikPortfolioApi.Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Repositories
{
    public class PhotoRepository
    {
        private readonly string _connectionString;
        private IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public PhotoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Photo>> ReadPhotos()
        {
            IEnumerable<Photo> photos;

            using (IDbConnection conn = Connection)
            {
                photos = await conn.QueryAsync<Photo>("SELECT * FROM photo");
            }

            return photos;
        }

        public async Task<Photo> WritePhoto(Photo photo)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<int>("INSERT INTO photo (path) VALUES (@path) RETURNING Id", new { path = photo.Path });
                photo.Id = result.Single();
            }

            return photo;
        }

        public async Task DeletePhoto(long id)
        {
            using (IDbConnection conn = Connection)
            {
                await conn.ExecuteAsync("DELETE FROM photo WHERE id = @Id", new { Id = id });
            }
        }
    }
}
