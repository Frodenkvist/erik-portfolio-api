using System.Collections.Generic;

namespace ErikPortfolioApi.Model
{
    public class FolderDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<FolderDto> Children { get; set; }
        public int Order { get; set; }
        public long? ParentFolderId { get; set; }
    }
}
