namespace ErikPortfolioApi.Model
{
    public class Folder
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Folder ParentFolder { get; set; }
    }
}
