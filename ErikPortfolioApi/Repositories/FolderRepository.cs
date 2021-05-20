using Dapper;
using ErikPortfolioApi.Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Repositories
{
    public class FolderRepository
    {
        private readonly string _connectionString;
        private IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(_connectionString);
            }
        }

        public FolderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Folder>> ReadTopFolders()
        {
            IEnumerable<Folder> topFolders;

            using (IDbConnection conn = Connection)
            {
                topFolders = await conn.QueryAsync<Folder>("SELECT id, name, parent_folder_id parentFolderId FROM folder WHERE parent_folder_id IS NULL");
            }

            return topFolders;
        }

        public async Task<IEnumerable<Folder>> ReadFoldersFromParentId(long parentFolderId)
        {
            IEnumerable<Folder> folders;

            using (IDbConnection conn = Connection)
            {
                folders = await conn.QueryAsync<Folder>("SELECT id, name, parent_folder_id parentFolderId FROM folder WHERE parent_folder_id = @parentFolderId", new { parentFolderId });
            }

            return folders;
        }

        public async Task<Folder> ReadFolder(long id)
        {
            Folder folder;

            using (IDbConnection conn = Connection)
            {
                folder = await conn.QueryFirstAsync<Folder>("SELECT id, name, parent_folder_id parentFolderId FROM folder WHERE id = @id", new { id });
            }

            return folder;
        }

        public async Task<IEnumerable<Folder>> ReadFolders()
        {
            IEnumerable<Folder> folders;

            using (IDbConnection conn = Connection)
            {
                folders = await conn.QueryAsync<Folder>("SELECT id, name, parent_folder_id parentFolderId FROM folder");
            }

            return folders;
        }

        public async Task<Folder> WriteFolder(Folder folder)
        {
            using (IDbConnection conn = Connection)
            {
                folder.Id = await conn.QueryFirstAsync<int>("INSERT INTO folder (name, parent_folder_id) VALUES (@name, @parentFolderId) RETURNING Id",
                    new { name = folder.Name, parentFolderId = folder.ParentFolderId });
            }

            return folder;
        }

        public async Task DeleteFolder(long id)
        {
            using (IDbConnection conn = Connection)
            {
                await conn.ExecuteAsync("DELETE FROM folder WHERE id = @id", new { id });
            }
        }
    }
}
