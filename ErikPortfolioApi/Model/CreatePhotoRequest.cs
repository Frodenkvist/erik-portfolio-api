using Microsoft.AspNetCore.Http;

namespace ErikPortfolioApi.Model
{
    public class CreatePhotoRequest
    {
        public IFormFileCollection Files { get; set; }
        public long ParentFolderId { get; set; }
    }
}
