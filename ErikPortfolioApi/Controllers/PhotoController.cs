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
        [Route("encoded/{id}")]
        public async Task<IActionResult> GetEncodedPhoto([FromRoute] long id)
        {
            return Ok(await _photoService.GetEncodedPhoto(id));
        }

        [HttpGet]
        [Route("present/{folderId}")]
        public async Task<IActionResult> GetPresentPhotos([FromRoute] long folderId)
        {
            return Ok(await _photoService.GetPresentPhotos(folderId));
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{id}/order")]
        public async Task<IActionResult> UpdatePhotoOrder([FromRoute] long id, [FromBody] UpdateOrderRequest request)
        {
            await _photoService.ChangePhotoOrder(id, request.Order);
            return NoContent();
        }
    }
}
