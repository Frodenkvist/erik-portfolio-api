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
    }
}
