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
            var folders = await _folderRepository.ReadFoldersFromParentId(folder.ParentFolderId);

            folder.Order = folders.Count();

            folder = await _folderRepository.WriteFolder(folder);

            await ChangeFolderOrder(folder.Id, 0);

            folder.Order = 0;

            return folder;
        }

        public async Task<IEnumerable<FolderDto>> GetFolderStructure()
        {
            var folderDtos = (await _folderRepository.ReadTopFolders()).Select(f => new FolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Children = new List<FolderDto>(),
                Order = f.Order,
                ParentFolderId = f.ParentFolderId
            }).OrderBy(f => f.Order);
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
            var folder = await _folderRepository.ReadFolder(id);
            var folders = (await _folderRepository.ReadFoldersFromParentId(folder.ParentFolderId)).OrderBy(f => f.Order).ToList();
            var children = await _folderRepository.ReadFoldersFromParentId(id);

            foreach (var child in children)
            {
                await RemoveFolder(child.Id);
            }

            await _folderRepository.DeleteFolder(id);

            for (var i = folder.Order + 1; i < folders.Count; ++i)
            {
                await _folderRepository.UpdateFolderOrder(folders[i].Id, i - 1);
            }
        }

        public async Task RenameFolder(long id, string name)
        {
            await _folderRepository.RenameFolder(id, name);
        }

        public async Task ChangeFolderOrder(long id, int order)
        {
            var folder = await _folderRepository.ReadFolder(id);
            if (folder.Order == order) return;

            var folders = await _folderRepository.ReadFoldersFromParentId(folder.ParentFolderId);

            var folderList = folders.OrderBy(f => f.Order).ToList();

            var orderDiff = folder.Order - order;

            if (orderDiff > 0)
            {
                for (var i = order; i < folder.Order; ++i)
                {
                    await _folderRepository.UpdateFolderOrder(folderList[i].Id, i + 1);
                }
            }
            else
            {
                for (var i = folder.Order + 1; i <= order; ++i)
                {
                    await _folderRepository.UpdateFolderOrder(folderList[i].Id, i - 1);
                }
            }

            await _folderRepository.UpdateFolderOrder(id, order);
        }

        private async Task<FolderDto> ConvertChildFolders(FolderDto folderDto, IEnumerable<Folder> folders)
        {
            var children = folders.Where(f => f.ParentFolderId == folderDto.Id).Select(f => new FolderDto
            {
                Id = f.Id,
                Name = f.Name,
                Children = new List<FolderDto>(),
                Order = f.Order,
                ParentFolderId = f.ParentFolderId
            })
                .OrderBy(f => f.Order).ToList();

            foreach (var child in children)
            {
                folderDto.Children = folderDto.Children.Concat(new[] { await ConvertChildFolders(child, folders) });
            }

            return folderDto;
        }
    }
}
