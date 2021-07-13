using Dapper;
using ErikPortfolioApi.Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Repositories
{
    public class PhotoRepository
    {
        private readonly string PHOTO_SELECT = "SELECT id, name, physical_path AS physicalPath, parent_folder_id AS parentFolderId, photo_order AS order FROM";
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
                photos = await conn.QueryAsync<Photo>($"{PHOTO_SELECT} photo WHERE parent_folder_id = @parentFolderId", new { parentFolderId = folderId });
            }

            return photos;
        }

        public async Task<Photo> ReadPhoto(long id)
        {
            Photo photo;

            using (IDbConnection conn = Connection)
            {
                photo = await conn.QueryFirstAsync<Photo>($"{PHOTO_SELECT} photo WHERE id = @id", new { id });
            }

            return photo;
        }

        public async Task<Photo> WritePhoto(Photo photo)
        {
            using (IDbConnection conn = Connection)
            {
                photo.Id = await conn.QueryFirstAsync<int>("INSERT INTO photo (physical_path, parent_folder_id, name, photo_order) VALUES (@physical_path, @parent_folder_id, @name, @order) RETURNING Id",
                    new
                    {
                        physical_path = photo.PhysicalPath,
                        parent_folder_id = photo.ParentFolderId,
                        name = photo.Name,
                        order = photo.Order
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

        public async Task UpdatePhotoOrder(long id, int order)
        {
            using (IDbConnection conn = Connection)
            {
                await conn.ExecuteAsync("UPDATE photo SET photo_order=@order WHERE id=@id", new { id, order });
            }
        }
    }
}
