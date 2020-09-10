using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class Guide
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Description { get; set; }

        public int OwnerId { get; set; }

        public User Owner { get; set; }

        [Required] public string Hidden { get; set; }

        public ICollection<PartGuide> PartGuides { get; set; }

        public ICollection<ModelAnnotation> ModelAnnotations { get; set; }
    }
}