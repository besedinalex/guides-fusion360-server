namespace GuidesFusion360Server.Models
{
    public class PartGuide
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Content { get; set; }
        
        public int SortKey { get; set; }
        
        public int GuideId { get; set; }
    }
}
