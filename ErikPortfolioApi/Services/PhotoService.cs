using ErikPortfolioApi.Model;
using ErikPortfolioApi.Repositories;
using ErikPortfolioApi.Transform;
using Microsoft.AspNetCore.Http;
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

        public async Task<IEnumerable<EncodedPhoto>> GetEncodedPhotos()
        {
            var photos = await _photoRepository.ReadPhotos();

            return photos.Select(p => p.ToEncodedPhoto());
        }

        public async Task<IEnumerable<Photo>> SavePhotos(List<IFormFile> files)
        {
            List<Photo> photos = new List<Photo>();
            var basePath = _configuration.GetSection("Photo").GetValue<string>("storagePath");

            foreach (var formFile in files)
            {
                if (formFile.Length == 0) continue;

                var filePath = Path.Combine(basePath, formFile.FileName);

                using (var stream = File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                photos.Add(await _photoRepository.WritePhoto(new Photo() { Path = filePath }));
            }

            return photos;
        }

        public async Task DeletePhoto(long id)
        {
            await _photoRepository.DeletePhoto(id);
        }
    }
}
