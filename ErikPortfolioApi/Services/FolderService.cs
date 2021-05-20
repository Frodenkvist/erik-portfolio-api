using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<FolderDto>> GetFolderStructure()
        {
            var folderDtos = (await _folderRepository.ReadTopFolders()).Select(f => new FolderDto { Id = f.Id, Name = f.Name, Children = new List<FolderDto>() });
            var folders = (await _folderRepository.ReadFolders()).Where(f => folderDtos.All(fd => fd.Id != f.Id));

            var structureDtos = new List<FolderDto>();

            foreach (var folderDto in folderDtos)
            {
                structureDtos.Add(await ConvertChildFolders(folderDto, folders));
            }

            return structureDtos;
        }

        public async Task RemoveFolder(long id)
        {
            var children = await _folderRepository.ReadFoldersFromParentId(id);

            foreach (var child in children)
            {
                await RemoveFolder(child.Id);
            }

            await _folderRepository.DeleteFolder(id);
        }

        private async Task<FolderDto> ConvertChildFolders(FolderDto folderDto, IEnumerable<Folder> folders)
        {
            var children = folders.Where(f => f.ParentFolderId == folderDto.Id).Select(f => new FolderDto { Id = f.Id, Name = f.Name, Children = new List<FolderDto>() }).ToList();

            foreach (var child in children)
            {
                folderDto.Children = folderDto.Children.Concat(new[] { await ConvertChildFolders(child, folders) });
            }

            return folderDto;
        }
    }
}
