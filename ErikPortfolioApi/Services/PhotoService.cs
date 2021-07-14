using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using ErikPortfolioApi.Transform;
using Microsoft.Extensions.Configuration;
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

        public PhotoService(PhotoRepository photoRepository, IConfiguration configuration)
        {
            _photoRepository = photoRepository;
            _configuration = configuration;
        }

        public async Task<EncodedPhoto> GetEncodedPhoto(long id)
        {
            var photo = await _photoRepository.ReadPhoto(id);

            return photo.ToEncodedPhoto();
        }

        public async Task<IEnumerable<PresentPhoto>> GetPresentPhotos(long folderId)
        {
            var photos = await _photoRepository.ReadPhotos(folderId);

            return photos.Select(p => p.ToPresentPhoto()).OrderBy(p => p.Order);
        }

        public async Task<IEnumerable<Photo>> SavePhotos(CreatePhotoRequest createPhotoRequest)
        {
            var basePath = _configuration.GetSection("Photo").GetValue<string>("storagePath");

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            var createdPhotos = new List<Photo>();

            var orderIndex = (await _photoRepository.ReadPhotos(createPhotoRequest.ParentFolderId)).Count();

            foreach (var file in createPhotoRequest.Files)
            {
                var filePath = Path.Combine(basePath, file.FileName);

                using (var stream = File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                createdPhotos.Add(await _photoRepository.WritePhoto(new Photo()
                {
                    PhysicalPath = filePath,
                    ParentFolderId = createPhotoRequest.ParentFolderId,
                    Name = file.FileName,
                    Order = orderIndex++
                }));
            }

            return createdPhotos;
        }

        public async Task DeletePhoto(long id)
        {
            var photo = await _photoRepository.ReadPhoto(id);
            var photos = (await _photoRepository.ReadPhotos(photo.ParentFolderId)).OrderBy(f => f.Order).ToList();

            await _photoRepository.DeletePhoto(id);

            File.Delete(photo.PhysicalPath);

            for (var i = photo.Order + 1; i < photos.Count; ++i)
            {
                await _photoRepository.UpdatePhotoOrder(photos[i].Id, i - 1);
            }
        }

        public async Task ChangePhotoOrder(long id, int order)
        {
            var photo = await _photoRepository.ReadPhoto(id);
            if (photo.Order == order) return;

            var photos = await _photoRepository.ReadPhotos(photo.ParentFolderId);

            var photoList = photos.OrderBy(p => p.Order).ToList();

            var orderDiff = photo.Order - order;

            if (orderDiff > 0)
            {
                for (var i = order; i < photo.Order; ++i)
                {
                    await _photoRepository.UpdatePhotoOrder(photoList[i].Id, i + 1);
                }
            }
            else
            {
                for (var i = photo.Order + 1; i <= order; ++i)
                {
                    await _photoRepository.UpdatePhotoOrder(photoList[i].Id, i - 1);
                }
            }

            await _photoRepository.UpdatePhotoOrder(id, order);
        }
    }
}
