using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using ErikPortfolioApi.Transform;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Services
{
    public class PhotoService
    {
        private readonly PhotoRepository _photoRepository;
        private readonly IConfiguration _configuration;
        private readonly FolderService _folderService;

        public PhotoService(PhotoRepository photoRepository, IConfiguration configuration, FolderService folderService)
        {
            _photoRepository = photoRepository;
            _configuration = configuration;
            _folderService = folderService;
        }

        public async Task<IEnumerable<EncodedPhoto>> GetEncodedPhotos()
        {
            var photos = await _photoRepository.ReadPhotos();

            return photos.Select(p => p.ToEncodedPhoto());
        }

        public async Task<Photo> SavePhotos(CreatePhotoRequest createPhotoRequest)
        {
            var basePath = _configuration.GetSection("Photo").GetValue<string>("storagePath");

            if (createPhotoRequest.File.Length == 0) throw new ArgumentException("File null");

            var folder = await _folderService.GetFolder(createPhotoRequest.ParentFolderId);

            var filePath = Path.Combine(basePath, createPhotoRequest.File.FileName);

            using (var stream = File.Create(filePath))
            {
                await createPhotoRequest.File.CopyToAsync(stream);
            }

            return await _photoRepository.WritePhoto(new Photo() { PhysicalPath = filePath, ParentFolder = folder });
        }

        public async Task DeletePhoto(long id)
        {
            Photo photo = await _photoRepository.ReadPhoto(id);

            await _photoRepository.DeletePhoto(id);

            File.Delete(photo.PhysicalPath);
        }
    }
}
