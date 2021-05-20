using ErikPortfolioApi.Model;
using System;
using System.Drawing;
using System.IO;

namespace ErikPortfolioApi.Transform
{
    public static class TransformExtensions
    {
        public static EncodedPhoto ToEncodedPhoto(this Photo photo)
        {
            var encodedPhoto = new EncodedPhoto()
            {
                Id = photo.Id,
                Name = photo.name
            };

            using (Image image = Image.FromFile(photo.PhysicalPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    var imageBytes = m.ToArray();

                    encodedPhoto.Data = $"data:image/{photo.PhysicalPath.Split(".")[1]};base64,{Convert.ToBase64String(imageBytes)}";
                }
            }

            return encodedPhoto;
        }
    }
}
