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

        public async Task<IEnumerable<Photo>> ReadPhotos(long folderId)
        {
            IEnumerable<Photo> photos;

            using (IDbConnection conn = Connection)
            {
                photos = await conn.QueryAsync<Photo>("SELECT id, name, physical_path physicalPath FROM photo WHERE parent_folder_id = @parentFolderId", new { parentFolderId = folderId });
            }

            return photos;
        }

        public async Task<Photo> ReadPhoto(long id)
        {
            Photo photo;

            using (IDbConnection conn = Connection)
            {
                photo = await conn.QueryFirstAsync<Photo>("SELECT id, name, physical_path physicalPath FROM photo WHERE id = @id", new { id });
            }

            return photo;
        }

        public async Task<Photo> WritePhoto(Photo photo)
        {
            using (IDbConnection conn = Connection)
            {
                photo.Id = await conn.QueryFirstAsync<int>("INSERT INTO photo (physical_path, parent_folder_id, name) VALUES (@physical_path, @parent_folder_id, @name) RETURNING Id",
                    new
                    {
                        physical_path = photo.PhysicalPath,
                        parent_folder_id = photo.ParentFolder.Id,
                        name = photo.name
                    });
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
