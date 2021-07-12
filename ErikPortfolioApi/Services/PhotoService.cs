using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using ErikPortfolioApi.Transform;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Services
{
    public class PhotoService
    {
        private readonly ILogger<PhotoService> _logger;
        private readonly PhotoRepository _photoRepository;
        private readonly IConfiguration _configuration;
        private readonly FolderService _folderService;

        public PhotoService(ILogger<PhotoService> logger, PhotoRepository photoRepository, IConfiguration configuration, FolderService folderService)
        {
            _logger = logger;
            _photoRepository = photoRepository;
            _configuration = configuration;
            _folderService = folderService;
        }

        public async Task<IEnumerable<EncodedPhoto>> GetEncodedPhotos(long folderId)
        {
            _logger.LogInformation("Started Encoding Photos");
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var photos = await _photoRepository.ReadPhotos(folderId);

            var encodedPhotos = photos.Select(p => p.ToEncodedPhoto());

            stopwatch.Stop();
            var ts = stopwatch.Elapsed;
            _logger.LogInformation("Encoding Photos time: {Hours}:{Minutes}:{Seconds}.{Milliseconds}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

            return encodedPhotos;
        }

        public async Task<IEnumerable<PresentPhoto>> GetPresentPhotos(long folderId)
        {
            var photos = await _photoRepository.ReadPhotos(folderId);

            return photos.Select(p => p.ToPresentPhoto());
        }

        public async Task<Photo> SavePhotos(CreatePhotoRequest createPhotoRequest)
        {
            var basePath = _configuration.GetSection("Photo").GetValue<string>("storagePath");

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            if (createPhotoRequest.File.Length == 0) throw new ArgumentException("File null");

            var folder = await _folderService.GetFolder(createPhotoRequest.ParentFolderId);

            var filePath = Path.Combine(basePath, createPhotoRequest.File.FileName);

            using (var stream = File.Create(filePath))
            {
                await createPhotoRequest.File.CopyToAsync(stream);
            }

            return await _photoRepository.WritePhoto(new Photo() { PhysicalPath = filePath, ParentFolder = folder, name = createPhotoRequest.File.FileName });
        }

        public async Task DeletePhoto(long id)
        {
            Photo photo = await _photoRepository.ReadPhoto(id);

            await _photoRepository.DeletePhoto(id);

            File.Delete(photo.PhysicalPath);
        }
    }
}
