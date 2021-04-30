using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Services
{
    public class FolderService
    {
        private readonly FolderRepository _folderRepository;

        public FolderService(FolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public async Task<IEnumerable<Folder>> GetTopFolders()
        {
            return await _folderRepository.ReadTopFolders();
        }

        public async Task<Folder> GetFolder(long id)
        {
            return await _folderRepository.ReadFolder(id);
        }

        public async Task<Folder> CreateFolder(Folder folder)
        {
            return await _folderRepository.WriteFolder(folder);
        }
    }
}
