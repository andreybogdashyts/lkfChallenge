namespace Lkf.Migration.Models
{
    public class Collection
    {
        public Collection()
        {
            Artists = new List<Artist>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public long Upc { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool IsCompilation { get; set; }
        public string Label { get; set; }
        public string ImageUrl { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
