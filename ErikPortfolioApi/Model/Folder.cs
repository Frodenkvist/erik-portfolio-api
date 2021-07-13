namespace ErikPortfolioApi.Model
{
    public class Folder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentFolderId { get; set; }
        public int Order { get; set; }
    }
}
