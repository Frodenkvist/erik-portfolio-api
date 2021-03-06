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
    public class FolderController : ControllerBase
    {
        private readonly ILogger<FolderController> _logger;
        private readonly FolderService _folderService;

        public FolderController(ILogger<FolderController> logger, FolderService folderService)
        {
            _logger = logger;
            _folderService = folderService;
        }

        [HttpGet]
        [Route("top-folders")]
        public async Task<IActionResult> GetTopFolders()
        {
            return Ok(await _folderService.GetTopFolders());
        }

        [HttpGet]
        [Route("structure")]
        public async Task<IActionResult> GetFolderStructure()
        {
            return Ok(await _folderService.GetFolderStructure());
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateFolder([FromBody] Folder folder)
        {
            return Ok(await _folderService.CreateFolder(folder));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveFolder([FromRoute] long id)
        {
            await _folderService.RemoveFolder(id);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> RenameFolder([FromRoute] long id, [FromBody] RenameFolderRequest request)
        {
            await _folderService.RenameFolder(id, request.Name);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [Route("{id}/order")]
        public async Task<IActionResult> UpdateFolderOrder([FromRoute] long id, [FromBody] UpdateOrderRequest request)
        {
            await _folderService.ChangeFolderOrder(id, request.Order);
            return NoContent();
        }
    }
}
