﻿using Microsoft.AspNetCore.Http;

namespace ErikPortfolioApi.Model
{
    public class CreatePhotoRequest
    {
        public IFormFile File { get; set; }
        public long ParentFolderId { get; set; }
    }
}
