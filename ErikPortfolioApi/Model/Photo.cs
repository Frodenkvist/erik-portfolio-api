namespace ErikPortfolioApi.Model
{
    public class Photo
    {
        public long Id { get; set; }
        public string PhysicalPath { get; set; }
        public Folder ParentFolder { get; set; }
        public string name { get; set; }
    }
}
