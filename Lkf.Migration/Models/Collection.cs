namespace Lkf.Migration.Models
{
    public class Collection
    {
        public Collection()
        {
            Artists = new List<Artist>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Upc { get; set; }
        public string ReleaseDate { get; set; }
        public string IsCompilation { get; set; }
        public string Label { get; set; }
        public string ImageUrl { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
