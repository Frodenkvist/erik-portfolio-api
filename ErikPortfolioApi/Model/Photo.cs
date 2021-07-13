namespace ErikPortfolioApi.Model
{
    public class Photo
    {
        public long Id { get; set; }
        public string PhysicalPath { get; set; }
        public long ParentFolderId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
