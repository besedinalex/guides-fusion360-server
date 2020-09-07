using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class GuideModel
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Description { get; set; }

        public int OwnerId { get; set; }

        public UserModel Owner { get; set; }

        [Required] public string Hidden { get; set; }

        public ICollection<PartGuideModel> PartGuides { get; set; }

        public ICollection<ModelAnnotationModel> ModelAnnotations { get; set; }
    }
}