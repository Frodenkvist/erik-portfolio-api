using ErikPortfolioApi.Model;
using ErikPortfolioApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly ILogger<PhotoController> _logger;
        private readonly PhotoService _photoService;

        public PhotoController(ILogger<PhotoController> logger, PhotoService photoService)
        {
            _logger = logger;
            _photoService = photoService;
        }

        [HttpGet]
        [Route("{folderId}")]
        public async Task<IActionResult> GetPhotos([FromRoute] long folderId)
        {
            return Ok(await _photoService.GetEncodedPhotos(folderId));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> UploadPhotos([FromForm] CreatePhotoRequest createPhotoRequest)
        {
            return Ok(await _photoService.SavePhotos(createPhotoRequest));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePhoto([FromRoute] long id)
        {
            await _photoService.DeletePhoto(id);

            return NoContent();
        }
    }
}
