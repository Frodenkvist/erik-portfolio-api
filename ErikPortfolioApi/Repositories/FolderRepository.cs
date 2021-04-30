using Dapper;
using ErikPortfolioApi.Model;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                topFolders = await conn.QueryAsync<Folder>("SELECT * FROM folder WHERE parent_folder_id IS NULL");
            }

            return topFolders;
        }

        public async Task<Folder> ReadFolder(long id)
        {
            Folder folder;

            using (IDbConnection conn = Connection)
            {
                folder = (await conn.QueryAsync<Folder>("SELECT * FROM folder WHERE id = @id", new { id })).First();
            }

            return folder;
        }

        public async Task<Folder> WriteFolder(Folder folder)
        {
            using (IDbConnection conn = Connection)
            {
                var result = await conn.QueryAsync<int>("INSERT INTO folder (name, parent_folder_id) VALUES (@name, @parentFolderId) RETURNING Id",
                    new { name = folder.Name, parentFolderId = folder.ParentFolder?.Id });

                folder.Id = result.Single();
            }

            return folder;
        }
    }
}
