using ErikPortfolioApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ErikPortfolioApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public async Task<IActionResult> GetPhotos()
        {
            return Ok(await _photoService.GetEncodedPhotos());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> UploadPhotos(List<IFormFile> files)
        {
            return Ok(await _photoService.SavePhotos(files));
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
