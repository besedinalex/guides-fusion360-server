using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class ModelAnnotation
    {
        public int Id { get; set; }

        public int GuideId { get; set; }

        public Guide Guide { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        [Required] public string Name { get; set; }

        public string Text { get; set; }
    }
}